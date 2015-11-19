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
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Templates;
using org.SharpTiles.Tiles.Configuration;
using org.SharpTiles.Tiles.Tile;

namespace org.SharpTiles.Tiles.Test.Configuration
{
    [TestFixture]
    public class TileXmlConfiguratorTest
    {
        private ITagLib _lib;

        private static string GetPath(ITile tile)
        {
            var templateTile = (TemplateTile)tile;
            return templateTile.Template.Path;
        }

        [SetUp]
        public void SetUp()
        {
            _lib=new TagLib();
            _lib.Register(new Tags.Tiles());
        }

        [Test]
        public void Deserialize()
        {
            var set = new TilesSet(new TileXmlConfigurator(_lib,"Configuration/tiles.config.xml"));

            Assert.That(set.Tiles.Count, Is.EqualTo(3));
            Assert.That(GetPath(set["a"]), Is.EqualTo(Path.GetFullPath("a.htm")));
            Assert.That(GetPath(set["b"]), Is.EqualTo(Path.GetFullPath("b.htm")));
            Assert.That(GetPath(set["c"]), Is.EqualTo(Path.GetFullPath("c.htm")));

        }

        [Test]
        public void DeserializeFromAssemlbyWithDoubleEntriesShouldThrowException()
        {
            try
            {
                new TilesSet(new TileXmlConfigurator(_lib,"Configuration/tiles.double.config.xml"));
                Assert.Fail("Expected exception");
            }
            catch (TileException e)
            {
                Assert.That(
                    e.Message,
                    Is.EqualTo(
                        TileException.DoubleDefinition(
                            "b"
                            ).Message
                        )
                    );
            }
        }

        [Test]
        public void DeserializeFromAssemlbyWithoutConfigFileShouldThrowException()
        {
            try
            {
                new TileXmlConfigurator(_lib,
                    Assembly.GetAssembly(typeof(String)));
                Assert.Fail("Expected exception");
            }
            catch (ResourceException e)
            {
                Assert.That(
                    e.Message,
                    Is.EqualTo(
                        ResourceException.ConfigFileNotFoundInAssembly(
                            "tiles.xml",
                            Assembly.GetAssembly(typeof(String))
                            ).Message
                        )
                    );
            }
        }

        [Test]
        public void DeserializeFromAssemlbyWithTilesConfigurationFileShouldLoadTilesSet()
        {
            var configurator = new TileXmlConfigurator(
                _lib,
                Assembly.GetAssembly(typeof(TileXmlConfiguratorTest)));
            var set = new TilesSet(configurator);

            Assert.That(set.Tiles.Count, Is.EqualTo(3));
            Assert.That(GetPath(set["a"]), Is.EqualTo("embedded_a.htm"));
            Assert.That(GetPath(set["b"]), Is.EqualTo("embedded_b.htm"));
            Assert.That(GetPath(set["c"]), Is.EqualTo("embedded_c.htm"));
        }

        [Test]
        public void DeserializeHalfFile()
        {
            try
            {
                new TileXmlConfigurator(_lib, "Configuration/tiles.config.half.xml");
                Assert.Fail("Expected exceptions");
            }
            catch (Exception e)
            {
                Assert.That(e.Message.Contains("XML"), Is.True);
            }
        }

        [Test]
        public void DeserializeIncnvalidTagFile()
        {
            try
            {
                new TileXmlConfigurator(_lib, "Configuration/tiles.config.invalidtag.xml");
                Assert.Fail("Expected exceptions");
            }
            catch (Exception e)
            {
                Assert.That(e.Message.Contains("XML"), Is.True);
            }
        }

        [Test]
        public void DeserializeIncorrectAttributeFile()
        {
            try
            {
                new TileXmlConfigurator(_lib, "Configuration/tiles.config.incorrect.attribute.xml");
                Assert.Fail("Expected exceptions");
            }
            catch (Exception e)
            {
                Assert.That(e.Message.Contains("XML"), Is.True);
            }
        }

        [Test]
        public void FilePrefixOnTilesDefinitionsWrong()
        {
            var xmlConfig = new TileXmlConfigurator(_lib, "tiles.config.xml", "Configuration/");
            try
            {
                new TilesSet(xmlConfig);
                Assert.Fail("Should fail");
            }
            catch (TemplateException Te)
            {
                Console.WriteLine(Te.Message);
                Assert.IsTrue(Te.Message.Contains(@"Configuration\a.htm"));
            }
        }

        [Test]
        public void FilePrefixWrong()
        {
            try
            {
                new TilesSet(new TileXmlConfigurator(_lib, "Configuration/tiles.config.xml", "fileprefix/"));
                Assert.Fail("Should fail");
            }
            catch (DirectoryNotFoundException DNFe)
            {
                Assert.IsTrue(DNFe.Message.Contains(@"fileprefix\Configuration\tiles.config.xml"));
            }
        }

        [Test]
        public void SerializeAndDeserialize()
        {
            var sourceConfig = new XmlConfigurationDefinitions();
            sourceConfig.Entries.Add(new XmlTileEntry("a", "a.htm"));
            sourceConfig.Entries.Add(new XmlTileEntry("b", "b.htm"));
            sourceConfig.Entries.Add(new XmlTileEntry("c", "c.htm"));

            var serializer = new XmlSerializer(typeof(XmlConfigurationDefinitions));
            var writer = new StringWriter();
            serializer.Serialize(writer, sourceConfig);


            string xml = writer.GetStringBuilder().ToString();

            var deserializedConfig =
                (XmlConfigurationDefinitions)serializer.Deserialize(new StringReader(xml));

            Assert.That(deserializedConfig.Entries.Count, Is.EqualTo(3));
            Assert.That(deserializedConfig.Entries[0].Name, Is.EqualTo("a"));
            Assert.That(deserializedConfig.Entries[0].Path, Is.EqualTo("a.htm"));
            Assert.That(deserializedConfig.Entries[1].Name, Is.EqualTo("b"));
            Assert.That(deserializedConfig.Entries[1].Path, Is.EqualTo("b.htm"));
            Assert.That(deserializedConfig.Entries[2].Name, Is.EqualTo("c"));
            Assert.That(deserializedConfig.Entries[2].Path, Is.EqualTo("c.htm"));
        }


        [Test]
        public void TilesWithAttributesShouldLoadCorrectly()
        {
            var xmlConfig = new TileXmlConfigurator(_lib, "Configuration/tiles.with.properties.xml");
            var set = new TilesSet(xmlConfig);
            Assert.That(set.Tiles.Count, Is.EqualTo(3));
            Assert.That(set["a"], Is.Not.Null);
        }

        [Test]
        public void TilesWithAutoAttributesShouldLoadCorrectly()
        {
            var xmlConfig = new TileXmlConfigurator(_lib, "Configuration/tiles.with.auto.properties.xml");
            var set = new TilesSet(xmlConfig);
            Assert.That(set.Tiles.Count, Is.EqualTo(3));
            Assert.That(set["a"], Is.Not.Null);
        }


        [Test]
        public void TilesWithExtendsReltionssAttributesShouldLoadCorrectly()
        {
            var xmlConfig = new TileXmlConfigurator(_lib, "Configuration/tiles.with.extends.xml");
            var set = new TilesSet(xmlConfig);
            Assert.That(set.Tiles.Count, Is.EqualTo(4));
            Assert.That(set["a"], Is.Not.Null);
        }
    }
}
