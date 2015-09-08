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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Connectors;
using org.SharpTiles.Templates;
using org.SharpTiles.Tiles;

namespace org.SharpTiles.ConnectorsTest
{
    [TestFixture]
    public class TilesCacheTest
    {
        [Test]
        public void GetView_Should_Be_Based_On_Cache()
        {
            var cache = new TilesCache();
            cache.GuardInit(typeof (TilesViewEngineTest).Assembly);
            foreach (ITile tile in cache.Cache.Tiles)
            {
                Console.WriteLine(tile.Name);
                Assert.That(cache.GetView(tile.Name), Is.SameAs(tile));
            }
        }

        [Test]
        public void GetView_Should_Retun_False_On_Unkown_View()
        {
            var cache = new TilesCache();
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
        public void HasView_Should_Be_Based_On_Cache()
        {
            var cache = new TilesCache();
            cache.GuardInit(typeof (TilesViewEngineTest).Assembly);
            foreach (ITile tile in cache.Cache.Tiles)
            {
                Assert.That(cache.HasView(tile.Name));
            }
        }

        [Test]
        public void HasView_Should_Retun_False_On_Unkown_View()
        {
            var cache = new TilesCache();
            cache.GuardInit(typeof (TilesViewEngineTest).Assembly);
            Assert.That(!cache.HasView("wrong"));
        }

        [Test]
        public void TestInit()
        {
            var cache = new TilesCache();
            Assert.That(cache.Cache, Is.Null);
            cache.GuardInit(typeof (TilesViewEngineTest).Assembly);
            Assert.That(cache.Cache.Contains("Index"));
            Assert.That(cache.Cache.Contains("Other"));
            Assert.That(cache.Cache.Contains("Alt"));
        }
    }
}