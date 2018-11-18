using System;
using CS_499_Project.Object_Classes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CS_499_Project.Controllers
{
    public class TellerController : Controller
    {
        // GET

        public IActionResult Dashboard()
        {
            ViewBag.Title = "Dashboard";
            if (Request.Path.Value.ToString().ToLower().EndsWith('/'))
            {
                ViewBag.path = ".";
            }
            else
            {
                ViewBag.path = "./Teller";
            }
            var type = "";
            var session = Request.Cookies["SESSION_ID"];
            ProfileInterface Verified = (new Database()).VerifySession(session);
            if(Verified == null)
            {
                return View("Denied");
            }
            else if (Verified.profile_type == ProfileInterface.ProfileType.CUSTOMER)
            {
                return View("Denied");
            }
            else if (Verified.profile_type == ProfileInterface.ProfileType.TELLER)
            {
                type = "Teller";
                ViewBag.user_role = "Teller";
            }
            else if (Verified.profile_type == ProfileInterface.ProfileType.ADMIN)
            {
                type = "Admin";
                ViewBag.user_role = "Admin";
            }
            else
            {
                return View("Denied");
            }
            if(ViewBag.set_username != "true")
            {
                ViewBag.LS = "";
            }
            ViewBag.User = Verified.username;
            ViewBag.user_header = Verified.username;
            if (Request.HasFormContentType)
            {
                ViewBag.LS = Request.Form["username"];
                ViewBag.set_username = "true";
                var lookup_db = new Database();
                ViewBag.info = lookup_db.CustomerLookup(Request.Form["username"], type);
            }
            return View();
        }

        public IActionResult Denied()
        {
            ViewBag.title = "Access Denied";
            return View();
        }

        public IActionResult ListAccounts()
        {
            var session = Request.Cookies["SESSION_ID"];
            ProfileInterface Verified = new Database().VerifySession(session);
            //Check if Verified is a TellerProfile type object, this means the session is valid
            if (Verified?.profile_type != ProfileInterface.ProfileType.TELLER)
            {
                return View("Denied");
            }
            List<Dictionary<string, string>> acct_list = new List<Dictionary<string, string>>();
            var userid = Request.Query["username"];
            ViewBag.lookup = userid;
            var Account_lookup = new Database();
            Console.WriteLine(userid);
             var cust_accounts = Account_lookup.CustomerAcctList(userid);
            foreach(AccountInterface account in cust_accounts)
            {
                acct_list.Add(account.toDict());
            }
            ViewBag.accounts = acct_list;
            return View();
        }

        public IActionResult Edit()
        {
            var session = Request.Cookies["SESSION_ID"];
            ProfileInterface Verified = new Database().VerifySession(session);
            //Check if Verified is a TellerProfile type object, this means the session is valid
            if (Verified.profile_type != ProfileInterface.ProfileType.TELLER)
            {
                return View("Denied");
            }
            var acct = Request.Query["acct"];
            ViewBag.num = acct;
            ViewBag.transactions = (new Database()).ListTransactions(Convert.ToInt32(acct));
            return View();
        }

        public IActionResult Transaction(string To, string amt, string From)
        {
            var session = Request.Cookies["SESSION_ID"];
            Database data = new Database();
            ProfileInterface Verified = data.VerifySession(session);
            //Check if Verified is a TellerProfile type object, this means the session is valid
            if (Verified?.profile_type != ProfileInterface.ProfileType.TELLER)
            {
                return View("Denied");
            }
            else
            {
                ViewBag.ResultDict = ((TellerProfile)Verified).AddAmount(Convert.ToInt32(ViewBag.To), Convert.ToDecimal(ViewBag.amt));
                return View();

            }
        }
        
        //Route for the Transfer Page site/Teller/Transfer
        public IActionResult Transfer()
        {

            var session = Request.Cookies["SESSION_ID"];
            Database Test_Auth = new Database();
            ProfileInterface Verified = Test_Auth.VerifySession(session);
            //Check if Verified is a TellerProfile type object, this means the session is valid
            if (Verified?.profile_type != ProfileInterface.ProfileType.TELLER)
            {
                return View("Denied");
            }


            var my_interface = Test_Auth.VerifySession(session);

            // TEST PURPOSES ONLY - REMOVE
            Test_Auth.setCurrentCustomer("clay", session);

            var customer = Test_Auth.getCurrentCustomer(session);
            if (customer == my_interface.username)
            {
                return RedirectToAction("Dashboard", "Teller"); // if you're not helping someone, go back to dashboard
            }
            ViewBag.cust = customer;
            ViewBag.searched = "yes";
            ViewBag.Accounts = ((TellerProfile)my_interface).ListAccounts(customer);
                

            if (Request.HasFormContentType )
            {

                 if (!String.IsNullOrEmpty(Request.Form["username"]))
                 {
                    
                    ViewBag.cust = customer;
                    ViewBag.searched = "yes";
                    ViewBag.Accounts = ((TellerProfile)my_interface).ListAccounts(customer);
                 }
                //At this point there should be an acctFrom in the title. this is just a sanity check
                //To make sure we don't run this on page load.
                if (!String.IsNullOrEmpty(Request.Form["acctTo"]))
                {
                    ((TellerProfile)my_interface)?.Transfer(Convert.ToInt32(Request.Form["acctTo"]),
                        Convert.ToInt32(Request.Form["acctFrom"]), Convert.ToDecimal(Request.Form["amount"]));
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

        public IActionResult Withdrawal()
        {
            var session = Request.Cookies["SESSION_ID"];
            Database Test_Auth = new Database();
            ProfileInterface Verified = Test_Auth.VerifySession(session);
            //Check if Verified is a TellerProfile type object, this means the session is valid
            if (Verified?.profile_type != ProfileInterface.ProfileType.TELLER)
            {
                return View("Denied");
            }


            var my_interface = Test_Auth.VerifySession(session);

            // TEST PURPOSES ONLY - REMOVE
            Test_Auth.setCurrentCustomer("clay", session);

            var customer = Test_Auth.getCurrentCustomer(session);
            if (customer == my_interface.username)
            {
                return RedirectToAction("Dashboard", "Teller"); // if you're not helping someone, go back to dashboard
            }
            ViewBag.cust = customer;
            ViewBag.searched = "yes";
            ViewBag.Accounts = ((TellerProfile)my_interface).ListAccounts(customer);


            if (Request.HasFormContentType)
            {

                if (!String.IsNullOrEmpty(Request.Form["username"]))
                {

                    ViewBag.cust = customer;
                    ViewBag.searched = "yes";
                    ViewBag.Accounts = ((TellerProfile)my_interface).ListAccounts(customer);
                }
                //At this point there should be an acctFrom in the title. this is just a sanity check
                //To make sure we don't run this on page load.
                if (!String.IsNullOrEmpty(Request.Form["acctFrom"]))
                {
                    ((TellerProfile)my_interface)?.Withdrawal(Convert.ToInt32(Request.Form["acctFrom"]), Convert.ToDecimal(Request.Form["amount"]));
                    ViewBag.amt = Request.Form["amount"];
                    ViewBag.From = Request.Form["acctFrom"];
                    ViewBag.To = null;
                    ViewBag.type = "Withdrawal";
                    return View("Transaction");

                }
            }

            ViewBag.User = my_interface.username;
            return View();
        }

        public IActionResult Deposit()
        {

            var session = Request.Cookies["SESSION_ID"];
            Database Test_Auth = new Database();
            ProfileInterface Verified = Test_Auth.VerifySession(session);
            //Check if Verified is a TellerProfile type object, this means the session is valid
            if (Verified?.profile_type != ProfileInterface.ProfileType.TELLER)
            {
                return View("Denied");
            }


            var my_interface = Test_Auth.VerifySession(session);

            // TEST PURPOSES ONLY - REMOVE
            Test_Auth.setCurrentCustomer("clay", session);

            var customer = Test_Auth.getCurrentCustomer(session);
            if (customer == my_interface.username)
            {
                return RedirectToAction("Dashboard", "Teller"); // if you're not helping someone, go back to dashboard
            }
            ViewBag.cust = customer;
            ViewBag.searched = "yes";
            ViewBag.Accounts = ((TellerProfile)my_interface).ListAccounts(customer);


            if (Request.HasFormContentType)
            {

                if (!String.IsNullOrEmpty(Request.Form["username"]))
                {

                    ViewBag.cust = customer;
                    ViewBag.searched = "yes";
                    ViewBag.Accounts = ((TellerProfile)my_interface).ListAccounts(customer);
                }
                //At this point there should be an acctFrom in the title. this is just a sanity check
                //To make sure we don't run this on page load.
                if (!String.IsNullOrEmpty(Request.Form["acctTo"]))
                {
                    ((TellerProfile)my_interface)?.Deposit(Convert.ToInt32(Request.Form["acctTo"]),
                        Convert.ToDecimal(Request.Form["amount"]));
                    ViewBag.To = Request.Form["acctTo"];
                    ViewBag.amt = Request.Form["amount"];
                    ViewBag.From = null;
                    ViewBag.type = "Deposit";
                    return View("Transaction");

                }
            }

            ViewBag.User = my_interface.username;
            return View();
        }
    }
}