using Microsoft.AspNetCore.Mvc;


namespace Authorization_Server.Controllers
{
 
    public class HomeController : Controller
    {

        public IActionResult Index()
        {

            return View();
        }


    }
}
