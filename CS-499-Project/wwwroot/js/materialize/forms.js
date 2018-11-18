public Dictionary<string, string> WithdrawAmt(int acct_from, decimal amount)
        {
            /*
             * This method removes money from one account.
             * Input parameters: Account number to transfer from,
             * decimal amount.
             *
             * It will return a dictionary with the keys of the pre transfer and post transfer balance
             * of both accounts
             */
            Dictionary<string, string> result_dict = new Dictionary<string, string>();

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


                LogTransaction(0, acct_from, amount, "Cash Withdrawal");

            }

            return result_dict;
        }

        public Dictionary<string, string> DepositAmt(int acct_to, decimal amount)
        {
            /*
             * This method deposits money into an account.
             * Input parameters: AccountNumber to transfer to,
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


            this.dbcmd.CommandText = "UPDATE customer_acct SET balance=@bal where acct_id=@act";
            this.dbcmd.Parameters.AddWithValue("bal", acct_to_balance + amount);
            this.dbcmd.Parameters.AddWithValue("act", acct_to);
            this.dbcmd.ExecuteNonQuery();


            this.dbcmd.CommandText = "SELECT balance from customer_acct where acct_id=@act";
            this.dbcmd.Parameters.AddWithValue("act", acct_to);
            var new_balance_reader = this.dbcmd.ExecuteReader();
            new_balance_reader = this.dbcmd.ExecuteReader();
                while (new_balance_reader.Read())
                {
                    result_dict.Add("Acct_To_New", new_balance_reader["balance"].ToString());
                }
                new_balance_reader.Close();
                LogTransaction(acct_to, 0, amount, "Deposit");

            }

            return result_dict;
        }
