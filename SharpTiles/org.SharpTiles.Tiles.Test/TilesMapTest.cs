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
 */using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Templates;
using org.SharpTiles.Tiles.Test.Tile;

namespace org.SharpTiles.Tiles.Test
{
    [TestFixture]
    public class TilesMapTest
    {

        [Test]
        public void TilesShouldReturnAllTilesInMap()
        {
            var map = new TilesMap();
            Assert.That(map.Tiles.Count, Is.EqualTo(0));
            map.AddTile(new MockTile("a"));
            Assert.That(map.Tiles.Count, Is.EqualTo(1));
            map.AddTile(new MockTile("b"));
            Assert.That(map.Tiles.Count, Is.EqualTo(2));
        }

        [Test]
        public void SameTileShouldBeReturned()
        {
            var ts = new TilesMap();
            ITile a = new MockTile("a");
            ITile b = new MockTile("b");

            ts.AddTile(a);
            ts.AddTile(b);

            Assert.That(ts.Get("a"), Is.SameAs(a));
            Assert.That(ts.Get("b"), Is.SameAs(b));
        }

        [Test]
        public void AnExceptionShouldBeThrownWhenEnteringWithTheSameName()
        {
            var ts = new TilesMap();
            ITile a = new MockTile("a");
            ITile b = new MockTile("a");

            ts.AddTile(a);
            try
            {
                ts.AddTile(b);
                Assert.Fail("Expected exception");
            } catch (TileException Te)
            {
                Assert.That(Te.Message, Is.EqualTo(TileException.DoubleDefinition("a").Message));
            }
        }

        [Test]
        public void TestCache()
        {
            var ts = new TilesMap();
            ITile a = new MockTile("a");
            ITile b = new MockTile("b");

            ts.AddTile(a);
            ts.AddTile(b);

            Assert.That(ts.Get("a"), Is.SameAs(a));
            Assert.That(ts.Get("b"), Is.SameAs(b));
        }


        [Test]
        public void TestContains()
        {
            var ts = new TilesMap();
            ITile a = new MockTile("a");

            Assert.That(ts.Contains("a"), Is.False);
            
            ts.AddTile(a);

            Assert.That(ts.Contains("a"), Is.True);
        }


        [Test]
        public void UnkownTileShouldThrowExceptionThroughIndexed()
        {
            var ts = new TilesMap();
            try
            {
                var x = ts["?"];
                Assert.Fail("Expected exception on " + x);
            }
            catch (TemplateException Te)
            {
                Assert.That(Te.Message, Is.EqualTo(TemplateException.TemplateNotFound("?").Message));
            }
        }


        [Test]
        public void UnkownTileShouldReturnNullThrowhSafeGet()
        {
            var ts = new TilesMap();
            Assert.IsNull(ts.SafeGet("?"));
        }

    }
}
