using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS_499_Project.Object_Classes
{
    public class Transaction
{
        private decimal amount;

        private string description;

        private string label;

        private DateTime transaction_time;

        public decimal getAmount() { return this.amount; }
        public string getDescription() { return this.description; }
        public string getLabel() { return this.label; }
        public DateTime getTransactionTime() { return this.transaction_time; }


        public void setAmount(decimal amount) => this.amount = amount;
        public void setDescription(string desc) => this.description = desc;
        public void setLabel(string label) => this.label = label;

        //commented bc idk if we need this one
        //public Transaction(decimal amount, string description, string label = null)
        //{
        //    this.amount = amount;
        //    this.description = description;
        //    this.label = label;
        //    this.transaction_time.Equals(DateTime.Now);
        //}

        public Transaction(decimal amnt, string desc, string date)
        {
            amount = amnt;
            description = desc;
            try
            {
                transaction_time = DateTime.Parse(date);
            }
            catch(FormatException)
            {
                transaction_time = DateTime.Now;
            }
        }
    }
}
