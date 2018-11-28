using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace CS_499_Project.Object_Classes
{
    public class CustomerProfile : ProfileInterface
    {
        private List<AccountInterface> accounts;
        public CustomerProfile()
        {
            AccountInterface account1 = new AccountInterface(100.00m, 123456, (int)AccountType.CHECKING, "my_user",
                "Checking Account", (decimal)0.00, DateTime.Now);
            this.accounts.Add(account1);
        }

        public CustomerProfile(string username)
        {
            this.accounts = new List<AccountInterface>();
            Database acctVerify = new Database();
            var acct_results = acctVerify.Login(username, "customer");
            this.username = username;
            this.profile_type = ProfileType.CUSTOMER;
            var temp_accounts = acctVerify.CustomerAcctList(username);
            foreach (var acct in temp_accounts)
            {
                try
                {
                    Console.WriteLine(acct.ToString());
                    this.accounts.Add(acct);
                }
                catch (System.NullReferenceException)
                {
                    //Do nothing to debug
                }
            }

            foreach (var acct in this.accounts)
            {
                Console.WriteLine(acct.ToString());
            }
        }

        public CustomerProfile(string username, string name, string email)
        {
            this.username = username;
            this.profile_type = ProfileType.CUSTOMER;
            this.full_name = name;
            this.email_address = email;
        }

        public CustomerProfile(string username, string name)
        {
            this.accounts = new List<AccountInterface>();
            Database acctVerify = new Database();
            var acct_results = acctVerify.Login(username, "customer");
            this.username = acct_results["username"];
            this.profile_type = ProfileType.CUSTOMER;
            var temp_accounts = acctVerify.CustomerAcctList(username);
            foreach(var acct in temp_accounts)
            {
                try
                {
                    Console.WriteLine(acct.ToString());
                    this.accounts.Add(acct);
                }
                catch (System.NullReferenceException)
                {
                    //Do nothing to debug
                }
            }

            foreach (var acct in this.accounts)
            {
                Console.WriteLine(acct.ToString());
            }

            this.full_name = name;
        }

        public void addAccount(decimal amount, long number, int type, string username, string name)
        {
            AccountInterface account = new AccountInterface(amount, number, type, username, name, (decimal)0.00, DateTime.Now);
            this.accounts.Add(account);
        }

        public void addAccount(decimal amount, long number, int type, string username, string name, decimal interest)
        {
            AccountInterface account = new AccountInterface(amount, number, type, username, name, interest, DateTime.Now);
            this.accounts.Add(account);
        }

        public List<AccountInterface> ListAccounts()
        {
            return this.accounts;
        }

        public void removeAccount(long account_number)
        {
            foreach (AccountInterface account in this.accounts)
            {
                if(account.accountBalance() == account_number)
                {
                    this.accounts.Remove(account);
                }
            }
        }
        public Dictionary<string, string> Transfer(int AcctTo, int AcctFrom, decimal amount, string note = "")
        {
            Database DB = new Database();
            Dictionary<string, string> results = DB.TransferAcct(AcctTo, AcctFrom, amount);
            return results;
        }
    }
}
