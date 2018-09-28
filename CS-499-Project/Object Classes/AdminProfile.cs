using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace CS_499_Project.Object_Classes
{
    public class AdminProfile : ProfileInterface
{

        public AdminProfile()
        {
            this.profile_type = ProfileType.ADMIN;
        }

        public bool CreateProfile(string username, string password, string role)
        {
            using (StreamWriter w = File.AppendText("./WriteLines.txt"))
            {
                w.WriteLine($"{username},{password},{role}");
            }
            return true;
        }
    
        public bool CreateProfile(ProfileInterface user)
        {
            //How to use Forms in ASP.Net Core
            //https://docs.microsoft.com/en-us/aspnet/core/mvc/views/working-with-forms?view=aspnetcore-2.1
            
            //DELETE THIS LATER. Flatfile for testing.
            using (var w = File.AppendText("./WriteLines.txt"))
            {
                w.WriteLine($"{user.username},{user.temp_password_field},{user.profile_type}");
            }
            
            
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
        var tempFile = Path.GetTempFileName();
        var temp = File.ReadLines("./WriteLines.txt").Where(l => l.Split(",")[0] != user.username);
        File.WriteAllLines(tempFile, temp);
        File.Delete("./WriteLines.txt");
        File.Move(tempFile, "./WriteLines.txt");
        

        user.LogOut();
        return !user.isAuthenticated();
    }
    
    public bool DeleteProfile(string username)
    {
        //Lookup the MongoDB and delete the record from the collection.
        /* MongoDB.deleteOne({ "_id"  : user.id_number });
         * 
         */
        var tempfile = Path.GetTempFileName();
        var temp = File.ReadLines("./WriteLines.txt").Where(l => l.Split(",")[0] != username);
        File.WriteAllLines(tempfile, temp);
        File.Delete("./WriteLines.txt");
        File.Move(tempfile, "./WriteLines.txt");

        return true;
    }

    public bool Check(string username)
    {
        var temp = File.ReadLines("./WriteLines.txt").Where(l => l.Split(",")[0] == username);
        return temp.Equals(null);
    }
}
}
