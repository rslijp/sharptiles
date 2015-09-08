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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Tiles.Configuration;

namespace org.SharpTiles.Templates.Test.Configuration
{
    [TestFixture]
    public class XmlAttributeEntryTest
    {
        [Test]
        public void WhenNoTypeSpecifiedTileTypeShouldReturnNull()
        {
            var entry = new XmlAttributeEntry
                            {
                                Type = null
                            };
            Assert.That(entry.TileType, Is.Null);
        }

        [Test]
        public void WhenTypeSpecifiedTileTypeShouldReturnTheMatchingEnum()
        {
            var entry = new XmlAttributeEntry
            {
                Type = TileType.Definition.ToString()
            };
            Assert.That(entry.TileType, Is.EqualTo(TileType.Definition));
        }

        [Test]
        public void WhenTypeSpecifiedInLowerCaseTileTypeShouldReturnTheMatchingEnum()
        {
            var entry = new XmlAttributeEntry
            {
                Type = TileType.Definition.ToString().ToLower()
            };
            Assert.That(entry.TileType, Is.EqualTo(TileType.Definition));
        }

        [Test]
        public void WhenTypeSpecifiedIsntAValidEnumValueTileTypeShouldThrowException()
        {
            var entry = new XmlAttributeEntry
            {
                Type = "unkown"
            };
            try
            {
                var type = entry.TileType;
                Assert.Fail("Expected error, shouldn't exist"+type);
            }
            catch (ArgumentException Ae)
            {
                Assert.That(Ae.Message.Contains("unkown"));
            }
            
        }
    }
}
