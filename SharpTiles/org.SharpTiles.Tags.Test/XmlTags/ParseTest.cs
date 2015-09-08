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
using System.Xml;
using System.Xml.XPath;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Expressions;
using org.SharpTiles.Tags.XmlTags;

namespace org.SharpTiles.Tags.Test.XmlTags
{
    [TestFixture]
    public class ParseTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _model = new TagModel(new Hashtable());
        }

        #endregion

        public const string SAMPLE_XML =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?><note><to>Tove</to><from>Jani</from><heading>Reminder</heading><body>Don't forget me this weekend!</body></note>";

        private TagModel _model;


        [Test]
        public void CheckParseOfDocument_InBody()
        {
            var tag = new Parse();
            tag.Var = new MockAttribute(new Constant("result"));
            tag.Body = new MockAttribute(new Constant(SAMPLE_XML));
            tag.Evaluate(_model);
            Assert.That(_model["result"], Is.Not.Null);
            Assert.That(_model["result"].GetType(), Is.EqualTo(typeof (XPathDocument)));
            var xDoc = (XPathDocument) _model["result"];
            XPathNavigator navigator = xDoc.CreateNavigator();
            navigator.MoveToFirstChild();
            Assert.That(navigator.LocalName, Is.EqualTo("note"));
        }

        [Test]
        public void CheckParseOfDocument_InDoc()
        {
            var tag = new Parse();
            tag.Var = new MockAttribute(new Constant("result"));
            tag.Doc = new MockAttribute(new Constant(SAMPLE_XML));
            tag.Evaluate(_model);
            Assert.That(_model["result"], Is.Not.Null);
            Assert.That(_model["result"].GetType(), Is.EqualTo(typeof (XPathDocument)));
            var xDoc = (XPathDocument) _model["result"];
            XPathNavigator navigator = xDoc.CreateNavigator();
            navigator.MoveToFirstChild();
            Assert.That(navigator.LocalName, Is.EqualTo("note"));
        }

        [Test]
        public void CheckParseOfDocument_InDoc_AsVariableOfTextReader()
        {
            using (var reader = new StringReader(SAMPLE_XML))
            {
                _model["input"] = reader;
                var tag = new Parse();
                tag.Var = new MockAttribute(new Constant("result"));
                tag.Doc = new MockAttribute(new Property("input"));
                tag.Evaluate(_model);
                Assert.That(_model["result"].GetType(), Is.EqualTo(typeof (XPathDocument)));
                var xDoc = (XPathDocument) _model["result"];
                XPathNavigator navigator = xDoc.CreateNavigator();
                navigator.MoveToFirstChild();
                Assert.That(navigator.LocalName, Is.EqualTo("note"));
            }
        }

        [Test]
        public void CheckParseOfDocument_InDoc_AsVariableOfTypeSream()
        {
            byte[] data = null;
            using (var stream = new MemoryStream())
            {
                var aDoc = new XmlDocument();
                aDoc.LoadXml(SAMPLE_XML);
                aDoc.Save(stream);
                data = stream.ToArray();
            }
            Assert.That(data.Length, Is.GreaterThan(0));
            using (var stream = new MemoryStream(data))
            {
                _model["input"] = stream;
                var tag = new Parse();
                tag.Var = new MockAttribute(new Constant("result"));
                tag.Doc = new MockAttribute(new Property("input"));
                tag.Evaluate(_model);
                Assert.That(_model["result"], Is.Not.Null);
                Assert.That(_model["result"].GetType(), Is.EqualTo(typeof (XPathDocument)));
                var xDoc = (XPathDocument) _model["result"];
                XPathNavigator navigator = xDoc.CreateNavigator();
                navigator.MoveToFirstChild();
                Assert.That(navigator.LocalName, Is.EqualTo("note"));
            }
        }

        [Test]
        public void CheckParseOfDocument_InDoc_AsVariableOfTypeString()
        {
            _model["input"] = SAMPLE_XML;
            var tag = new Parse();
            tag.Var = new MockAttribute(new Constant("result"));
            tag.Doc = new MockAttribute(new Property("input"));
            tag.Evaluate(_model);
            Assert.That(_model["result"], Is.Not.Null);
            Assert.That(_model["result"].GetType(), Is.EqualTo(typeof (XPathDocument)));
            var xDoc = (XPathDocument) _model["result"];
            XPathNavigator navigator = xDoc.CreateNavigator();
            navigator.MoveToFirstChild();
            Assert.That(navigator.LocalName, Is.EqualTo("note"));
        }

        [Test]
        public void CheckParseOfDocument_InDoc_AsVariableOfTypeXmlDocument()
        {
            TextReader reader = new StringReader(SAMPLE_XML);
            var input = new XPathDocument(reader);
            _model["input"] = input;
            var tag = new Parse();
            tag.Var = new MockAttribute(new Constant("result"));
            tag.Doc = new MockAttribute(new Property("input"));
            tag.Evaluate(_model);
            Assert.That(_model["result"], Is.Not.Null);
            Assert.That(_model["result"].GetType(), Is.EqualTo(typeof (XPathDocument)));
            var xDoc = (XPathDocument) _model["result"];
            XPathNavigator navigator = xDoc.CreateNavigator();
            navigator.MoveToFirstChild();
            Assert.That(navigator.LocalName, Is.EqualTo("note"));
        }

        [Test]
        public void CheckParseOfDocument_InDoc_AsVariableOfXmlReader()
        {
            using (var reader = new StringReader(SAMPLE_XML))
            using (XmlReader xmlReader = XmlReader.Create(reader))
            {
                _model["input"] = xmlReader;
                var tag = new Parse();
                tag.Var = new MockAttribute(new Constant("result"));
                tag.Doc = new MockAttribute(new Property("input"));
                tag.Evaluate(_model);
                Assert.That(_model["result"], Is.Not.Null);
                Assert.That(_model["result"].GetType(), Is.EqualTo(typeof (XPathDocument)));
                var xDoc = (XPathDocument) _model["result"];
                XPathNavigator navigator = xDoc.CreateNavigator();
                navigator.MoveToFirstChild();
                Assert.That(navigator.LocalName, Is.EqualTo("note"));
            }
        }

        [Test]
        public void CheckParseOfDocumentEmptyString()
        {
            var tag = new Parse();
            tag.Var = new MockAttribute(new Constant("result"));
            tag.Body = new MockAttribute(new Constant(""));
            tag.Evaluate(_model);
            Assert.That(_model["result"], Is.Null);
        }

        [Test]
        public void CheckParseOfDocumentNull()
        {
            var tag = new Parse();
            tag.Var = new MockAttribute(new Constant("result"));
            tag.Evaluate(_model);
            Assert.That(_model["result"], Is.Null);
        }

        [Test]
        public void CheckParsing()
        {
            var tag = new Parse();
            tag.Var = new MockAttribute(new Constant("result"));
            tag.Body = new MockAttribute(new Constant(SAMPLE_XML));
            tag.Evaluate(_model);
            Assert.That(_model["result"], Is.Not.Null);
            Assert.That(_model["result"].GetType(), Is.EqualTo(typeof (XPathDocument)));
            var xDoc = (XPathDocument) _model["result"];
            XPathNavigator navigator = xDoc.CreateNavigator();
            navigator.MoveToFirstChild();
            Assert.That(navigator.LocalName, Is.EqualTo("note"));
            navigator.MoveToFirstChild();
            Assert.That(navigator.LocalName, Is.EqualTo("to"));
            Assert.That(navigator.Value, Is.EqualTo("Tove"));
            Assert.IsTrue(navigator.MoveToNext());
            Assert.That(navigator.LocalName, Is.EqualTo("from"));
            Assert.That(navigator.Value, Is.EqualTo("Jani"));
            Assert.IsTrue(navigator.MoveToNext());
            Assert.That(navigator.LocalName, Is.EqualTo("heading"));
            Assert.That(navigator.Value, Is.EqualTo("Reminder"));
            Assert.IsTrue(navigator.MoveToNext());
            Assert.That(navigator.LocalName, Is.EqualTo("body"));
            Assert.That(navigator.Value, Is.EqualTo("Don't forget me this weekend!"));
            Assert.IsFalse(navigator.MoveToNext());
        }

        [Test]
        public void CheckRequired()
        {
            var tag = new Parse();
            try
            {
                RequiredAttribute.Check(tag);
                Assert.Fail("Expected Exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message, Is.EqualTo(TagException.MissingRequiredAttribute(typeof (Parse), "Var").Message));
            }
            tag.Var = new MockAttribute(new Constant("a"));
            RequiredAttribute.Check(tag);
        }
    }
}
