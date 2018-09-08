namespace FinanceManagement
{
    public class Vendor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string TransactionKey { get; set; }
        public int TransactionCount { get; set; }
        public Vendor() { }
        public Vendor(int id) { Id = id; }
    }
}
