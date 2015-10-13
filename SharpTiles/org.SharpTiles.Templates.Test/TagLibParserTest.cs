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
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.CoreTags;
using org.SharpTiles.Tags.FormatTags;

namespace org.SharpTiles.Templates.Test
{
    [TestFixture]
    public class TagLibParserTest : RequiresEnglishCulture
    {
        private static string GetUrl(string relativeFileUrl)
        {
            string fileUrl = Path.GetFullPath(relativeFileUrl);
            fileUrl = fileUrl.Replace("\\", "/");
            fileUrl = "file://" + fileUrl;
            return fileUrl;
        }

        [Test]
        public void TestGetSetGetRemoveAndGetAgainOfAVariable()
        {
            var model = new Hashtable();
            var reflection = new TagModel(model);
            model.Add(VariableScope.Model.ToString(), new Hashtable());
            Assert.That(
                new TagLibParserFactory().Parse("<c:out value=\"${" + VariableScope.Model + ".text}\"/>").Evaluate(reflection),
                Is.EqualTo(String.Empty));
            new TagLibParserFactory().Parse("<c:set var=\"text\" value=\"Hello world\"/>").Evaluate(reflection);
            Assert.That(
                new TagLibParserFactory().Parse("<c:out value=\"${" + VariableScope.Page + ".text}\"/>").Evaluate(reflection),
                Is.EqualTo("Hello world"));
            new TagLibParserFactory().Parse("<c:remove var=\"text\"/>").Evaluate(reflection);
            Assert.That(
                new TagLibParserFactory().Parse("<c:out value=\"${" + VariableScope.Page + ".text}\"/>").Evaluate(reflection),
                Is.EqualTo(String.Empty));
        }

        [Test]
        public void TestOfDifferentClosingTag()
        {
            try
            {
                new TagLibParserFactory().Parse("<c:forTokens step='2' items=\"1,2,3,4,5,6\" delims=\",\">${Item}</c:forEach>");
                Assert.Fail("Expected exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.MessageWithOutContext,
                            Is.EqualTo(TagException.UnbalancedCloseingTag(typeof (ForTokens), typeof (ForEach)).Message));
            }
        }

        [Test]
        public void TestOfParseOfForEachfBodyWithVar()
        {
            var model = new Hashtable();
            model.Add(VariableScope.Model.ToString(), new Hashtable());
            var reflection = new TagModel(model);
            var list = new ArrayList(new[] {1, 2, 3, 4, 5, 6});
            reflection["Model.list"] = list;
            ITag tag = new TagLibParserFactory().Parse("<c:forEach items=\"${Model.list}\">${Item}</c:forEach>");
            Assert.That(tag.Evaluate(reflection), Is.EqualTo("123456"));
        }

        [Test]
        public void TestOfParseOfForEachfComplexBody()
        {
            var model = new Hashtable();
            model.Add(VariableScope.Model.ToString(), new Hashtable());
            var reflection = new TagModel(model);
            var list = new ArrayList(new[] {1, 2, 3, 4, 5, 6});
            reflection["Model.list"] = list;
            ITag tag =
                new TagLibParserFactory().Parse(
                    "<c:forEach items=\"${Model.list}\"><c:set value=\"${Item}\" var=\"last\"/></c:forEach>");
            Assert.That(tag.Evaluate(reflection), Is.EqualTo(String.Empty));
            Assert.That(reflection["Page.last"], Is.EqualTo(6));
        }

        [Test]
        public void TestOfParseOfForEachfSimpleBody()
        {
            var model = new Hashtable();
            model.Add(VariableScope.Model.ToString(), new Hashtable());
            var reflection = new TagModel(model);
            var list = new ArrayList(new[] {1, 2, 3, 4, 5, 6});
            reflection["Model.list"] = list;
            ITag tag = new TagLibParserFactory().Parse("<c:forEach items=\"${Model.list}\">.</c:forEach>");
            Assert.That(tag.Evaluate(reflection), Is.EqualTo("......"));
        }

        [Test]
        public void TestOfParseOfForEachfWithStep()
        {
            var model = new Hashtable();
            model.Add(VariableScope.Model.ToString(), new Hashtable());
            var reflection = new TagModel(model);
            var list = new ArrayList(new[] {1, 2, 3, 4, 5, 6});
            reflection["Model.list"] = list;
            ITag tag = new TagLibParserFactory().Parse("<c:forEach step=\"2\" items=\"${Model.list}\">${Item}</c:forEach>");
            Assert.That(tag.Evaluate(reflection), Is.EqualTo("135"));
        }

        [Test]
        public void TestOfParseOfForTokensBodyWithVar()
        {
            var model = new Hashtable();
            model.Add(VariableScope.Model.ToString(), new Hashtable());
            var reflection = new TagModel(model);

            ITag tag = new TagLibParserFactory().Parse("<c:forTokens items=\"1,2,3,4,5,6\" delims=\",\">${Item}</c:forTokens>");
            Assert.That(tag.Evaluate(reflection), Is.EqualTo("123456"));
        }

        [Test]
        public void TestOfParseOfForTokensComplexBody()
        {
            var model = new Hashtable();
            model.Add(VariableScope.Model.ToString(), new Hashtable());
            var reflection = new TagModel(model);
            ITag tag =
                new TagLibParserFactory().Parse(
                    "<c:forTokens items=\"1,2,3,4,5,6\" delims=\",\"><c:set value=\"${Item}\" var=\"last\"/></c:forTokens>");
            Assert.That(tag.Evaluate(reflection), Is.EqualTo(String.Empty));
            Assert.That(reflection["Page.last"], Is.EqualTo("6"));
        }

        [Test]
        public void TestOfParseOfForTokensfSimpleBody()
        {
            var model = new Hashtable();
            model.Add(VariableScope.Model.ToString(), new Hashtable());
            var reflection = new TagModel(model);

            ITag tag = new TagLibParserFactory().Parse("<c:forTokens items=\"1,2,3,4,5,6\" delims=\",\">.</c:forTokens>");
            Assert.That(tag.Evaluate(reflection), Is.EqualTo("......"));
        }

        [Test]
        public void TestOfParseOfForTokensWithStep()
        {
            var model = new Hashtable();
            model.Add(VariableScope.Model.ToString(), new Hashtable());
            var reflection = new TagModel(model);

            ITag tag =
                new TagLibParserFactory().Parse("<c:forTokens step='2' items=\"1,2,3,4,5,6\" delims=\",\">${Item}</c:forTokens>");
            Assert.That(tag.Evaluate(reflection), Is.EqualTo("135"));
        }

        [Test]
        public void TestOfParseOfIfComplexBody()
        {
            ITag tag = new TagLibParserFactory().Parse("<c:if test=\"true\"><c:out value=\"'bla'\"></c:out></c:if>");
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("&apos;bla&apos;"));
        }

        [Test]
        public void TestOfParseOfIfSimpleBodyFalse()
        {
            ITag tag = new TagLibParserFactory().Parse("<c:if test=\"false\">XYZ</c:if>");
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo(String.Empty));
        }

        [Test]
        public void TestOfParseOfIfSimpleBodyTrue()
        {
            ITag tag = new TagLibParserFactory().Parse("<c:if test=\"true\">XYZ</c:if>");
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("XYZ"));
        }

        [Test]
        public void TestOfParseOfNestedForTokens()
        {
            var model = new Hashtable();
            model.Add(VariableScope.Model.ToString(), new Hashtable());
            var reflection = new TagModel(model);

            ITag tag =
                new TagLibParserFactory().Parse(
                    "<c:forTokens step='2' items=\"1,2,3,4,5,6\" delims=\",\">${Status.Index}.[<c:forTokens step='2' begin='1' items=\"1,2,3,4,5,6\" delims=\",\">${Status.Index}</c:forTokens>]</c:forTokens>");
            Assert.That(tag.Evaluate(reflection), Is.EqualTo("0.[135]2.[135]4.[135]"));
        }

        [Test]
        public void TestParseAndEvaluationOfChooseOtherwise()
        {
            ITag tag =
                new TagLibParserFactory().Parse("<c:choose><c:when test=\"false\">a</c:when><c:otherwise>b</c:otherwise></c:choose>");
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("b"));
        }

        [Test]
        public void TestParseAndEvaluationOfChooseTwoWhenOneOtherwise()
        {
            ITag tag =
                new TagLibParserFactory().Parse(
                    "<c:choose><c:when test=\"true\">a</c:when><c:when test=\"true\">b</c:when><c:otherwise>c</c:otherwise></c:choose>");
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("a"));
        }

        [Test]
        public void TestParseAndEvaluationOfChooseTwoWhenTrueFalseOneOtherwise()
        {
            ITag tag =
                new TagLibParserFactory().Parse(
                    "<c:choose><c:when test=\"false\">a</c:when><c:when test=\"true\">b</c:when><c:otherwise>c</c:otherwise></c:choose>");
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("b"));
        }

        [Test]
        public void TestParseAndEvaluationOfChooseWhen()
        {
            ITag tag =
                new TagLibParserFactory().Parse("<c:choose><c:when test=\"true\">a</c:when><c:otherwise>b</c:otherwise></c:choose>");
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("a"));
        }


        [Test]
        public void TestParseAndEvaluationOfChooseWhenWithNonWhiteSpace()
        {
            try
            {
                new TagLibParserFactory().Parse(
                    "<c:choose>X<c:when test=\"true\">a</c:when>\t<c:otherwise>b</c:otherwise>\r</c:choose>");
                Assert.Fail("Expected exception");
            }
            catch (ParseException Pe)
            {
                Assert.That(Pe.MessageWithOutContext, Is.EqualTo(ParseException.ExpectedToken("<").Message));
            }
        }

        [Test]
        public void TestParseAndEvaluationOfChooseWhenWithWhiteSpaces()
        {
            ITag tag =
                new TagLibParserFactory().Parse(
                    "<c:choose> <c:when test=\"true\">a</c:when> <c:otherwise>b</c:otherwise> </c:choose>");
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("a"));
        }

        [Test]
        public void TestParseAndEvaluationOfChooseWhenWithWhiteSpecials()
        {
            ITag tag =
                new TagLibParserFactory().Parse(
                    "<c:choose>\n<c:when test=\"true\">a</c:when>\t<c:otherwise>b</c:otherwise>\r</c:choose>");
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("a"));
        }

        [Test]
        public void TestParseAndEvaluationOfCoreOutTagWithBody()
        {
            ITag tag = new TagLibParserFactory().Parse("<c:out>the nested body</c:out>");
            Assert.IsTrue(tag is Out);
            Assert.That(tag.State, Is.EqualTo(TagState.OpenedAndClosed));
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("the nested body"));
        }

        [Test]
        public void TestParseAndEvaluationOfNestedTags()
        {
            ITag tag = new TagLibParserFactory().Parse("<c:out><c:out value=\"12\"/></c:out>");
            Assert.IsTrue(tag is Out);
            Assert.That(tag.State, Is.EqualTo(TagState.OpenedAndClosed));
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("12"));
        }

        [Test]
        public void TestParseAndEvaluationOfNestedTagsWithXmlEscape()
        {
            ITag tag = new TagLibParserFactory().Parse("<c:out escapeXml=\"true\"><c:out value=\"<br/>\"/></c:out>");
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("&amp;lt;br/&amp;gt;"));
            tag = new TagLibParserFactory().Parse("<c:out escapeXml=\"false\"><c:out value=\"<br/>\"/></c:out>");
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("&lt;br/&gt;"));
            tag = new TagLibParserFactory().Parse("<c:out><c:out escapeXml=\"false\" value=\"<br/>\"/></c:out>");
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("&lt;br/&gt;"));
        }

        [Test]
        public void TestParseWithCamelCaseAttributeNames()
        {
            ITag tag = new TagLibParserFactory().Parse("<c:out escape-xml=\"true\"><c:out value=\"<br/>\"/></c:out>");
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("&amp;lt;br/&amp;gt;"));
        }

        [Test]
        public void TestParseAndEvaluationOfNonFreeTag()
        {
            var model = new Hashtable();
            var reflection = new TagModel(model);
            model.Add(VariableScope.Model.ToString(), new Hashtable());
            Assert.That(
                new TagLibParserFactory().Parse("<c:out value=\"${" + VariableScope.Page + ".text}\"></c:out>").Evaluate(reflection),
                Is.EqualTo(String.Empty));
            new TagLibParserFactory().Parse("<c:set var=\"text\" value=\"Hello world\"></c:set>").Evaluate(reflection);
            Assert.That(
                new TagLibParserFactory().Parse("<c:out value=\"${" + VariableScope.Page + ".text}\"></c:out>").Evaluate(reflection),
                Is.EqualTo("Hello world"));
            new TagLibParserFactory().Parse("<c:remove var=\"text\"></c:remove>").Evaluate(reflection);
            Assert.That(
                new TagLibParserFactory().Parse("<c:out value=\"${" + VariableScope.Page + ".text}\"></c:out>").Evaluate(reflection),
                Is.EqualTo(String.Empty));
        }

        [Test]
        public void TestParseAndEvaluationOfNonFreeTagWithIllegalCharacters()
        {
            try
            {
                new TagLibParserFactory().Parse("<c:remove var=\"text\">x</c:remove>");
                Assert.Fail("Expected exception");
            }
            catch (ParseException Pe)
            {
                Assert.That(Pe.MessageWithOutContext, Is.EqualTo(ParseException.ExpectedToken("<").Message));
            }
        }

        [Test]
        public void TestParseAndEvaluationOfNotAllowedFreeText()
        {
            try
            {
                new TagLibParserFactory().Parse(
                    "<c:choose>Slip<c:when test=\"true\">a</c:when><c:when test=\"true\">b</c:when><c:otherwise>c</c:otherwise></c:choose>");
                Assert.Fail("Expected exception");
            }
            catch (ParseException Pe)
            {
                Assert.That(Pe.MessageWithOutContext, Is.EqualTo(ParseException.ExpectedToken("<").Message));
            }
        }

        [Test]
        public void TestParseAndEvaluationOfNotAllowedNestedTag()
        {
            try
            {
                new TagLibParserFactory().Parse(
                    "<c:choose><c:when test=\"true\">a</c:when><c:when test=\"true\">b</c:when><c:out>c</c:out></c:choose>");
                Assert.Fail("Expected exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.MessageWithOutContext,
                            Is.EqualTo(
                                TagException.OnlyNestedTagsOfTypeAllowed(typeof (Out), typeof (When), typeof (Otherwise))
                                    .Message));
            }
        }

        [Test]
        public void TestParseAndEvaluationOfNotNestedTagWhenNoneExpected()
        {
            try
            {
                new TagLibParserFactory().Parse("<c:remove var=\"text\"><c:remove var=\"text\"/></c:remove>");
                Assert.Fail("Expected exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.MessageWithOutContext, Is.EqualTo(TagException.ExpectedCloseTag(typeof (Remove)).Message));
            }
        }

        [Test]
        public void TestParseAndEvaluationOfUnClosedTag()
        {
            try
            {
                new TagLibParserFactory().Parse("<c:remove var=\"text\">x");
                Assert.Fail("Expected exception");
            }
            catch (ParseException Pe)
            {
                Assert.That(Pe.MessageWithOutContext, Is.EqualTo(ParseException.ExpectedToken("<").Message));
            }
        }

        [Test]
        public void TestParseCloseWithOutOpening()
        {
            try
            {
                new Formatter("abcdefg</c:out>").Parse();
                Assert.Fail("Expected exception");
            }
            catch (ParseException Pe)
            {
                Assert.That(Pe.MessageWithOutContext,
                            Is.EqualTo(ParseException.UnexpectedCloseTag(typeof (Out).Name).Message));
            }
        }

        [Test]
        public void TestParseNoParams()
        {
            ITag tag = new TagLibParserFactory().Parse("<c:out/>");
            Assert.IsTrue(tag is Out);
            Assert.That(tag.State, Is.EqualTo(TagState.OpenedAndClosed));
        }

        [Test]
        public void TestParseOfCapitalizedAttribute()
        {
            ITag tag = new TagLibParserFactory().Parse("<c:if Test=\"true\">XYZ</c:if>");
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("XYZ"));
        }

        [Test]
        public void TestParseOfCoreOutTag()
        {
            ITag tag = new TagLibParserFactory().Parse("<c:out value=\"12\"/>");
            Assert.IsTrue(tag is Out);
            Assert.That(tag.State, Is.EqualTo(TagState.OpenedAndClosed));
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("12"));
        }

        [Test]
        public void TestParseOfCoreOutTagWithAdditionalSpaces()
        {
            try
            {
                ITag tag = new TagLibParserFactory().Parse("<c:out  value=\"12\"  escapeXml=\"false\" />");
                Assert.IsTrue(tag is Out);
                Assert.That(tag.State, Is.EqualTo(TagState.OpenedAndClosed));
                Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("12"));
            }
            catch (ExceptionWithContext e)
            {
                Console.WriteLine(e.Context);
                throw;
            }
        }

        [Test]
        public void TestParseOfCoreOutTagWithBody()
        {
            ITag tag = new TagLibParserFactory().Parse("<c:out value=\"12\">the nested body</c:out>");
            Assert.IsTrue(tag is Out);
            Assert.That(tag.State, Is.EqualTo(TagState.OpenedAndClosed));
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("12"));
        }

        [Test]
        public void TestParseOfDoubleAttribute()
        {
            try
            {
                new TagLibParserFactory().Parse(
                    "<c:if test=\"true\" test=\"true\">Y</c:if>");
                Assert.Fail("Expected exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.MessageWithOutContext, Is.EqualTo(TagException.PropertyAlReadySet("Test").Message));
            }
        }

        [Test]
        public void TestParseOfImport()
        {
            var model = new Hashtable();
            var reflection = new TagModel(model);
            model.Add(VariableScope.Page.ToString(), new Hashtable());
            string fileUrl = GetUrl("formatted.htm");
            ITag tag = new TagLibParserFactory().Parse("<c:import url='" + fileUrl + "' var='file'></c:import>");
            Assert.That(tag.Evaluate(reflection), Is.EqualTo(String.Empty));
            string expected = File.ReadAllText("formatted.htm");
            Assert.That(reflection["Page.file"], Is.EqualTo(expected));
        }

        [Test]
        public void TestParseOfMissingRequiredttribute()
        {
            try
            {
                new TagLibParserFactory().Parse(
                    "<c:if>Y</c:if>");
                Assert.Fail("Expected exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.MessageWithOutContext,
                            Is.EqualTo(TagException.MissingRequiredAttribute(typeof (If), "Test").Message));
            }
        }

        [Test]
        public void TestParseOfRedirect()
        {
            ITag tag = new TagLibParserFactory().Parse("<c:redirect url='www.google.com'/>");
            Assert.IsTrue(tag is Redirect);
            var redirect = (Redirect) tag;
            Assert.That(redirect.Url.ConstantValue, Is.EqualTo("www.google.com"));
        }

        [Test]
        public void TestParseOfUnKnownAttribute()
        {
            try
            {
                new TagLibParserFactory().Parse(
                    "<c:if someTest=\"true\">Y</c:if>");
                Assert.Fail("Expected exception");
            }
            catch (ReflectionException Re)
            {
                Assert.That(Re.Message,
                            Is.EqualTo(ReflectionException.PropertyNotFound("SomeTest", typeof (If)).Message));
            }
        }

        [Test]
        public void TestParseOfUrl()
        {
            ITag tag =
                new TagLibParserFactory().Parse("<c:url value='www.google.com'><c:param name='search'>banana</c:param></c:url>");
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("www.google.com?search=banana"));
        }

        [Test]
        public void TestParseOfXml()
        {
            var model = new Hashtable();
            var reflection = new TagModel(model);
            model.Add(VariableScope.Page.ToString(), new Hashtable());
            string fileUrl = GetUrl("cd_catalog.xml");
            new TagLibParserFactory().Parse("<c:import url='" + fileUrl + "' var='file'></c:import>").Evaluate(reflection);
            new TagLibParserFactory().Parse("<x:parse doc='${file}' var='fileAsXml'></x:parse>").Evaluate(reflection);
            ITag tag = new TagLibParserFactory().Parse("<x:out source='fileAsXml' select='//CD[position()=2]/TITLE'/>");
            Assert.That(tag.Evaluate(reflection), Is.EqualTo("Hide your heart"));
        }

        [Test]
        public void TestParseOfXmlFromBody()
        {
            var model = new Hashtable();
            var reflection = new TagModel(model);
            model.Add(VariableScope.Page.ToString(), new Hashtable());
            new TagLibParserFactory().Parse(
                "<x:parse var='fileAsXml'><CATALOG><CD><TITLE>Empire Burlesque</TITLE></CD><CD><TITLE>Hide your heart</TITLE></CD></CATALOG></x:parse>")
                .Evaluate(reflection);
            ITag tag =
                new TagLibParserFactory().Parse("<x:set var='result' source='fileAsXml' select='//CD[position()=last()]/TITLE'/>");
            tag.Evaluate(reflection);
            Assert.That(((XPathNavigator) reflection["result"]).Value, Is.EqualTo("Hide your heart"));
        }

        [Test]
        public void TestParsePartialClose()
        {
            ITag tag = new TagLibParserFactory().Parse("</c:out>");
            Assert.IsTrue(tag is Out);
            Assert.That(tag.State, Is.EqualTo(TagState.Closed));
        }

        [Test]
        public void TestParseDollarSignShouldNotBeIgnoredAfterCloseOfTag()
        {
            var model = new Hashtable();
            var reflection = new TagModel(model);
            reflection["text"] = "A";
            var x = new Formatter("<c:out value=\"${text}\"/>${text}<c:out value=\"${text}\"/>").Parse();
            Assert.That(x.Format(reflection), Is.EqualTo("AAA"));
        }

        [Test]
        public void TestParseDollarSignShouldNotBeIgnoredAfterCloseOfTag_With_Explicit_Open_And_Close_Tag()
        {
            var model = new Hashtable();
            var reflection = new TagModel(model);
            reflection["text"] = "A";
            var x = new Formatter("<c:out value=\"${text}\"></c:out>${text}<c:out value=\"${text}\"></c:out>").Parse();
            Assert.That(x.Format(reflection), Is.EqualTo("AAA"));
        }


        [Test]
        public void TestParseDollarSignShouldNotBeIgnoredCloseAccolade()
        {
            var model = new Hashtable();
            var reflection = new TagModel(model);
            reflection["text"] = "A";
            var x = new Formatter("${text}${text}").Parse();
            Assert.That(x.Format(reflection), Is.EqualTo("AA"));
        }


        [Test]
        public void TestParseDollarSignShouldNotBeIgnoredAfterCloseOfNonInteresingTag()
        {
            var model = new Hashtable();
            var reflection = new TagModel(model);
            reflection["text"] = "A";
            var x = new Formatter("<a>${text}</a>").Parse(); ;
            Assert.That(x.Format(reflection), Is.EqualTo("<a>A</a>"));
        }

        [Test]
        public void TestParsePartialOpen()
        {
            try
            {

                new TagLibParserFactory().Parse("<c:out value=\"${Model.Text}\">");
                Assert.Fail("Expected exception");
            }
            catch (ParseException Pe)
            {
                Assert.That(Pe.MessageWithOutContext, Is.EqualTo(ParseException.ExpectedCloseTag().Message));
            }
        }



        [Test]
        public void TestParsePartialOpenWithPartialNested()
        {
            try
            {
                new TagLibParserFactory().Parse("<c:out value=\"12\">something");
                Assert.Fail("Expected exception");
            }
            catch (ParseException Pe)
            {
                Assert.That(Pe.MessageWithOutContext, Is.EqualTo(ParseException.ExpectedCloseTag().Message));
            }
        }

        [Test]
        public void TestParseWithWhiteSpacesAndNewLines()
        {
            ITag tag = new TagLibParserFactory().Parse(
                "<c:choose>\n\r" +
                "\t <c:when test=\"false\">a</c:when>\n\r" +
                "\t <c:when test=\"true\">b</c:when>\n\r" +
                "\t <c:otherwise>c</c:otherwise>\n\r" +
                " </c:choose>"
                );
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("b"));
        }

        [Test]
        public void TestParsingOfEncoding() //Formatting is handled by page model
        {
            var model = new TagModel(this);
            ITag tag = new TagLibParserFactory().Parse("<fmt:requestEncoding value=\"ISO-8859-1\"/>/>");
            Assert.That(tag is RequestEncoding);
        }

        [Test]
        public void TestPaseAndFormatNumbers()
        {
            var model = new TagModel(this);
            new TagLibParserFactory().Parse("<fmt:setLocale scope=\"Page\" Value=\"en-US\"/>").Evaluate(model);
            new TagLibParserFactory().Parse("<fmt:parseNumber var=\"result\" scope=\"Page\">0.33</fmt:parseNumber>").Evaluate(model);
            new TagLibParserFactory().Parse("<fmt:setLocale scope=\"Page\" Value=\"nl-NL\"/>").Evaluate(model);
            ITag tag = new TagLibParserFactory().Parse("<fmt:formatNumber value=\"${result}\" Type=\"Percentage\"/>");
            Assert.That(tag.Evaluate(model), Is.EqualTo("33,00 %"));
        }

        [Test]
        public void TestPaseAndFormatOfDates()
        {
            var model = new TagModel(this);
            new TagLibParserFactory().Parse("<fmt:setLocale scope=\"Page\" Value=\"en-US\"/>").Evaluate(model);
            new TagLibParserFactory().Parse("<fmt:parseDate var=\"result\" Type=\"Time\" scope=\"Page\">4:55 PM</fmt:parseDate>").
                Evaluate(model);
            new TagLibParserFactory().Parse("<fmt:setLocale scope=\"Page\" Value=\"nl-NL\"/>").Evaluate(model);
            ITag tag = new TagLibParserFactory().Parse("<fmt:formatDate Value=\"${result}\" Type=\"Time\"/>");
            Assert.That(tag.Evaluate(model), Is.EqualTo("16:55"));
        }

        [Test]
        public void TestResourceBundle()
        {
            ITag tag =
                new TagLibParserFactory().Parse(
                    "<fmt:bundle baseName=\"compiled\" prefix=\"pre_\"><fmt:message key=\"b\"/></fmt:bundle>");
            Assert.That(tag.Evaluate(new TagModel(this)), Is.EqualTo("prefixedB"));
        }

        [Test]
        public void TestResourceBundleDifferentLanguage()
        {
            var model = new TagModel(this);
            new TagLibParserFactory().Parse("<fmt:setLocale scope=\"Page\" Value=\"nl-NL\"/>").Evaluate(model);
            ITag tag = new TagLibParserFactory().Parse(
                "<fmt:bundle baseName=\"compiled\" prefix=\"pre_\"><fmt:message key=\"b\"/></fmt:bundle>");
            Assert.That(tag.Evaluate(model), Is.EqualTo("bMetVoorvoegsel"));
        }

        [Test]
        public void TestResourceBundleInStoredVariable()
        {
            var model = new TagModel(this);
            new TagLibParserFactory().Parse("<fmt:setBundle baseName=\"compiled\" var=\"tilesMessages\" scope=\"Page\"/>").Evaluate(
                model);
            ITag tag = new TagLibParserFactory().Parse("<fmt:message key=\"b\" bundle=\"tilesMessages\"/>");
            Assert.That(tag.Evaluate(model), Is.EqualTo("defaultB"));
        }

        [Test]
        public void TestResourceBundleWithParams()
        {
            var model = new TagModel(this);
            new TagLibParserFactory().Parse("<fmt:setBundle baseName=\"complex\" var=\"tilesMessages\" scope=\"Page\"/>").Evaluate(
                model);
            ITag tag =
                new TagLibParserFactory().Parse(
                    "<fmt:message key=\"twovars\" bundle=\"tilesMessages\"><fmt:param>A</fmt:param><fmt:param>B</fmt:param></fmt:message>");
            Assert.That(tag.Evaluate(model), Is.EqualTo("two A, B vars"));
        }

        [Test]
        public void TestSetAVariableWithANestedTag()
        {
            var model = new Hashtable();
            var reflection = new TagModel(model);
            model.Add(VariableScope.Model.ToString(), new Hashtable());
            new TagLibParserFactory().Parse("<c:set var=\"text\"><c:out value=\"the nested body\"/></c:set>").Evaluate(reflection);
            Assert.That(
                new TagLibParserFactory().Parse("<c:out value=\"${" + VariableScope.Page + ".text}\"/>").Evaluate(reflection),
                Is.EqualTo("the nested body"));
        }

        [Test]
        public void TestSetGetSetAndGetAgainOfAVariable()
        {
            var model = new Hashtable();
            var reflection = new TagModel(model);
            model.Add(VariableScope.Model.ToString(), new Hashtable());
            new TagLibParserFactory().Parse("<c:set var=\"text\" value=\"Hello world\"/>").Evaluate(reflection);
            Assert.That(
                new TagLibParserFactory().Parse("<c:out value=\"${" + VariableScope.Page + ".text}\"/>").Evaluate(reflection),
                Is.EqualTo("Hello world"));
            new TagLibParserFactory().Parse("<c:set var=\"text\" value=\"Hi there\"/>").Evaluate(reflection);
            Assert.That(
                new TagLibParserFactory().Parse("<c:out value=\"${" + VariableScope.Page + ".text}\"/>").Evaluate(reflection),
                Is.EqualTo("Hi there"));
        }

        [Test]
        public void TestSimpleCatchNoCatch()
        {
            var model = new Hashtable();
            var reflection = new TagModel(model);
            Assert.That(new TagLibParserFactory().Parse("<c:catch>[<c:out value=\"'Hi'\"/>]</c:catch>").Evaluate(reflection),
                        Is.EqualTo("[&apos;Hi&apos;]"));
        }


        [Test]
        public void TestSimpleCatchWithCatch()
        {
            var model = new Hashtable();
            var reflection = new TagModel(model).BecomeStrict();
            Assert.That(
                new TagLibParserFactory().Parse("<c:catch>[<c:out value=\"${" + VariableScope.Model + ".a.text}\"/>]</c:catch>").
                    Evaluate(reflection), Is.EqualTo(""));
        }

        [Test]
        public void TestSimpleCatchWithCatchIntoVar()
        {
            var model = new Hashtable();
            model.Add(VariableScope.Model.ToString(), new Hashtable());
            var reflection = new TagModel(model).BecomeStrict(); ;
            Assert.That(
                new TagLibParserFactory().Parse("<c:catch var=\"error\"><c:out value=\"${Model.asa.bb.text}\"/></c:catch>").Evaluate(
                    reflection), Is.EqualTo(String.Empty));
            Assert.IsNotNull(reflection[VariableScope.Model + ".error"]);
            Assert.IsTrue(
                reflection[VariableScope.Model + ".error"] is ReflectionException.ReflectionExceptionWithContext);
        }

        [Test]
        public void TestXmlChoose()
        {
            var model = new Hashtable();
            var reflection = new TagModel(model);
            model.Add(VariableScope.Page.ToString(), new Hashtable());
            new TagLibParserFactory().Parse(
                "<x:parse var='fileAsXml'><VALUES><TRUE>true</TRUE><FALSE>false</FALSE></VALUES></x:parse>").Evaluate(
                reflection);
            ITag tag =
                new TagLibParserFactory().Parse(
                    "<x:choose><x:when source='fileAsXml' select='//FALSE'>Yeah</x:when><x:otherwise>Nope</x:otherwise></x:choose>");
            Assert.That(tag.Evaluate(reflection), Is.EqualTo("Nope"));
        }

        [Test]
        public void TestXmlForEach()
        {
            var model = new Hashtable();
            var reflection = new TagModel(model);
            model.Add(VariableScope.Page.ToString(), new Hashtable());
            string fileUrl = GetUrl("cd_catalog.xml");
            new TagLibParserFactory().Parse("<c:import url='" + fileUrl + "' var='file'></c:import>").Evaluate(reflection);
            new TagLibParserFactory().Parse("<x:parse doc='${file}' var='fileAsXml'></x:parse>").Evaluate(reflection);
            ITag tag =
                new TagLibParserFactory().Parse(
                    "<x:forEach source='fileAsXml' select='//CD'>[<x:out source='Item' select='./YEAR'/>]</x:forEach>");
            Assert.That(tag.Evaluate(reflection),
                        Is.EqualTo(
                            "[1985][1988][1982][1990][1997][1998][1973][1990][1996][1987][1995][1999][1995][1997]"));
        }

        [Test]
        public void TestXmlIf()
        {
            var model = new Hashtable();
            var reflection = new TagModel(model);
            model.Add(VariableScope.Page.ToString(), new Hashtable());
            new TagLibParserFactory().Parse(
                "<x:parse var='fileAsXml'><VALUES><TRUE>true</TRUE><FALSE>false</FALSE></VALUES></x:parse>").Evaluate(
                reflection);
            ITag tag = new TagLibParserFactory().Parse("<x:if source='fileAsXml' select='//TRUE'>Yeah</x:if>");
            Assert.That(tag.Evaluate(reflection), Is.EqualTo("Yeah"));
        }

        [Test]
        public void TestXmlTransform()
        {
            var model = new Hashtable();
            var reflection = new TagModel(model);
            model.Add(VariableScope.Page.ToString(), new Hashtable());
            string fileUrl = GetUrl("cd_catalog.xml");
            new TagLibParserFactory().Parse("<c:import url='" + fileUrl + "' var='file'></c:import>").Evaluate(reflection);
            new TagLibParserFactory().Parse("<x:parse doc='${file}' var='fileAsXml'></x:parse>").Evaluate(reflection);
            new TagLibParserFactory().Parse("<c:set var='xsltAsString'>" +
                               "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>" +
                               "<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">" +
                               "<xsl:output method=\"text\" media-type=\"text/plain\" omit-xml-declaration=\"yes\"/>" +
                               "<xsl:template match=\"/\">" +
                               "<xsl:for-each select=\"CATALOG/CD\">" +
                               "<xsl:value-of select=\"ARTIST\"/>." +
                               "</xsl:for-each>" +
                               "</xsl:template>" +
                               "</xsl:stylesheet>" +
                               "</c:set>"
                ).Evaluate(reflection);
            ITag tag = new TagLibParserFactory().Parse("<x:transform Doc='${file}' Xslt='${xsltAsString}'></x:transform>");
            Assert.That(tag.Evaluate(reflection),
                        Is.EqualTo(
                            "Bob Dylan.Bonnie Tyler.Dolly Parton.Gary Moore.Eros Ramazzotti.Bee Gees.Dr.Hook.Rod Stewart.Andrea Bocelli.Percy Sledge.Savage Rose.Many.Kenny Rogers.Will Smith."));
        }

        [Test]
        public void TestXmlTransformFromFile()
        {
            var model = new Hashtable();
            var reflection = new TagModel(model);
            model.Add(VariableScope.Page.ToString(), new Hashtable());
            string fileUrl = GetUrl("cd_catalog.xml");
            string fileUrl2 = GetUrl("cd_catalog.xsl");
            new TagLibParserFactory().Parse("<c:import url='" + fileUrl + "' var='xmlFile'></c:import>").Evaluate(reflection);
            new TagLibParserFactory().Parse("<c:import url='" + fileUrl2 + "' var='xslFile'></c:import>").Evaluate(reflection);
            new TagLibParserFactory().Parse("<x:parse doc='${xmlFile}' var='xml'></x:parse>").Evaluate(reflection);
            new TagLibParserFactory().Parse("<x:parse doc='${xslFile}' var='xslt'></x:parse>").Evaluate(reflection);
            ITag tag = new TagLibParserFactory().Parse("<x:transform Doc='${xml}' Xslt='${xslt}'></x:transform>");
            string result = File.ReadAllText("cd_catalog.html");
            Assert.That(tag.Evaluate(reflection), Is.EqualTo(result));
        }

        [Test]
        public void TestParseBug()
        {
            var model = new Hashtable();
            var reflection = new TagModel(model);
            var page = "<ul id=\"menu\">" +
                       "<li>" +
                       "<a id=\"menu_to_index\" href=\"<c:url value='~/Home/Index'/>\">Index</a>" +
                       "</li>" +
                       "<li>" +
                       "<a id=\"menu_to_about\" href=\"<c:url value='~/Home/About'/>\">About</a>" +
                       "</li>" +
                       "</ul>";
            var expected = "<ul id=\"menu\">" +
                           "<li>" +
                           "<a id=\"menu_to_index\" href=\"/Home/Index\">Index</a>" +
                           "</li>" +
                           "<li>" +
                           "<a id=\"menu_to_about\" href=\"/Home/About\">About</a>" +
                           "</li>" +
                           "</ul>";
            var tag = new Formatter(page).Parse();
            Assert.That(tag.Format(reflection), Is.EqualTo(expected));
        }

        [Test]
        public void TestParseBugSimple()
        {
            var model = new Hashtable();
            var reflection = new TagModel(model);
            var page = "<a href=\"<c:out value='~/Index'/>\">Index</a>";
            var expected = "<a href=\"~/Index\">Index</a>";
                var tag = new Formatter(page).Parse();
                Assert.That(tag.Format(reflection), Is.EqualTo(expected));
        }

        [Test]
        public void TestParseBugLiteralHandlingWithNestedLiteral()
        {
            var model = new Hashtable();
            var reflection = new TagModel(model);
            var page = "<a href=\"<c:out value='~/Index'/>\">Index</a>";
            var expected = "<a href=\"~/Index\">Index</a>";
            var tag = new Formatter(page).Parse();
            Assert.That(tag.Format(reflection), Is.EqualTo(expected));
        }

        [Test]
        public void TestParseBugLiteralHandlingWithNestedStack()
        {
            var model = new Hashtable();
            var reflection = new TagModel(model);
            var page = "<a href=\"<c:out>~/Index</c:out>\">Index</a>";
            var expected = "<a href=\"~/Index\">Index</a>";
            var tag = new Formatter(page).Parse();
            Assert.That(tag.Format(reflection), Is.EqualTo(expected));
        }

        [Test]
        public void TestParseBugLiteralHandling()
        {
            var model = new Hashtable();
            var reflection = new TagModel(model);
            var page = "[\"<c:out value='~/Index'/>\"]";
            var expected = "[\"~/Index\"]";
            var tag = new Formatter(page).Parse();
            Assert.That(tag.Format(reflection), Is.EqualTo(expected));
        }

        [Test]
        public void TestParseNestedLiteralHandling()
        {
            var model = new Hashtable();
            var reflection = new TagModel(model);
            var page = "[\"'Index'\"]";
            var expected = "[\"'Index'\"]";
            var tag = new Formatter(page).Parse();
            Assert.That(tag.Format(reflection), Is.EqualTo(expected));
        }

        [Test]
        public void ErrorMessageAtUnterminatedLiteral()
        {
            var page = "<c:out value='=\"dadsjfhskfhs/>";
            try
            {
                new Formatter(page).Parse();
                Assert.Fail("Expected error");
            } catch (TokenException Te)
            {
                Assert.That(Te.MessageWithOutContext, Is.EqualTo(TokenException.UnTerminatedLiteral(14).Message));
            }
            
        }

        [Test]
        public void ErrorMessageAtUnterminatedLiteralUsesContextOfStartPoint()
        {
            var page = "<c:out value='\"dadsjfhskfhs/>";
            try
            {
                new Formatter(page).Parse();
                Assert.Fail("Expected error");
            }
            catch (TokenException Te)
            {
                Assert.That(Te.Context.Index, Is.EqualTo(13));
            }

        }
    }
}