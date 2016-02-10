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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Connectors;
using org.SharpTiles.HtmlTags;
using org.SharpTiles.Tags;
using org.SharpTiles.Templates;
using org.SharpTiles.Templates.Templates;
using org.SharpTiles.Tiles;

namespace org.SharpTiles.ConnectorsTest
{
    [TestFixture]
    public class NstlCacheTest
    {
        [SetUp]
        public void SetUp()
        {
            NstlCache.PostFix = null;
        }

        [Test]
        public void GetView_Should_Be_Cached()
        {
            //Given
            var cache = new NstlCache();
            cache.GuardInit(typeof (TilesViewEngineTest).Assembly);
            Assert.That(cache.Factory.Handle("mvc_Index.htm", false), Is.Not.Null);

            //Then
            Assert.That(cache.GetView("mvc_Index.htm"), Is.SameAs(cache.GetView("mvc_Index.htm")));
        }

        [Test]
        public void GetView_Use_PostFix()
        {
            NstlCache.PostFix = ".htm";
            //Given
            var cache = new NstlCache();
            cache.GuardInit(typeof(TilesViewEngineTest).Assembly);
            Assert.That(cache.Factory.Handle("mvc_Index.htm", false), Is.Not.Null);

            //Then
            Assert.That(cache.GetView("mvc_Index"), Is.SameAs(cache.GetView("mvc_Index")));
        }

        [Test]
        public void GetView_Should_Retun_False_On_Unkown_View()
        {
            var cache = new NstlCache();
            cache.GuardInit(typeof (TilesViewEngineTest).Assembly);
            try
            {
                cache.GetView("wrong");
                Assert.Fail("Expected exception");
            }
            catch (TemplateException e)
            {
                Assert.That(e.HttpErrorCode, Is.EqualTo(404));
            }
        }

        [Test]
        public void GetView_Should_Return_View_From_Factory()
        {
            //Given
            var cache = new NstlCache();
            cache.GuardInit(typeof (TilesViewEngineTest).Assembly);
            Assert.That(cache.Factory.Handle("mvc_Index.htm", false), Is.Not.Null);
            Assert.That(!cache.Pages.Contains("mvc_Index.htm"));

            //Then
            Assert.That(cache.GetView("mvc_Index.htm").Name,
                        Is.EqualTo(cache.Factory.Handle("mvc_Index.htm", false).Path));
        }

        [Test]
        public void HasView_Should_Be_Based_On_Locator()
        {
            //Given
            var cache = new NstlCache();
            cache.GuardInit(typeof (TilesViewEngineTest).Assembly);
            Assert.That(cache.Factory.Handle("mvc_Index.htm", true), Is.Not.Null);
            Assert.That(!cache.Pages.Contains("mvc_Index.htm"));

            //Then
            Assert.That(cache.HasView("mvc_Index.htm"));
        }


        [Test]
        public void HasView_Should_Use_PostFix()
        {
            NstlCache.PostFix = ".htm";
            //Given
            var cache = new NstlCache();
            cache.GuardInit(typeof(TilesViewEngineTest).Assembly);
            Assert.That(cache.Factory.Handle("mvc_Index.htm", true), Is.Not.Null);
            Assert.That(!cache.Pages.Contains("mvc_Index.htm"));

            //Then
            Assert.That(cache.HasView("mvc_Index"));
        }

        [Test]
        public void HasView_Should_Retun_False_On_Unkown_View()
        {
            //Given
            var cache = new NstlCache();
            cache.GuardInit(typeof (TilesViewEngineTest).Assembly);
            Assert.That(cache.Factory.Handle("Wrong.htm", false), Is.Null);
            Assert.That(!cache.Pages.Contains("Wrong.htm"));

            //Then
            Assert.That(!cache.HasView("Wrong"));
        }

        [Test]
        public void TestInit()
        {
            var cache = new NstlCache();
            Assert.That(cache.Pages, Is.Not.Null);
            Assert.That(cache.Pages.Tiles, Is.Empty);
            Assert.That(cache.Factory, Is.Null);
            cache.GuardInit(typeof (TilesViewEngineTest).Assembly);
            Assert.That(cache.Factory, Is.Not.Null);
            Assert.That(cache.Pages.Tiles, Is.Empty);
        }


        [Test]
        public void GetView_Should_Update_Locator_Correct()
        {
            //Given
            var lib = new TagLib();
            lib.Register(new Html());
            lib.Register(new Tiles.Tags.Tiles());

            var factory = new FileLocatorFactory("Views").CloneForTagLib(lib);
            var cache = new NstlCache { Factory = factory };
            var view = cache.GetView("Home/Index.htm");
            Assert.That(view, Is.Not.Null);
            var model = new TagModel(new Dictionary<string, string> { { "Message", "Test" } });
            Assert.That(view.Render(model).Contains("VIEWS"));
            //Then
        }
    }
}