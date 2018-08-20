using FinanceManagement.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        public List<Transaction> ParseMetroStatementsFromCsv()
        {
            List<Transaction> transactionList = new List<Transaction>();
            // All metro statements are provided with ".TXT" extension
            string[] filePaths = Directory.GetFiles(Path.Combine(_path, "Metro"), "*.csv", SearchOption.TopDirectoryOnly);
            foreach (string filePath in filePaths)
            {
                try
                {
                    Logger.Log($"Processing statement at {filePath}.");
                    string text = File.ReadAllText(filePath);
                    string data = text.Substring(text.IndexOf("\r\n") + 2);
                    MatchCollection matches = Regex.Matches(data, "^(?<account>.*?),(?<date>.*?),(?<serial>.*?),\"(?<desc>.*?)\",(?<amount>.*?),(?<method>.*?)\r\n", RegexOptions.Multiline);
                    foreach (Match match in matches)
                    {
                        Transaction transaction = new Transaction()
                        {
                            Account = match.Result("${account}").Trim(),
                            PostDate = DateTime.Parse(match.Result("${date}")),
                            SerialNumber = match.Result("${serial}").Trim(),
                            Description = (match.Result("${desc}").Trim()),
                            Amount = Convert.ToDecimal(match.Result("${amount}")),
                            Category = "Misc"
                        };
                        switch (match.Result("${method}"))
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
                        Logger.Log($"Processed {transaction.Type} transaction of amount ${transaction.Amount}");
                        transactionList.Add(transaction);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
            return transactionList;
        }
        public List<Transaction> ParseMetroStatements()
        {
            List<Transaction> transactionList = new List<Transaction>();
            // All metro statements are provided with ".TXT" extension
            string[] filePaths = Directory.GetFiles(Path.Combine(_path, "Metro"), "*.txt", SearchOption.TopDirectoryOnly);
            foreach (string filePath in filePaths)
            {
                try
                {
                    Logger.Log($"Processing statement at {filePath}.");
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
                        Logger.Log($"Processed {transaction.Type} transaction of amount ${transaction.Amount}");
                        transactionList.Add(transaction);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
            return transactionList;
        }
        public void ParseLowesStatements()
        {
            // Finish later
            string[] filePaths = Directory.GetFiles(Path.Combine(_path, "Lowes"), "*.pdf", SearchOption.TopDirectoryOnly);
            foreach (string filePath in filePaths)
            {
                string[] statement = PdfParser.ParsePDFStatement(filePath);
                StringBuilder transactionText = new StringBuilder();
                // First parse out actual transactions from all the other junk
                // Go through all pages except last.
                Match match;
                for (int i = 0; i < statement.Length; i++)
                {
                    string text = statement[i];
                    // each set of transactions should start with column headers and end with (Continued on next page) ...
                    match = Regex.Match(statement[i], @"Reference Number/\nTran Date Post Date Description of Transaction or Credit Amount\nInvoice Number\n(?<transactions>(.|\n)*)\(Continued on next page\)");
                    if (match.Success)
                    {
                        transactionText.Append(match.Result("${transactions}"));
                    }
                    else
                    {
                        // ... unless it's the last page, where the transactions end with fees
                        match = Regex.Match(statement[i], @"Reference Number/\nTran Date Post Date Description of Transaction or Credit Amount\nInvoice Number\n(?<transactions>(.|\n)*)FEES\nTOTAL FEES FOR THIS PERIOD");
                        if (match.Success)
                        {
                            transactionText.Append(match.Result("${transactions}"));
                        }
                    }
                }
                // Once the transactions are parsed, pick them out one by one
                string transactionBlob = transactionText.ToString();
                Match transMatch = Regex.Match(transactionBlob, @"\d{2}/\d{2}\s+\d+\sSTORE\s");
            }
        }
        public List<Transaction> ParseAlaskaStatements()
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
    }
}
