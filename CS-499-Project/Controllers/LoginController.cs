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
        [HttpGet]
        public IActionResult Index()
        {
            string User = Request.Query["username"];
            string Password = Request.Query["password"];
            string role = Request.Query["role"];
            string session_id;
            ViewBag.User = User;
            ViewBag.Pass = Password;

            var LoginDB = new Database();
            var sessionID = Request.Cookies["SESSION_ID"] != null;
            session_id = LoginDB.Login(User, Password, role, sessionID);
            if (session_id != null)
            {
                ViewBag.Status = "Yes";
                Response.Cookies.Append("SESSION_ID", session_id);
            }
            
            //https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication?view=aspnetcore-2.1
            //Read this later future me
            
            return View();
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