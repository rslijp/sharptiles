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
using Castle.MonoRail.Framework;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Tiles;

namespace org.SharpTiles.Connectors.Monorail
{
    public class TilesView
    {
        private readonly string _viewName;
        private readonly IViewCache _cache;
        public bool HttpErrors { get; set; }

        public TilesView(string viewName, IViewCache cache)
            : this(viewName, cache, false)
        {
        }

        public TilesView(string viewName, IViewCache cache, bool httpErrors)
        {
            _viewName = viewName;
            _cache = cache;
            HttpErrors = httpErrors;
        }

        public void Render(IRailsEngineContext railsEngineContext, TextWriter writer)
        {
            var contextWrapper = new HttpContextWrapper(railsEngineContext.UnderlyingContext);

            try
            {
                var tile = _cache.GetView(_viewName);
                RenderTile(railsEngineContext, contextWrapper, writer, tile);
            }
            catch (Exception e)
            {
                HandleErrors(contextWrapper, e);
            }
        }

        private void RenderTile(IRailsEngineContext railsEngineContext, HttpContextBase context, TextWriter writer,
                                ITile tile)
        {
            var model = railsEngineContext.CurrentController.PropertyBag;
            if (railsEngineContext.LastException != null) model["Exception"] = railsEngineContext.LastException;

            var request = context.Request;
            var tagModel = new TagModel(model,
                                        new SimpleHttpSessionState(),
                                        new HttpRequestBaseWrapper(context.Request),
                                        new HttpContextBaseResponseWrapper(context, request.ApplicationPath),
                                        _cache.Factory).UpdateFactory(_cache.Factory);

            tagModel.Page[TagModel.PAGE_MODEL_PRINCIPAL_INSTANCE] = context.User;
            writer.Write(tile.Render(tagModel));
        }

        private void HandleErrors(HttpContextBase context, Exception e)
        {
            if (!HttpErrors) throw e;
            var errorCode = default(int?);
            if (e is IHaveHttpErrorCode)
            {
                errorCode = ((IHaveHttpErrorCode) e).HttpErrorCode;
            }
            context.Response.StatusCode = (errorCode ?? 500);
        }
    }
}