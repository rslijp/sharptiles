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
using System.Collections;
using System.Xml;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Expressions;
using org.SharpTiles.Tags.XmlTags;

namespace org.SharpTiles.Tags.Test.XmlTags
{
    [TestFixture]
    public class ChooseTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _model = new TagModel(new Hashtable());
            var xdoc = new XmlDocument();
            xdoc.LoadXml(SAMPLE_XML);
            _model["xml"] = xdoc;
        }

        #endregion

        public const string SAMPLE_XML =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
            "<results>" +
            "<value>true</value>" +
            "<value>false</value>" +
            "<value>oops</value>" +
            "<value></value>" +
            "</results>";

        private TagModel _model;

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
                Assert.That(Te.Message,
                            Is.EqualTo(TagException.MissingRequiredAttribute(typeof (When), "Source", "Select").Message));
            }
            tag.Select = new MockAttribute(new Constant("/results/value[position()=1]"));
            tag.Source = new MockAttribute(new Constant("xml"));
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void ChooseOneTrueTwoFalseAndOtherWise()
        {
            var tag = new Choose();
            var whenTrue = new When();
            whenTrue.Body = new MockAttribute(new Constant("True"));
            whenTrue.Select = new MockAttribute(new Constant("/results/value[position()=1]"));
            whenTrue.Source = new MockAttribute(new Constant("xml"));
            tag.AddNestedTag(whenTrue);
            var whenFalse = new When();
            whenFalse.Body = new MockAttribute(new Constant("False1"));
            whenFalse.Select = new MockAttribute(new Constant("/results/value[position()=4]"));
            whenFalse.Source = new MockAttribute(new Constant("xml"));
            tag.AddNestedTag(whenFalse);
            var whenFalse2 = new When();
            whenFalse2.Body = new MockAttribute(new Constant("False2"));
            whenFalse2.Select = new MockAttribute(new Constant("/results/value[position()=2]"));
            whenFalse2.Source = new MockAttribute(new Constant("xml"));
            tag.AddNestedTag(whenFalse2);
            var otherwise = new Otherwise();
            otherwise.Body = new MockAttribute(new Constant("Otherwise"));
            tag.AddNestedTag(otherwise);
            Assert.That(tag.Evaluate(_model), Is.EqualTo("True"));
        }

        [Test]
        public void ChooseOnlyOneWhenFalse()
        {
            var tag = new Choose();
            var whenFalse = new When();
            whenFalse.Body = new MockAttribute(new Constant("Body2"));
            whenFalse.Select = new MockAttribute(new Constant("/results/value[position()=2]"));
            whenFalse.Source = new MockAttribute(new Constant("xml"));
            tag.AddNestedTag(whenFalse);
            Assert.That(tag.Evaluate(_model), Is.EqualTo(String.Empty));
        }

        [Test]
        public void ChooseOnlyOneWhenTrue()
        {
            var tag = new Choose();
            var whenTrue = new When();
            whenTrue.Body = new MockAttribute(new Constant("Body1"));
            whenTrue.Select = new MockAttribute(new Constant("/results/value[position()=1]"));
            whenTrue.Source = new MockAttribute(new Constant("xml"));
            tag.AddNestedTag(whenTrue);
            Assert.That(tag.Evaluate(_model), Is.EqualTo("Body1"));
        }

        [Test]
        public void ChooseOnlyOtherwise()
        {
            var tag = new Choose();
            var otherwise = new Otherwise();
            otherwise.Body = new MockAttribute(new Constant("Body1"));
            tag.AddNestedTag(otherwise);
            Assert.That(tag.Evaluate(_model), Is.EqualTo("Body1"));
        }

        [Test]
        public void ChooseThreeTrueAndOtherWise()
        {
            var tag = new Choose();
            var whenTrue = new When();
            whenTrue.Body = new MockAttribute(new Constant("True1"));
            whenTrue.Select = new MockAttribute(new Constant("/results/value[position()=1]"));
            whenTrue.Source = new MockAttribute(new Constant("xml"));
            tag.AddNestedTag(whenTrue);
            var whenTrue2 = new When();
            whenTrue2.Body = new MockAttribute(new Constant("True2"));
            whenTrue2.Select = new MockAttribute(new Constant("/results/value[position()=1]"));
            whenTrue2.Source = new MockAttribute(new Constant("xml"));
            tag.AddNestedTag(whenTrue2);
            var whenTrue3 = new When();
            whenTrue3.Body = new MockAttribute(new Constant("True3"));
            whenTrue3.Select = new MockAttribute(new Constant("/results/value[position()=1]"));
            whenTrue3.Source = new MockAttribute(new Constant("xml"));
            tag.AddNestedTag(whenTrue2);
            var otherwise = new Otherwise();
            otherwise.Body = new MockAttribute(new Constant("Ötherwise"));
            tag.AddNestedTag(otherwise);
            Assert.That(tag.Evaluate(_model), Is.EqualTo("True1"));
        }

        [Test]
        public void ChooseTrueAndFalse()
        {
            var tag = new Choose();
            var whenTrue = new When();
            whenTrue.Body = new MockAttribute(new Constant("Body1"));
            whenTrue.Select = new MockAttribute(new Constant("/results/value[position()=1]"));
            whenTrue.Source = new MockAttribute(new Constant("xml"));
            tag.AddNestedTag(whenTrue);
            var whenFalse = new When();
            whenFalse.Body = new MockAttribute(new Constant("Body2"));
            whenFalse.Select = new MockAttribute(new Constant("/results/value[position()=2]"));
            whenFalse.Source = new MockAttribute(new Constant("xml"));
            tag.AddNestedTag(whenFalse);
            Assert.That(tag.Evaluate(_model), Is.EqualTo("Body1"));
        }

        [Test]
        public void ChooseTwoFalseAndOtherWise()
        {
            var tag = new Choose();
            var whenFalse = new When();
            whenFalse.Body = new MockAttribute(new Constant("Body1"));
            whenFalse.Select = new MockAttribute(new Constant("/results/value[position()=2]"));
            whenFalse.Source = new MockAttribute(new Constant("xml"));
            tag.AddNestedTag(whenFalse);
            var whenFalse2 = new When();
            whenFalse2.Body = new MockAttribute(new Constant("Body2"));
            whenFalse2.Select = new MockAttribute(new Constant("/results/value[position()=2]"));
            whenFalse2.Source = new MockAttribute(new Constant("xml"));
            tag.AddNestedTag(whenFalse2);
            var otherwise = new Otherwise();
            otherwise.Body = new MockAttribute(new Constant("Body3"));
            tag.AddNestedTag(otherwise);
            Assert.That(tag.Evaluate(_model), Is.EqualTo("Body3"));
        }
    }
}
