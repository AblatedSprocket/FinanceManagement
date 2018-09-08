using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FinanceManagement.StatementProcessing
{
    public class StatementParser
    {
        private string _path;
        public StatementParser(string path)
        {
            _path = path;
        }
        public List<Transaction> ParseAlaskaStatementsFromCsv()
        {
            try
            {
                List<Transaction> transactionList = new List<Transaction>();
                string[] filePaths = Directory.GetFiles(Path.Combine(_path, "Alaska"), "*.csv", SearchOption.TopDirectoryOnly);
                foreach (string filePath in filePaths)
                {
                    IEnumerable<string> lines = File.ReadLines(filePath);
                    int index = 0;
                    foreach (string line in lines)
                    {
                        if (index++ == 0)
                        {
                            if (line != "Posted Date,Reference Number,Payee,Address,Amount")
                            {
                                throw new FormatException("File contents were not formatted correctly!");
                            }
                        }
                        else
                        {
                            Match match = Regex.Match(line, "^(?<date>[0-9]{2}/[0-9]{2}/[0-9]{4}?),(?<serialNo>[0-9]+?),\"(?<description>.*?)\",\"(?<address>[A-Z0-9\\- ]*?)\",(?<amount>-?[0-9]+\\.[0-9]{2}?)");
                            if (match.Success)
                            {
                                transactionList.Add(new Transaction
                                {
                                    PostDate = Convert.ToDateTime(match.Result("${date}")),
                                    SerialNumber = match.Result("${serialNo}"),
                                    Description = match.Result("${description}"),
                                    Amount = Math.Abs(Convert.ToDecimal(match.Result("${amount}"))),
                                    Type = Convert.ToDecimal(match.Result("${amount}")) > 0 ? "CR" : "DR"
                                });
                            }
                            else
                            {
                                throw new FormatException("Line item is not formatted correctly.");
                            }
                        }
                    }
                }
                return transactionList;
            }
            catch (Exception ex)
            {
                throw new Exception("Error parsing Alaska statements from CSV file.", ex);
            }
        }
        public void ParseLowesStatementsFromCsv()
        {
            throw new NotImplementedException("Parsing for Lowe's Statements is not yet implemented");
        }
        /// <summary>
        /// This method works on .csv files downloaded from the Activity tab on Bank of America's website
        /// </summary>
        /// <returns></returns>
        public List<Transaction> ParseMetroStatementsFromCsv()
        {
            try
            {
                List<Transaction> transactionList = new List<Transaction>();
                string[] filePaths = Directory.GetFiles(Path.Combine(_path, "Metro"), "*.csv", SearchOption.TopDirectoryOnly);
                foreach (string filePath in filePaths)
                {
                    IEnumerable<string> lines = File.ReadLines(filePath);
                    int index = 0;
                    foreach (string line in lines)
                    {
                        if (index++ == 0)
                        {
                            if (line != "Account Designator,Posted Date,Serial Number,\"Description\",Amount,CR/DR")
                            {
                                throw new FormatException("File contents were not formatted correctly!");
                            }
                        }
                        else
                        {
                            Match match = Regex.Match(line, @"^(?<account>[0-9]{4}\s[A-Z]+?)\s+,(?<date>[0-9]{2}/[0-9]{2}/[0-9]{2}?),(?<serialNo>[0-9]+?),(?<description>.*?),(?<amount>[0-9]+\.[0-9]{2}?),(?<type>(CR|DR))");
                            if (match.Success)
                            {
                                transactionList.Add(new Transaction
                                {
                                    Account = match.Result("${account}"),
                                    PostDate = Convert.ToDateTime(match.Result("${date}")),
                                    SerialNumber = match.Result("${serialNo}"),
                                    Description = match.Result("${description}"),
                                    Amount = Convert.ToDecimal(match.Result("${amount}")),
                                    Type = match.Result("${type}")
                                });
                            }
                            else
                            {
                                throw new FormatException("Line item is not formatted correctly.");
                            }
                        }
                    }
                }
                return transactionList;
            }
            catch (Exception ex)
            {
                throw new Exception("Error parsing Metro statements from CSV file.", ex);
            }
        }
        #region Unused
        [Obsolete("ParseAlaskaStatements is deprecated, please use ParseAlaskaStatementsFromCsv")]
        public List<Transaction> ParseAlaskaStatements()
        {
            try
            {
                List<Transaction> transactionList = new List<Transaction>();
                string[] filPaths = Directory.GetFiles(Path.Combine(_path, "Alaska"), "*.txt", SearchOption.TopDirectoryOnly);
                foreach (string filePath in filPaths)
                {
                    string statementText = File.ReadAllText(filePath);
                    Match matchtest = Regex.Match(statementText, @"^(?<transDate>\d{2}/\d{2})\s+(?<postDate>\d{2}/\d{2})\s+(?<desc>.+)\s{24}(?<refNum>\s\d+)\s+(?<account>\d+)\s+(?<amount>[\d\.]+)", RegexOptions.Multiline);
                    if (matchtest.Success)
                    {
                        string transDate = matchtest.Result("${transDate}").Trim();
                        string postDate = matchtest.Result("${postDate}").Trim();
                        string desc = matchtest.Result("${desc}").Trim();
                        string refnum = matchtest.Result("${refNum}").Trim();
                        string account = matchtest.Result("${account}").Trim();
                        string amount = matchtest.Result("${amount}").Trim();
                    }
                    MatchCollection matches = Regex.Matches(statementText, @"^(?<transDate>\d{2}/\d{2})\s+(?<postDate>\d{2}/\d{2})\s+(?<desc>.+)\s{24}(?<refNum>\s\d+)\s+(?<account>\d+)\s+(?<amount>[\d\.]+)", RegexOptions.Multiline);
                    foreach (Match match in matches)
                    {
                        Transaction transaction = new Transaction
                        {
                            Account = match.Result("${account}"),
                            TransactionDate = DateTime.Parse(match.Result("${transDate}")),
                            PostDate = DateTime.Parse(match.Result("${postDate}")),
                            SerialNumber = match.Result("${refNum}"),
                            Description = match.Result("${desc}"),
                            Amount = Convert.ToDecimal(match.Result("${amount}")),
                            Type = "Debit",
                            Category = "Misc"
                        };
                        transactionList.Add(transaction);
                    }
                }
                return transactionList;
            }
            catch (Exception ex)
            {
                throw new Exception("Error parsing Alaska statements.", ex);
            }
        }
        [Obsolete("ParseMetroStatements is deprecated, please use ParseAlaskaStatementsFromCsv")]
        public List<Transaction> ParseMetroStatements()
        {
            List<Transaction> transactionList = new List<Transaction>();
            // All metro statements are provided with ".TXT" extension
            string[] filePaths = Directory.GetFiles(Path.Combine(_path, "Metro"), "*.txt", SearchOption.TopDirectoryOnly);
            foreach (string filePath in filePaths)
            {
                try
                {
                    string statementText = File.ReadAllText(filePath);
                    MatchCollection matches = Regex.Matches(statementText, @"^(?<account>\d+\s\w+)\s+(?<date>\d{2}/\d{2}/\d{2})\s+(?<serial>\d+)\s+(?<desc>.+)\:\s+(?<method>.+?)\s+(?<amount>[\d\.]+?)\s+(?<type>\w+)\s+", RegexOptions.Multiline);
                    foreach (Match match in matches)
                    {
                        Transaction transaction = new Transaction();
                        transaction.Account = match.Result("${account}").Trim();
                        transaction.PostDate = DateTime.Parse(match.Result("${date}"));
                        transaction.SerialNumber = match.Result("${serial}").Trim();
                        transaction.Description = string.Concat(match.Result("${desc}").Trim(), ' ', match.Result("${method}").Trim());
                        transaction.Amount = Convert.ToDecimal(match.Result("${amount}"));
                        switch (match.Result("${type}"))
                        {
                            case "DR":
                                transaction.Type = "Debit";
                                break;
                            case "CR":
                                transaction.Type = "Credit";
                                break;
                            default:
                                transaction.Type = "Debit";
                                break;

                        }
                        transactionList.Add(transaction);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error parsing metro statements.", ex);
                }
            }
            return transactionList;
        }
        #endregion
    }
}
