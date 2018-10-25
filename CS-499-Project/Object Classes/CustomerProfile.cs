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
        public CustomerProfile()
        {
            AccountInterface account1 = new AccountInterface(100.00m, 123456, (int)AccountType.CHECKING, "my_user", "Checking Account");
            this.accounts.Add(account1);
        }

        public CustomerProfile(string username)
        {
            Database acctVerify = new Database();
            var acct_results = acctVerify.Login(username, "customer");
            this.username = acct_results["username"];
            this.profile_type = ProfileType.CUSTOMER;
            foreach(var acct in acctVerify.CustomerAcctList(username))
            {
                this.accounts.Add(acct);
            }
        }

        private List<AccountInterface> accounts;

        public void addAccount(decimal amount, long number, int type, string username, string name)
        {
            AccountInterface account = new AccountInterface(amount, number, type, username, name);
            this.accounts.Add(account);
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
    }
}
