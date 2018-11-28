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
/*
 *
 * At the top of every method you will see some Request.Cookies["SESSION_ID"],
 * VerifySession(CookieValue)
 * ...
 * ((<role>Profile).Method());
 *
 * This Verifies the user is of the current type, and if it cannot be it returns the Denied View.
 * This comment block servers as an explanation once, instead of every time.
 * 
 */
namespace CS_499_Project.Controllers
{
    public class AdminController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return RedirectToAction("Dashboard", "Teller");
        }



        public IActionResult CreateAccountForm()
        {
            var database = new Database();
            var current_user = database.VerifySession(Request.Cookies["SESSION_ID"]);
            if (current_user?.profile_type != ProfileInterface.ProfileType.ADMIN)
            {
                return View("Denied");
            }

            ViewBag.owner = Request.Query["user"];
            List<string> results = new List<string>();
            results = database.GetCustomers();
            ViewBag.users = results;
            return View();
        }

        public IActionResult CreateAccountConfirmation(string username, string name, decimal deposit, int type, decimal interest)
        {
            var current_user = new Database().VerifySession(Request.Cookies["SESSION_ID"]);
            if (current_user?.profile_type != ProfileInterface.ProfileType.ADMIN)
            {
                return View("Denied");
            }

            try
            {
                if (((AdminProfile) current_user).CreateCustAccount(username, deposit, type, name, interest))
                {
                    // What is this hoping to accomplish? results has no contents. 
                    ViewBag.acct_user = username;
                    ViewBag.acct_dep = deposit;
                    ViewBag.acct_type = type;
                    ViewBag.acct_name = name;
                }
            }
            catch (SQLiteException) {
                ViewBag.errorMessage = "Unable to create account";
            }
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
            //View to present a form to create a new profile.
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

                try
                {
                    //Call the create profile method
                    if (((AdminProfile) current_user).CreateProfile(User, Password, role, name, email))
                    {
                        ViewBag.User = User;
                        ViewBag.role = role;
                        ViewBag.name = name;
                        ViewBag.email = email;
                        return View("CreateProfileConfirmation");
                    }
                        ViewBag.errorMessage = "COULD NOT CREATE USER";
                }
                catch (SQLiteException)
                {
                    ViewBag.errorMessage = "USERNAME ALREADY EXISTS";
                    return View();
                }
            }


            return View();
        }
     
        
        public IActionResult DeleteUser(string username)
        {
            var current_user = new Database().VerifySession(Request.Cookies["SESSION_ID"]);
            if (current_user?.profile_type != ProfileInterface.ProfileType.ADMIN)
            {
                return View("Denied");
            }
            //Create basic admin profile. and call it's Delete Profile method.
            ((AdminProfile)current_user).DeleteProfile(username);
            return View("../Teller/Dashboard");
        }

        public IActionResult DeleteAccount(string username, int account_id)
        {
            (new Database()).DeleteCustAcct(username, account_id);
            return RedirectToAction("Dashboard", "User");
        }
            

    }
}
