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
 */using System.Web.Mvc;
using org.SharpTiles.Connectors;

namespace org.SharpTiles.ConnectorsTest
{
    public class TestController : Controller
    {
        public ActionResult Index()
        {
            ViewData["str"] = "c";
            return View();
        }

        public ActionResult AlternativeViewData()
        {
            return View(new Model());
        }

        public ActionResult AlternativeView()
        {
            ViewData["str"] = "c";
            return View("Alt");
        }

        public ActionResult AlternativeModel()
        {
            return View("Alt", new Model());
        }

        #region Nested type: Model

        private class Model
        {
            // ReSharper disable UnusedMemberInPrivateClass
            public string str
                // ReSharper restore UnusedMemberInPrivateClass
            {
                get { return "c"; }
            }
        }

        #endregion

    }
}
