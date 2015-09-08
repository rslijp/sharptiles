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
using org.SharpTiles.Expressions;
 using org.SharpTiles.NUnit;
 using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Tags.Test.CoreTags
{
    [TestFixture]
    public class IfTest
    {
        public string Null
        {
            get { return null; }
        }

        public string Body
        {
            get { return "<body>12345</body>"; }
        }

        public string VarName
        {
            get { return "Result"; }
        }

        public VariableScope SessionScope
        {
            get { return VariableScope.Session; }
        }

        public string False
        {
            get { return false.ToString(); }
        }

        public string True
        {
            get { return true.ToString(); }
        }

        [Test]
        public void CheckWhenRequired()
        {
            var tag = new If();
            try
            {
                RequiredAttribute.Check(tag);
                Assert.Fail("Expected exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message, Is.EqualTo(TagException.MissingRequiredAttribute(typeof (If), "Test").Message));
            }
            tag.Test = new MockAttribute(new Property("True"));
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void SimpleFalse()
        {
            var tag = new If();
            tag.Body = new MockAttribute(new Property("Body"));
            tag.Test = new MockAttribute(new Property("False"));
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo(String.Empty));
        }

        [Test]
        public void SimpleTrue()
        {
            var tag = new If();
            tag.Body = new MockAttribute(new Property("Body"));
            tag.Test = new MockAttribute(new Property("True"));
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo(Body));
        }

        [Test]
        public void VariableFalse()
        {
            var model = new TagModel(this);
            var tag = new If();
            tag.Body = new MockAttribute(new Property("Body"));
            tag.Test = new MockAttribute(new Property("False"));
            tag.Var = new MockAttribute(new Property("VarName"));
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo(String.Empty));
            Assert.That(model.Page[VarName], Is.Null);
        }

        [Test]
        public void VariableTrue()
        {
            var model = new TagModel(this);
            var tag = new If();
            tag.Body = new MockAttribute(new Property("Body"));
            tag.Test = new MockAttribute(new Property("True"));
            tag.Var = new MockAttribute(new Property("VarName"));
            Assert.That(tag.Evaluate(model), Is.EqualTo(String.Empty));
            Assert.That(model[VarName], Is.EqualTo(Body));
        }

        [Test]
        public void VariableTrueDifferentPageScope()
        {
            var session = new MockSessionState();
            var model = new TagModel(this, session);

            var tag = new If();
            tag.Body = new MockAttribute(new Property("Body"));
            tag.Test = new MockAttribute(new Property("True"));
            tag.Var = new MockAttribute(new Property("VarName"));
            tag.Scope = new MockAttribute(new Property("SessionScope"));
            Assert.That(tag.Evaluate(model), Is.EqualTo(String.Empty));
            Assert.That(model.Session[VarName], Is.EqualTo(Body));
        }
    }
}
