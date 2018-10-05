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
        TellerProfile foo = new TellerProfile();
        foo.Transfer(AcctTo, AcctFrom, amount);
        return true;
    }

    public bool LogOut()
    {
        this._authenticated = false;
        return this._authenticated;
    }
}
}
