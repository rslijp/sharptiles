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
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Connectors;
using org.SharpTiles.Templates;

namespace org.SharpTiles.ConnectorsTest
{
    [TestFixture]
    public class TilesViewEngineTest
    {
        static TilesViewEngineTest()
        {
            TilesViewEngine.GuardInit(typeof(TilesViewEngineTest).Assembly);
        }

        [Test]
        public void TilesViewEngine_AlternativeModel()
        {
            TestController controller = GetController();
            var result = (ViewResult) controller.AlternativeModel();
            Assert.That(result.ViewName, Is.EqualTo("Alt"));
            var engineResult = new TilesViewEngine().FindView(null, result.ViewName, null, false);
            var viewContext = new ViewContext(controller.ControllerContext, engineResult.View, result.ViewData,
                                              new TempDataDictionary());
            using (var writer = new StringWriter())
            {
                (engineResult.View).Render(viewContext, writer);
                Assert.That(writer.ToString(), Is.EqualTo("Alt c"));
            }
        }

        private TestController GetController()
        {
            var data = new RouteData();
            data.Values.Add("controller", "Welcome");
            var controller = new TestController();
            controller.ControllerContext = new ControllerContext(new RequestContext(new MockContextBase(), data), controller);
            return controller;
        }

        [Test]
        public void TilesViewEngine_AlternativeView()
        {
            TestController controller = GetController();
            var result = (ViewResult)controller.AlternativeView();
            Assert.That(result.ViewName, Is.EqualTo("Alt"));
            var engineResult = new TilesViewEngine().FindView(null, result.ViewName, null, false);
            var viewContext = new ViewContext(controller.ControllerContext, engineResult.View, result.ViewData,
                                              new TempDataDictionary());
            using (var writer = new StringWriter())
            {
                (engineResult.View).Render(viewContext, writer);
                Assert.That(writer.ToString(), Is.EqualTo("Alt c"));
            }
        }

        [Test]
        public void TilesViewEngine_RenderAlternativeModel()
        {
            TestController controller = GetController();
            var result = (ViewResult)controller.AlternativeViewData();
            var engineResult = new TilesViewEngine().FindView(null, "Other", null, false);
            var viewContext = new ViewContext(controller.ControllerContext, engineResult.View, result.ViewData,
                                              new TempDataDictionary());
            using (var writer = new StringWriter())
            {
                (engineResult.View).Render(viewContext, writer);
                Assert.That(writer.ToString(), Is.EqualTo("Other c"));
            }
        
        }

        [Test]
        public void TilesViewEngine_RenderViewData()
        {
            TestController controller = GetController();
            var result = (ViewResult)controller.Index();
//            Assert.That(result.ViewName, Is.EqualTo("Welcome."));
            var engineResult = new TilesViewEngine().FindView(null, "Index", null, false);
            var viewContext = new ViewContext(controller.ControllerContext, engineResult.View, result.ViewData,
                                              new TempDataDictionary());
            using (var writer = new StringWriter())
            {
                (engineResult.View).Render(viewContext, writer);
                Assert.That(writer.ToString(), Is.EqualTo("Index c"));
            }
        }


        [Test]
        public void TilesViewEngine_Should_Throw_Template_Exception_Incase_Of_UndefinedView_And_Non_Http_Errors()
        {
            TestController controller = GetController();
            var result = (ViewResult)controller.Index();
            var engineResult = new TilesViewEngine().FindView(null, "NonExisting", null, false);
            var viewContext = new ViewContext(controller.ControllerContext, engineResult.View, result.ViewData,
                                              new TempDataDictionary());
            Assert.That(((TilesView) engineResult.View).HttpErrors, Is.False);
            try {
                (engineResult.View).Render(viewContext, null);
            }
            catch (TemplateException Te)
            {
                Assert.That(Te.Message, Is.EqualTo(TemplateException.TemplateNotFound("NonExisting").Message));
            }
        }


        [Test]
        public void TilesViewEngine_Should_Send_404_Incase_Of_UndefinedView_And_Http_Errors()
        {
            TestController controller = GetController();
            var result = (ViewResult)controller.Index();
            var engineResult = new TilesViewEngine().FindView(null, "NonExisting", null, false);
            var viewContext = new ViewContext(controller.ControllerContext, engineResult.View, result.ViewData,
                                              new TempDataDictionary());
            
            ((TilesView)engineResult.View).HttpErrors=true;
            
            (engineResult.View).Render(viewContext, null);
            Assert.That(controller.ControllerContext.HttpContext.Response.StatusCode, Is.EqualTo(404));
        }
    }
}