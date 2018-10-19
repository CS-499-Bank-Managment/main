using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CS_499_Project.Object_Classes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CS_499_Project.Controllers
{
    public class AdminController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {

            return View();
        }

        public IActionResult ProfileList()
        {
            List<string> results = new List<string>();
            Database test = new Database();
            results = test.GetAllProfiles();
            ViewBag.profiles = results;
            return View();
        }


        public IActionResult AddAccount()
        {
            List<string> results = new List<string>();
            Database test = new Database();
            results = test.GetCustomers();
            ViewBag.users = results;
            return View();
        }

        public IActionResult NewAccount(string username, string name, decimal deposit, int type)
        {
            List<string> results = new List<string>();
            Database test = new Database();
            results = test.CreateCustAcct(username, deposit, type, name);
            ViewBag.acct_user = results[0];
            ViewBag.acct_num  = results[1];
            ViewBag.acct_dep  = results[2];
            ViewBag.acct_type = results[3];
            ViewBag.acct_name = results[4];
            return View();
        }

        //Action method for creating an account
        public IActionResult AccountCreated(string username, string password, string confirm, string role)
        {
            //Create basic admin profile class - later we'll need to verify this with session info.
            
            AdminProfile foo = new AdminProfile();
            if (username == null && ViewBag.username == null)
            {
                return View();
            }
            ViewBag.username = username;
            ViewBag.password = password;
            ViewBag.role = role;

            if(confirm != password)
            {
                ViewBag.confirm = "";
                throw (new System.FormatException("The Confirm password field does not match the password you entered!"));
            }

            //Call the create profile method
            foo.CreateProfile(username, password, role);
            return View();
        }

        public IActionResult myAccounts(string username)
        {
            Dictionary<long, string> results = new Dictionary<long, string>();
            Database test = new Database();
            results = test.GetMyAccounts(username);
            ViewBag.accounts = results;
            ViewBag.username = username;
            return View();
        }

        public IActionResult DeleteAccount(string username)
        {
            Database test = new Database();
            Dictionary<long, string> accounts = new Dictionary<long, string>();
            accounts = test.GetMyAccounts(username);
            ViewBag.accounts = accounts;
            ViewBag.username = username;
            return View();
        }

        public IActionResult AccountDeleted(string username, long acct_id)
        {
            ViewBag.username = username;
            ViewBag.acct_id = acct_id;
            Database test = new Database();
            test.DeleteCustAcct(username, (int)acct_id);
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public bool submitForm(string username, string password, string role)
        {
            AdminProfile foo = new AdminProfile();
            if (username == "" && ViewBag.username == null)
            {
                return false;
            }
            ViewBag.username = username;
            ViewBag.password = password;
            ViewBag.role = role;
            foo.CreateProfile(username, password, role);


            Index();
            return true;
        }

        public IActionResult Delete(string username)
        {
            //Create basic admin profile. and call it's Delete Profile method.
            AdminProfile foo = new AdminProfile();
            ViewBag.deleting = username;
            foo.DeleteProfile(username);
            ViewBag.status = foo.Check(username);
               
            return View();
        }
        
        //Test method to verify dbs
        public IActionResult Mongo(string username, string password, string role)
        {
            List<string> results = new List<string>();
            Database test = new Database();
            results = test.Login(username, password, role);
            ViewBag.results = results;
            foreach (var item in results)
            {
                Console.WriteLine(item);
            }
            return View();
        }
            
        //Commented because they aren't used and don't use real functions

        ////Method to create a customer account
        //public IActionResult CustAcct(string username)
        //{
        //    AdminProfile creation = new AdminProfile();
        //    ViewBag.results = creation.CreateCustAccount(username);
        //    return View("Mongo");
        //}
        
        ////Method to delete an account within a customer profile.
        //public IActionResult DeleteCustAcct(string username, string acct_id)
        //{
        //    var foo = new Database();
        //    foo.DeleteCustAcct(username, Convert.ToInt32(acct_id));
        //    return View("Index");
        //}
    }
}
