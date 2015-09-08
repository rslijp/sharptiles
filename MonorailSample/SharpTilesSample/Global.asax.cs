using System;
using System.Web;
using org.SharpTiles.Connectors.Monorail;

namespace SharpTilesSample
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            new TilesViewEngine().Init();
        }
    }
}