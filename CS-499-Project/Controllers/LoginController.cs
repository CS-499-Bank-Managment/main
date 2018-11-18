using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CS_499_Project.Object_Classes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace CS_499_Project.Controllers
{
    public class LoginController : Controller
    {
        // GET
        public IActionResult Index()
        {
            if (!String.IsNullOrEmpty(Request.Cookies["SESSION_ID"]))
            {
                Database Redirect = new Database();
                var session = Redirect.VerifySession(Request.Cookies["SESSION_ID"]);
                if (session != null)
                {
                    var type = session.profile_type;
                    if (type == ProfileInterface.ProfileType.ADMIN)
                    {
                        ViewBag.redirect = "../Admin/";
                    }

                    if (type == ProfileInterface.ProfileType.TELLER)
                    {
                        ViewBag.redirect = "../Teller/";
                    }

                    if (type == ProfileInterface.ProfileType.CUSTOMER)
                    {
                        ViewBag.redirect = "../User/Dashboard";
                    }
                }
            }
            ViewBag.Form = false;
            if (Request.HasFormContentType)
            {
                ViewBag.Form = true;
                string User = Request.Form["username"];
                string Password = Request.Form["password"];
                string role = "customer"; // because this is the customer login page, we can assume it's a customer
                ViewBag.User = User;
                ViewBag.Pass = Password;

                var LoginDB = new Database();
                var sessionID = Request.Cookies["SESSION_ID"] != null;
                try
                {
                    var session_id = LoginDB.Login(User, Password, role, sessionID);
                    if (session_id != null)
                    {
                        ViewBag.Status = "Yes";
                        Response.Cookies.Append("SESSION_ID", session_id);
                        ViewBag.redirect = LoginController.RoleIndex(role);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    ViewBag.errorMessage = "Invalid Credentials. Try Again";
                    ViewBag.Form = false;
                    return View();
                }
            }

            //https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication?view=aspnetcore-2.1
            //Read this later future me
           
            return View();
        }

        public IActionResult Employee()
        {
            if (!String.IsNullOrEmpty(Request.Cookies["SESSION_ID"]))
            {
                Database Redirect = new Database();
                var session = Redirect.VerifySession(Request.Cookies["SESSION_ID"]);
                if (session != null)
                {
                    var type = session.profile_type;
                    if (type == ProfileInterface.ProfileType.ADMIN)
                    {
                        ViewBag.redirect = "../../Admin/";
                    }

                    if (type == ProfileInterface.ProfileType.TELLER)
                    {
                        ViewBag.redirect = "../../Teller/";
                    }
                }
            }
            ViewBag.Form = false;
            if (Request.HasFormContentType)
            {
                ViewBag.Form = true;
                string User = Request.Form["username"];
                string Password = Request.Form["password"];
                string role = "teller"; // check if this person is a teller
                ViewBag.User = User;
                ViewBag.Pass = Password;

                var LoginDB = new Database();
                var sessionID = Request.Cookies["SESSION_ID"] != null;
                try
                {
                    var session_id = LoginDB.Login(User, Password, role, sessionID);
                    if (session_id != null)
                    {
                        ViewBag.Status = "Yes";
                        Response.Cookies.Append("SESSION_ID", session_id);
                        ViewBag.redirect = LoginController.RoleIndex(role);
                    }
                }
                catch(UnauthorizedAccessException)
                {
                    role = "admin";
                }
                try
                {
                    var session_id = LoginDB.Login(User, Password, role, sessionID);
                    if (session_id != null)
                    {
                        ViewBag.Status = "Yes";
                        Response.Cookies.Append("SESSION_ID", session_id);
                        ViewBag.redirect = LoginController.RoleIndex(role);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    ViewBag.errorMessage = "Invalid Credentials. Try Again";
                    ViewBag.Form = false;
                    return View();
                }
            }

            //https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication?view=aspnetcore-2.1
            //Read this later future me

            return View();
        }

        public static string RoleIndex(string role)
        {
            //Function that returns the index page for the specified role.
            switch (role.ToLower())
            {
                case "admin":
                    return "../../Admin/";
                case "teller":
                    return "../../Teller/";
                case "customer":
                    return "./User/";
            }

            return "";
        }
        public IActionResult Logout()
        {
            var session = Request.Cookies["SESSION_ID"];
            Database Logout_DB = new Database();
            Logout_DB.Logout(session);
            Response.Cookies.Delete("SESSION_ID");
            return View();



        }
    }
    
}