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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Expressions.Functions;

namespace org.SharpTiles.Expressions.Test.Functions
{
    [TestFixture]
    public class RegExReplaceFunctionTest
    {
        [Test]
        public void TestAllFilled()
        {
            var function = new RegExReplaceFunction();
            Assert.That(function.Evaluate("a-b-c-d-e-f", "-", "+"), Is.EqualTo("a+b+c+d+e+f"));
        }

        [Test]
        public void ComplexPattern()
        {
            var function = new RegExReplaceFunction();
            var pattern = @"(^\+)|(^00)|(\ |\(|\)|\-)";
            Assert.That(function.Evaluate("0614664954", pattern, ""), Is.EqualTo("0614664954"));
            Assert.That(function.Evaluate("+31614664954", pattern, ""), Is.EqualTo("31614664954"));
            Assert.That(function.Evaluate("061+4664954", pattern, ""), Is.EqualTo("061+4664954"));
            Assert.That(function.Evaluate("0031614664954", pattern, ""), Is.EqualTo("31614664954"));
            Assert.That(function.Evaluate("0031614664954", pattern, ""), Is.EqualTo("31614664954"));
            Assert.That(function.Evaluate("06 14 66 49 54", pattern, ""), Is.EqualTo("0614664954"));
            Assert.That(function.Evaluate("(06) 14 66 49 54", pattern, ""), Is.EqualTo("0614664954"));
            Assert.That(function.Evaluate("06-14664954", pattern, ""), Is.EqualTo("0614664954"));
        }

        [Test]
        public void TestNullAll()
        {
            var function = new RegExReplaceFunction();
            try
            {
                function.Evaluate(null, null, null);
                Assert.Fail("Expected exception");
            }
            catch (ArgumentException)
            {
                //oke
            }
        }

        [Test]
        public void TestNullEmptyAfter()
        {
            var function = new RegExReplaceFunction();
            Assert.That(function.Evaluate("a-b-c-d-e-f", "-", null), Is.EqualTo("abcdef"));
        }

        [Test]
        public void TestNullEmptyBefore()
        {
            var function = new RegExReplaceFunction();
            try
            {
                function.Evaluate("abcdef", null, "-");
                Assert.Fail("Expected exception");
            }
            catch (ArgumentException)
            {
                //oke
            }
        }

        [Test]
        public void TestNullSource()
        {
            var function = new RegExReplaceFunction();
            Assert.That(function.Evaluate(null, "a", "b"), Is.EqualTo(""));
        }
    }
}
