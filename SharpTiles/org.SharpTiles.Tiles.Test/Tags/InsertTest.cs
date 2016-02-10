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
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Expressions;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.Templates.SharpTags;
using org.SharpTiles.Templates.Templates;
using org.SharpTiles.Tiles.Tags;
using org.SharpTiles.Tiles.Tile;

namespace org.SharpTiles.Tiles.Test
{
    [TestFixture]
    public class InsertTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _lib = new TagLib();
            _lib.Register(new Tags.Tiles());
            _lib.Register(new Sharp());
            _factory = new FileLocatorFactory().CloneForTagLib(_lib) as FileLocatorFactory;
            new TilesSet();
            _map = new TilesMap();
            _data = new Hashtable();
            _model = new TagModel(_data);
            _nestedAttributes = new AttributeSet(
                "nested",
                new TileAttribute("aAttribute", new StringTile("aAttributeValue"))
                );
            _map.AddTile(new TemplateTile("fileWithAttributes", _factory.Handle("filewithtileattributes.htm",true), _nestedAttributes));
            _attributes = new AttributeSet(
                "main",
                new TileAttribute("simple", new StringTile("simpleValue")),
                new TileAttribute("file", new TemplateTile(null, _factory.Handle("a.htm",true), null)),
                new TileAttribute("fileWithVars", new TemplateTile(null, _factory.Handle("b.htm",true), null)),
                new TileAttribute("fileWithTilesAttributes", new TileReference("fileWithAttributes", _map))
                );
            _model.Decorate().With(_attributes);
            _data["simpleAsProperty"] = "simple";
            _data["some"] = new Hashtable { { "a", "AA" } };
        }

        [TearDown]
        public void TearDown()
        {
            _map = null;
            _data = null;
            _model = null;
            _attributes = null;
            _nestedAttributes = null;
        }

        #endregion

        private TilesMap _map;
        private Hashtable _data;
        private TagModel _model;
        private AttributeSet _attributes;
        private AttributeSet _nestedAttributes;
        private FileLocatorFactory _factory;
        private TagLib _lib;

        [Test]
        public void CheckWhenRequired()
        {
            var tag = new Insert();
            try
            {
                RequiredAttribute.Check(tag);
                Assert.Fail("Expected exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message,
                            Is.EqualTo(TagException.MissingRequiredAttribute(typeof(Insert), "Name").Message));
            }
            tag.Name = new MockAttribute(new Property("tiles"));
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void InsertShouldHandleDefitionWithAtributesTile()
        {
            var tag = new Insert
                          {
                              Name = new MockAttribute(new Constant("fileWithTilesAttributes"))
                          };
            Assert.That(tag.Evaluate(_model), Is.EqualTo("aAttributeValue"));
        }

        [Test]
        public void InsertShouldHandleFileWithVarsTile()
        {
            var tag = new Insert
                          {
                              Name = new MockAttribute(new Constant("fileWithVars"))
                          };
            Assert.That(tag.Evaluate(_model), Is.EqualTo("bbAAbb"));
        }

        [Test]
        public void InsertShouldHandleSimpleFileTile()
        {
            var tag = new Insert
                          {
                              Name = new MockAttribute(new Constant("file"))
                          };
            Assert.That(tag.Evaluate(_model), Is.EqualTo("aa"));
        }

        [Test]
        public void InsertShouldHandleSimpleStringTileDefinedByConstant()
        {
            var tag = new Insert
                          {
                              Name = new MockAttribute(new Constant("simple"))
                          };
            Assert.That(tag.Evaluate(_model), Is.EqualTo("simpleValue"));
        }

        [Test]
        public void InsertShouldHandleSimpleStringTileDefinedByProperty()
        {
            var tag = new Insert
                          {
                              Name = new MockAttribute(new Property("simpleAsProperty"))
                          };
            Assert.That(tag.Evaluate(_model), Is.EqualTo("simpleValue"));
        }

        [Test]
        public void OnNonExistingAttributgesTagShouldThrowException()
        {
            var tag = new Insert
                          {
                              Name = new MockAttribute(new Constant("nonexisting"))
                          };
            try
            {
                tag.Evaluate(_model);
                Assert.Fail("Expect exception");
            }
            catch (TileExceptionWithContext Te)
            {
                Assert.That(Te.Message, Is.EqualTo(TileException.AttributeNotFound("nonexisting", "main").Message));
            }
        }
    }
}
