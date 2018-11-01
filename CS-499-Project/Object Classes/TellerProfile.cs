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


        public TellerProfile(string name)
        {
            this._FullName = name;
            this.username = name;
            this.profile_type = ProfileType.TELLER;
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
        public bool LogOut()
        {
            this._authenticated = false;
            return this._authenticated;
        }
}
}
