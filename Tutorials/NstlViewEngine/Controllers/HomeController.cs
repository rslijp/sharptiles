using System;
using System.Diagnostics;
using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;
using org.SharpTiles.Connectors;

namespace NstlViewEngine.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Debug.WriteLine("HOSTINGENVIRONMENT: " + HostingEnvironment.ApplicationPhysicalPath);
            Debug.WriteLine("MAP .: " + HttpContext.Request.MapPath("."));
            Debug.WriteLine("MAP /: " + HttpContext.Request.MapPath("/"));
            //var path = HostingEnvironment.ApplicationPhysicalPath;
            //var path = HttpContext.Current.Server.MapPath(".");
            ViewData["Message"] = "Welcome to ASP.NET MVC!";
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

    }
}