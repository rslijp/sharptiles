using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.FormatTags;

namespace ResourceBundles.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }


        public ActionResult Bundle()
        {
            var keys = new List<String>();
            var bundle = (ResourceBundle)TagModel.GlobalModel[FormatConstants.BUNDLE];
            var enumerator = bundle.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, false, true).GetEnumerator();
            while (enumerator.MoveNext())
            {
                keys.Add(enumerator.Key.ToString());
            }
            ViewData["Keys"] = keys;
            return View();
        }
    }
}
