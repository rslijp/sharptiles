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
    public class TemplateTileCreatorTest
    {
        [Test]
        public void AppliesShouldNotMatchOnATileEntryWithOutPathWithOutExtends()
        {
            var entry = new XmlTileEntry
                            {
                                Path = null,
                                Extends = null
                            };
            Assert.That(new TemplateTileCreator().Applies(entry), Is.False);
        }

        [Test]
        public void AppliesShouldNotMatchOnATileEntryWithPathAndExtends()
        {
            var entry = new XmlTileEntry
                            {
                                Path = "a.htm",
                                Extends = "b"
                            };
            Assert.That(new TemplateTileCreator().Applies(entry), Is.False);
        }

        [Test]
        public void AppliesShouldOnlyMatchOnATileEntryWithPathWithOutExtends()
        {
            var entry = new XmlTileEntry
                            {
                                Path = "a.htm",
                                Extends = null
                            };
            Assert.That(new TemplateTileCreator().Applies(entry));
        }

        [Test]
        public void CreateShouldAssembleFileTileWithCorrectPath()
        {
            var lib = new TagLib();
            lib.Register(new Tags.Tiles());
            lib.Register(new Sharp());
            var locatorFactory = new FileLocatorFactory().CloneForTagLib(lib) as FileLocatorFactory;
            var factory = new TilesFactory(new MockConfiguration("x", DateTime.Now) {Factory = locatorFactory });
            var entry = new MockTileEntry
                            {
                                Name = "name",
                                Path = "a.htm",
                                Extends = null
                            };
            var tile = new TemplateTileCreator().Create(entry, factory);
            Assert.That(tile, Is.Not.Null);
            Assert.That(tile.GetType(), Is.EqualTo(typeof (TemplateTile)));
            Assert.That(tile.Name, Is.EqualTo("name"));
            var templateTile = (TemplateTile) tile;
            var fileTemplate = (FileTemplate) templateTile.Template;
            Assert.That(fileTemplate.Path.EndsWith("a.htm"));
        }
    }
}
