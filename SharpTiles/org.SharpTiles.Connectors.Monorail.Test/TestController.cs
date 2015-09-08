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
using Castle.MonoRail.Framework;
using Castle.MonoRail.TestSupport;

namespace org.SharpTiles.Connectors.Monorail.Test
{
    [Rescue("Rescue")]
    public class TestController : SmartDispatcherController
    {
        public void Index()
        {
            RenderView("Index");
        }

        public void AlternativeView()
        {
            PropertyBag["str"] = "c";
            RenderView("Alt");
        }

        public void AlternativeModel()
        {
            PropertyBag["model"] = new Model();
            RenderView("AltModel");
        }

        public void CauseException()
        {
            throw new Exception("Test exception");
        }

        #region Nested type: Model

        private class Model
        {
            public string str
            {
                get { return "c (model)"; }
            }
        }

        #endregion
    }

    public class TestControllerHelper : BaseControllerTest
    {
        public TestController GetPreparedTestController()
        {
            var controller = new TestController();
            PrepareController(controller);
            return controller;
        }
    }
}
