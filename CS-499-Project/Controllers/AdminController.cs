using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CS_499_Project.Object_Classes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult Create(string username, string password, string role)
        {
            AdminProfile foo = new AdminProfile();
            ViewBag.username = username;
            ViewBag.password = password;
            ViewBag.role = role;
            foo.CreateProfile(username, password, role);
            return View();
        }
    }
}
