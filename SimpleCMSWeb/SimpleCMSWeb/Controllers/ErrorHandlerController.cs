using Microsoft.AspNetCore.Mvc;

namespace SimpleCMSWeb.Controllers
{
    public class ErrorHandlerController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult BadGateWay()
        {
            return View();
        }

        public IActionResult Maintenance()
        {
            return View();
        }
    }
}
