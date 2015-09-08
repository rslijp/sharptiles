using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.FormatTags;

namespace CustomTaglibsTagsAndFunctions.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";
            System.Web.HttpContext.Current.Session["AMessage"] = "A message stored in the session";
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

    }
}
