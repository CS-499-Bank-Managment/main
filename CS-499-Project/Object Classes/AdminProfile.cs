using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace CS_499_Project.Object_Classes
{
    public class AdminProfile : ProfileInterface
{

        public AdminProfile()
        {
            this.profile_type = ProfileType.ADMIN;
        }

        public AdminProfile(string username, string name, string email)
        {
            this.username = username;
            this.profile_type = ProfileType.ADMIN;
            this.full_name = name;
            this.email_address = email;
        }

    public AdminProfile(string username, string name)
    {
        Database loginDB = new Database();
        var query_results = loginDB.Login(username, "admins");
        this.username = query_results["username"];
        this.profile_type = ProfileType.ADMIN;
        this.full_name = name;
    }

    public bool CreateCustAccount(string acct_owner, decimal balance, int type, string acct_name, decimal interest)
    {
        Console.WriteLine($"Creating Account with the Following parameters: {acct_owner} {balance} {type} {acct_name} {interest}");
        new Database().CreateCustAcct(acct_owner, balance, type, acct_name, interest);
        return true;
    }

        public bool CreateProfile(string username, string password, string role, string name, string email)
        {
            Database test = new Database();
            bool created = test.NewUser(username, password, role, name, email);
            return created;
        }

    
    public bool DeleteProfile(string username)
    {
        (new Database()).DeleteProfile(username, "customer");

        return true;
    }


}
}
