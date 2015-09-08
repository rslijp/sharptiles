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
 using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace org.SharpTiles.Common.Test
{
    [TestFixture]
    public class CollectionUtilsTest
    {
        [Test]
        public void EmptyEnum()
        {
            IList<string> list = new List<string>();
            Assert.That(CollectionUtils.ToString(list), Is.EqualTo(""));
        }

        [Test]
        public void EnumWithMultipleEntities()
        {
            IList<string> list = new List<string>();
            list.Add("A");
            list.Add("B");
            list.Add("C");
            Assert.That(CollectionUtils.ToString(list), Is.EqualTo("A, B, C"));
        }

        [Test]
        public void EnumWithMultipleEntitiesAndOneNullEnitity()
        {
            IList<string> list = new List<string>();
            list.Add("A");
            list.Add(null);
            list.Add("C");
            Assert.That(CollectionUtils.ToString(list), Is.EqualTo("A, , C"));
        }

        [Test]
        public void EnumWithOneEntity()
        {
            IList<string> list = new List<string>();
            list.Add("A");
            Assert.That(CollectionUtils.ToString(list), Is.EqualTo("A"));
        }
    }
}
