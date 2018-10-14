using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CS_499_Project.Controllers
{
    public class HelloController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        //You can set which method to work on by annotating the function with
        //[HttpGet] directly above the function, like this: 
	//Last test commit
        [HttpGet]
        public IActionResult Login(string username, string password)
        {
            ViewBag.Name = username;
            ViewBag.Pass = password;
            return View();
        }
    }
}
