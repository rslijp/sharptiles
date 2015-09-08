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
    public class EscapeXmlFunctionTest
    {
        [Test]
        public void TestEscapeAmpersand()
        {
            var function = new EscapeXmlFunction();
            Assert.That(function.Evaluate("Johnson & Johnon"), Is.EqualTo("Johnson &amp; Johnon"));
        }

        [Test]
        public void TestEscapeApostrorhe()
        {
            var function = new EscapeXmlFunction();
            Assert.That(function.Evaluate("some 'text'"), Is.EqualTo("some &apos;text&apos;"));
        }

        [Test]
        public void TestEscapeNull()
        {
            var function = new EscapeXmlFunction();
            Assert.That(function.Evaluate(new string[1]), Is.EqualTo(""));
        }

        [Test]
        public void TestEscapeQuotes()
        {
            var function = new EscapeXmlFunction();
            Assert.That(function.Evaluate("some \"text\""), Is.EqualTo("some &quot;text&quot;"));
        }

        [Test]
        public void TestEscapTags()
        {
            var function = new EscapeXmlFunction();
            Assert.That(function.Evaluate("You are <b>fired</b>"), Is.EqualTo("You are &lt;b&gt;fired&lt;/b&gt;"));
        }


        [Test]
        public void TestNestedAndMixed()
        {
            var function = new EscapeXmlFunction();
            Assert.That(function.Evaluate(
                            "I said use <code>\" not '</code> at <i>AT&T</i>. You are <b>fired</b>"),
                        Is.EqualTo(
                            "I said use &lt;code&gt;&quot; not &apos;&lt;/code&gt; at &lt;i&gt;AT&amp;T&lt;/i&gt;. You are &lt;b&gt;fired&lt;/b&gt;"));
        }

        [Test]
        public void TestNothingToEscape()
        {
            var function = new EscapeXmlFunction();
            Assert.That(function.Evaluate("some text"), Is.EqualTo("some text"));
        }
    }
}
