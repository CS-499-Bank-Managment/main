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

        public IActionResult AccountCreated(string username, string password, string confirm, string role)
        {
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
            foo.CreateProfile(username, password, role);
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
            AdminProfile foo = new AdminProfile();
            ViewBag.deleting = username;
            foo.DeleteProfile(username);
            ViewBag.status = foo.Check(username);
                
            
            return View();
        }

        public IActionResult Mongo()
        {
            List<string> results = new List<string>();
            Database test = new Database();
            results = test.Login("Clay", "foo", "admin");
            ViewBag.results = results;
            foreach (var item in results)
            {
                Console.WriteLine(item);
            }
            return View();
        }
    }
}
