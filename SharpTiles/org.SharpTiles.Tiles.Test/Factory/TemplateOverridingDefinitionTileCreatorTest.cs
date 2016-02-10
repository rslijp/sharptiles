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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.Templates.SharpTags;
using org.SharpTiles.Templates.Templates;
using org.SharpTiles.Tiles.Configuration;
using org.SharpTiles.Tiles.Factory;
using org.SharpTiles.Tiles.Test.Configuration;
using org.SharpTiles.Tiles.Tile;

namespace org.SharpTiles.Tiles.Test.Factory
{
    [TestFixture]
    public class TemplateOverridingDefinitionTileCreatorTest
    {
        [Test]
        public void AppliesShouldOnlyMatchOnATileEntryWithBothPathAndExtends()
        {
            var entry = new XmlTileEntry
                            {
                                Path = "a.htm",
                                Extends = "b"
                            };
            Assert.That(new TemplateOverridingDefinitionTileCreator().Applies(entry));
        }

        [Test]
        public void AppliesShouldNotMatchOnATileEntryWithPathAndWithOutExtends()
        {
            var entry = new XmlTileEntry
                            {
                                Path = "a.htm",
                                Extends = null
                            };
            Assert.That(new TemplateOverridingDefinitionTileCreator().Applies(entry), Is.False);
        }

        [Test]
        public void AppliesShouldNotMatchOnATileEntryWithOutPathAndWithExtends()
        {
            var entry = new XmlTileEntry
            {
                Path = null,
                Extends = "b"
            };
            Assert.That(new TemplateOverridingDefinitionTileCreator().Applies(entry), Is.False);
        }

        [Test]
        public void CreateShouldAssembleFileTileWithCorrectExtendsAndPath()
        {
            var lib = new TagLib();
            lib.Register(new Tags.Tiles());
            lib.Register(new Sharp());
            var locatorFactory = new FileLocatorFactory().CloneForTagLib(lib) as FileLocatorFactory;
            var factory = new TilesFactory( new MockConfiguration("a", DateTime.Now) {Factory = locatorFactory });

            var entry = new MockTileEntry
                            {
                                Name = "name",
                                Path = "b.htm",
                                Extends = "definition"
                            };
            ITile tile = new TemplateOverridingDefinitionTileCreator().Create(entry, factory);
            Assert.That(tile, Is.Not.Null);
            Assert.That(tile.GetType(), Is.EqualTo(typeof(TemplateOveridingDefinitionTile)));
            Assert.That(tile.Name, Is.EqualTo("name"));

            
            var definition = (TemplateOveridingDefinitionTile)tile;
            var tileTemplate = (FileTemplate)definition.Template;

            Assert.That(tileTemplate.Path.EndsWith("b.htm"));

            Assert.That(definition.Extends, Is.Not.Null);
            Assert.That(definition.Extends.GetType(), Is.EqualTo(typeof (TileReference)));

            var reference = (TileReference) definition.Extends;
            Assert.That(reference.Name, Is.EqualTo("definition"));
        }
    }
}
