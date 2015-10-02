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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Expressions;
using org.SharpTiles.Tags.CoreTags;
using org.SharpTiles.Templates;

namespace org.SharpTiles.Tags.Test.CoreTags
{
    [TestFixture]
    public class ForTokensTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _model = new Hashtable();
            _page = new Hashtable();
        }

        #endregion

        private Hashtable _page;
        private Hashtable _model;

        public string Null
        {
            get { return null; }
        }

        public string Body
        {
            get { return "a"; }
        }

        public string ComplexBody
        {
            get { return "[${Item}]"; }
        }

        public string ComplexStatusBody
        {
            get { return "[${Status.Index}/${Status.Count}]"; }
        }

        public string AList
        {
            get { return "1,2,3,4,5,6,7"; }
        }

        public string Delimeter
        {
            get { return ","; }
        }


        public Hashtable Model
        {
            get { return _model; }
            set { _model = value; }
        }

        public Hashtable Page
        {
            get { return _page; }
            set { _page = value; }
        }

        public VariableScope PageScope
        {
            get { return VariableScope.Page; }
        }

        public string False
        {
            get { return false.ToString(); }
        }

        public string True
        {
            get { return true.ToString(); }
        }

        public BaseIterationTag.ForEachStatus Status { get; set; }

        public object Item { get; set; }

        [Test]
        public void CheckRequired()
        {
            var tag = new ForTokens();
            try
            {
                RequiredAttribute.Check(tag);
                Assert.Fail("Expected exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message,
                            Is.EqualTo(
                                TagException.MissingRequiredAttribute(typeof (ForTokens), "Items", "Delims").Message));
            }
            tag.Items = new MockAttribute(new Property("AList"));
            tag.Delims = new MockAttribute(new Property("Delimeter"));
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void LoopNoBody()
        {
            var tag = new ForTokens();
            tag.Items = new MockAttribute(new Property("AList"));
            tag.Delims = new MockAttribute(new Property("Delimeter"));
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo(String.Empty));
        }

        [Test]
        public void LoopWithComplexBody()
        {
            var tag = new ForTokens();
            tag.Items = new MockAttribute(new Property("AList"));
            tag.Delims = new MockAttribute(new Property("Delimeter"));
            tag.Body = new TemplateAttribute(new Formatter(ComplexBody).Parse().ParsedTemplate);
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("[1][2][3][4][5][6][7]"));
        }

        [Test]
        public void LoopWithComplexStatusBody()
        {
            var tag = new ForTokens();
            tag.Items = new MockAttribute(new Property("AList"));
            tag.Delims = new MockAttribute(new Property("Delimeter"));
            tag.Body = new TemplateAttribute(new Formatter(ComplexStatusBody).Parse().ParsedTemplate);
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("[0/7][1/7][2/7][3/7][4/7][5/7][6/7]"));
        }

        [Test]
        public void LoopWithEndSimpleBody()
        {
            var tag = new ForTokens();
            tag.Items = new MockAttribute(new Property("AList"));
            tag.Delims = new MockAttribute(new Property("Delimeter"));
            tag.Body = new MockAttribute(new Property("Body"));
            tag.End = new MockAttribute(new Constant("5"));
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo(Body + Body + Body + Body + Body));
        }

        [Test]
        public void LoopWithFirstEndAndStepComplexBody()
        {
            var tag = new ForTokens();
            tag.Items = new MockAttribute(new Property("AList"));
            tag.Delims = new MockAttribute(new Property("Delimeter"));
            tag.Begin = new MockAttribute(new Constant("1"));
            tag.End = new MockAttribute(new Constant("6"));
            tag.Step = new MockAttribute(new Constant("2"));
            tag.Body = new TemplateAttribute(new Formatter(ComplexBody).Parse().ParsedTemplate);
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("[2][4][6]"));
        }

        [Test]
        public void LoopWithFirstSimpleBody()
        {
            var tag = new ForTokens();
            tag.Items = new MockAttribute(new Property("AList"));
            tag.Delims = new MockAttribute(new Property("Delimeter"));
            tag.Body = new MockAttribute(new Property("Body"));
            tag.Begin = new MockAttribute(new Constant("3"));
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo(Body + Body + Body + Body));
        }

        [Test]
        public void LoopWithMultiDelimiterStringAndMultiDelimiter()
        {
            var tag = new ForTokens();
            tag.Items = new MockAttribute(new Constant("1,2.3,4.5,6.7"));
            tag.Delims = new MockAttribute(new Constant(",."));
            tag.Body = new TemplateAttribute(new Formatter(ComplexBody).Parse().ParsedTemplate);
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("[1][2][3][4][5][6][7]"));
        }

        [Test]
        public void LoopWithMultiDelimiterStringButSingleDelimterDelimiter()
        {
            var tag = new ForTokens();
            tag.Items = new MockAttribute(new Constant("1,2.3,4.5,6.7"));
            tag.Delims = new MockAttribute(new Property("Delimeter"));
            tag.Body = new TemplateAttribute(new Formatter(ComplexBody).Parse().ParsedTemplate);
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("[1][2.3][4.5][6.7]"));
        }

        [Test]
        public void LoopWithSimpleBody()
        {
            var tag = new ForTokens();
            tag.Items = new MockAttribute(new Property("AList"));
            tag.Delims = new MockAttribute(new Property("Delimeter"));
            tag.Body = new MockAttribute(new Property("Body"));
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo(Body + Body + Body + Body + Body + Body + Body));
        }

        [Test]
        public void LoopWithStartEndSimpleBody()
        {
            var tag = new ForTokens();
            tag.Items = new MockAttribute(new Property("AList"));
            tag.Delims = new MockAttribute(new Property("Delimeter"));
            tag.Body = new MockAttribute(new Property("Body"));
            tag.Begin = new MockAttribute(new Constant("3"));
            tag.End = new MockAttribute(new Constant("5"));
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo(Body + Body));
        }

        [Test]
        public void LoopWithStepAndBeginSimpleBody()
        {
            var tag = new ForTokens();
            tag.Items = new MockAttribute(new Property("AList"));
            tag.Delims = new MockAttribute(new Property("Delimeter"));
            tag.Body = new MockAttribute(new Property("Body"));
            tag.Begin = new MockAttribute(new Constant("3"));
            tag.Step = new MockAttribute(new Constant("2"));
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo(Body + Body));
        }

        [Test]
        public void LoopWithStepAndSimpleBody()
        {
            var tag = new ForTokens();
            tag.Items = new MockAttribute(new Property("AList"));
            tag.Delims = new MockAttribute(new Property("Delimeter"));
            tag.Body = new MockAttribute(new Property("Body"));
            tag.Step = new MockAttribute(new Constant("2"));
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo(Body + Body + Body + Body));
        }
    }
}
