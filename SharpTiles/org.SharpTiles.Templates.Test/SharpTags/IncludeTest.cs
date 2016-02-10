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
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.Creators;
using org.SharpTiles.Tags.Templates.SharpTags;
using org.SharpTiles.Templates.SharpTags;
using org.SharpTiles.Templates.Templates;

namespace org.SharpTiles.Templates.Test.SharpTags
{
    [TestFixture]
    public class IncludeTest
    {
//        [Test]
//        public void CheckConstantValue()
//        {
//            var tag = new Include();
//            try
//            {
//                tag.File =
//                    new TemplateAttribute(new ParsedTemplate(new FileBasedResourceLocator(),
//                                                             new ExpressionPart(new Constant("a.htm"))));
//                Assert.Fail("Expected exception");
//            }
//            catch (TemplateException Te)
//            {
//                Assert.That(Te.Message,
//                            Is.EqualTo(
//                                TemplateException.TemplatePartCannotBeUsedAsContant(typeof (ExpressionPart)).Message));
//            }
//        }

        [Test]
        public void CheckIncludeRequired()
        {
            var tag = new Include();
            try
            {
                RequiredAttribute.Check(tag);
                Assert.Fail("Expected exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message,
                            Is.EqualTo(TagException.MissingRequiredAttribute(typeof (Include), "File").Message));
            }

            tag.File =
                new TemplateAttribute(new ParsedTemplate(new FileBasedResourceLocator(),
                                                         new TextPart("SharpTags\\a.htm", null)));
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void CheckPassThroughOfContentException()
        {
            //new FileLocatorFactory("SharpTags/")
            new Formatter("just for init");
            ITag tag = CreateFactory().Parse("<sharp:include file='SharpTags/error.htm'/>");
            try
            {
                tag.Evaluate(new TagModel(this));
                Assert.Fail("Expected exception");
            }
            catch (Include.IncludeException IIe)
            {
                Assert.That(IIe.Message.StartsWith("Error in file SharpTags/error.htm"));
            }
        }

        private static TagLibParserFactory CreateFactory()
        {
            ITagLib lib = new TagLib();
            lib.Register(new Sharp());
            return new TagLibParserFactory(new TagLibForParsing(lib), new FileLocatorFactory());
        }

        [Test]
        public void CheckPassThroughOfContentParsed()
        {
            ITag tag = CreateFactory().Parse("<sharp:include file='SharpTags/a.htm'/>");
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("aa"));
        }

        [Test]
        public void CheckPassThroughOfNestedException()
        {
            ITag tag = CreateFactory().Parse("<sharp:include file='SharpTags/nestederror.htm'/>");
            try
            {
                tag.Evaluate(new TagModel(this));
                Assert.Fail("Expected exception");
            }
            catch (Include.IncludeException IIe)
            {
                Assert.That(IIe.Message.StartsWith("Error in file SharpTags/nestederror.htm"));
                Assert.That(IIe.InnerException.Message.StartsWith("Error in file error.htm"));
            }
        }


        [Test]
        public void CheckReturnOfTemplate()
        {
            var tag = new Include
                          {
                              File = new TemplateAttribute(new ParsedTemplate(new FileBasedResourceLocator(), new TextPart("SharpTags\\a.htm", null)))
                          };
            tag.Factory=new FileLocatorFactory();
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("aa"));
        }

        [Test]
        public void HandNestedInclude()
        {
            var some = new Hashtable {{"a", "##"}};
            var table = new Hashtable {{"some", some}};

            var formatter = new Formatter("<sharp:include file='SharpTags/d.htm'/>").Parse();
            Assert.That(
                formatter.Format(new TagModel(table)),
                Is.EqualTo("|bb##bb|bb##bb|bb##bb|")
            );
        }

        [Test]
        public void HandNestedIncludeWithDirs()
        {
            var formatter = new Formatter("<sharp:include file='SharpTags/withnested.htm'/>").Parse(); 
            Assert.That(formatter.Format(new TagModel(new Hashtable())),
                        Is.EqualTo("FILE IN DIR"));
        }

        [Test]
        public void HandNestedIncludeWithNestedParentDirs()
        {
            var formatter = new Formatter("<sharp:include file='SharpTags/nested/withparent.htm'/>").Parse();
            Assert.That(formatter.Format(new TagModel(new Hashtable())),
                        Is.EqualTo("parent aa"));
        }

        [Test]
        public void HandNestedIncludeWithParentDirs()
        {
            var formatter = new Formatter("<sharp:include file='SharpTags/withparent.htm'/>").Parse(); 
            Assert.That(formatter.Format(new TagModel(new Hashtable())),
                        Is.EqualTo("parent aa"));
        }

        [Test]
        public void HandOverVariableScope()
        {
            var table = new Hashtable();
            table.Add("some", new Hashtable());
            var formatter =
                new Formatter("<c:set var='some.a' scope='Model'>AA</c:set><sharp:include file='SharpTags/c.htm'/>").Parse();
            Assert.That(formatter.Format(new TagModel(table)),
                        Is.EqualTo("bbAAbb"));
        }

        [Test]
        public void HandOverVariableScopeNested()
        {
            var some = new Hashtable();
            some.Add("a", "##");

            var table = new Hashtable();
            table.Add("some", some);

            var formatter = new Formatter("<sharp:include file='SharpTags/b.htm'/>").Parse();
            Assert.That(formatter.Format(new TagModel(table)),
                        Is.EqualTo("bb##bb"));
        }

        [Test]
        public void StrangeBytesAppear()
        {
            var some = new Hashtable();
            some.Add("a", "##");

            var table = new Hashtable();
            table.Add("some", some);

            var formatter = new Formatter("<sharp:include file='SharpTags/d.htm'/>").Parse();
            string fileName = Path.GetTempFileName();
            formatter.FormatAndSave(new TagModel(table), fileName);
            byte[] generated = File.ReadAllBytes(fileName);
            byte[] expected = File.ReadAllBytes("SharpTags/expected.htm");
            //"|bb##bb|bb##bb|bb##bb|"
            Assert.That(generated, Is.EqualTo(expected));
        }

        [Test]
        public void StrangeBytesAppear2()
        {
            var some = new Hashtable();
            some.Add("a", "##");

            var table = new Hashtable();
            table.Add("some", some);

            var formatter = new Formatter("<sharp:include file='SharpTags/e.htm'/>").Parse();
            string fileName = Path.GetTempFileName();
            formatter.FormatAndSave(
                new TagModel(table),
                fileName);
            byte[] generated = File.ReadAllBytes(fileName);
            byte[] expected = File.ReadAllBytes("SharpTags/expected2.htm");
            Assert.That(Encoding.UTF8.GetString(generated), Is.EqualTo(Encoding.UTF8.GetString(expected)));
            Assert.That(generated, Is.EqualTo(expected));
        }
    }
}