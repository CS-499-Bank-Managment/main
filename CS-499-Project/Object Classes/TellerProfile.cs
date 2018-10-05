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
        }

        public TellerProfile(string name)
        {
            this._FullName = name;
        }
        
        public bool IsAuthenticated()
        {
            return this._authenticated;
        }
        
        public bool Transfer(int AcctTo, int AcctFrom, decimal amount)
        {
            Database DB = new Database();
            DB.TransferAcct(AcctTo, AcctFrom, amount);
            return true;
        }
        
        
        public bool AddAmount(int AcctTo, decimal amount)
        {
            Database DB = new Database();
            DB.AddAmount(AcctTo, amount);
            
            return true;
        }    
        public bool LogOut()
        {
            this._authenticated = false;
            return this._authenticated;
        }
}
}
