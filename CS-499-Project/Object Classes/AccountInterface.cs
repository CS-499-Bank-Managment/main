using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Mvc.Formatters;

//using System.DateTime;

namespace CS_499_Project.Object_Classes
{
    //an enum to make account types simple
    public enum AccountType
    {
        CHECKING,
        SAVINGS,
        MONEY_MARKET,
        MORTGAGE,
        CREDIT
    }

    public class AccountInterface
    {
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
        protected List<TransactionInterface> transactionList;

        //TODO: May add extra data that other banks have, but for now, this is the minimum

        //get for balance
        public decimal accountBalance() { return this.balance; }

        public AccountType getAccountType()
        {
            return (AccountType) account_type;
        }

        public decimal getInterestRate()
        {
            return interest_rate;
        }

        //set for balance
        public void changeAccountBalance(decimal bal) { this.balance = bal; }

        public long accountNumber() { return this.account_number; }

        public string username() { return this.user; }

        public string DisplayName => display_name;

        //add transaction to the list
        public void addTransaction(TransactionInterface transaction)
        {
            transactionList.Add(transaction);
        }

        public List<TransactionInterface> getTransactions()
        {
            return transactionList;
        }

        public AccountInterface(decimal initial_amount, 
                                long number, 
                                int type, 
                                string user_name,
                                string account_name, decimal interestRate, DateTime date)
        {
            //TODO: define the interest rate and date based on the account type
            this.balance = initial_amount;
            this.account_type = type;
            this.user = user_name;
            this.display_name = account_name;
            this.account_number = number;
            this.interest_rate = interestRate;
            this.transactionList = new List<TransactionInterface>();
            this.interest_date = date;
        }

        
        public override string ToString()
        {
            return $"Account name: {this.display_name} Balance: {this.balance} Number: {this.account_number} " +
                   $"owner: {this.user} type: {this.account_type}";
        }

        public Dictionary<string, string> toDict()
        {
            Dictionary<string, string> return_dict = new Dictionary<string, string>();
            return_dict["name"] = this.display_name;
            return_dict["balance"] = this.balance.ToString();
            return_dict["number"] = this.account_number.ToString();
            return_dict["owner"] = this.user;
            return_dict["type"] = ((AccountType)this.account_type).ToString();
            return_dict["interest rate"] = this.interest_rate.ToString();
            return return_dict;

        }
        public static int ParseAccount(string input)
        {
            string func_inp = input.ToUpper();
            switch (func_inp)
            {
                case "CHECKING":
                    return 1;
                case "SAVINGS":
                    return 2;
                case "MONEY_MARKET":
                    return 3;
                case "MORTGAGE":
                    return 4;
                case "CREDIT":
                    return 5;
            }

            return -1;
        }

        public string getType()
        {
            switch((AccountType) this.account_type)
            {
                case AccountType.CHECKING: return "Checking";
                case AccountType.MONEY_MARKET: return "Money Market";
                case AccountType.MORTGAGE: return "Mortgage";
                case AccountType.SAVINGS: return "Savings";
                case AccountType.CREDIT: return "Credit";
                default: return null;
            }
        }
    }
}
