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
 using System.Collections;
using System.IO;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Expressions;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.CoreTags;
using org.SharpTiles.Tags.Templates.SharpTags;
using org.SharpTiles.Templates.SharpTags;

namespace org.SharpTiles.Templates.Test.SharpTags
{
    [TestFixture]
    public class ScopeTest
    {
        [SetUp]
        public void SetUp()
        {
            new Formatter("Triggers init");
        }

        [Test]
        public void CheckNothingRequired()
        {
            var tag = new Scope();
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void CheckPassThroughOfContentParsed()
        {
            ITag tag = CreateFactory().Parse("<sharp:scope>aa</sharp:scope>");
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("aa"));
        }

        private static TagLibParserFactory CreateFactory()
        {
            var lib = new TagLib();
            lib.Register(new Sharp());
            return new TagLibParserFactory(new TagLibForParsing(lib));
        }

        [Test]
        public void CheckPassThroughOfmodel()
        {
            Hashtable table = new Hashtable();
            table.Add("niceValue", "Hello");
            ITag tag = CreateFactory().Parse("<sharp:scope><c:out value=\"${niceValue}\"/></sharp:scope>");
            Assert.That(tag.Evaluate(new TagModel(table)), Is.EqualTo("Hello"));
        }

        [Test]
        public void CheckCreationOfNewScope()
        {
            Hashtable table = new Hashtable();
            table.Add("niceValue", "Hello");
            ITag tag = CreateFactory().Parse("<sharp:scope><c:set value=\"Hi\" var=\"niceValue\" scope=\"Tag\"/><c:out value=\"${niceValue}\"/></sharp:scope>");
            Assert.That(tag.Evaluate(new TagModel(table)), Is.EqualTo("Hi"));
            Assert.That(table["niceValue"], Is.EqualTo("Hello"));
        }
    }
}
