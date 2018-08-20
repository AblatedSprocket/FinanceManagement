using System;

namespace FinanceManagement
{
    public class Transaction
    {
        public int Id { get; }
        public string Vendor { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime PostDate { get; set; }
        public string Description { get; set; }
        public string Account { get; set; }
        public string SerialNumber { get; set; }
        public Transaction() { }
        public Transaction(int id) { Id = id; }
    }
}
