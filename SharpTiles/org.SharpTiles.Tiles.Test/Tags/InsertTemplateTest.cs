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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Common;
using org.SharpTiles.Expressions;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.CoreTags;
using org.SharpTiles.Templates.Templates;
using org.SharpTiles.Tiles.Tags;
using org.SharpTiles.Tiles.Tile;

namespace org.SharpTiles.Tiles.Test
{
    [TestFixture]
    public class InsertTemplateTest
    {
        #region Setup/Teardown
        private ITagLib _lib;

        [SetUp]
        public void SetUp()
        {
            _lib = new TagLib();
            _lib.Register(new Tags.Tiles());

            _map = new TilesMap();
            _data = new Hashtable();
            _model = new TagModel(_data);
            _nestedAttributes = new AttributeSet(
                "nested",
                new TileAttribute("aAttribute", new StringTile("aAttributeValue"))
                );
            _map.AddTile(new TemplateTile("fileWithAttributes", new FileTemplate("filewithtileattributes.htm"),
                                          _nestedAttributes));
            _attributes = new AttributeSet(
                "main",
                new TileAttribute("simple", new StringTile("simpleValue")),
                new TileAttribute("file", new TemplateTile(null, new FileTemplate("a.htm"), null)),
                new TileAttribute("fileWithVars", new TemplateTile(null, new FileTemplate("b.htm"), null)),
                new TileAttribute("fileWithTilesAttributes", new TileReference("fileWithAttributes", _map))
                );
            _model.Decorate().With(_attributes);
            _data["simpleAsProperty"] = "simple";
            _data["some"] = new Hashtable {{"a", "AA"}};
            _model.UpdateFactory(new FileLocatorFactory());
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

        [Test]
        public void CheckWhenRequired()
        {
            var tag = new InsertTemplate();
            try
            {
                RequiredAttribute.Check(tag);
                Assert.Fail("Expected exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message,
                            Is.EqualTo(
                                TagException.MissingRequiredAttribute(typeof (InsertTemplate), "Template").Message));
            }
            tag.Template = new MockAttribute(new Property("tiles"));
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void InsertTemplate()
        {
            var tag = new InsertTemplate()
                          {
                              Template = new MockAttribute(new Constant("insertAMaster.htm"))
                          };
            string expected = (new StreamReader("insertAExpected.htm")).ReadToEnd();
            string value = tag.Evaluate(_model);
            Assert.That(value, Is.EqualTo(expected));
        }

        [Test]
        public void InsertTemplateWithBodyAttribute()
        {
            var tag = new InsertTemplate()
                          {
                              Template = new MockAttribute(new Constant("insertEMaster.htm"))
                          };
            var bodyAttribute = new PutAttribute()
                                    {
                                        Name = new MockAttribute(new Constant("body")),
                                        Body = new MockAttribute(new Constant("Dit is de Test Body"))
                                    };
            tag.AddNestedTag(bodyAttribute);
            string expected = (new StreamReader("insertEExpected.htm")).ReadToEnd();
            string value = tag.Evaluate(_model);
            Assert.That(value, Is.EqualTo(expected));
        }

        [Test]
        public void InsertTemplateCachesTile()
        {
            var tag = new InsertTemplate()
            {
                Template = new MockAttribute(new Constant("insertEMaster.htm"))
            };
            var bodyAttribute = new PutAttribute()
            {
                Name = new MockAttribute(new Constant("body")),
                Body = new MockAttribute(new Constant("Dit is de Test Body"))
            };
            tag.AddNestedTag(bodyAttribute);
            tag.Evaluate(_model);
            var tile = tag.Tile;
            tag.Evaluate(_model);
            Assert.That(tag.Tile, Is.SameAs(tile));
        }

        [Test]
        public void InsertTemplateWithIllegalAttribute()
        {
            var tag = new InsertTemplate()
                          {
                              Template = new MockAttribute(new Constant("insertEMaster.htm"))
                          };
            var setAttribute = new Set
                                   {
                                       Var = new MockAttribute(new Constant("body")),
                                       Body = new MockAttribute(new Constant("Dit is de Test Body"))
                                   };
            try
            {
                tag.AddNestedTag(setAttribute);
                string expected = (new StreamReader("insertEExpected.htm")).ReadToEnd();
                tag.Evaluate(_model);
                Assert.Fail("Expect exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message,
                            Is.EqualTo(
                                TagException.OnlyNestedTagsOfTypeAllowed(setAttribute.GetType(), typeof (PutAttribute)).
                                    Message));
            }
        }

        [Test]
        public void InsertTemplateWithOneSimpleAttribute()
        {
            var tag = new InsertTemplate()
                          {
                              Template = new MockAttribute(new Constant("insertBMaster.htm"))
                          };
            var titleAttribute = new PutAttribute()
                                     {
                                         Name = new MockAttribute(new Constant("title")),
                                         Value = new MockAttribute(new Constant("Dit is de Test Title"))
                                     };
            tag.AddNestedTag(titleAttribute);
            string expected = (new StreamReader("insertBExpected.htm")).ReadToEnd();
            string value = tag.Evaluate(_model);
            Assert.That(value, Is.EqualTo(expected));
        }

        [Test]
        public void InsertTemplateWithSimpleAttributes()
        {
            var tag = new InsertTemplate()
                          {
                              Template = new MockAttribute(new Constant("insertCMaster.htm"))
                          };
            var titleAttribute = new PutAttribute()
                                     {
                                         Name = new MockAttribute(new Constant("title")),
                                         Value = new MockAttribute(new Constant("Dit is de Test Title"))
                                     };
            tag.AddNestedTag(titleAttribute);
            var bodyAttribute = new PutAttribute()
                                    {
                                        Name = new MockAttribute(new Constant("body")),
                                        Value = new MockAttribute(new Constant("Dit is de Test Body"))
                                    };
            tag.AddNestedTag(bodyAttribute);
            string expected = (new StreamReader("insertCExpected.htm")).ReadToEnd();
            string value = tag.Evaluate(_model);
            Assert.That(value, Is.EqualTo(expected));
        }

        [Test]
        public void HandNestedIncludeWithParentDirs()
        {
            ITile a = new TemplateTile("a", new FileTemplate("Home/Index.htm"), new List<TileAttribute>());
            var result = a.Render(new TagModel(new Dictionary<string, string> { { "Message", "Test" } }).UpdateFactory(new FileLocatorFactory()));
            string expected = File.ReadAllText("expected_insert_template.htm");
            result = CleanUp(result);
            expected = CleanUp(expected);
            Assert.That(result, Is.EqualTo(expected));
           
        }


        [Test]
        public void HandNestedIncludeWithPrefixDirs()
        {

            var template = new ResourceTemplate(_lib,new FileBasedResourceLocator("Views"), new FileLocatorFactory(), "Home/Index.htm");
            ITile a = new TemplateTile("a", template, new List<TileAttribute>());
            var result = a.Render(new TagModel(new Dictionary<string, string> { { "Message", "Test" } }).UpdateFactory(new FileLocatorFactory()));
            string expected = File.ReadAllText("expected_insert_template.htm");
            result = CleanUp(result);
            expected = CleanUp("VIEWS" + expected);
            Assert.That(result, Is.EqualTo(expected));

        }

        private string CleanUp(string value)
        {
            value = value.Replace("\t", "");
            value = value.Replace("\r\n", "");
            value = value.Replace(" ", "");
            return value.Trim();
        }


        [Test]
        public void InsertTemplateWithUrlAttribute()
        {
            var tag = new InsertTemplate()
                          {
                              Template = new MockAttribute(new Constant("insertDMaster.htm"))
                          };
            var moduleAttribute = new PutAttribute()
                                      {
                                          Name = new MockAttribute(new Constant("modules")),
                                          Value = new MockAttribute(new Constant("insertDAdditional.htm"))
                                      };
            tag.AddNestedTag(moduleAttribute);
            var bodyAttribute = new PutAttribute()
                                    {
                                        Name = new MockAttribute(new Constant("body")),
                                        Value = new MockAttribute(new Constant("Dit is de Test Body"))
                                    };
            tag.AddNestedTag(bodyAttribute);
            string expected = (new StreamReader("insertDExpected.htm")).ReadToEnd();
            string value = tag.Evaluate(_model);
            Assert.That(value, Is.EqualTo(expected));
        }
    }
}