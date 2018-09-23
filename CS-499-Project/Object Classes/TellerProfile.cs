using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS_499_Project.Object_Classes
{
    public class TellerProfile : ProfileInterface
    {
        private bool authenticated;
        private string FullName;
        public TellerProfile()
        {
            this.authenticated = false;
        }

    public TellerProfile(string name)
    {
        this.FullName = name;
    }

    public bool IsAuthenticated()
    {
        return this.authenticated;
    }

    public bool LogOut()
    {
        this.authenticated = false;
        return this.authenticated;
    }
}
}
