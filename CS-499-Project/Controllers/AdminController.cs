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
        //Action method for creating an account
        public IActionResult Create(string username, string password, string role)
        {   
            //Create basic admin profile class - later we'll need to verify this with session info.
            AdminProfile foo = new AdminProfile();
            ViewBag.username = username;
            ViewBag.password = password;
            ViewBag.role = role;
            //Call the create profile method
            foo.CreateProfile(username, password, role);
            return View();
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
            
        //Method to create a customer account
        public IActionResult CustAcct(string username)
        {
            AdminProfile creation = new AdminProfile();
            ViewBag.results = creation.CreateCustAccount(username);
            return View("Mongo");
        }
        
        //Method to delete an account within a customer profile.
        public IActionResult DeleteCustAcct(string username, string acct_id)
        {
            var foo = new Database();
            foo.DeleteCustAcct(username, Convert.ToInt32(acct_id));
            return View("Index");
        }
    }
}
