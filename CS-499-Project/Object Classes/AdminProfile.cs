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

        public bool CreateProfile(ProfileInterface user)
        {
            
            //TODO: input the new teller into MongoDB.
            //MongoDB.Insert, Collection = Accounts.
            /* Mongo Json:
             * { "_id" : getNextSequenceValue("id"),
             * "username" : user.username,
             * "password" : user.password,
             * "fullname" : user.user_firstname + user.user_surname,
             * "role" : user.profile_type
             * }
             */
            return true;
        }

    public bool DeleteProfile(ProfileInterface user)
    {
        //Lookup the MongoDB and delete the record from the collection.
        /* MongoDB.deleteOne({ "_id"  : user.id_number });
         * 
         */
        
        //Logout the teller

        user.LogOut();
        return !user.isAuthenticated();
    }
}
}
