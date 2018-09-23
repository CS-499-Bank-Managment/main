using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS_499_Project.Object_Classes
{
    public abstract class ProfileInterface
    {
        public enum ProfileType
        {
            ADMIN,
            TELLER,
            CUSTOMER
        }

        // Also referred to as username. The alias a customer, teller, or admin may enter to login 
        public string username { get; set; }

        // TODO: This should be replaced with some sort of authentication token.
        // for now, this field will take care of basic login until authentication functionality is added.
        public string temp_password_field { get; set; }

        // The ID associated with the profile. For use with database
        private int id_number { get; set; }

        // contact info used to verify account
        private string email_address { get; set; }
        private string phone_number { get; set; }

        // The first and last name associated with the profile.
        private string user_firstname { get; set; }
        private string user_surname { get; set; }

        // explains whether the profile is an ADMIN, TELLER, or CUSTOMER type
        public ProfileType profile_type { get; set; }
        /// <summary>
        /// Checks whether or not the password the user entered matches the password 
        /// associated with this username.
        /// </summary>
        /// <returns> true if the user is correctly authenticated and false if not. </returns>
        /// <param name="password"> the string password entered during login </param>
        /// 

        public virtual bool isAuthenticated(string password)
        {
            if (password == temp_password_field)
                return true;
            else
                return false;
        }

        public virtual bool isAuthenticated()
        {
            //checks for authentication of users already logged in.
            return true;
        }

        public virtual void LogOut() 
            {
            //Clear session info & cookies in DB / server.
            
        }                                    

        public ProfileInterface()
        {
            
        }

        public ProfileInterface(string username, string password, string role)
        {
        }
        public ProfileInterface(string username, string password)
            {
            //Constructor that can be used to login.
}           //MongoDB.FineOne({"username" : username, "password" : password})
            //Construct object based on role record.

    }
}