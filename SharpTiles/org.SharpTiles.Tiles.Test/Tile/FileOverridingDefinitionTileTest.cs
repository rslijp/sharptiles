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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Tags;
using org.SharpTiles.Templates.Templates;
using org.SharpTiles.Tiles.Tile;

namespace org.SharpTiles.Tiles.Test.Tile
{
    [TestFixture]
    public class FileOverridingDefinitionTileTest
    {
        private TagModel model;
        private TemplateTile tile;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            tile = new TemplateTile(
                "test",
                new FileTemplate("b.htm"),
                new List<TileAttribute>()
                );
            model = new TagModel(new Hashtable { { "some", new Hashtable { { "a", "b" } } } });
        }

        [TearDown]
        public void TearDown()
        {
            tile = null;
        }

        #endregion


        [Test]
        public void DefinitionMergesExtendsAttributes()
        {
            var parent = new DefinitionTile(
                "main",
                tile,
                new List<TileAttribute>
                    {
                        new TileAttribute("second", new StringTile("1")),
                        new TileAttribute("first", new StringTile("2"))
                    }
                );
            var extends = new TemplateOveridingDefinitionTile(
                "def",
                new FileTemplate("a.htm"),
                parent,
                new List<TileAttribute> { new TileAttribute("first", new StringTile("A")) }
                );
            Assert.That(extends.Attributes.Count, Is.EqualTo(2));
            Assert.That(extends.Attributes["first"].Value.Render(null), Is.EqualTo("A"));
        }

        [Test]
        public void LoadFromFile()
        {
            var definition = new TemplateOveridingDefinitionTile(
                "def",
                new FileTemplate("a.htm"),
                tile,
                new List<TileAttribute>()
                );
            Assert.That(definition.Name, Is.EqualTo("def"));
            Assert.That(definition.Render(model), Is.Not.Empty);
            Assert.That(definition.Render(model), Is.Not.EqualTo(tile.Render(model)));
        }

        [Test]
        public void RenderOfADefinitionWithOutAnyAttributesShouldRenderTheSameAsTheUnderlyingFileTile()
        {
            var definition = new TemplateOveridingDefinitionTile(
                "def",
                new FileTemplate("a.htm"),
                tile,
                new List<TileAttribute>()
                );
            Assert.That(definition.Name, Is.EqualTo("def"));
            Assert.That(definition.Render(model), Is.Not.Empty);
            Assert.That(definition.Render(model), Is.Not.EqualTo(tile.Render(model)));
        }
    }
}
