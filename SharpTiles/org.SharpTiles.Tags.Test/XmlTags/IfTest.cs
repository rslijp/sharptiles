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
    public class IfTest
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


        [Test]
        public void CheckRequired()
        {
            var tag = new If();
            try
            {
                RequiredAttribute.Check(tag);
                Assert.Fail("Expected Exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message,
                            Is.EqualTo(TagException.MissingRequiredAttribute(typeof (If), "Source", "Select").Message));
            }
            tag.Select = new MockAttribute(new Constant("a"));
            tag.Source = new MockAttribute(new Constant("a"));
            RequiredAttribute.Check(tag);
        }


        [Test]
        public void IfApplies()
        {
            var tag = new If();
            tag.Select = new MockAttribute(new Constant("/results/value[position()=1]"));
            tag.Source = new MockAttribute(new Constant("xml"));
            tag.Body = new MockAttribute(new Constant("Show me"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo("Show me"));
        }

        [Test]
        public void IfDontApply()
        {
            var tag = new If();
            tag.Select = new MockAttribute(new Constant("/results/value[position()=last()]"));
            tag.Source = new MockAttribute(new Constant("xml"));
            tag.Body = new MockAttribute(new Constant("Show me"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(String.Empty));
        }

        [Test]
        public void IfDontApplyBecauseOfNullReturningXPath()
        {
            var tag = new If();
            tag.Select = new MockAttribute(new Constant("/results/value[position()=99]"));
            tag.Source = new MockAttribute(new Constant("xml"));
            tag.Body = new MockAttribute(new Constant("Show me"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(String.Empty));
        }

        [Test]
        public void IfShouldThrowExceptionWhenEmptyStringIsFound()
        {
            var tag = new If();
            tag.Select = new MockAttribute(new Constant("/results/value[position()=4]"));
            tag.Source = new MockAttribute(new Constant("xml"));
            tag.Body = new MockAttribute(new Constant("Show me"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(String.Empty));
        }

        [Test]
        public void IfShouldThrowExceptionWhenNoneBoolIsFound()
        {
            var tag = new If
                  {
                      Select = new MockAttribute(new Constant("/results/value[position()=3]")),
                      Source = new MockAttribute(new Constant("xml")),
                      Body = new MockAttribute(new Constant("Show me"))
                  };
            try
            {
                tag.Evaluate(_model);
                Assert.Fail("Expected exception");
            }
            catch (TagException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(TagException.IllegalXPath(new FormatException()).Message);
                Assert.That(e.Message.StartsWith(TagException.IllegalXPath(new FormatException("")).Message));
            }
        }
    }
}