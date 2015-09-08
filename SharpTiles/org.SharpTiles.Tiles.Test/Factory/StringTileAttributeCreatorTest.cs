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
 */using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Tiles.Configuration;
using org.SharpTiles.Tiles.Factory;
using org.SharpTiles.Tiles.Tile;

namespace org.SharpTiles.Tiles.Test.Factory
{
    [TestFixture]
    public class StringTileAttributeCreatorTest
    {
        [Test]
        public void CreatorShouldApplyWhenAttributeTileTypeIsSetToString()
        {
            var entry = new XmlAttributeEntry
                            {
                                Type = TileType.String.ToString()
                            };
            Assert.That(new StringTileAttributeCreator().Applies(entry));
        }

        [Test]
        public void CreatorShouldAssembleTileAttributeWithEmbeddedStringTile()
        {
            var entry = new XmlAttributeEntry
                            {
                                Name = "name",
                                Value = "value",
                                Type = TileType.String.ToString()
                            };
            TileAttribute tile = new StringTileAttributeCreator().Create(entry, null);
            Assert.That(tile, Is.Not.Null);
            Assert.That(tile.Name, Is.EqualTo("name"));
            Assert.That(tile.Value, Is.Not.Null);
            Assert.That(tile.Value.GetType(), Is.EqualTo(typeof (StringTile)));
            Assert.That(((StringTile) tile.Value).Value, Is.EqualTo("value"));
        }

        [Test]
        public void CreatorShouldNotApplyWhenAttributeTileTypeIsNotSet()
        {
            var entry = new XmlAttributeEntry
                            {
                                Type = null
                            };
            Assert.That(new StringTileAttributeCreator().Applies(entry), Is.False);
        }

        [Test]
        public void CreatorShouldNotApplyWhenAttributeTileTypeIsSetToOtherValueThanString()
        {
            var entry = new XmlAttributeEntry
                            {
                                Type = TileType.Definition.ToString()
                            };
            Assert.That(new StringTileAttributeCreator().Applies(entry), Is.False);
        }
    }
}
