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
    public class ContainsIgnoreCaseFunctionTest
    {
        [Test]
        public void TestContains()
        {
            var function = new ContainsIgnoreCaseFunction();
            Assert.That(function.Evaluate("abcdefg", "d"), Is.EqualTo(true));
        }

        [Test]
        public void TestContainsSourceInCapital()
        {
            var function = new ContainsIgnoreCaseFunction();
            Assert.That(function.Evaluate("ABCDEFG", "d"), Is.EqualTo(true));
        }

        [Test]
        public void TestContainsSubstringInCapital()
        {
            var function = new ContainsIgnoreCaseFunction();
            Assert.That(function.Evaluate("abcdefgh", "D"), Is.EqualTo(true));
        }

        [Test]
        public void TestNotContains()
        {
            var function = new ContainsIgnoreCaseFunction();
            Assert.That(function.Evaluate("abcefg", "d"), Is.EqualTo(false));
        }

        [Test]
        public void TestNullBoth()
        {
            var function = new ContainsIgnoreCaseFunction();
            Assert.That(function.Evaluate(null, null), Is.EqualTo(true));
        }

        [Test]
        public void TestNullSource()
        {
            var function = new ContainsIgnoreCaseFunction();
            Assert.That(function.Evaluate(null, "a"), Is.EqualTo(false));
        }

        [Test]
        public void TestNullSubString()
        {
            var function = new ContainsIgnoreCaseFunction();
            Assert.That(function.Evaluate("a", null), Is.EqualTo(true));
        }
    }
}
