using System;
using System.Web.Mvc;
using org.SharpTiles.Connectors;

namespace SharpTilesSample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Title"] = "Home Page";
            ViewData["Message"] = "Welcome to ASP.NET MVC with SharpTiles!";
            return View();
        }

        public ActionResult About()
        {
            ViewData["Title"] = "About Page";

            return View();
        }
        
    }
}