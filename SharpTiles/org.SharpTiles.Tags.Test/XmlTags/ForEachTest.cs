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
using org.SharpTiles.Templates;

namespace org.SharpTiles.Tags.Test.XmlTags
{
    [TestFixture]
    public class ForEachTest
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
            "<value flag=\"uppercase\">A</value>" +
            "<value flag=\"lowercase\">b</value>" +
            "<value flag=\"uppercase\">C</value>" +
            "<value flag=\"lowercase\">d</value>" +
            "<value flag=\"uppercase\">E</value>" +
            "<value flag=\"lowercase\">f</value>" +
            "<value flag=\"uppercase\">G</value>" +
            "<value flag=\"lowercase\">h</value>" +
            "<value flag=\"uppercase\">I</value>" +
            "<value flag=\"lowercase\">j</value>" +
            "<value flag=\"uppercase\">K</value>" +
            "<value flag=\"lowercase\">l</value>" +
            "<value flag=\"uppercase\">M</value>" +
            "<value flag=\"lowercase\">n</value>" +
            "<value flag=\"uppercase\">O</value>" +
            "<value flag=\"lowercase\">p</value>" +
            "<value flag=\"uppercase\">Q</value>" +
            "<value flag=\"lowercase\">r</value>" +
            "<value flag=\"uppercase\">S</value>" +
            "<value flag=\"lowercase\">t</value>" +
            "<value flag=\"uppercase\">U</value>" +
            "<value flag=\"lowercase\">v</value>" +
            "<value flag=\"uppercase\">W</value>" +
            "<value flag=\"lowercase\">x</value>" +
            "<value flag=\"uppercase\">Y</value>" +
            "<value flag=\"lowercase\">z</value>" +
            "</results>";

        private TagModel _model;


        [Test]
        public void CheckRequired()
        {
            var tag = new ForEach();
            try
            {
                RequiredAttribute.Check(tag);
                Assert.Fail("Expected exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message,
                            Is.EqualTo(
                                TagException.MissingRequiredAttribute(typeof (ForEach), "Source", "Select").Message));
            }
            tag.Select = new MockAttribute(new Constant("AList"));
            tag.Source = new MockAttribute(new Constant("ASource"));
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void LoopNoBody()
        {
            var tag = new ForEach();
            tag.Select = new MockAttribute(new Constant("//value"));
            tag.Source = new MockAttribute(new Constant("xml"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(String.Empty));
        }

        [Test]
        public void LoopWithBody()
        {
            var tag = new ForEach();
            tag.Select = new MockAttribute(new Constant("//value"));
            tag.Source = new MockAttribute(new Constant("xml"));
            tag.Body = new MockAttribute(new Constant("."));

            Assert.That(tag.Evaluate(_model), Is.EqualTo(".........................."));
        }

        [Test]
        public void LoopWithComplexStatusBody()
        {
            var tag = new ForEach();
            tag.Select = new MockAttribute(new Constant("//value"));
            tag.Source = new MockAttribute(new Constant("xml"));
            tag.End = new MockAttribute(new Constant("7"));
            tag.Body = new TemplateAttribute(new Formatter("[${Status.Index}/${Status.Count}]").ParsedTemplate);
            Assert.That(tag.Evaluate(_model), Is.EqualTo("[0/26][1/26][2/26][3/26][4/26][5/26][6/26]"));
        }

        [Test]
        public void LoopWithEndBody()
        {
            var tag = new ForEach();
            tag.Select = new MockAttribute(new Constant("//value"));
            tag.Source = new MockAttribute(new Constant("xml"));
            tag.Body = new TemplateAttribute(new Formatter("<x:out source=\"Item\" select=\".\"/>").ParsedTemplate);
            tag.End = new MockAttribute(new Constant("13"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo("AbCdEfGhIjKlM"));
        }

        [Test]
        public void LoopWithEvaluatingBody()
        {
            var tag = new ForEach();
            tag.Select = new MockAttribute(new Constant("//value"));
            tag.Source = new MockAttribute(new Constant("xml"));
            tag.Body = new TemplateAttribute(new Formatter("<x:out source=\"Item\" select=\".\"/>").ParsedTemplate);
            Assert.That(tag.Evaluate(_model), Is.EqualTo("AbCdEfGhIjKlMnOpQrStUvWxYz"));
        }

        [Test]
        public void LoopWithFirstBody()
        {
            var tag = new ForEach();
            tag.Select = new MockAttribute(new Constant("//value"));
            tag.Source = new MockAttribute(new Constant("xml"));
            tag.Body = new TemplateAttribute(new Formatter("<x:out source=\"Item\" select=\".\"/>").ParsedTemplate);
            tag.Begin = new MockAttribute(new Constant("13"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo("nOpQrStUvWxYz"));
        }

        [Test]
        public void LoopWithStartEndBody()
        {
            var tag = new ForEach();
            tag.Select = new MockAttribute(new Constant("//value"));
            tag.Source = new MockAttribute(new Constant("xml"));
            tag.Body = new TemplateAttribute(new Formatter("<x:out source=\"Item\" select=\".\"/>").ParsedTemplate);
            tag.Begin = new MockAttribute(new Constant("4"));
            tag.End = new MockAttribute(new Constant("13"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo("EfGhIjKlM"));
        }

        [Test]
        public void LoopWithStepAndBeginBody()
        {
            var tag = new ForEach();
            tag.Select = new MockAttribute(new Constant("//value"));
            tag.Source = new MockAttribute(new Constant("xml"));
            tag.Body = new TemplateAttribute(new Formatter("<x:out source=\"Item\" select=\".\"/>").ParsedTemplate);
            tag.Begin = new MockAttribute(new Constant("13"));
            tag.Step = new MockAttribute(new Constant("2"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo("nprtvxz"));
        }

        [Test]
        public void LoopWithStepAndBody()
        {
            var tag = new ForEach();
            tag.Select = new MockAttribute(new Constant("//value"));
            tag.Source = new MockAttribute(new Constant("xml"));
            tag.Body = new TemplateAttribute(new Formatter("<x:out source=\"Item\" select=\".\"/>").ParsedTemplate);
            tag.Step = new MockAttribute(new Constant("2"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo("ACEGIKMOQSUWY"));
        }

        [Test]
        public void LoopWithStepAndBodyWithFilterInSelect()
        {
            var tag = new ForEach();
            tag.Select = new MockAttribute(new Constant("//value[@flag=\"uppercase\"]"));
            tag.Source = new MockAttribute(new Constant("xml"));
            tag.Body = new TemplateAttribute(new Formatter("<x:out source=\"Item\" select=\".\"/>").ParsedTemplate);
            Assert.That(tag.Evaluate(_model), Is.EqualTo("ACEGIKMOQSUWY"));
        }
    }
}
