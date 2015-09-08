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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Expressions;
 using org.SharpTiles.NUnit;
 using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Tags.Test.CoreTags
{
    [TestFixture]
    public class SetTest
    {
        public string Null
        {
            get { return null; }
        }

        public string Value
        {
            get { return "<value>12345</value>"; }
        }

        public string Var
        {
            get { return "SomeVar"; }
        }

        private readonly Hashtable _model = new Hashtable();

        public Hashtable Model
        {
            get { return _model; }
        }


        private readonly Hashtable _page = new Hashtable();

        public Hashtable Page
        {
            get { return _page; }
        }


        public string Body
        {
            get { return "<body>12345</body>"; }
        }

        public VariableScope SessionScope
        {
            get { return VariableScope.Session; }
        }

        [Test]
        public void CheckWhenRequired()
        {
            var tag = new Set();
            try
            {
                RequiredAttribute.Check(tag);
                Assert.Fail("Expected exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message, Is.EqualTo(TagException.MissingRequiredAttribute(typeof (Set), "Var").Message));
            }
            tag.Var = new MockAttribute(new Property("Var"));
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void TestAttributesNoParamsAtAll()
        {
            var tag = new Set();
            var reflection = new TagModel(this);
            tag.Evaluate(reflection);
        }

        [Test]
        public void TestAttributesNoValueOrBody()
        {
            var tag = new Set();
            tag.Var = new MockAttribute(new Property("Var"));
            var reflection = new TagModel(this);
            tag.Evaluate(reflection);
        }

        [Test]
        public void TestAttributesSetByBody()
        {
            var tag = new Set();
            tag.Body = new MockAttribute(new Property("Body"));
            tag.Var = new MockAttribute(new Property("Var"));
            var reflection = new TagModel(this);
            tag.Evaluate(reflection);
            Assert.That(Body, Is.EqualTo(reflection["Page." + Var].ToString()));
        }

        [Test]
        public void TestAttributesSetByProperty()
        {
            var tag = new Set();
            tag.Value = new MockAttribute(new Property("Value"));
            tag.Var = new MockAttribute(new Property("Var"));
            var reflection = new TagModel(this);
            tag.Evaluate(reflection);
            Assert.That(Value, Is.EqualTo(reflection["Page." + Var].ToString()));
        }

        [Test]
        public void TestAttributesSetByPropertyPageScope()
        {
            var tag = new Set();
            tag.Value = new MockAttribute(new Property("Value"));
            tag.Var = new MockAttribute(new Property("Var"));
            tag.Scope = new MockAttribute(new Property("SessionScope"));
            var reflection = new TagModel(this, new MockSessionState());
            tag.Evaluate(reflection);
            Assert.That(Value, Is.EqualTo(reflection["Session." + Var].ToString()));
        }
    }
}
