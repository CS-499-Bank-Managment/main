using CS_499_Project.Object_Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Collections.Generic;

namespace CS_499_Project.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            //temporary variable for customer
            var customer_served = "clay";
            //temporary variable for user
            var username = "clay2";
            //Commented for testing
            //var session = Request.Cookies["SESSION_ID"];
            //ProfileInterface Verified = new Database().VerifySession(session);
            ViewBag.Title = "Dashboard";
            ViewBag.user_header = username;
            ProfileInterface Verified = new AdminProfile(username, "Clay Turner");
            if(Verified.profile_type == ProfileInterface.ProfileType.TELLER || 
               Verified.profile_type == ProfileInterface.ProfileType.ADMIN)
            {
                ViewBag.allowed = true;
            }
            else
            {
                ViewBag.allowed = false;
            }

            switch (Verified.profile_type)
            {
                case ProfileInterface.ProfileType.ADMIN: ViewBag.user_role = "Admin"; break;
                case ProfileInterface.ProfileType.TELLER: ViewBag.user_role = "Teller"; break;
                case ProfileInterface.ProfileType.CUSTOMER: ViewBag.user_role = "customer"; break;
            }

            ViewBag.accounts = new Database().CustomerAcctList(customer_served);
            ViewBag.full_name = Verified.full_name;
            foreach (AccountInterface account in ViewBag.accounts)
            {
                List<Transaction> transactions = new Database().ListTransactions(account.accountNumber());
                foreach(Transaction transaction in transactions)
                {
                    account.addTransaction(transaction);
                }
            }
            return View();
        }
        
        // GET
        public IActionResult Login(string username, string password)
        {
            
            int counter = 0;  
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
    }
}