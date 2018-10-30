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
            string session_id;
            ViewBag.User = User;
            ViewBag.Pass = Password;

            var LoginDB = new Database();
            if (LoginDB.Login(User, Database.PasswordHash(Password), "teller") != null)
            {
                ViewBag.Status = "Yes";
                using (SHA256 SessionAlgorithm = SHA256.Create())
                {
                    byte[] Hash_Bytes = SessionAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(User + "Salt"));
                    StringBuilder Hash_Builder = new StringBuilder();
                    for (int i = 0; i < Hash_Bytes.Length; i++)
                    {
                        Hash_Builder.Append(Hash_Bytes[i].ToString("x2"));
                    }

                    session_id = Hash_Builder.ToString();
                    ViewBag.Sess = session_id;
                }
                LoginDB.LogSessionID(session_id, User, "teller");
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
            
            Response.Cookies.Delete("SESSION_ID");
            return View();



        }
    }
    
}