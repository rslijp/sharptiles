/*
 * SharpTiles, R.Z. Slijp(2008), www.sharptiles.org
 *
 * This file is part of SharpTiles.
 * 
 * SharpTiles is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * SharpTiles is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with SharpTiles.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using org.SharpTiles.Common;
using org.SharpTiles.HtmlTags;
using org.SharpTiles.Tags;
using org.SharpTiles.Tiles;

namespace org.SharpTiles.Connectors
{
    public class TilesView : IView
    {
        private readonly string _viewName;
        private readonly IViewCache _cache;

        public TilesView(string viewName, IViewCache cache) : this(viewName, cache, false)
        {
        }

        public TilesView(string viewName, IViewCache cache, bool httpErrors)
        {
            _viewName = viewName;
            _cache = cache;
            HttpErrors = httpErrors;
        }

        public bool HttpErrors { get; set; }

        #region IView Members

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            var context = viewContext.HttpContext;
            try
            {
                var tile = _cache.GetView(_viewName);
                RenderTile(viewContext, context, writer, tile);
            } catch (Exception e)
            {
                HanddleErrors(context, e);
            }
        }

        
        private void RenderTile(ViewContext viewContext, HttpContextBase context, TextWriter writer, ITile tile)
        {
            var model = viewContext.ViewData;
            var request = context.Request;
            var tagModel = new TagModel(model.Model ?? model,
                                        new SimpleHttpSessionState(),
                                        new HttpRequestBaseWrapper(request),
                                        new HttpContextBaseResponseWrapper(context, request.ApplicationPath)
                                        );

            tagModel.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = new HtmlHelper(viewContext,
                                                                                new SimpleViewDataContainer
                                                                                    {
                                                                                        ViewData = viewContext.ViewData
                                                                                    });
            tagModel.Page[Html.PAGE_MODEL_URLHELPER_INSTANCE] = new UrlHelper(viewContext.Controller.ControllerContext.RequestContext);
            tagModel.Page[TagModel.PAGE_MODEL_PRINCIPAL_INSTANCE] = viewContext.HttpContext.User;
            writer.Write(tile.Render(tagModel));
        }

        private void HanddleErrors(HttpContextBase context, Exception e)
        {
            if (!HttpErrors) throw e;
            var errorCode = default(int?);
            if(e is IHaveHttpErrorCode)
            {
                errorCode = ((IHaveHttpErrorCode) e).HttpErrorCode;
            }
            context.Response.StatusCode = (errorCode ?? 500);
        }

        #endregion

    }

    public class SimpleViewDataContainer : IViewDataContainer
    {
        #region IViewDataContainer Members

        public ViewDataDictionary ViewData { get; set; }

        #endregion
    }
}