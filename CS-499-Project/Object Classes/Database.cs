using System;
using System.Data;
using System.Data.SQLite;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CS_499_Project.Object_Classes
{
    /*
     *
     *Class to abstract the database access
     * 
     */
    public class Database
    {
        //Variables for connection and command string.
        private SQLiteConnection db;
        private SQLiteCommand dbcmd;
        public Database()
        {
            //Create a connection and open it in the driver
            this.db = new SQLiteConnection("Data Source=Accounts.sqlite;Version=3;");
            this.db.Open();
            this.dbcmd = this.db.CreateCommand();
            this.dbcmd.CommandType = CommandType.Text;
        }


        public bool NewUser(string username, string password, string role)
        {
            //Add the username and password into the role diagram. TODO: fix sqli in roles.
            this.dbcmd.CommandText = $"INSERT INTO {role}s (username,password) VALUES (@user, @pwd)";
            this.dbcmd.Parameters.AddWithValue("user", username);
            this.dbcmd.Parameters.AddWithValue("pwd", password);
            this.dbcmd.ExecuteNonQuery();

            return true; //TODO: check for success inputting.

        }

        public List<string> Login(string username, string password, string role)
        {
            //Login method, TODO: fix sqli in role
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

        public List<string> CreateCustAcct(string username)
        {
            /*
             * To create a customer account, first you pass their username,
             * it then pulls the userid from the customer DB
             *  then, it adds another row into the customer_acct table with a new account
             *  todo: initial balance?
             */
            List<string> results = new List<string>();
            this.dbcmd.CommandText = "SELECT * from customers where username=@user";
            this.dbcmd.Parameters.AddWithValue("user", username);
            var reader = this.dbcmd.ExecuteReader();
            var userid = "";
            while(reader.Read())
            {
                Console.Out.WriteLine(Convert.ToString(reader["userid"]));
                userid = Convert.ToString(reader["userid"]);
            }

            reader.Close();
            this.dbcmd.CommandText = "INSERT INTO customer_acct (owner_id) VALUES (@user)";
            this.dbcmd.Parameters.AddWithValue("user", userid);
            this.dbcmd.ExecuteNonQuery();

            this.dbcmd.CommandText = "SELECT * from customer_acct where owner_id=@owner";
            this.dbcmd.Parameters.AddWithValue("owner", userid);
            var reader_debug = this.dbcmd.ExecuteReader();
            while (reader_debug.Read())
            {
                results.Add(reader_debug["owner_id"].ToString());
                results.Add(reader_debug["acct_id"].ToString());
                results.Add(reader_debug["balance"].ToString());
            }

            return results;
        }

        public bool DeleteCustAcct(string username, int acct_id)
        {
            /* Method for deleting customer account given username and acct id,
             first it queries the customer DB for user ID, then it deletes
             the row in the accounts DB where the account id and customer id match */
            this.dbcmd.CommandText = "Select * from customers where username=@user";
            this.dbcmd.Parameters.AddWithValue("user", username);
            var reader = this.dbcmd.ExecuteReader();
            var userid = "";
            while (reader.Read())
            {
                userid = reader["userid"].ToString();
            }
            Console.Out.WriteLine("USER ID!!!" + userid);
            reader.Close();

            this.dbcmd.CommandText = "DELETE from customer_acct where acct_id=@act and owner_id=@owner";
            this.dbcmd.Parameters.AddWithValue("act", acct_id);
            this.dbcmd.Parameters.AddWithValue("owner", userid);
            this.dbcmd.ExecuteNonQuery();

            this.dbcmd.CommandText = "Select * from customer_acct where owner_id=@usr";
            this.dbcmd.Parameters.AddWithValue("usr", userid);
            var reader_debug = this.dbcmd.ExecuteReader();
            while (reader_debug.Read())
            {
                foreach (var item in reader_debug)
                {
                    Console.Out.WriteLine(item.ToString());
                }
            }

            return true;
        }
        
        
        //Destructor for database to make sure nothing stays open.
        ~Database()
        {
            this.dbcmd = null;
            this.db.Close();
            this.db = null;
        }
    }
}