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
using System.Collections.Specialized;
using System.Web;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Test;
using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Templates;
using Subtext.TestLibrary;

namespace org.SharpTiles.Connectors.Monorail.Test
{
    [TestFixture]
    public class TilesViewEngineTest
    {
        private HttpSimulator _httpSimulator;
        private Mock<IRailsEngineContext> _mockRailsEngine;
        private MockResponse _mockResponse;

        static TilesViewEngineTest()
        {
            try
            {
                BaseViewEngine<TilesCache>.GuardInit(typeof(TilesViewEngineTest).Assembly);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("0000000000000000000000000000");
                Console.WriteLine(e.StackTrace);
                throw e;
            }
        }

        private static TestController GetController()
        {
            return new TestControllerHelper().GetPreparedTestController();
        }

        [SetUp]
        public void SetUp()
        {
            try
            {
                _httpSimulator = new HttpSimulator().DisableConsoleDebugInfo().SimulateRequest();
                _mockResponse = new MockResponse(new HybridDictionary(true));

                _mockRailsEngine = new Mock<IRailsEngineContext>(MockBehavior.Strict);
                _mockRailsEngine.SetupGet(e => e.UnderlyingContext).Returns(HttpContext.Current);
                _mockRailsEngine.SetupGet(e => e.Response).Returns(_mockResponse);
            } catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("0000000000000000000000000000");
                Console.WriteLine(e.StackTrace);
                throw e;
            }
        }

        [TearDown]
        public void TearDown()
        {
            _mockRailsEngine.VerifyAll();
            _httpSimulator.Dispose();
            BaseViewEngine<TilesCache>.UseHttpErrors = false;
        }

        [Test]
        public void TilesViewEngine_AlternativeView()
        {
            // Given
            var controller = GetController();
            controller.AlternativeView();

            // I Expect
            _mockRailsEngine.SetupGet(e => e.LastException).Returns(null as Exception);
            _mockRailsEngine.SetupGet(e => e.CurrentController).Returns(controller);

            // When
            new TilesViewEngine().Process(_mockRailsEngine.Object, controller, "Alt");

            // Then
            Assert.That(_mockResponse.Output.ToString(), Is.EqualTo("Alt c"));
        }

        [Test]
        public void TilesViewEngine_AlternativeModel()
        {
            // Given
            var controller = GetController();
            controller.AlternativeModel();

            // I Expect
            _mockRailsEngine.SetupGet(e => e.LastException).Returns(null as Exception);
            _mockRailsEngine.SetupGet(e => e.CurrentController).Returns(controller);

            // When
            new TilesViewEngine().Process(_mockRailsEngine.Object, controller, "AltModel");

            // Then
            Assert.That(_mockResponse.Output.ToString(), Is.EqualTo("Alt c (model)"));
        }

        [Test]
        public void TilesViewEngine_Should_Throw_Template_Exception_Incase_Of_UndefinedView_And_Non_Http_Errors()
        {
            var controller = GetController();
            controller.Index();
            
            try
            {
                new TilesViewEngine().Process(_mockRailsEngine.Object, controller, "NonExisting");
            }
            catch (TemplateException ex)
            {
                Assert.That(ex.Message, Is.EqualTo(TemplateException.TemplateNotFound("NonExisting").Message));
            }
        }

        [Test]
        public void TilesViewEngine_Should_Send_404_Incase_Of_UndefinedView_And_Http_Errors()
        {
            // Given
            var controller = GetController();
            controller.Index();
            BaseViewEngine<TilesCache>.UseHttpErrors = true;

            // When
            new TilesViewEngine().Process(_mockRailsEngine.Object, controller, "NonExisting");

            // Then
            Assert.That(_mockRailsEngine.Object.UnderlyingContext.Response.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public void TilesViewEngine_Should_Add_Exception_To_PropertyBag_When_RailEngineContext_LastException_Is_Not_Null()
        {
            // Given
            var controller = GetController();
            Exception exception = null;
            
            try
            {
                controller.CauseException();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // I Expect
            _mockRailsEngine.SetupGet(e => e.LastException).Returns(exception);
            _mockRailsEngine.SetupGet(e => e.CurrentController).Returns(controller);

            // When
            new TilesViewEngine().Process(_mockRailsEngine.Object, controller, "Rescue");

            // Then
            Assert.IsNotNull(exception);
            Assert.That(exception.Message, Is.EqualTo("Test exception"));
            
            Assert.That(controller.PropertyBag.Count, Is.EqualTo(1));
            Assert.That(controller.PropertyBag.Contains("Exception"));
            Assert.IsNotNull(controller.PropertyBag["Exception"]);
            Assert.That(controller.PropertyBag["Exception"], Is.EqualTo(exception));
        }
    }
}