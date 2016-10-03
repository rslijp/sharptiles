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
 using org.SharpTiles.Common;
 using org.SharpTiles.Expressions;
using org.SharpTiles.Tags;
 using org.SharpTiles.Tags.Creators;
 using org.SharpTiles.Tags.Templates.SharpTags;
 using org.SharpTiles.Templates.Templates;

namespace org.SharpTiles.Templates.Test.SharpTags
{
    [TestFixture]
    public class MarkerTest
    {
        [Test]
        public void CheckMarkerRequired()
        {
            var tag = new Marker();
            try
            {
                RequiredAttribute.Check(tag);
                Assert.Fail("Expected exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message, Is.EqualTo(TagException.MissingRequiredAttribute(typeof (Marker), "Id").Message));
            }

            tag.Id = new TemplateAttribute(new ParsedTemplate(new FileBasedResourceLocator(), new ExpressionPart(new Constant("id"))));
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void CheckNoBody()
        {
            var tag = new Marker();
            tag.Id = new TemplateAttribute(new ParsedTemplate(new FileBasedResourceLocator(), new ExpressionPart(new Constant("id"))));
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo(String.Empty));
        }

        [Test]
        public void CheckPassThroughOfContent()
        {
            var tag = new Marker();
            tag.Id = new TemplateAttribute(new ParsedTemplate(new FileBasedResourceLocator(), new ExpressionPart(new Constant("id"))));
            tag.Body = new TemplateAttribute(new ParsedTemplate(new FileBasedResourceLocator(), new ExpressionPart(new Constant("body"))));
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("body"));
        }

        [Test]
        public void CheckPassThroughOfContentNoXmlEscaping()
        {
            var tag = new Marker();
            tag.Id = new TemplateAttribute(new ParsedTemplate(new FileBasedResourceLocator(), new ExpressionPart(new Constant("id"))));
            tag.Body = new TemplateAttribute(new ParsedTemplate(new FileBasedResourceLocator(), new ExpressionPart(new Constant("body&body"))));
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("body&body"));
        }

        [Test]
        public void CheckPassThroughOfContentParsed()
        {
            var lib = new TagLib().Register(new Sharp());
            ITag tag = new TagLibParserFactory(new TagLibForParsing(lib), new ExpressionLib(), new FileLocatorFactory(), null).Parse("<sharp:marker id='id'>body</sharp:marker>");
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("body"));
        }
    }
}
