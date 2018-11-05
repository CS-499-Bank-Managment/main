using CS_499_Project.Object_Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace CS_499_Project.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult AccountDashboard()
        {
            return View();
        }

        // GET
        public IActionResult Login(string username, string password)
        {
            
            int counter = 0;  
            string line;
            ViewBag.status = "no";

            // Read the file and display it line by line.  
            System.IO.StreamReader file =   
                new System.IO.StreamReader(@"./WriteLines.txt");

            while ((line = file.ReadLine()) != null)
            {
                string[] tempstr = line.Split(",");
                if (username == tempstr[0])
                {
                    if (password == tempstr[1])
                    {
                        string role = tempstr[2];
                        //Create a new object of role type.
                        if (role == "admin")
                        {
                            //do stuff.
                            ViewBag.status = "Admin";
                        }

                        if (role == "teller")
                        {
                            ViewBag.status = "Teller";
                        }

                        if (role == "customer")
                        {
                            ViewBag.status = "Customer";
                        }
                    }
                }
            }  

            return
            View();
        }
    }
}