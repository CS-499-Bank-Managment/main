using System;
using CS_499_Project.Object_Classes;
using Microsoft.AspNetCore.Mvc;

namespace CS_499_Project.Controllers
{
    public class TellerController : Controller
    {
        // GET
        public IActionResult Transaction(string acct_to, string amount)
        {
            TellerProfile foo = new TellerProfile();
            foo.AddAmount(Convert.ToInt32(acct_to), Convert.ToDecimal(amount) );
            return View();
        }

        public IActionResult Between(string acct_to, string acct_from, string amount)
        {
            TellerProfile foo = new TellerProfile();
            ViewBag.ResultDict = foo.Transfer(Convert.ToInt32(acct_to), 
                Convert.ToInt32(acct_from), Convert.ToDecimal(amount));
            return View("Transaction");
        }
        
        
    }
}