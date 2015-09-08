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
 using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Expressions.Functions;

namespace org.SharpTiles.Expressions.Test.Functions
{
    [TestFixture]
    public class SubStringBeforeFunctionTest
    {
        [Test]
        public void TestSubStringBeforeAllNull()
        {
            var function = new SubStringBeforeFunction();
            Assert.That(function.Evaluate(null, null), Is.EqualTo(""));
        }

        [Test]
        public void TestSubStringBeforeBeforeNull()
        {
            var function = new SubStringBeforeFunction();
            Assert.That(function.Evaluate("abcdefg", null), Is.EqualTo("abcdefg"));
        }

        [Test]
        public void TestSubStringBeforeExisting()
        {
            var function = new SubStringBeforeFunction();
            Assert.That(function.Evaluate("abcdefg", "de"), Is.EqualTo("abc"));
        }

        [Test]
        public void TestSubStringBeforeNonExistingBefore()
        {
            var function = new SubStringBeforeFunction();
            Assert.That(function.Evaluate("abcdefg", "xyz"), Is.EqualTo("abcdefg"));
        }
    }
}
