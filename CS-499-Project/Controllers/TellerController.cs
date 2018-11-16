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
            ProfileInterface Verified = new Database().VerifySession(session);
            if (Verified.profile_type == ProfileInterface.ProfileType.CUSTOMER)
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
            if (Verified.profile_type != ProfileInterface.ProfileType.TELLER)
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

        public IActionResult Transaction(string acct_to, string amount)
        {
            var session = Request.Cookies["SESSION_ID"];
            ProfileInterface Verified = new Database().VerifySession(session);
            //Check if Verified is a TellerProfile type object, this means the session is valid
            if (Verified.profile_type != ProfileInterface.ProfileType.TELLER)
            {
                return View("Denied");
            }
            ViewBag.To = acct_to;
            ViewBag.amt = amount;
            ViewBag.ResultDict = ((TellerProfile)Verified).AddAmount(Convert.ToInt32(acct_to), Convert.ToDecimal(amount) );
            return View();
        }

        public IActionResult Between(string acct_to, string acct_from, string amount)
        {
            var session = Request.Cookies["SESSION_ID"];
            ProfileInterface Verified = new Database().VerifySession(session);
            //Check if Verified is a TellerProfile type object, this means the session is valid
            if (Verified.profile_type != ProfileInterface.ProfileType.TELLER)
            {
                return View("Denied");
            }
            TellerProfile foo = new TellerProfile();
            ViewBag.ResultDict = foo.Transfer(Convert.ToInt32(acct_to), 
                Convert.ToInt32(acct_from), Convert.ToDecimal(amount));
            return View("Transaction");
        }
        
        //Route for the Transfer Page site/Teller/Transfer
        public IActionResult Transfer()
        {

            var session = Request.Cookies["SESSION_ID"].Trim();
            ViewBag.Sess = session;

            Database Test_Auth = new Database();
            var my_interface = Test_Auth.VerifySession(session);

            if (!String.IsNullOrEmpty(Request.Query["username"]))
            {
                var customer = Request.Query["username"].ToString().Trim();
                ViewBag.cust = customer;
                ViewBag.searched = "yes";
                ViewBag.Accounts = ((TellerProfile)my_interface).ListAccounts(customer);
            }

            if (Request.HasFormContentType )
            {

                 if (!String.IsNullOrEmpty(Request.Form["username"]))
                 {
                    var customer = Request.Form["username"].ToString().Trim();
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
                }
            } 
            
            ViewBag.User = my_interface.username;
            return View();
        }
    }
}