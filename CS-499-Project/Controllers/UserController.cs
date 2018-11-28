using CS_499_Project.Object_Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System;

/*
 *
 * At the top of every method you will see some Request.Cookies["SESSION_ID"],
 * VerifySession(CookieValue)
 * ...
 * ((<role>Profile).Method());
 *
 * This Verifies the user is of the current type, and if it cannot be it returns the Denied View.
 * This comment block servers as an explanation once, instead of every time.
 * 
 */

namespace CS_499_Project.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Dashboard", "User");
        }

        public IActionResult AccountDashboard(int account_number)
        {
            var database = new Database();
            var session = Request.Cookies["SESSION_ID"];
            ProfileInterface Verified = database.VerifySession(session);
            var username = Verified.username;
            ViewBag.Title = "Account Dashboard";
            ViewBag.user_header = username;
            var number = account_number;
            var customer_served = database.getCurrentCustomer(session);
            AccountInterface account = database.getAccount(number, customer_served);
            ViewBag.account = account;
            switch (Verified.profile_type)
            {
                case ProfileInterface.ProfileType.ADMIN: ViewBag.user_role = "Admin"; break;
                case ProfileInterface.ProfileType.TELLER: ViewBag.user_role = "Teller"; break;
                case ProfileInterface.ProfileType.CUSTOMER: ViewBag.user_role = "User"; break;
            }
            List<TransactionInterface> transactions = database.ListTransactions(account.accountNumber());
            foreach (TransactionInterface transaction in transactions)
            {
                account.addTransaction(transaction);
            }
            ViewBag.account_type = "Unknown";
            switch(account.getAccountType())
            {
                case AccountType.CHECKING:
                    ViewBag.account_type = "Checking";
                    break;
                case AccountType.MONEY_MARKET:
                    ViewBag.account_type = "Money Market";
                    break;
                case AccountType.MORTGAGE:
                    ViewBag.account_type = "Mortgage";
                    break;
                case AccountType.CREDIT:
                    ViewBag.account_type = "Credit";
                    break;
                case AccountType.SAVINGS:
                    ViewBag.account_type = "Savings";
                    break;
            }
            return View();
        }
        
        public IActionResult Dashboard()
        {
            var session = Request.Cookies["SESSION_ID"];
            var database = new Database();
            ProfileInterface Verified = database.VerifySession(session);
            var username = Verified.username;
            ViewBag.Title = "Customer Dashboard";
            ViewBag.user_header = username;
            var customer_served = database.getCurrentCustomer(session);
            if (Verified.profile_type == ProfileInterface.ProfileType.TELLER || 
               Verified.profile_type == ProfileInterface.ProfileType.ADMIN)
            {
                ViewBag.allowed = true;
            }
            else
            {
                ViewBag.allowed = false;
            }

            if(Verified.profile_type == ProfileInterface.ProfileType.ADMIN)
            {
                ViewBag.isAdmin = true;
            }
            else
            {
                ViewBag.isAdmin = false;
            }

            switch (Verified.profile_type)
            {
                case ProfileInterface.ProfileType.ADMIN: ViewBag.user_role = "Admin"; break;
                case ProfileInterface.ProfileType.TELLER: ViewBag.user_role = "Teller"; break;
                case ProfileInterface.ProfileType.CUSTOMER: ViewBag.user_role = "User"; break;
            }

            ViewBag.full_name = database.GetFullName(customer_served);
            if (ViewBag.full_name != null)
            {
                ViewBag.accounts = database.CustomerAcctList(customer_served);
                foreach (AccountInterface account in ViewBag.accounts)
                {
                    List<TransactionInterface> transactions = database.ListTransactions(account.accountNumber());
                    int count = 0;
                    foreach (TransactionInterface transaction in transactions)
                    {
                        if (count < 5)
                        {
                            account.addTransaction(transaction);
                            count++;
                        }
                    }
                }
                ViewBag.isCustomer = true;
            }
            else
            {
                ViewBag.full_name = database.GetEmployeeName(customer_served);
                ViewBag.isCustomer = false;
            }

            ViewBag.current_customer = customer_served;

            return View();
        }

        public IActionResult Denied()
        {
            ViewBag.title = "Access Denied";
            return View();
        }

        // GET
        public IActionResult Login(string username, string password)
        {
            
            //int counter = 0;  
            string line;
            ViewBag.status = "no";

            // Read the file and display it line by line.  
            System.IO.StreamReader file =   
                new System.IO.StreamReader(@"./WriteLines.txt");  
            while((line = file.ReadLine()) != null)
            {
                string[] tempstr = line.Split(",");
                if (username == tempstr[0])
                {
                    if (password == tempstr[1])
                    {
                        string role = tempstr[2];
                        //Create a new object of role type.
                        if (role == "admin")
                        {
                            //do stuff.
                            ViewBag.status = "Admin";
                        }

                        if (role == "teller")
                        {
                            ViewBag.status = "Teller";
                        }

                        if (role == "customer")
                        {
                            ViewBag.status = "Customer";
                        }
                    }
                }
            }  

            return
            View();
        }

        public IActionResult PrintReport(int account_number)
        {
            var session = Request.Cookies["SESSION_ID"];
            var LookupDB = new Database();
            var profile = (LookupDB).VerifySession(session);
            var username = LookupDB.getCurrentCustomer(session);
            Console.Clear();
            Console.WriteLine($"Function Parameter: {account_number}");
            if (profile?.profile_type != null)
            {
                List<TransactionInterface> TransList = new List<TransactionInterface>();
                TransList.AddRange(LookupDB.ListTransactions(account_number));
                ViewBag.account = LookupDB.getAccount(account_number, username);
                ViewBag.Transactions = TransList;
                return View();
            }
            else
            {
                return View("Denied");
            }
        }

        public IActionResult Transaction(string To, string amt, string From)
        {
            var session = Request.Cookies["SESSION_ID"];
            Database data = new Database();
            ProfileInterface Verified = data.VerifySession(session);
            //Check if Verified is a TellerProfile type object, this means the session is valid
            if (Verified?.profile_type != ProfileInterface.ProfileType.CUSTOMER && 
                Verified?.profile_type != ProfileInterface.ProfileType.TELLER &&
                Verified?.profile_type != ProfileInterface.ProfileType.ADMIN)
            {
                return View("Denied");
            }
            else
            {
                ViewBag.ResultDict = ((TellerProfile)Verified).AddAmount(Convert.ToInt32(ViewBag.To), Convert.ToDecimal(ViewBag.amt));
                return View();

            }
        }

        public IActionResult Transfer()
        {

            var session = Request.Cookies["SESSION_ID"];
            Database Test_Auth = new Database();
            ProfileInterface Verified = Test_Auth.VerifySession(session);
            //Check if Verified is a TellerProfile type object, this means the session is valid
            if (Verified?.profile_type != ProfileInterface.ProfileType.CUSTOMER &&
                Verified?.profile_type != ProfileInterface.ProfileType.TELLER &&
                Verified?.profile_type != ProfileInterface.ProfileType.ADMIN)
            {
                return View("Denied");
            }


             var my_interface = Test_Auth.VerifySession(session);

             var customer = Test_Auth.getCurrentCustomer(session);
            
            //Get the customners profile, and set a flag that we've searched.
            ViewBag.cust = customer;
            ViewBag.searched = "yes";
            if (Verified.profile_type != ProfileInterface.ProfileType.CUSTOMER)
            {
                CustomerProfile custprof = new CustomerProfile(customer);
                ViewBag.Accounts = custprof.ListAccounts();
            }
            else
            {

                ViewBag.Accounts = ((CustomerProfile)my_interface).ListAccounts();
            }


            if (Request.HasFormContentType)
            {

                if (!String.IsNullOrEmpty(Request.Form["username"]))
                {

                    ViewBag.cust = customer;
                    ViewBag.searched = "yes";
                    ViewBag.Accounts = ((CustomerProfile)my_interface).ListAccounts();
                }
                //At this point there should be an acctFrom in the title. this is just a sanity check
                //To make sure we don't run this on page load.
                if (!String.IsNullOrEmpty(Request.Form["acctTo"]))
                {
                    if (Verified.profile_type != ProfileInterface.ProfileType.CUSTOMER)
                    {
                        new CustomerProfile(customer).Transfer(Convert.ToInt32(Request.Form["acctTo"]),
                            Convert.ToInt32(Request.Form["acctFrom"]), Convert.ToDecimal(Request.Form["amount"]));
                    }
                    else
                    {
                        ((CustomerProfile) my_interface).Transfer(Convert.ToInt32(Request.Form["acctTo"]),
                            Convert.ToInt32(Request.Form["acctFrom"]), Convert.ToDecimal(Request.Form["amount"]));
                    }

                    ViewBag.To = Request.Form["acctTo"];
                    ViewBag.amt = Request.Form["amount"];
                    ViewBag.From = Request.Form["acctFrom"];
                    ViewBag.type = "Transfer";
                    return View("Transaction");

                }
            }

            ViewBag.User = my_interface.username;
            return View();
        }

        public IActionResult Bill()
        {
            CustomerProfile custprof = null;
            var session = Request.Cookies["SESSION_ID"];
            Database Test_Auth = new Database();
            ProfileInterface Verified = Test_Auth.VerifySession(session);
            //Check if Verified is a TellerProfile type object, this means the session is valid
            if (Verified?.profile_type != ProfileInterface.ProfileType.CUSTOMER &&
               Verified?.profile_type != ProfileInterface.ProfileType.TELLER &&
               Verified?.profile_type != ProfileInterface.ProfileType.ADMIN)
            {
                return View("Denied");
            }


            var my_interface = Test_Auth.VerifySession(session);

            var customer = Test_Auth.getCurrentCustomer(session);
            ViewBag.cust = customer;
            if (Verified.profile_type != ProfileInterface.ProfileType.CUSTOMER)
            {
                custprof = new CustomerProfile(customer);
                ViewBag.Accounts = custprof.ListAccounts();
            }
            else {
                
                ViewBag.Accounts = ((CustomerProfile)my_interface).ListAccounts();
            }
            ViewBag.searched = "yes";

            if (Request.HasFormContentType)
            {

                if (!String.IsNullOrEmpty(Request.Form["username"]))
                {

                    ViewBag.cust = customer;
                    ViewBag.searched = "yes";
                    if (Verified.profile_type != ProfileInterface.ProfileType.CUSTOMER)
                    {
                        custprof = new CustomerProfile(customer);
                        ViewBag.Accounts = custprof.ListAccounts();
                    }
                    else
                    {

                        ViewBag.Accounts = ((CustomerProfile)my_interface).ListAccounts();
                    }
                }
                //At this point there should be an acctFrom in the title. this is just a sanity check
                //To make sure we don't run this on page load.
                if (!String.IsNullOrEmpty(Request.Form["acctTo"]))
                {
                    if (my_interface.profile_type == ProfileInterface.ProfileType.CUSTOMER)
                    {
                        ((CustomerProfile) my_interface)?.Transfer(Convert.ToInt32(Request.Form["acctTo"]),
                            Convert.ToInt32(Request.Form["acctFrom"]), Convert.ToDecimal(Request.Form["amount"]));
                    }
                    else
                    {
                        custprof.Transfer(Convert.ToInt32(Request.Form["acctTo"]),
                            Convert.ToInt32(Request.Form["acctFrom"]), Convert.ToDecimal(Request.Form["amount"]));
                    }

                    ViewBag.To = Request.Form["acctTo"];
                    ViewBag.amt = Request.Form["amount"];
                    ViewBag.From = Request.Form["acctFrom"];
                    ViewBag.type = "Bill Pay";
                    return View("Transaction");

                }
            }

            ViewBag.User = my_interface.username;
            return View();
        }
    }
}