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
            
            //This method shows the Default index page for the Admin Dashboard.
            ProfileInterface current_user = (new Database().VerifySession(Request.Cookies["SESSION_ID"]));
            ViewBag.username = current_user.username;
            ViewBag.role = current_user.profile_type;
            Console.WriteLine( current_user.GetType() == typeof(AdminProfile));
            if (current_user.GetType() != typeof(AdminProfile))
            {
                return View("Denied");
            }
            return View();
        }

        public IActionResult DeleteProfileForm()
        {
            var current_user = new Database().VerifySession(Request.Cookies["SESSION_ID"]);
            if (current_user.profile_type != ProfileInterface.ProfileType.ADMIN)
            {
                return View("Denied");
            }
            ViewBag.profiles = (new Database()).GetAllProfiles();
            return View();
        }


        public IActionResult CreateAccountForm()
        {
            var current_user = new Database().VerifySession(Request.Cookies["SESSION_ID"]);
            if (current_user.profile_type != ProfileInterface.ProfileType.ADMIN)
            {
                return View("Denied");
            }
            List<string> results = new List<string>();
            Database test = new Database();
            results = test.GetCustomers();
            ViewBag.users = results;
            return View();
        }

        public IActionResult CreateAccountConfirmation(string username, string name, decimal deposit, int type)
        {
            var current_user = new Database().VerifySession(Request.Cookies["SESSION_ID"]);
            if (current_user.profile_type != ProfileInterface.ProfileType.ADMIN)
            {
                return View("Denied");
            }
            ((AdminProfile)current_user).CreateCustAccount(username, deposit, type, name);
            List<string> results = new List<string>();
            // What is this hoping to accomplish? results has no contents. 
            ViewBag.acct_user = results[0];
            ViewBag.acct_num  = results[1];
            ViewBag.acct_dep  = results[2];
            ViewBag.acct_type = results[3];
            ViewBag.acct_name = results[4];
            return View();
        }

        //Action method for creating an account
        public IActionResult CreateProfileConfirmation(string username, string password, string confirm, string role)
        {
            //Create basic admin profile class - later we'll need to verify this with session info.

            var current_user = new Database().VerifySession(Request.Cookies["SESSION_ID"]);
            if (current_user?.profile_type != ProfileInterface.ProfileType.ADMIN)
            {
                return View("Denied");
            }

            if (username == null && ViewBag.username == null)
            {
                return View();
            }
            ViewBag.username = username;
            ViewBag.password = password;
            ViewBag.role = role;

            return View();
        }

        public IActionResult CreateProfileForm()
        {
            var current_user = new Database().VerifySession(Request.Cookies["SESSION_ID"]);
            if (current_user?.profile_type != ProfileInterface.ProfileType.ADMIN)
            {
                return View("Denied");
            }

            ViewBag.Form = false;
            if (Request.HasFormContentType)
            {
                ViewBag.Form = true;
                string User = Request.Form["username"];
                string Password = Request.Form["password"];
                string ConfirmPass = Request.Form["confirm"];
                string role = Request.Form["role"];
                string email = Request.Form["email"];
                string name = Request.Form["name"];
                ViewBag.User = User;
                ViewBag.Pass = Password;

                if (ConfirmPass != Password)
                {
                    ViewBag.errorMessage = "The passwords you entered do not match.";
                    return View();
                }

                //Call the create profile method
                if (((AdminProfile)current_user).CreateProfile(User, Password, role, name, email)){
                    return View("CreateProfileConfirmation(User, Password, role)");
                }
                else
                {
                    ViewBag.errorMessage = "COULD NOT CREATE USER";
                }
            }


            return View();
        }


        public IActionResult DeleteProfileConfirmation(string username)
        {
            var current_user = new Database().VerifySession(Request.Cookies["SESSION_ID"]);
            if (current_user?.profile_type != ProfileInterface.ProfileType.ADMIN)
            {
                return View("Denied");
            }
            //Create basic admin profile. and call it's Delete Profile method.
            ViewBag.deleting = username;
            ((AdminProfile)current_user).DeleteProfile(username);
            ViewBag.status = ((AdminProfile)current_user).Check(username);
               
            return View();
        }      
        
    }
}
