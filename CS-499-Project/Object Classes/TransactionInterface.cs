using System;

//More or less a message object to make passing information about the transaction easier.

namespace CS_499_Project.Object_Classes
{
    public class TransactionInterface
    {
        public int acct_to { get; }
        public int acct_from { get; }
        public decimal amount { get; }
        public string note { get; }
        public DateTime transaction_time { get; }
        public string to_name { get; }
        public string from_name { get; }
        public TransactionInterface(int to, int from, decimal amount, string note, string date)
        {
            this.acct_to = to;
            this.acct_from = from;
            this.amount = amount;
            this.note = note;

            try
            {
                transaction_time = DateTime.Parse(date);
            }
            catch (FormatException)
            {
                transaction_time = DateTime.Today;
            }
        }        
    }
}