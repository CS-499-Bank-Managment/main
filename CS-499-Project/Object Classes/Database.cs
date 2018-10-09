using System;
using System.Data;
using System.Data.SQLite;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CS_499_Project.Object_Classes
{
    public class Database
    {
        private SQLiteConnection db;
        private SQLiteCommand dbcmd;
        public Database()
        {
            this.db = new SQLiteConnection("Data Source=Accounts.sqlite;Version=3;");
            this.db.Open();
            this.dbcmd = this.db.CreateCommand();
            this.dbcmd.CommandType = CommandType.Text;
        }


        public bool NewUser(string username, string password, string role)
        {
            this.dbcmd.CommandText = $"INSERT INTO {role}s (username,password) VALUES (@user, @pwd)";
            this.dbcmd.Parameters.AddWithValue("user", username);
            this.dbcmd.Parameters.AddWithValue("pwd", password);
            this.dbcmd.ExecuteNonQuery();

            return true; //TODO: check for success inputting.

        }

        public List<String> GetAllProfiles()
        {
            List<string> profiles = new List<string>();
            this.dbcmd.CommandText = $"SELECT * FROM customers UNION SELECT * FROM admins UNION SELECT * FROM tellers";
            SQLiteDataReader results = this.dbcmd.ExecuteReader();
            while (results.Read())
            {
                profiles.Add(Convert.ToString(results["username"]));
            }
            return profiles;
        }

        public List<string> Login(string username, string password, string role)
        {
            List<string> temp = new List<string>();
            this.dbcmd.CommandText = $"SELECT * from {role}s where username=@user and password=@pwd";
            this.dbcmd.Parameters.AddWithValue("user", username);
            this.dbcmd.Parameters.AddWithValue("pwd", password);
            SQLiteDataReader results = this.dbcmd.ExecuteReader();
            while (results.Read())
            {
                temp.Add(Convert.ToString(results["username"]));
                temp.Add(Convert.ToString(results["password"]));
                temp.Add(Convert.ToString(results["userid"]));             
            }

            return temp;
        }
        
        ~Database()
        {
            this.dbcmd = null;
            this.db.Close();
            this.db = null;
        }
    }
}