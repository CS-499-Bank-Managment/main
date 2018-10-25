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
            this.dbcmd.CommandText = "INSERT INTO @role (username,password) VALUES (@user, @pwd)";
            this.dbcmd.Parameters.AddWithValue("role", $"{role}s");
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

        public List<String> GetCustomers()
        {
            List<string> profiles = new List<string>();
            this.dbcmd.CommandText = $"SELECT * FROM customers";
            SQLiteDataReader results = this.dbcmd.ExecuteReader();
            while (results.Read())
            {
                profiles.Add(Convert.ToString(results["username"]));
            }
            return profiles;
        }

        public List<string> Login(string username, string password, string role)
        {
            //Login method, TODO: fix sqli in role
            List<string> temp = new List<string>();
            switch (role)
            {
                case "admin":
                    this.dbcmd.CommandText = $"SELECT * from admins where username=@user and password=@pwd";
                    break;
                case "user":
                    this.dbcmd.CommandText = $"SELECT * from users where username=@user and password=@pwd";
                    break;
                case "teller":
                    this.dbcmd.CommandText = $"SELECT * from tellers where username=@user and password=@pwd";
                    break;
                    
            }
            this.dbcmd.Parameters.AddWithValue("user_role", $"{role}s");
            this.dbcmd.Parameters.AddWithValue("user", username);
            this.dbcmd.Parameters.AddWithValue("pwd", password);
            SQLiteDataReader results = this.dbcmd.ExecuteReader();
            while (results.Read())
            {
                temp.Add(Convert.ToString(results["username"]));
                temp.Add(Convert.ToString(results["password"]));
                temp.Add(Convert.ToString(results["userid"]));             
            }
            results.Close();
            return temp;
        }

        public List<string> CreateCustAcct(string username, decimal balance, int type, string name)
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
            this.dbcmd.CommandText = "INSERT INTO customer_acct (owner_id, balance, type, name) VALUES (@user, @balance, @type, @name)";
            this.dbcmd.Parameters.AddWithValue("user", userid);
            this.dbcmd.Parameters.AddWithValue("balance", balance);
            this.dbcmd.Parameters.AddWithValue("type", type);
            this.dbcmd.Parameters.AddWithValue("name", name);
            this.dbcmd.ExecuteNonQuery();

            this.dbcmd.CommandText = "SELECT last_insert_rowid()";
            var id = this.dbcmd.ExecuteScalar();

            this.dbcmd.CommandText = "SELECT * from customer_acct where acct_id=@id";
            this.dbcmd.Parameters.AddWithValue("id", id);
            var reader_debug = this.dbcmd.ExecuteReader();
            while (reader_debug.Read())
            {
                results.Add(reader_debug["owner_id"].ToString());
                results.Add(reader_debug["acct_id"].ToString());
                results.Add(reader_debug["balance"].ToString());
                results.Add(reader_debug["type"].ToString());
                results.Add(reader_debug["name"].ToString());
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


        public Dictionary<string, string> TransferAcct(int acct_to, int acct_from, decimal amount)
        {
            Dictionary<string, string> result_dict = new Dictionary<string, string>();
            this.dbcmd.CommandText = "Select balance from customer_acct where acct_id=@act";
            this.dbcmd.Parameters.AddWithValue("act", acct_to);
            var acct_to_balance_reader = this.dbcmd.ExecuteReader();
            var acct_to_balance = new decimal();
            result_dict.Add("amount", amount.ToString());
            while (acct_to_balance_reader.Read())
            {
                acct_to_balance = Convert.ToDecimal(acct_to_balance_reader["balance"]);
            }
            result_dict.Add("Acct_To_Original", acct_to_balance.ToString());
            acct_to_balance_reader.Close();


            this.dbcmd.CommandText = "select balance from customer_acct where acct_id=@act";
            this.dbcmd.Parameters.AddWithValue("act", acct_from);
            var acct_from_balance_reader = this.dbcmd.ExecuteReader();
            var acct_from_balance = new decimal();
            while (acct_from_balance_reader.Read())
            {
                acct_from_balance = Convert.ToDecimal(acct_from_balance_reader["balance"]);
            }
            result_dict.Add("Acct_From_Original", acct_from_balance.ToString());

            acct_from_balance_reader.Close();

            if (acct_from_balance >= amount)
            {
                this.dbcmd.CommandText = "UPDATE customer_acct set balance=@bal where acct_id=@act";
                this.dbcmd.Parameters.AddWithValue("bal", acct_from_balance - amount);
                this.dbcmd.Parameters.AddWithValue("act", acct_from);
                this.dbcmd.ExecuteNonQuery();

                this.dbcmd.CommandText = "SELECT balance from customer_acct where acct_id=@act";
                this.dbcmd.Parameters.AddWithValue("act", acct_from);
                var new_balance_reader = this.dbcmd.ExecuteReader();
                while (new_balance_reader.Read())
                {
                    result_dict.Add("Acct_From_New", new_balance_reader["balance"].ToString());
                }
                new_balance_reader.Close();


                this.dbcmd.CommandText = "UPDATE customer_acct SET balance=@bal where acct_id=@act";
                this.dbcmd.Parameters.AddWithValue("bal", acct_to_balance + amount);
                this.dbcmd.Parameters.AddWithValue("act", acct_to);
                this.dbcmd.ExecuteNonQuery();
                
                
                this.dbcmd.CommandText = "SELECT balance from customer_acct where acct_id=@act";
                this.dbcmd.Parameters.AddWithValue("act", acct_to);
                new_balance_reader = this.dbcmd.ExecuteReader();
                while (new_balance_reader.Read())
                {
                    result_dict.Add("Acct_To_New",new_balance_reader["balance"].ToString());
                }
                new_balance_reader.Close();
                
            }

            return result_dict;
        }

        public Dictionary<string, string> AddAmount(int acct, decimal amount)
        {
            Dictionary<string, string> results = new Dictionary<string, string>();
            results.Add("amount", amount.ToString());
            this.dbcmd.CommandText = "select balance from customer_acct where acct_id=@act";
            this.dbcmd.Parameters.AddWithValue("act", acct);
            var balance_reader = this.dbcmd.ExecuteReader();
            var balance = new decimal();
            while (balance_reader.Read())
            {
                balance = Convert.ToDecimal(balance_reader["balance"]);
                results.Add("Old Balance", balance.ToString());
            }
            balance_reader.Close();
            if ((balance + amount) >= 0)
            {
                this.dbcmd.CommandText = "UPDATE customer_acct set balance=@bal where acct_id=@act";
                this.dbcmd.Parameters.AddWithValue("bal", balance + amount);
                this.dbcmd.ExecuteNonQuery();

                this.dbcmd.CommandText = "SELECT balance from customer_acct where acct_id=@act";
                this.dbcmd.Parameters.AddWithValue("act", acct);
                var temporary_reader = dbcmd.ExecuteReader();
                while (temporary_reader.Read())
                {
                    results.Add("New Balance", temporary_reader["balance"].ToString());
                }

                temporary_reader.Close();
            }
            

            return results;

        }

        public void LogTransaction(int to_acct, int from_acct, decimal amount, string note = null)
        {
            /*
             *
             * Logs transactions into the transactions database.
             * TODO: integrate this function with all the above functions.
             * 
             * 
             */
            this.dbcmd.CommandText =
                "INSERT into transactions (acct_to, acct_from, amount, note) VALUES (@to, @from, @amount, @note)";
            this.dbcmd.Parameters.AddWithValue("to", to_acct);
            this.dbcmd.Parameters.AddWithValue("from", from_acct);
            this.dbcmd.Parameters.AddWithValue("amount", amount);
            this.dbcmd.Parameters.AddWithValue("note", note);
            this.dbcmd.ExecuteNonQuery();

        }


        public void LogSessionID(string Session_ID, string username, string role)
        {
            this.dbcmd.CommandText = "INSERT into sessions (ID, username, role) VALUES (@Sess_ID, @user, @roll)";
            this.dbcmd.Parameters.AddWithValue("user", username);
            this.dbcmd.Parameters.AddWithValue("roll", role);
            this.dbcmd.Parameters.AddWithValue("Sess_ID", Session_ID);
            this.dbcmd.ExecuteNonQuery();
        }


        public ProfileInterface VerifySession(string sessionID)
        {
            this.dbcmd.CommandText = $"SELECT * FROM sessions";
//            this.dbcmd.Parameters.AddWithValue("session_variable", sessionID);
            Console.WriteLine(this.dbcmd.CommandText);
            var user = this.dbcmd.ExecuteReader();
            ProfileInterface returning = null;
            while (user.Read())
            {
                if (user["ID"].ToString() == sessionID)
                {
                    switch (user["role"].ToString())
                    {
                        case "admin":
                            returning = new AdminProfile(user["username"].ToString());
                            break;
                        case "teller":
                            returning = new TellerProfile(user["username"].ToString());
                            break;
                        case "customer":
                            returning = new CustomerProfile(user["username"].ToString());
                            break;
                    }
                }
            }
            user.Close();
            return returning;
        }

        public Dictionary<string, string> Login(string username, string profileType)
        {
            var returning = new Dictionary<string, string>();
            switch (profileType)
            {
                case "admins":
                    this.dbcmd.CommandText = "SELECT * FROM admins where username=@USR";
                    break;
                case "teller":
                    this.dbcmd.CommandText = "SELECT * FROM tellers where username=@USR";
                    break;
                case "customer":
                    this.dbcmd.CommandText = "SELECT * FROM customers where username=@USR";
                    break;
            }
            this.dbcmd.Parameters.AddWithValue("USR", username);

            var login_reader = this.dbcmd.ExecuteReader();
            while (login_reader.Read())
            {
                returning.Add("username", login_reader["username"].ToString());
                returning.Add("password", login_reader["password"].ToString());
                returning.Add("userid", login_reader["userid"].ToString());
                
            }

            return returning;
        }

        public List<AccountInterface> CustomerAcctList(string username)
        {
            /*
             * Returns the customer accounts that match the owner_id column as a list of lists,
             * where list[0] is the first account, list[0][0] is owner_id, and so forth.
             */

            List<AccountInterface> acctList = new List<AccountInterface>();
            //select the username from customer DB such that we have the user's ID.
            dbcmd.CommandText = "SELECT userid from customers where username=@user";
            dbcmd.Parameters.AddWithValue("user", username);
            var reader = dbcmd.ExecuteScalar().ToString();
                        
            dbcmd.CommandText = "SELECT * from customer_acct where owner_id=@owner";
            dbcmd.Parameters.AddWithValue("owner", reader);
            var acctReader = dbcmd.ExecuteReader();
            while (acctReader.Read())
            {
                acctList.Add(
                    new AccountInterface(Convert.ToDecimal(acctReader["balance"]),
                        (long) acctReader["acct_id"],
                        (int) Enum.Parse<AccountType>(acctReader["type"].ToString()),
                        username,
                        acctReader["name"].ToString()
                    )
                );

            }
            acctReader.Close();
            return acctList;
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