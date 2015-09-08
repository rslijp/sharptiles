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
    public class SetTest : RequiresEnglishCulture
    {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp(); 
            _model = new TagModel(new Hashtable());
            var xdoc = new XmlDocument();
            xdoc.LoadXml(SAMPLE_XML);
            _model["xml"] = xdoc;
        }

        #endregion

        public const string SAMPLE_XML =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
            "<results>" +
            "<var id=\"1\"><type>Boolean</type><value>true</value></var>" +
            "<var id=\"2\"><type>Number</type><value>45.4</value></var>" +
            "<var id=\"3\"><type>SomeNode</type><value>*</value></var>" +
            "</results>";

        private TagModel _model;


        [Test]
        public void CheckRequired()
        {
            var tag = new Set();
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void CheckSelectBool()
        {
            var tag = new Set();
            tag.Select = new MockAttribute(new Constant("/results/var[@id=\"1\"]/value"));
            tag.Source = new MockAttribute(new Constant("xml"));
            tag.Var = new MockAttribute(new Constant("result"));
            tag.Evaluate(_model);
            Assert.That(_model["result"], Is.True);
        }

        [Test]
        public void CheckSelectFirstWithPosition()
        {
            var tag = new Set();
            tag.Select = new MockAttribute(new Constant("/results/var[position()=last()]/value"));
            tag.Source = new MockAttribute(new Constant("xml"));
            tag.Var = new MockAttribute(new Constant("result"));
            tag.Evaluate(_model);
            Assert.That(_model["result"] is XPathNavigator, Is.True);
            Assert.That(((XPathNavigator) _model["result"]).Value, Is.EqualTo("*"));
        }

        [Test]
        public void CheckSelectNode()
        {
            var tag = new Set();
            tag.Select = new MockAttribute(new Constant("results/var[@id=\"3\"]/value"));
            tag.Source = new MockAttribute(new Constant("xml"));
            tag.Var = new MockAttribute(new Constant("result"));
            tag.Evaluate(_model);
            Assert.That(_model["result"] is XPathNavigator, Is.True);
            Assert.That(((XPathNavigator) _model["result"]).Value, Is.EqualTo("*"));
        }


        [Test]
        public void CheckSelectNodeList()
        {
            var tag = new Set();
            tag.Select = new MockAttribute(new Constant("/results/*"));
            tag.Source = new MockAttribute(new Constant("xml"));
            tag.Var = new MockAttribute(new Constant("result"));
            tag.Evaluate(_model);
            Assert.That(_model["result"] is XPathNodeIterator);
        }

        [Test]
        public void CheckSelectNumber()
        {
            var tag = new Set();
            tag.Select = new MockAttribute(new Constant("/results/var[@id=\"2\"]/value"));
            tag.Source = new MockAttribute(new Constant("xml"));
            tag.Var = new MockAttribute(new Constant("result"));
            tag.Evaluate(_model);
            Assert.That(_model["result"], Is.EqualTo(45.4m));
        }
    }
}
