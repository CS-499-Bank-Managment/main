using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.DateTime;

namespace CS_499_Project.Object_Classes
{
    public abstract class AccountInterface
    {
        //an enum to make account types simple
        enum AccountType
        {
            CHECKING,
            SAVINGS,
            MONEY_MARKET,
            MORTGAGE,
            CREDIT
        }

        //How much is in the account
        protected decimal balance;
        //Account number, whatever we use internally for that
        protected long account_number;
        //Interest rate. May differ in application depending on the account type
        protected decimal interest_rate;
        //The next time interest will be accrued; again, will depend on account type
        protected DateTime interest_date;
        //Which of the above types in falls into
        protected int account_type;
        //The name of the user that created the account
        protected string user;
        //The name of the account that the user sees
        protected string display_name;
        //The balance after pending transactions, such as transfers or payments
        protected decimal pending_balance;
        //The list of all transactions under this account
        protected List<Transaction> transactionList;

        //TODO: May add extra data that other banks have, but for now, this is the minimum

        //get for balance
        public decimal accountBalance() { return this.balance; }

        //set for balance
        public void changeAccountBalance(decimal bal) { this.balance = bal; }

        //add transaction to the list
        public void addTransaction(Transaction transaction)
        {
            transactionList.add(transaction);
        }
    }
}
