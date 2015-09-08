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
using System.Xml.XPath;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Expressions;
using org.SharpTiles.Tags.XmlTags;

namespace org.SharpTiles.Tags.Test.XmlTags
{
    [TestFixture]
    public class OutTest
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
            "<notes>" +
            "<note id=\"1\"><to>Tove</to><from>Jani</from><heading>Reminder</heading><body>Don't forget me this weekend!</body></note>" +
            "<note id=\"2\"><to>John</to><from>Tovi</from><heading>Not</heading><body>Read the previous memo &amp; note</body></note>" +
            "</notes>";

        private TagModel _model;


        [Test]
        public void CheckRequired()
        {
            var tag = new Out();
            try
            {
                RequiredAttribute.Check(tag);
                Assert.Fail("Expected Exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message,
                            Is.EqualTo(TagException.MissingRequiredAttribute(typeof (Out), "Source", "Select").Message));
            }
            tag.Select = new MockAttribute(new Constant("a"));
            tag.Source = new MockAttribute(new Constant("a"));
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void CheckSelect()
        {
            var tag = new Out();
            tag.Select = new MockAttribute(new Constant("//note/to"));
            tag.Source = new MockAttribute(new Constant("xml"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo("Tove, John"));
        }

        [Test]
        public void CheckSelectNoneExistingNode()
        {
            var tag = new Out();
            tag.Select = new MockAttribute(new Constant("//note[@id=\"3\"]/body"));
            tag.Source = new MockAttribute(new Constant("xml"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(String.Empty));
        }

        [Test]
        public void CheckSelectNoneExistingVar()
        {
            var tag = new Out();
            tag.Select = new MockAttribute(new Constant("//note[@id=\"1\"]/body"));
            tag.Source = new MockAttribute(new Constant("xmlnonexisting"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(String.Empty));
        }

        [Test]
        public void CheckSelectNoVar()
        {
            var tag = new Out();
            tag.Select = new MockAttribute(new Constant("//note[@id=\"1\"]/body"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(String.Empty));
        }

        [Test]
        public void CheckSelectNoXPath()
        {
            var tag = new Out();
            tag.Source = new MockAttribute(new Constant("xmlnonexisting"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(String.Empty));
        }

        [Test]
        public void CheckSelectOneNode()
        {
            var tag = new Out();
            tag.Select = new MockAttribute(new Constant("//note[@id=\"1\"]/to"));
            tag.Source = new MockAttribute(new Constant("xml"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo("Tove"));
        }

        [Test]
        public void CheckSelectOneNodeDefaultEscaping()
        {
            var tag = new Out();
            tag.Select = new MockAttribute(new Constant("//note[@id=\"2\"]/body"));
            tag.Source = new MockAttribute(new Constant("xml"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo("Read the previous memo &amp; note"));
        }

        [Test]
        public void CheckSelectOneNodeExplicitEscaping()
        {
            var tag = new Out();
            tag.Select = new MockAttribute(new Constant("//note[@id=\"2\"]/body"));
            tag.Source = new MockAttribute(new Constant("xml"));
            tag.EscapeXml = new MockAttribute(new Constant("true"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo("Read the previous memo &amp; note"));
        }

        [Test]
        public void CheckSelectOneNodeNoEscaping()
        {
            var tag = new Out();
            tag.Select = new MockAttribute(new Constant("//note[@id=\"2\"]/body"));
            tag.Source = new MockAttribute(new Constant("xml"));
            tag.EscapeXml = new MockAttribute(new Constant("false"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo("Read the previous memo & note"));
        }

        [Test]
        public void CheckSelectOneNodeNoEscapingSingleSlash()
        {
            var tag = new Out();
            tag.Select = new MockAttribute(new Constant("//notes/note[@id=\"2\"]/body"));
            tag.Source = new MockAttribute(new Constant("xml"));
            tag.EscapeXml = new MockAttribute(new Constant("false"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo("Read the previous memo & note"));
        }

        [Test]
        public void CheckSelectWrongXpathSyntax()
        {
            var tag = new Out();
            tag.Select = new MockAttribute(new Constant("//note[@id=\"3\"/body"));
            tag.Source = new MockAttribute(new Constant("xml"));
            try
            {
                Assert.That(tag.Evaluate(_model), Is.EqualTo(String.Empty));
                Assert.Fail("Expected exception");
            }
            catch (TagException XPe)
            {
                Assert.That(XPe.Message.StartsWith(TagException.IllegalXPath(new Exception("")).Message), Is.True);
            }
        }
    }
}
