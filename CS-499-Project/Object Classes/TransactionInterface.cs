namespace CS_499_Project.Object_Classes
{
    public class TransactionInterface
    {
        public int acct_to { get; }
        public int acct_from { get; }
        public decimal amount { get; }
        public string note { get; }
        public TransactionInterface(int to, int from, decimal amount, string note)
        {
            this.acct_to = to;
            this.acct_from = from;
            this.amount = amount;
            this.note = note;
        }        
    }
}