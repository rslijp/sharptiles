using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjaxWithTiles.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return this.RenderLayout();
        }


        public void RenderHome()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";
            this.Update("content", "Home.Home");
        }


        public ActionResult Home()
        {
            RenderHome();
            return this.Render();
        }

        public ActionResult About()
        {
            return this.Update("content").Render();
        }
    }
}
