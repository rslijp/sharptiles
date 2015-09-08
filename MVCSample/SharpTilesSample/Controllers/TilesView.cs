using System.IO;
using System.Web;
using System.Web.Mvc;
using org.SharpTiles.Connectors;
using org.SharpTiles.Tags;
using org.SharpTiles.Tiles;

namespace SharpTilesSample.Controllers
{
    public class TilesView : IView
    {
        private readonly string _viewName;

        public TilesView(string viewName)
        {
            _viewName = viewName;
        }

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            HttpContextBase context = viewContext.HttpContext;
            ViewDataDictionary model = viewContext.ViewData;
            HttpRequestBase request = context.Request;
            ITile tile = GetView(_viewName);
            var tagModel = new TagModel(model.Model ?? model,
                                        new SimpleHttpSessionState(),
                                        new HttpContextBaseResponseWrapper(context, request.ApplicationPath));
            tagModel.Page["request"] = request.Params;
            writer.Write(tile.Render(tagModel));
        }

        public static ITile GetView(string view)
        {
            return TilesCache.GetView(view);
        }
    }
}