using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ActionsAndUrls;
using CustomTaglibsTagsAndFunctions.Controllers;
using org.SharpTiles.Common;
using org.SharpTiles.Connectors;
using org.SharpTiles.Expressions.Functions;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.FormatTags;

namespace CustomTaglibsTagsAndFunctions
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
            FunctionLib.Register(new MathLib());
            TagLib.Register(new MyTagLib());
            var engine = new TilesViewEngine();
            ViewEngines.Engines.Add(engine.Init());
            engine.LoadResourceBundle("CustomTaglibsTagsAndFunctions.Views.default"); 
        }
    }

    
}