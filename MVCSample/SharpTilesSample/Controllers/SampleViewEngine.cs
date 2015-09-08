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
using System.Web.Mvc;

namespace SharpTilesSample.Controllers
{
    public class SampleViewEngine : IViewEngine
    {
        #region IViewEngine Members

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName)
        {
            return FindView(controllerContext, partialViewName, null);
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName)
        {
            return new ViewEngineResult(
                new TilesView(viewName),
                this
                );
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
        }

        #endregion

        public ViewResult RendeTilesView(Controller controller)
        {
            return new ViewResult
                       {
                           ViewData = controller.ViewData,
                           ViewEngine = this
                       };
        }

        public ViewResult RendeTilesView(Controller controller, object model)
        {
            return new ViewResult
                       {
                           ViewData = new ViewDataDictionary(model),
                           ViewEngine = this
                       };
        }
    }
}