using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS_499_Project.Object_Classes
{
    public class CustomerProfile : ProfileInterface
    {
        public CustomerProfile()
        {
            AccountInterface account1 = new AccountInterface(100.00m, 123456, (int)AccountType.CHECKING, "my_user", "Checking Account");
            this.accounts.Add(account1);
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
