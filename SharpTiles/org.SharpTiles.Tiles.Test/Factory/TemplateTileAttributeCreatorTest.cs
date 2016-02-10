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
using System.IO;
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
    public class TemplateTileAttributeCreatorTest
    {
        private TagLib _lib;
        private FileLocatorFactory _locatorFactory;

        [SetUp]
        public void SetUp()
        {
            _lib = new TagLib();
            _lib.Register(new Tags.Tiles());
            _lib.Register(new Sharp());
            _locatorFactory = new FileLocatorFactory().CloneForTagLib(_lib) as FileLocatorFactory;
        }

        [Test]
        public void CreatorShouldApplyWhenAttributeTileTypeIsSetToFile()
        {
            var entry = new XmlAttributeEntry
                            {
                                Type = TileType.File.ToString()
                            };
            Assert.That(new TemplateTileAttributeCreator().Applies(entry));
        }

        [Test]
        public void CreatorShouldAssembleTileAttributeWithEmbeddedFileTile()
        {
            var entry = new XmlAttributeEntry
                            {
                                Name = "name",
                                Value = "a.htm",
                                Type = TileType.File.ToString()
                            };
            var factory = new TilesFactory(new MockConfiguration("a.htm", DateTime.Now) {Factory = _locatorFactory });
            var tile = new TemplateTileAttributeCreator().Create(entry, factory);
            Assert.That(tile, Is.Not.Null);
            Assert.That(tile.Name, Is.EqualTo("name"));
            Assert.That(tile.Value, Is.Not.Null);
            Assert.That(tile.Value.GetType(), Is.EqualTo(typeof (TemplateTile)));
            var templateTile = (TemplateTile) tile.Value;
            var template = (FileTemplate) templateTile.Template;
            Assert.That(template.Path.EndsWith("a.htm"));
        }

        [Test]
        public void CreatorShouldAssembleTileAttributeShoulApplyFilePrefix()
        {
            var entry = new XmlAttributeEntry
            {
                Name = "name",
                Value = "a.htm",
                Type = TileType.File.ToString()
            };
            var config = new MockConfiguration("a.htm", DateTime.Now) { Factory = _locatorFactory };
            var factory = new TilesFactory(config);
            var tile = new TemplateTileAttributeCreator().Create(entry, factory);
            Assert.That(tile, Is.Not.Null);
            config.FilePrefix = @"nonexisting\";
            try
            {
                new TemplateTileAttributeCreator().Create(entry, factory);
            } catch (TileException Te)
            {
                Console.WriteLine(Te.Message);
                Assert.That(Te.Message.Contains("a.htm could not be found"));
            }
        }

        [Test]
        public void CreatorShouldNotApplyWhenAttributeTileTypeIsNotSet()
        {
            var entry = new XmlAttributeEntry
                            {
                                Type = null
                            };
            Assert.That(new TemplateTileAttributeCreator().Applies(entry), Is.False);
        }

        [Test]
        public void CreatorShouldNotApplyWhenAttributeTileTypeIsSetToOtherValueThanFile()
        {
            var entry = new XmlAttributeEntry
                            {
                                Type = TileType.Definition.ToString()
                            };
            Assert.That(new TemplateTileAttributeCreator().Applies(entry), Is.False);
        }
    }
}
