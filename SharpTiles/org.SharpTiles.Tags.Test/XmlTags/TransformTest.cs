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
using System.IO;
using System.Xml.XPath;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Expressions;
using org.SharpTiles.Tags.XmlTags;

namespace org.SharpTiles.Tags.Test.XmlTags
{
    [TestFixture]
    public class TransformTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _model = new TagModel(new Hashtable());
            var xdoc = new XPathDocument(new StringReader(SAMPLE_XML));
            var xslt = new XPathDocument(new StringReader(SAMPLE_XSLT));
            _model["xml"] = xdoc;
            _model["xslt"] = xslt;
        }

        #endregion

        public const string SAMPLE_XML =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
            "<notes>" +
            "<note id=\"1\"><to>Tove</to><from>Jani</from><heading>Reminder</heading><body>Don't forget me this weekend!</body></note>" +
            "<note id=\"2\"><to>John</to><from>Tovi</from><heading>Not</heading><body>Read the previous memo &amp; note</body></note>" +
            "</notes>";

        public const string SAMPLE_XSLT =
            "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>" +
            "<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">" +
            "<xsl:template match=\"notes\"><ul><xsl:for-each select=\"note\"><li><xsl:value-of select=\"body\"/></li></xsl:for-each></ul></xsl:template>" +
            "</xsl:stylesheet>";

        public const string SAMPLE_XSLT_WITH_PARAMS =
            "<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">" +
            "<xsl:param name=\"testparam\">Default</xsl:param>" +
            "<xsl:template match=\"/notes\">" +
            "<test>" +
            "<xsl:value-of select=\"$testparam\"/>" +
            "</test>" +
            "</xsl:template>" +
            "</xsl:stylesheet>";


        public const string SAMPLE_HTML =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<ul>" +
            "<li>Don't forget me this weekend!</li>" +
            "<li>Read the previous memo &amp; note</li>" +
            "</ul>";

        public const string SAMPLE_HTML_WITH_PARAMS =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<test>" +
            "SharpTiles" +
            "</test>";

        private TagModel _model;


        [Test]
        public void CheckRequired()
        {
            var tag = new Transform();
            try
            {
                RequiredAttribute.Check(tag);
                Assert.Fail("Expected Exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message,
                            Is.EqualTo(TagException.MissingRequiredAttribute(typeof (Transform), "Doc", "Xslt").Message));
            }
            tag.Doc = new MockAttribute(new Constant("a"));
            tag.Xslt = new MockAttribute(new Constant("a"));
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void SimpleTransform()
        {
            var tag = new Transform();
            tag.Doc = new MockAttribute(new Property("xml"));
            tag.Xslt = new MockAttribute(new Property("xslt"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(SAMPLE_HTML));
        }

        [Test]
        public void SimpleTransformAsString()
        {
            var tag = new Transform();
            tag.Doc = new MockAttribute(new Property("xml"));
            tag.Xslt = new MockAttribute(new Constant(SAMPLE_XSLT));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(SAMPLE_HTML));
        }

        [Test]
        public void SimpleTransformWithParams()
        {
            var tag = new Transform();
            tag.Doc = new MockAttribute(new Property("xml"));
            tag.Xslt = new MockAttribute(new Constant(SAMPLE_XSLT_WITH_PARAMS));
            var param = new Param();
            param.Name = new MockAttribute(new Constant("testparam"));
            param.Body = new MockAttribute(new Constant("SharpTiles"));
            tag.AddNestedTag(param);
            Assert.That(tag.Evaluate(_model), Is.EqualTo(SAMPLE_HTML_WITH_PARAMS));
        }

        [Test]
        public void TransformEmptyDocVar()
        {
            var tag = new Transform();
            tag.Doc = new MockAttribute(new Property("noxml"));
            tag.Xslt = new MockAttribute(new Property("xslt"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(String.Empty));
        }

        [Test]
        public void TransformEmptyTransformVar()
        {
            var tag = new Transform();
            tag.Doc = new MockAttribute(new Property("xml"));
            tag.Xslt = new MockAttribute(new Property("noxslt"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(String.Empty));
        }
    }
}
