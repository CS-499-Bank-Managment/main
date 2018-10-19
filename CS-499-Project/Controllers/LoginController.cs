using System.Collections.Generic;
using System.Net;
using CS_499_Project.Object_Classes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

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
            if (LoginDB.Login(User, Password, "admin") != null)
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
                LoginDB.LogSessionID(session_id, User, "admin");
                Response.Cookies.Append("SESSION_ID", session_id);
            }
            
            //https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication?view=aspnetcore-2.1
            //Read this later future me
            
            return View();
        }

        public IActionResult Test()
        {
            var session = Request.Cookies["SESSION_ID"].Trim();
            ViewBag.Sess = session;

            Database Test_Auth = new Database();
            var my_interface = Test_Auth.VerifySession(session);
            ViewBag.User = my_interface.username;

            return View("Index");
        }
    }
    
}