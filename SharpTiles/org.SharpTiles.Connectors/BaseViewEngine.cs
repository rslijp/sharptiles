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
using System.Configuration;
using System.Reflection;
using System.Web.Mvc;
using org.SharpTiles.HtmlTags;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.FormatTags;

namespace org.SharpTiles.Connectors
{

    public class BaseViewEngine<T> : IViewEngine where T : IViewCache, new()
    {
        private const string ROUTEDATE_KEY_CONTROLLER = "controller";
        public const string UNDEFINED = "undefined";

        public static bool UseHttpErrors { get; set; }
        public static IViewCache Cache { get; set; }
        private static readonly object _cacheLock = new object();
        
        #region IViewEngine Members

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName,
                                                bool useCache)
        {
            return FindView(controllerContext, partialViewName, null, useCache);
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName,
                                         bool useCache)
        {
            var view = new TilesView(viewName, Cache, UseHttpErrors);
            string longName = controllerContext != null ? GetTilesViewName(controllerContext, viewName) : null;
            if (longName != null && Cache.HasView(longName))
            {
                view = new TilesView(longName, Cache, UseHttpErrors);
            }
            return new ViewEngineResult(
                view,
                this
                );
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
        }

        #endregion

        public IViewEngine Init()
        {
            GuardInit(Assembly.GetCallingAssembly());
            return this;
        }

        public static void GuardInit(Assembly assembly)
        {
            lock (_cacheLock)
            {
                if (Cache != null) return;
                Cache = new T().GuardInit(assembly);
                if (!Cache.IsRegisterd(Html.HTML_GROUP_NAME)) Cache.Register(new Html());
                if (!Cache.IsRegisterd(Tiles.Tags.Tiles.TILES_GROUP_NAME)) Cache.Register(new Tiles.Tags.Tiles());
            }
        }


        public void LoadResourceBundle(string path)
        {
            TagModel.GlobalModel[FormatConstants.BUNDLE] = new ResourceBundle(path, "", Cache.Factory.GetNewLocator());
        }

        public string GetTilesViewName(ControllerContext controller, object viewName)
        {
            try
            {
                var controllerStr = controller.RouteData.Values[ROUTEDATE_KEY_CONTROLLER];
                return string.Format("{0}{1}{2}", controllerStr, Cache.PathSeperator, viewName);
            }
            catch
            {
                return UNDEFINED;
            }
        }
    }
}