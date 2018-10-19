using CS_499_Project.Object_Classes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CS_499_Project.Controllers
{
    public class LoginController : Controller
    {
        // GET
        public IActionResult Index()
        {
            string User = Request.Query["username"];
            string Password = Request.Query["password"];
            
            //https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication?view=aspnetcore-2.1
            //Read this later future me
            
            return View();
        }
    }
}