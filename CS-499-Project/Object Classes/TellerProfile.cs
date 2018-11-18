using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS_499_Project.Object_Classes
{
    public class TellerProfile : ProfileInterface
    {
        private bool _authenticated;
        private string _FullName;
        public TellerProfile()
        {
            this._authenticated = false;
            this.profile_type = ProfileType.TELLER;
        }


        public TellerProfile(string username, string name)
        {
            this._FullName = username;
            this.username = username;
            this.profile_type = ProfileType.TELLER;
            this.full_name = name;
        }

        public TellerProfile(string username, string name, string email)
        {
            this.username = username;
            this.profile_type = ProfileType.TELLER;
            this.full_name = name;
            this.email_address = email;
            this._FullName = name;
        }
        
        public bool IsAuthenticated()
        {
            return this._authenticated;
        }
        
        public Dictionary<string, string> Transfer(int AcctTo, int AcctFrom, decimal amount)
        {
            Database DB = new Database();
            Dictionary<string, string> results = DB.TransferAcct(AcctTo, AcctFrom, amount);
            return results;
        }

        public Dictionary<string, string> Withdrawal(int AcctFrom, decimal amount)
        {
            Database DB = new Database();
            Dictionary<string, string> results = DB.WithdrawAmt(AcctFrom, amount);
            return results;
        }
        public Dictionary<string, string> Deposit(int AcctTo, decimal amount)
        {
            Database DB = new Database();
            Dictionary<string, string> results = DB.DepositAmt(AcctTo, amount);
            return results;
        }

        public List<AccountInterface> ListAccounts(string username)
        {
            var customer = new CustomerProfile(username);
            return customer.ListAccounts();
        }

        public Dictionary<string, string> AddAmount(int AcctTo, decimal amount)
        {
            Database DB = new Database();
            return DB.AddAmount(AcctTo, amount);
        }   
        
}
}
