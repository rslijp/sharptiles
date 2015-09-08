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
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Tags.Test.CoreTags
{
    [TestFixture]
    public class ChooseTest
    {
        public void BadTag()
        {
            var tag = new Choose();
            var outTag = new Out();
            try
            {
                tag.AddNestedTag(outTag);
            }
            catch (TagException Te)
            {
                Assert.AreEqual(Te.Message,
                                TagException.OnlyNestedTagsOfTypeAllowed(typeof (Out), typeof (When), typeof (When)));
            }
        }

        public string Null
        {
            get { return null; }
        }

        public string Body1
        {
            get { return "<body>1</body>"; }
        }

        public string Body2
        {
            get { return "<body>2</body>"; }
        }

        public string Body3
        {
            get { return "<body>3</body>"; }
        }

        public string Body4
        {
            get { return "<body>4</body>"; }
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
        public void CheckOtherwiseRequired()
        {
            var tag = new Otherwise();
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void CheckRequired()
        {
            var tag = new Choose();
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void CheckWhenRequired()
        {
            var tag = new When();
            try
            {
                RequiredAttribute.Check(tag);
                Assert.Fail("Expected exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message, Is.EqualTo(TagException.MissingRequiredAttribute(typeof (When), "Test").Message));
            }
            tag.Test = new MockAttribute(new Property("True"));
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void ChooseOneTrueTwoFalseAndOtherWise()
        {
            var tag = new Choose();
            var whenTrue = new When();
            whenTrue.Body = new MockAttribute(new Property("Body3"));
            whenTrue.Test = new MockAttribute(new Property("True"));
            tag.AddNestedTag(whenTrue);
            var whenFalse = new When();
            whenFalse.Body = new MockAttribute(new Property("Body2"));
            whenFalse.Test = new MockAttribute(new Property("False"));
            tag.AddNestedTag(whenFalse);
            var whenFalse2 = new When();
            whenFalse2.Body = new MockAttribute(new Property("Body2"));
            whenFalse2.Test = new MockAttribute(new Property("False"));
            tag.AddNestedTag(whenFalse2);
            var otherwise = new Otherwise();
            otherwise.Body = new MockAttribute(new Property("Body1"));
            tag.AddNestedTag(otherwise);
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo(Body3));
        }

        [Test]
        public void ChooseOnlyOneWhenFalse()
        {
            var tag = new Choose();
            var whenFalse = new When();
            whenFalse.Body = new MockAttribute(new Property("Body2"));
            whenFalse.Test = new MockAttribute(new Property("False"));
            tag.AddNestedTag(whenFalse);
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo(String.Empty));
        }

        [Test]
        public void ChooseOnlyOneWhenTrue()
        {
            var tag = new Choose();
            var whenTrue = new When();
            whenTrue.Body = new MockAttribute(new Property("Body2"));
            whenTrue.Test = new MockAttribute(new Property("True"));
            tag.AddNestedTag(whenTrue);
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo(Body2));
        }

        [Test]
        public void ChooseOnlyOtherwise()
        {
            var tag = new Choose();
            var otherwise = new Otherwise();
            otherwise.Body = new MockAttribute(new Property("Body1"));
            tag.AddNestedTag(otherwise);
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo(Body1));
        }

        [Test]
        public void ChooseThreeTrueAndOtherWise()
        {
            var tag = new Choose();
            var whenTrue = new When();
            whenTrue.Body = new MockAttribute(new Property("Body1"));
            whenTrue.Test = new MockAttribute(new Property("True"));
            tag.AddNestedTag(whenTrue);
            var whenTrue2 = new When();
            whenTrue2.Body = new MockAttribute(new Property("Body2"));
            whenTrue2.Test = new MockAttribute(new Property("True"));
            tag.AddNestedTag(whenTrue2);
            var whenTrue3 = new When();
            whenTrue3.Body = new MockAttribute(new Property("Body3"));
            whenTrue3.Test = new MockAttribute(new Property("True"));
            tag.AddNestedTag(whenTrue2);
            var otherwise = new Otherwise();
            otherwise.Body = new MockAttribute(new Property("Body4"));
            tag.AddNestedTag(otherwise);
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo(Body1));
        }

        [Test]
        public void ChooseTrueAndFalse()
        {
            var tag = new Choose();
            var whenTrue = new When();
            whenTrue.Body = new MockAttribute(new Property("Body2"));
            whenTrue.Test = new MockAttribute(new Property("True"));
            tag.AddNestedTag(whenTrue);
            var whenFalse = new When();
            whenFalse.Body = new MockAttribute(new Property("Body2"));
            whenFalse.Test = new MockAttribute(new Property("False"));
            tag.AddNestedTag(whenFalse);
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo(Body2));
        }

        [Test]
        public void ChooseTwoFalseAndOtherWise()
        {
            var tag = new Choose();
            var whenFalse = new When();
            whenFalse.Body = new MockAttribute(new Property("Body2"));
            whenFalse.Test = new MockAttribute(new Property("False"));
            tag.AddNestedTag(whenFalse);
            var whenFalse2 = new When();
            whenFalse2.Body = new MockAttribute(new Property("Body2"));
            whenFalse2.Test = new MockAttribute(new Property("False"));
            tag.AddNestedTag(whenFalse2);
            var otherwise = new Otherwise();
            otherwise.Body = new MockAttribute(new Property("Body1"));
            tag.AddNestedTag(otherwise);
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo(Body1));
        }
    }
}
