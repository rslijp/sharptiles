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
using org.SharpTiles.Tiles.Test.Configuration;
using org.SharpTiles.Tiles.Tile;

namespace org.SharpTiles.Tiles.Test.Factory
{
    [TestFixture]
    public class DefinitionTileCreatorTest
    {
        [Test]
        public void AppliesShouldNotMatchOnATileEntryWithOutPathWithOutExtends()
        {
            var entry = new XmlTileEntry
                            {
                                Path = null,
                                Extends = null
                            };
            Assert.That(new DefinitionTileCreator().Applies(entry), Is.False);
        }

        [Test]
        public void AppliesShouldNotMatchOnATileEntryWithPathAndExtends()
        {
            var entry = new XmlTileEntry
                            {
                                Path = "a.htm",
                                Extends = "b"
                            };
            Assert.That(new DefinitionTileCreator().Applies(entry), Is.False);
        }

        [Test]
        public void AppliesShouldOnlyMatchOnATileEntryWithOutPathWithExtends()
        {
            var entry = new XmlTileEntry
                            {
                                Path = null,
                                Extends = "b"
                            };
            Assert.That(new DefinitionTileCreator().Applies(entry));
        }

        [Test]
        public void CreateShouldAssembleFileTileWithCorrectExtends()
        {
//            var set = new TilesMap();
//            set.AddTile(new FileTile(
//                            "definition",
//                            "a.htm",
//                            null
//                            )
//                );
            var factory = new TilesFactory(new MockConfiguration());

            var entry = new MockTileEntry
                            {
                                Name = "name",
                                Path = null,
                                Extends = "definition"
                            };
            ITile tile = new DefinitionTileCreator().Create(entry, factory);
            Assert.That(tile, Is.Not.Null);
            Assert.That(tile.GetType(), Is.EqualTo(typeof (DefinitionTile)));
            Assert.That(tile.Name, Is.EqualTo("name"));

            var definition = (DefinitionTile) tile;
            Assert.That(definition.Extends, Is.Not.Null);
            Assert.That(definition.Extends.GetType(), Is.EqualTo(typeof (TileReference)));

            var reference = (TileReference) definition.Extends;
            Assert.That(reference.Name, Is.EqualTo("definition"));
        }
    }
}
