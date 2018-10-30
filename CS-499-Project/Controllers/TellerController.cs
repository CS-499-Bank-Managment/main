using System;
using CS_499_Project.Object_Classes;
using Microsoft.AspNetCore.Mvc;

namespace CS_499_Project.Controllers
{
    public class TellerController : Controller
    {
        // GET

        public IActionResult Index()
        {
            var session = Request.Cookies["SESSION_ID"];
            ProfileInterface Verified = new Database().VerifySession(session);
            //Check if Verified is a TellerProfile type object, this means the session is valid
            if (Verified is TellerProfile)
            {
                ViewBag.User = Verified.username;
                return View();
            }

            return View("Denied");
        }
        public IActionResult Transaction(string acct_to, string amount)
        {
            TellerProfile foo = new TellerProfile();
            ViewBag.ResultDict = foo.AddAmount(Convert.ToInt32(acct_to), Convert.ToDecimal(amount) );
            return View();
        }

        public IActionResult Between(string acct_to, string acct_from, string amount)
        {
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
            Console.WriteLine("Calling Verify with parameter " + session);
            var my_interface = Test_Auth.VerifySession(session);

            if (Request.HasFormContentType )
            {
                //At this point there should be an acctFrom in the title. this is just a sanity check
                //To make sure we don't run this on page load.
                ((TellerProfile)my_interface)?.Transfer( Convert.ToInt32(Request.Form["acctTo"]),
                    Convert.ToInt32(Request.Form["acctFrom"]), Convert.ToDecimal(Request.Form["amount"]));
            } 
            
            ViewBag.User = my_interface.username;
            ViewBag.Accounts = ((CustomerProfile) my_interface).ListAccounts();
            return View();
        }
    }
}