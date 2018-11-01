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
                var type = Redirect.VerifySession(Request.Cookies["SESSION_ID"]).profile_type;
                if (type == ProfileInterface.ProfileType.ADMIN)
                {
                    ViewBag.redirect = "./Admin/";
                }

                if (type == ProfileInterface.ProfileType.TELLER)
                {
                    ViewBag.redirect = "./Teller/";
                }

                if (type == ProfileInterface.ProfileType.CUSTOMER)
                {
                    ViewBag.redirect = "./User/";
                }
            }
            ViewBag.Form = false;
            if (Request.HasFormContentType)
            {
                ViewBag.Form = true;
                string User = Request.Form["username"];
                string Password = Request.Form["password"];
                string role = Request.Form["role"];
                ViewBag.User = User;
                ViewBag.Pass = Password;

                var LoginDB = new Database();
                var sessionID = Request.Cookies["SESSION_ID"] != null;
                var session_id = LoginDB.Login(User, Password, role, sessionID);
                if (session_id != null)
                {
                    ViewBag.Status = "Yes";
                    Response.Cookies.Append("SESSION_ID", session_id);
                    ViewBag.redirect = LoginController.RoleIndex(role);
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
                    return "./Admin/";
                case "teller":
                    return "./Teller/";
                case "customer":
                    return "./Customer/";
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