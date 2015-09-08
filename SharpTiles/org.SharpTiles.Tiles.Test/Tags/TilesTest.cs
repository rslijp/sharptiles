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

namespace org.SharpTiles.Tiles.Test
{
    [TestFixture]
    public class TilesTest
    {
        [Test]
        public void TilesShouldHaveInstertTagRegistered()
        {
            var lib = new Tags.Tiles();
            Assert.That(lib.Get("insert", null), Is.Not.Null);
            Assert.That(lib.Get("insert", null).TagName, Is.EqualTo("insert"));
        }

        [Test]
        public void TilesShouldHaveNameTiles()
        {
            var lib = new Tags.Tiles();
            Assert.That(lib.Name, Is.EqualTo("tiles"));
        }
    }
}
