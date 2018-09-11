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
        
        //TODO: We also need a transaction object so we can make a list of transactions in here
        //TODO: May add extra data that other banks have, but for now, this is the minimum

        //get for balance
        public decimal accountBalance()
        {
            return balance;
        }

        //set for balance
        public void changeAccountBalance(decimal bal)
        {
            balance = bal;
        }

        //will be used to add transactions when we get those
        public void addTransaction(/*put some kind of Transaction object here*/)
        {
            //transactionList.add(transaction) or something like that
        }
    }
}
