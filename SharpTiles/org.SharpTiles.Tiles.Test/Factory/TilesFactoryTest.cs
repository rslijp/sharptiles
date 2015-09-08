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
using org.SharpTiles.Tiles.Configuration;
using org.SharpTiles.Tiles.Factory;
using org.SharpTiles.Tiles.Test.Configuration;
using org.SharpTiles.Tiles.Tile;

namespace org.SharpTiles.Tiles.Test.Factory
{
    [TestFixture]
    public class TilesFactoryTest
    {
        [Test]
        public void DecorateOfOnlyOneFileTiles()
        {
            var config = new MockConfiguration("a", DateTime.Now);
            config.Entries.Add(new MockTileEntry
                                   {
                                       Name = "a",
                                       Path = "a.htm"
                                   });
            var set = TilesFactory.AssembleFor(config).Map;
            Assert.That(set.Tiles.Count, Is.EqualTo(1));
            Assert.That(set.Contains("a"));
        }

        [Test]
        public void DecorateOfTwoFileTiles()
        {
            var config = new MockConfiguration("a", DateTime.Now);
            config.Entries.Add(new MockTileEntry
            {
                Name = "a",
                Path = "a.htm"
            });
            config.Entries.Add(new MockTileEntry
            {
                Name = "b",
                Path = "b.htm"
            });
            
            var set = TilesFactory.AssembleFor(config).Map;
            Assert.That(set.Tiles.Count, Is.EqualTo(2));
            Assert.That(set.Contains("a"));
            Assert.That(set.Contains("b"));
        }

        [Test]
        public void DecorateOfOneFileOneAttributeTiles()
        {
            var config = new MockConfiguration("a", DateTime.Now);
            config.Entries.Add(new MockTileEntry
            {
                Name = "a",
                Path = "a.htm",
                TileAttributes = new List<IAttributeEntry>{new MockAttributeEntry
                                                           {
                                                               Name="b",
                                                               Value = "value"
                                                           }}
            });
            var set = TilesFactory.AssembleFor(config).Map;
            Assert.That(set.Tiles.Count, Is.EqualTo(1));
            Assert.That(set.Contains("a"));
            TemplateTile tile = (TemplateTile) set.Get("a");
            Assert.That(tile.Attributes, Is.Not.Null);
            Assert.That(tile.Attributes.Count, Is.EqualTo(1));
            Assert.That(tile.Attributes["b"], Is.Not.Null);
        }

    }
}
