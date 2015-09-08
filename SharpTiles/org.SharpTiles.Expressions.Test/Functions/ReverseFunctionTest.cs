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
using org.SharpTiles.Expressions.Functions;

namespace org.SharpTiles.Expressions.Test.Functions
{
    [TestFixture]
    public class ReverseFunctionTest
    {
        [Test]
        public void TestNullReverse()
        {
            var function = new ReverseFunction();
            Assert.That(function.Evaluate(null), Is.EqualTo(null));
        }

        [Test]
        public void TestArrayReverse()
        {
            var function = new ReverseFunction();
            Assert.That(function.Evaluate(new[] {1, 2, 3}), Is.EqualTo(new[] {3, 2, 1}));
        }

        [Test]
        public void TestStringReverse()
        {
            var function = new ReverseFunction();
            Assert.That(function.Evaluate("abc"), Is.EqualTo("cba"));
        }

        [Test]
        public void TestListReverse()
        {
            var function = new ReverseFunction();
            var normal = new List<int>(new[] {1, 2, 3});
            var reversed = new List<int>(new[] {3, 2, 1});
            Assert.That(function.Evaluate(normal), Is.EqualTo(reversed));
        }

   }
}
