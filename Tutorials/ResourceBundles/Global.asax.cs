using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using org.SharpTiles.Common;
using org.SharpTiles.Connectors;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.FormatTags;
using ResourceBundles.Controllers;

namespace ResourceBundles
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);
            ViewEngines.Engines.Clear();
            var engine = new TilesViewEngine();
            ViewEngines.Engines.Add(engine.Init());
            //TagModel.GlobalModel[FormatConstants.BUNDLE] = new ResourceBundle("Views\\default", null, new VirtualDirFileLocator());
            engine.LoadResourceBundle("ResourceBundles.Views.default");
        }
    }
}