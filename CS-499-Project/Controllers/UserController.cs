using CS_499_Project.Object_Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Server.Kestrel.Core;

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
            var session = Request.Cookies["SESSION_ID"];
            ProfileInterface Verified = new Database().VerifySession(session);
            var username = Verified.username;
            ViewBag.Title = "Account Dashboard";
            ViewBag.user_header = username;
            var number = account_number;
            var customer_served = new Database().getCurrentCustomer(session);
            AccountInterface account = (new Database()).getAccount(number, customer_served);
            ViewBag.account = account;
            switch (Verified.profile_type)
            {
                case ProfileInterface.ProfileType.ADMIN: ViewBag.user_role = "Admin"; break;
                case ProfileInterface.ProfileType.TELLER: ViewBag.user_role = "Teller"; break;
                case ProfileInterface.ProfileType.CUSTOMER: ViewBag.user_role = "User"; break;
            }
            List<TransactionInterface> transactions = new Database().ListTransactions(account.accountNumber());
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
            ProfileInterface Verified = new Database().VerifySession(session);
            var username = Verified.username;
            ViewBag.Title = "Customer Dashboard";
            ViewBag.user_header = username;
            var customer_served = new Database().getCurrentCustomer(session);
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

            ViewBag.accounts = new Database().CustomerAcctList(customer_served);
            ViewBag.full_name = Verified.full_name;
            foreach (AccountInterface account in ViewBag.accounts)
            {
                List<TransactionInterface> transactions = new Database().ListTransactions(account.accountNumber());
                int count = 0;
                foreach(TransactionInterface transaction in transactions)
                {
                    if (count < 5)
                    {
                        account.addTransaction(transaction);
                        count++;
                    }
                }
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
    }
}