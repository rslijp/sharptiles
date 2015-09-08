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
    public class JoinFunctionTest
    {
        public string[] NullString
        {
            get { return null; }
        }

        public string[] EmptyString
        {
            get { return new string[] {}; }
        }

        public string[] OneEntryString
        {
            get { return new[] {"one"}; }
        }

        public string[] FilledString
        {
            get { return new[] {"one", "two", "three"}; }
        }

        [Test]
        public void TestEmptySource()
        {
            var function = new JoinFunction();
            Assert.That(function.Evaluate(EmptyString, "-"), Is.EqualTo(""));
        }

        [Test]
        public void TestFilledEntrySource()
        {
            var function = new JoinFunction();
            Assert.That(function.Evaluate(FilledString, "-"), Is.EqualTo("one-two-three"));
        }

        [Test]
        public void TestFilledEntrySourceEmptySeperator()
        {
            var function = new JoinFunction();
            Assert.That(function.Evaluate(FilledString, ""), Is.EqualTo("onetwothree"));
        }

        [Test]
        public void TestFilledEntrySourceNullSeperator()
        {
            var function = new JoinFunction();
            Assert.That(function.Evaluate(FilledString, null), Is.EqualTo("onetwothree"));
        }

        [Test]
        public void TestNullSource()
        {
            var function = new JoinFunction();
            Assert.That(function.Evaluate(NullString, "-"), Is.EqualTo(""));
        }

        [Test]
        public void TestOneEntrySource()
        {
            var function = new JoinFunction();
            Assert.That(function.Evaluate(OneEntryString, "-"), Is.EqualTo("one"));
        }
    }
}
