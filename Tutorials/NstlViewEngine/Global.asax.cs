﻿using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using org.SharpTiles.Tags;
using org.SharpTiles.Tiles.Tags;

namespace NstlViewEngine
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new {controller = "Home", action = "Index", id = ""} // Parameter defaults
                );
        }

        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);
            ViewEngines.Engines.Clear();
            TagLib.Register(new Tiles());
            ViewEngines.Engines.Add((IViewEngine) new org.SharpTiles.Connectors.NstlViewEngine().Init());
        }
    }
}