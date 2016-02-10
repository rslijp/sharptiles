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
    public class AutoTileAttributeCreatorTest
    {

        private FileLocatorFactory _locatorFactory;
        private TagLib _lib;
        private TilesFactory _factory;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _lib = new TagLib();
            _lib.Register(new Tags.Tiles());
            _lib.Register(new Sharp());
            _locatorFactory = new FileLocatorFactory().CloneForTagLib(_lib) as FileLocatorFactory;
            var config = new MockConfiguration() {Factory = _locatorFactory };
            _factory = new TilesFactory(config);
        }

        [TearDown]
        public void TearDown()
        {
            _factory = null;
        }

        #endregion

        [Test]
        public void CreatorShouldApplyWhenAttributeTileTypeIsNotSet()
        {
            var entry = new XmlAttributeEntry
                            {
                                Type = null
                            };
            Assert.That(new AutoTileAttributeCreator().Applies(entry));
        }

        [Test]
        public void CreatorShouldAssembleAStringTileWhenTileIsNeitherADefinitionOrAFile()
        {
            var entry = new XmlAttributeEntry
                            {
                                Name = "name",
                                Value = "string",
                            };
            TileAttribute tile = new AutoTileAttributeCreator().Create(entry, _factory);
            Assert.That(tile, Is.Not.Null);
            Assert.That(tile.Name, Is.EqualTo("name"));
            Assert.That(tile.Value, Is.Not.Null);
            Assert.That(tile.Value.GetType(), Is.EqualTo(typeof (TileReference)));
            var reference = (TileReference) tile.Value;
            Assert.That(reference.FallBack, Is.Not.Null);
            Assert.That(reference.Name, Is.EqualTo("string"));
            Assert.That(((StringTile) reference.FallBack).Value, Is.EqualTo("string"));
        }

        [Test]
        public void CreatorShouldAssembleDefinitionTileWhenTileIsntAnExistingFileName()
        {
            var entry = new XmlAttributeEntry
                            {
                                Name = "name",
                                Value = "definition",
                            };
            TileAttribute tile = new AutoTileAttributeCreator().Create(entry, _factory);
            Assert.That(tile, Is.Not.Null);
            Assert.That(tile.Name, Is.EqualTo("name"));
            Assert.That(tile.Value, Is.Not.Null);
            Assert.That(tile.Value.GetType(), Is.EqualTo(typeof (TileReference)));
            var reference = (TileReference) tile.Value;
            Assert.That(reference.FallBack, Is.Not.Null);
            Assert.That(reference.Name, Is.EqualTo("definition"));
            Assert.That(((StringTile) reference.FallBack).Value, Is.EqualTo("definition"));
        }

        [Test]
        public void CreatorShouldAssembleFileTileWhenTileIsFilledWithAnExistingFileName()
        {
            var entry = new XmlAttributeEntry
                            {
                                Name = "name",
                                Value = "a.htm",
                            };
            TileAttribute tile = new AutoTileAttributeCreator().Create(entry, _factory);
            Assert.That(tile, Is.Not.Null);
            Assert.That(tile.Name, Is.EqualTo("name"));
            Assert.That(tile.Value, Is.Not.Null);
            Assert.That(tile.Value.GetType(), Is.EqualTo(typeof (TemplateTile)));
            var templateTile = (TemplateTile) tile.Value;
            var fileTemplate = (FileTemplate) templateTile.Template;
            Assert.That(fileTemplate.Path.EndsWith("a.htm"));
        }

        [Test]
        public void CreatorShouldAssembleFileTileWhenTileIsFilledShouldTakePrefixIntoAccount()
        {
            var config = new MockConfiguration() {Factory = _locatorFactory};
            _factory = new TilesFactory(config);
            var entry = new XmlAttributeEntry
            {
                Name = "name",
                Value = "a.htm",
            };
            var tile = new AutoTileAttributeCreator().Create(entry, _factory);
            Assert.That(tile, Is.Not.Null);
            config.FilePrefix = @"nonexisting\";
            try
            {
                new AutoTileAttributeCreator().Create(entry, _factory);
            }
            catch (TileException Te)
            {
                Assert.That(Te.Message.Contains("not find a part of the path"));
            }
        }

        [Test]
        public void CreatorShouldNotApplyWhenAttributeTileTypeIsSetToString()
        {
            var entry = new XmlAttributeEntry
                            {
                                Type = TileType.String.ToString()
                            };
            Assert.That(new AutoTileAttributeCreator().Applies(entry), Is.False);
        }
    }
}
