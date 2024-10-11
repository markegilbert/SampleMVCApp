using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SampleApp.Config;
using SampleApp.Database;
using SampleApp.Models;
using System.Diagnostics;

namespace SampleApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
