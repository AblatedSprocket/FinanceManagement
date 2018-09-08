using System.Collections.Generic;
using System.Linq;

namespace FinanceManagement.StatementProcessing
{
    public class StatementProcessor
    {
        private string _path;
        public StatementProcessor(string path)
        {
            _path = path;
        }
        public IEnumerable<Transaction> ProcessStatements()
        {
            List<Transaction> transactions = new List<Transaction>();
            StatementParser parser = new StatementParser(_path);
            transactions.AddRange(parser.ParseMetroStatementsFromCsv());
            // Don't want to account for card payments twice. Remove any transactions from Metro that are payments already accounted for by credit cards.
            IEnumerable<Transaction> redundantTransactions = transactions
                                                                .Where(t => t.Description.Contains("BK OF AMER MC ACH Withdrawal")
                                                                    || t.Description.Contains("CARDMEMBER SERV ACH")
                                                                    || t.Description.Contains("BA ELECTRONIC PAYMENT")
                                                                    || t.Description.Contains("BK OF AMER VISA ACH Withdrawal")
                                                                    || t.Description.Contains("BK OF AMER VISA ACH Withdrawal")
                                                                    || t.Description.Contains("Lowes CC ACH"))
                                                                    .ToList();
            foreach (Transaction transaction in redundantTransactions)
            {
                transactions.Remove(transaction);
            }
            // Add in credit card transactions
            transactions.AddRange(parser.ParseAlaskaStatementsFromCsv());
            return transactions;
        }
    }
}
