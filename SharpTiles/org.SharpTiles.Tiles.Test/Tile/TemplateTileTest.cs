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
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Common;
using org.SharpTiles.Templates;
using org.SharpTiles.Templates.Templates;
using org.SharpTiles.Tiles.Tile;

namespace org.SharpTiles.Tiles.Test.Tile
{
    [TestFixture]
    public class TemplateTileTest
    {
        [Test]
        public void LoadFromFile()
        {
            var tile = new TemplateTile(
                "test",
                new FileTemplate("a.htm"),
                null
                );
            Assert.That(tile.ParsedTemplate.Count, Is.EqualTo(1));
            Assert.That(tile.ParsedTemplate.TemplateParsed[0].ConstantValue, Is.EqualTo(File.ReadAllText("a.htm")));
        }


        [Test]
        public void WrongFileName()
        {
            try
            {
                new TemplateTile("name", new FileTemplate("somepath.htm"), null);
                Assert.Fail("Expected exception");
            }
            catch (TemplateException Te)
            {
                var expected = ResourceException.FileNotFound(Path.GetFullPath("somepath.htm"));
                Assert.That(
                    Te.InnerException.Message,
                    Is.EqualTo(expected.Message)
                    );
                Assert.That(
                    Te.Message,
                    Is.EqualTo(TemplateException.TemplateFailedToInitialize(Path.GetFullPath("somepath.htm"), expected).Message));
            }
        }
    }
}
