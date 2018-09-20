using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS_499_Project.Object_Classes
{
    public class AdminProfile : ProfileInterface
{

        public AdminProfile()
        {
            this.profile_type = ProfileType.ADMIN;
        }

        public bool CreateTeller()
        {
            TellerProfile newteller = new TellerProfile();
            //TODO: input the new teller into MongoDB.
            return true;
        }

    public bool DeleteTeller(TellerProfile teller)
    {
        //Lookup the MongoDB and delete the record from the collection.
        //Logout the teller
        teller.LogOut();
        return !teller.IsAuthenticated();
    }
}
}
