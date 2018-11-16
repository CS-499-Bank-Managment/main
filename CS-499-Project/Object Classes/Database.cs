using System;
using System.Data;
using System.Data.SQLite;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.AspNetCore.Mvc;

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

        public string getCurrentCustomer(string session)
        {
            this.dbcmd.CommandText = "SELECT customer FROM sessions WHERE ID=@session";
            this.dbcmd.Parameters.AddWithValue("session", session);
            return this.dbcmd.ExecuteScalar().ToString();
        }

        public void setCurrentCustomer(string customer, string session)
        {
            this.dbcmd.CommandText = "UPDATE sessions SET customer=@customer WHERE ID=@session";
            this.dbcmd.Parameters.AddWithValue("customer", customer);
            this.dbcmd.Parameters.AddWithValue("session", session);
        }

        public bool NewUser(string username, string password, string role, string name, string email)
        {
            Console.WriteLine($"Function called with {username} {password} {role} {name} {email}");
            //Add the username and password into the role diagram. TODO: fix sqli in roles.
            switch (role)
            {
                case "admin":
                    this.dbcmd.CommandText = "INSERT INTO admins (username,password,name,email) VALUES (@user, @pwd, @nm, @eml)";
                    break;
                
                case "teller":
                    this.dbcmd.CommandText = "INSERT INTO tellers (username,password,name,email) VALUES (@user, @pwd, @nm, @eml)";
                    break;
                
                case "customer":
                    this.dbcmd.CommandText = "INSERT INTO customers (username,password,name,email) VALUES (@user, @pwd, @nm, @eml)";
                    break;
            }
            this.dbcmd.Parameters.AddWithValue("user", username);
            this.dbcmd.Parameters.AddWithValue("pwd", Database.PasswordHash(password));
            this.dbcmd.Parameters.AddWithValue("nm", name);
            this.dbcmd.Parameters.AddWithValue("eml", email);
            this.dbcmd.ExecuteNonQuery();


            Dictionary<string, string> info_dict = new Dictionary<string, string>();
            switch (role)
            {
                case "admin":
                    this.dbcmd.CommandText = "SELECT * from admins where username=@user";
                    break;

                case "teller":
                    this.dbcmd.CommandText = "SELECT * from tellers where username=@user";
                    break;

                case "customer":
                    this.dbcmd.CommandText = "SELECT * from customers where username=@user";
                    break;
            }
            dbcmd.Parameters.AddWithValue("user", username);
            SQLiteDataReader results = dbcmd.ExecuteReader();
            results.Close();
            if (!results.HasRows)
            {
                return false;
            }
            else
            {
                return true;
            }

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

        public string Login(string username, string password, string role, bool session)
        {

            dbcmd.CommandText = "DELETE FROM sessions WHERE username=@user";
            dbcmd.Parameters.AddWithValue("user", username);
            dbcmd.ExecuteNonQuery();

            List<string> temp = new List<string>();
            switch (role)
            {
                case "admin":
                    this.dbcmd.CommandText = $"SELECT * from admins where username=@user and password=@pwd";
                    break;
                case "customer":
                    this.dbcmd.CommandText = $"SELECT * from customers where username=@user and password=@pwd";
                    break;
                case "teller":
                    this.dbcmd.CommandText = $"SELECT * from tellers where username=@user and password=@pwd";
                    break;
                    
            }
            this.dbcmd.Parameters.AddWithValue("user", username);
            this.dbcmd.Parameters.AddWithValue("pwd", Database.PasswordHash(password));
            Console.WriteLine(Database.PasswordHash(password));
            SQLiteDataReader results = dbcmd.ExecuteReader();
            if (!results.HasRows)
            {
                results.Close();
                throw new UnauthorizedAccessException();
            }

            var name = "";
            while(results.Read())
            {
                name = Convert.ToString(results["name"]);
            }

            results.Close();

            string session_id;
            using (SHA256 SessionAlgorithm = SHA256.Create())
            {
                byte[] Hash_Bytes = SessionAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(username + "Salt"));
                StringBuilder Hash_Builder = new StringBuilder();
                for (int i = 0; i < Hash_Bytes.Length; i++)
                {
                    Hash_Builder.Append(Hash_Bytes[i].ToString("x2"));
                }

                session_id = Hash_Builder.ToString();
                LogSessionID(session_id, username, role, name);
            }
            return session_id;
        }

        public List<string> CreateCustAcct(string username, decimal balance, int type, string name, 
            decimal interest=0.00m)
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
            this.dbcmd.CommandText = "INSERT INTO customer_acct (owner_id, balance, type, name) VALUES (@user, @balance, @type, @name, @interest)";
            this.dbcmd.Parameters.AddWithValue("user", userid);
            this.dbcmd.Parameters.AddWithValue("balance", balance);
            this.dbcmd.Parameters.AddWithValue("type", type);
            this.dbcmd.Parameters.AddWithValue("name", name);
            this.dbcmd.Parameters.AddWithValue("interest", interest);
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
            /*
             * This method transfers money from one account to another account.
             * Input parameters: AccountNumber to transfer to,
             * Account number to transfer from,
             * decimal amount.
             *
             * It will return a dictionary with the keys of the pre transfer and post transfer balance
             * of both accounts
             */
            Dictionary<string, string> result_dict = new Dictionary<string, string>();
            //Get the balance from the account where the ID matches the to account.
            //Add it to the dictionary
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

            //Get the original balance of the account_from and add it to the dictionary
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
            //Make sure the transfer won't put the account_from below zero.
            if (acct_from_balance >= amount)
            {
                //If it doesn't, update both columns with the amount being removed
                //Or added respectively
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
                LogTransaction(acct_to, acct_from, amount, "Transfer");
                
            }

            return result_dict;
        }

        public AccountInterface getAccount(int account_number, string customer)
        {
            AccountInterface account = null;
            this.dbcmd.CommandText = "select * from customer_acct where acct_id=@act";
            this.dbcmd.Parameters.AddWithValue("act", account_number);
            var balance_reader = this.dbcmd.ExecuteReader();
            while (balance_reader.Read())
            {
                account = new AccountInterface(
                    Convert.ToDecimal(balance_reader["balance"]),
                    Convert.ToInt32(account_number),
                    Convert.ToInt32(balance_reader["type"]),
                    customer, 
                    balance_reader["name"].ToString(),
                    Convert.ToDecimal(balance_reader["interest"]),
                    DateTime.Parse(balance_reader["date"].ToString()));
            }
            return account;
        }

        public Dictionary<string, string> AddAmount(int acct, decimal amount)
        {
            /*
             * This method adds the amount into an account. Pass it the account number and the amount
             * Returns a dictionary with old balance and new balance as keys
             */
            
            Dictionary<string, string> results = new Dictionary<string, string>();
            results.Add("amount", amount.ToString());
            //Get the balance from the user act
            this.dbcmd.CommandText = "select balance from customer_acct where acct_id=@act";
            this.dbcmd.Parameters.AddWithValue("act", acct);
            var balance_reader = this.dbcmd.ExecuteReader();
            var balance = new decimal();
            while (balance_reader.Read())
            {
                balance = Convert.ToDecimal(balance_reader["balance"]);
                results.Add("Old Balance", balance.ToString());
            }
            //Make sure that withdrawing the money from the account will not put it in the negative.
            //This is important because a deposit and a withdraw are the same operation with 
            //a different sign.
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
                LogTransaction(acct, acct, amount, "Deposit.");
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


        public void LogSessionID(string Session_ID, string username, string role, string name)
        {
            //This is a helper function to log the session into the Sessions table.
            this.dbcmd.CommandText = "INSERT into sessions (ID, username, role, name) VALUES (@Sess_ID, @user, @roll, @name)";
            this.dbcmd.Parameters.AddWithValue("user", username);
            this.dbcmd.Parameters.AddWithValue("roll", role);
            this.dbcmd.Parameters.AddWithValue("Sess_ID", Session_ID);
            this.dbcmd.Parameters.AddWithValue("name", name);
            this.dbcmd.ExecuteNonQuery();
        }


        public ProfileInterface VerifySession(string sessionID)
        {
            /*
             * This method is called with SessionID, which is the cookie under the key
             * SESSION_ID, it will look up the sessions table, find a user with a corresponding value
             * then create a profile interface of that user.
             * TODO: Make profile types more distinguishable somehow?
             */
            this.dbcmd.CommandText = $"SELECT * FROM sessions";
            Console.WriteLine(this.dbcmd.CommandText);
            var session= this.dbcmd.ExecuteReader();
            ProfileInterface returning = null;
            while (session.Read())
            {
                if (session["ID"].ToString() == sessionID)
                {
                    switch (session["role"].ToString())
                    {
                        case "admin":
                            returning = new AdminProfile(session["username"].ToString(), session["name"].ToString());
                            break;
                        case "teller":
                            returning = new TellerProfile(session["username"].ToString(), session["name"].ToString());
                            break;
                        case "customer":
                            returning = new CustomerProfile(session["username"].ToString(), session["name"].ToString());
                            break;
                    }
                }
            }
            session.Close();
            return returning;
        }

        public Dictionary<string, string> Login(string username, string profileType)
        {
            /*
             * Returns a Dictionary of all the rows columns and their names from the DB
             * given a users username and profileType.
             */
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
                returning.Add("name", login_reader["name"].ToString());
            }
            login_reader.Close();
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
            
            //Select all the accounts the user owns
            //Add them to a list
            //return that list of accountinterface()
            dbcmd.CommandText = "SELECT * from customer_acct where owner_id=@owner";
            dbcmd.Parameters.AddWithValue("owner", reader);
            var acctReader = dbcmd.ExecuteReader();
            while (acctReader.Read())
            {
                acctList.Add(
                    new AccountInterface(Convert.ToDecimal(acctReader["balance"]),
                        (long) acctReader["acct_id"],
                        Convert.ToInt32(acctReader["type"]),
                        username,
                        acctReader["name"].ToString(),
                        Convert.ToDecimal(acctReader["interest"]),
                        Convert.ToDateTime(acctReader["created"])
                    )
                );

            }

            acctReader.Close();
            return acctList;
        }

        public void Logout(string session)
        {
            /*
             * Deletes the given session from the sessions DB for logging out.
             */
            dbcmd.CommandText = "DELETE FROM sessions WHERE ID=@sess";
            dbcmd.Parameters.AddWithValue("sess", session);
            dbcmd.ExecuteNonQuery();
        }

        public static string PasswordHash(string password)
        {
            /*
             * Static method that returns the SHA 512 hash of the given string.
             */
            StringBuilder pwdhash_builder = new StringBuilder();
            using (SHA512 PDWHash = SHA512.Create())
            {
                byte[] PWD_Hash = PDWHash.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < PWD_Hash.Length; i++)
                {
                    pwdhash_builder.Append(PWD_Hash[i].ToString("x2"));
                }
            }

            return pwdhash_builder.ToString();

        }

        public List<ProfileInterface> CustomerLookup(string username, string type)
        {
            List<ProfileInterface> profile_list = new List<ProfileInterface>();
             dbcmd.CommandText = "SELECT * from customers where username LIKE @user1 OR name LIKE @user2";
            dbcmd.Parameters.AddWithValue("user1", "%" + username + "%");
            dbcmd.Parameters.AddWithValue("user2", "%" + username + "%");
            var reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                profile_list.Add(new CustomerProfile(reader["username"].ToString(), reader["name"].ToString(), reader["email"].ToString()));
            }
            reader.Close();
            if(type == "Admin")
            {
                dbcmd.CommandText = "SELECT * from tellers where username LIKE @user1 OR name LIKE @user2 UNION SELECT * from admins where username LIKE @user3 OR name LIKE @user4";
                dbcmd.Parameters.AddWithValue("user1", "%" + username + "%");
                dbcmd.Parameters.AddWithValue("user2", "%" + username + "%");
                dbcmd.Parameters.AddWithValue("user3", "%" + username + "%");
                dbcmd.Parameters.AddWithValue("user4", "%" + username + "%");
                var reader2 = dbcmd.ExecuteReader();
                while (reader2.Read())
                {
                    profile_list.Add(new TellerProfile(reader2["username"].ToString(), reader2["name"].ToString(), reader2["email"].ToString()));
                }
                reader2.Close();
            }
            return profile_list;
        }

        public string GetUsername(int account_id)
        {
            string username = "";
            dbcmd.CommandText = "SELECT * from customers where userid=@id";
            dbcmd.Parameters.AddWithValue("id", account_id);
            var reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                username = reader["username"].ToString();
            }
            reader.Close();
            return username;
        }

        public List<TransactionInterface> ListTransactions(long acct_id)
        {
            List<TransactionInterface> transaction_list = new List<TransactionInterface>();
            dbcmd.CommandText = "SELECT * from transactions where acct_to=@id OR acct_from=@id";
            dbcmd.Parameters.AddWithValue("id", acct_id);
            var reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                TransactionInterface temp = new TransactionInterface((int) reader["acct_to"], (int)reader["acct_from"], 
                    Convert.ToDecimal(reader["amount"]), 
                    reader["note"].ToString(), reader["date"].ToString());
                transaction_list.Add(temp);
            }
      
            return transaction_list;
        }
        //Destructor for database to make sure nothing stays open.
        ~Database()
        {
            this.dbcmd = null;
        }
    }
}