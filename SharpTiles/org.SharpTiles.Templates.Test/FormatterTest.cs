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
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Common;
 using org.SharpTiles.Tags;
 using org.SharpTiles.Tags.CoreTags;
 using org.SharpTiles.Tags.FormatTags;

namespace org.SharpTiles.Templates.Test
{
    [TestFixture]
    public class FormatterTest
    {
        public const long BENCHMARK_FIX = 560000;
        public static double BENCHMARK_RATIO;
        private static bool _benchMarkSet = false;

        [SetUp]
        public void SetUp()
        {
            if (!_benchMarkSet)
            {
                int i = 0;
                DateTime start = DateTime.Now;
                while (DateTime.Now.Subtract(start).TotalMilliseconds <= 1000)
                {
                    String.Format("{0}:{1}", i, DateTime.Now);
                    i++;
                }
                BENCHMARK_RATIO = 0.8*(i/(double) BENCHMARK_FIX);
                Console.WriteLine("BENCHMARK RATIO {0} ({1}/{2}:", BENCHMARK_RATIO, i, BENCHMARK_FIX);
                _benchMarkSet = true;
            }

        }



        [Test]
        public void TestParseOfNonTagQuotes()
        {
            var formatter = new Formatter("<a href=\"${'1' + '1'}\"/>").Parse();
            Assert.That(formatter.Format(new TagModel(this)), Is.EqualTo("<a href=\"2\"/>"));
        }


        [Test]
        public void TestParseOfRandomDollarSigns()
        {
            var formatter = new Formatter("A$A").Parse();
            Assert.That(formatter.Format(new TagModel(this)), Is.EqualTo("A$A"));
        }



        [Test]
        public void TestParseOfRandomDollarSignsAndParantheses()
        {
            var formatter = new Formatter("A$(A)").Parse();
            Assert.That(formatter.Format(new TagModel(this)), Is.EqualTo("A$(A)"));
        }

        [Test]
        public void TestParseOfRandomDollarSignsAndParanthesesWithQuotes()
        {
            var formatter = new Formatter("A$('A')").Parse();
            Assert.That(formatter.Format(new TagModel(this)), Is.EqualTo("A$('A')"));
        }

        private static string GetUrl(string relativeFileUrl)
        {
            string fileUrl = Path.GetFullPath(relativeFileUrl);
            fileUrl = fileUrl.Replace("\\", "/");
            fileUrl = "file://" + fileUrl;
            return fileUrl;
        }

        private static void BenchMarkReUse(string fileNameTemplate, Hashtable model, int run)
        {
            Formatter formatter = Formatter.FileBasedFormatter(fileNameTemplate);
            formatter.Format(model);

            DateTime start = DateTime.Now;
            for (int i = 0; i < run; i++)
            {
                formatter.Format(model);
            }
            DateTime end = DateTime.Now;
            TimeSpan time = end.Subtract(start);
            double avg = (run/(time.TotalMilliseconds/1000.0));
            //the number in run should be able to be processed in a second
            //the benchmark ratio is to cope with individual performance of different systems
            Assert.That(avg, Is.GreaterThanOrEqualTo(0.33*BENCHMARK_RATIO*run));
            Console.WriteLine(fileNameTemplate + ": " + avg + " average formats per second no one parse");
        }

        private static void BenchMarkNoReUse(string fileNameTemplate, Hashtable model, int run)
        {
            Formatter formatter = Formatter.FileBasedFormatter(fileNameTemplate);
            formatter.Format(model);

            DateTime start = DateTime.Now;
            for (int i = 0; i < run; i++)
            {
                formatter = Formatter.FileBasedFormatter(fileNameTemplate);
                formatter.Format(model);
            }
            DateTime end = DateTime.Now;
            TimeSpan time = end.Subtract(start);
            double avg = (run/(time.TotalMilliseconds/1000.0));
            //the number in run should be able to be processed in a second
            //the benchmark ratio is to cope with individual performance of different systems
            Assert.That(avg, Is.GreaterThanOrEqualTo(BENCHMARK_RATIO*run));
            Console.WriteLine(fileNameTemplate + ": " + avg + " average formats per second no one parse");
        }

        public class TestModel
        {
            public string Text { get; set; }
        }

        [Test]
        public void EmptyObjectPropertiesShouldEvaluateInEmptyString()
        {
            const string TEMPLATE = "abcd${Text}efgh${Text}";
            const string FORMATTED = "abcdefgh";
            var model = new TestModel();
            model.Text = "";
            var formatter = new Formatter(TEMPLATE).Parse();
            string formatted = formatter.Format(model);
            Assert.That(formatted, Is.EqualTo(FORMATTED));
        }

        
        [Test]
        public void Setting_Local_Affects_Formatting_Of_Number_EN()
        {
            const string TEMPLATE = "<fmt:setLocale value='en-GB' scope='Page'/>${'0.00'+'1.00'}%";
            const string FORMATTED = "1.00%";
            var model = new TestModel();
            model.Text = "";
            var formatter = new Formatter(TEMPLATE).Parse();
            string formatted = formatter.Format(model);
            Assert.That(formatted, Is.EqualTo(FORMATTED));
        }

        [Test]
        public void Setting_Empty_String_Should_Be_Parsed_Correctly()
        {
            const string TEMPLATE = "<c:set value='' var='Text' scope='Model'/>";
            var model = new TestModel();
            model.Text = "aaa";
            var formatter = new Formatter(TEMPLATE).Parse();
            formatter.Format(model);
            Console.WriteLine($"[{model.Text}]");
        }

        [Test]
        public void Should_Throw_Correct_Error_Message_On_Unknown_Tag()
        {
            const string TEMPLATE = "<choose><unknown></unknown><otherwise></otherwise></choose>";
            var model = new TestModel();
            try
            {
                var formatter = new Formatter(TEMPLATE).SwitchToMode(TagLibMode.RelaxedResolve).Parse();
                Assert.Fail("Expected exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.MessageWithOutContext, Text.StartsWith(TagException.UnkownTag("unknown").Message));
            }
            
        }

        [Test]
        public void Setting_Local_Affects_Formatting_Of_Number_EN_After_Cout()
        {
            const string TEMPLATE = "<fmt:setLocale value='en-GB' scope='Page'/><c:out value=\"${'0.00'+'1.00'}%\"/>";
            const string FORMATTED = "1.00%";
            var model = new TestModel();
            model.Text = "";
            var formatter = new Formatter(TEMPLATE).Parse();
            string formatted = formatter.Format(model);
            Assert.That(formatted, Is.EqualTo(FORMATTED));
        }


        [Test]
        public void FileTemplateWithExpressions()
        {
            var model = new Hashtable();
            var list = new ArrayList();
            model.Add("small", 3);
            model.Add("large", 11);
            model.Add("text", "some text");
            model.Add("greet", "Hello");
            model.Add("to", "world");

            Formatter formatter = Formatter.FileBasedFormatter("templatewithexpressions.htm");
            string randomFile = Path.GetRandomFileName();
            try
            {
                Encoding enc = Encoding.UTF8;
                formatter.FormatAndSave(model, randomFile, enc);
                string result = enc.GetString(File.ReadAllBytes(randomFile));
                string expected = enc.GetString(File.ReadAllBytes("formattedwithexpressions.htm"));
                Assert.That(result, Is.EqualTo(expected));
            }
            finally
            {
                File.Delete(randomFile);
            }
        }

        [Test]
        public void FileTemplateWithFormatTags()
        {
            var model = new Hashtable();
            Formatter formatter = Formatter.FileBasedFormatter("templatewithformattags.htm");
            string randomFile = Path.GetRandomFileName();
            try
            {
                Encoding enc = Encoding.UTF8;
                formatter.FormatAndSave(model, randomFile, enc);
                string result = enc.GetString(File.ReadAllBytes(randomFile));
                string expected = enc.GetString(File.ReadAllBytes("formattedwithformattags.htm"));
                Assert.That(result, Is.EqualTo(expected));
            }
            finally
            {
                File.Delete(randomFile);
            }
        }

        [Test]
        public void FileTemplateWithTags()
        {
            var model = new Hashtable();
            model.Add("small", 3);
            model.Add("large", 11);
            model.Add("text", "some text");
            model.Add("greet", "Hello");
            model.Add("to", "world");

            var list = new ArrayList(new[] {"H1", "H2", "H3"});
            model.Add("list", list);

            Formatter formatter = Formatter.FileBasedFormatter("templatewithtags.htm");
            string randomFile = Path.GetRandomFileName();
            try
            {
                Encoding enc = Encoding.UTF8;
                formatter.FormatAndSave(model, randomFile, enc);
                string result = enc.GetString(File.ReadAllBytes(randomFile));
                string expected = enc.GetString(File.ReadAllBytes("formattedwithtags.htm"));
                Assert.That(result, Is.EqualTo(expected));
            }
            finally
            {
                File.Delete(randomFile);
            }
        }

        [Test]
        public void NestedParseContext()
        {
            try
            {
                new Formatter("<c:out>${math:max(a)}</c:out>").Parse();
                Assert.Fail();
            }
            catch (ExceptionWithContext ewc)
            {
                Assert.That(ewc.Context.Index, Is.EqualTo(13));
            }
        }

        [Test]
        public void Should_Still_Collect_Parse_Fragment()
        {
            var f = new Formatter("<c:out>${a}<c:out>${a}</c:out>");
            try
            {
                f.Parse();
                Assert.Fail();
            }
            catch (ExceptionWithContext ewc)
            {
                Assert.That(ewc.Context.Index, Is.EqualTo(26));
            }
            Assert.That(f.ParsedTemplate, Is.Not.Null);
        }

        [Test]
        public void FileTemplateWithTagsAndAttributesOnNewLine()
        {
            var model = new Hashtable();
            model.Add("small", 3);
            model.Add("large", 11);
            model.Add("text", "some text");
            model.Add("greet", "Hello");
            model.Add("to", "world");

            var list = new ArrayList(new[] { "H1", "H2", "H3" });
            model.Add("list", list);

            Formatter formatter = Formatter.FileBasedFormatter("templatewithtagsandattributesonnewline.htm");
            string randomFile = Path.GetRandomFileName();
            try
            {
                Encoding enc = Encoding.UTF8;
                formatter.FormatAndSave(model, randomFile, enc);
                string result = enc.GetString(File.ReadAllBytes(randomFile));
                string expected = enc.GetString(File.ReadAllBytes("formattedwithtagsandattributesonnewline.htm"));
                Assert.That(result, Is.EqualTo(expected));
            }
            finally
            {
                File.Delete(randomFile);
            }
        }

        [Test]
        public void FileTemplateWithStrictResolveTags()
        {
            var model = new Hashtable();
            model.Add("small", 3);
            model.Add("large", 11);
            model.Add("text", "some text");
            model.Add("greet", "Hello");
            model.Add("to", "world");

            var list = new ArrayList(new[] { "H1", "H2", "H3" });
            model.Add("list", list);

            Formatter formatter = Formatter.FileBasedFormatter("templatewithstrictresolvetags.htm", TagLibMode.StrictResolve);
            string randomFile = Path.GetRandomFileName();
            try
            {
                Encoding enc = Encoding.UTF8;
                formatter.FormatAndSave(model, randomFile, enc);
                string result = enc.GetString(File.ReadAllBytes(randomFile));
                string expected = enc.GetString(File.ReadAllBytes("formattedwithstrictresolvetags.txt"));
                Assert.That(result, Is.EqualTo(expected));
            }
            finally
            {
                //Console.WriteLine(randomFile);
                File.Delete(randomFile);
            }
        }

        [Test]
        public void BugSequenceContainsNo()
        {
            var model = new Hashtable();
            model.Add("small", 3);
            model.Add("large", 11);
            model.Add("text", "some text");
            model.Add("greet", "Hello");
            model.Add("to", "world");

            var list = new ArrayList(new[] { "H1", "H2", "H3" });
            model.Add("list", list);

            
            try
            {
                Formatter formatter = Formatter.FileBasedFormatter("bugsequencenoelement.htm",
                TagLibMode.StrictResolve,
                new TagLib(TagLibMode.StrictResolve, new Core(), new Format()));
            }
            catch (ExceptionWithContext Pe)
            {
                Assert.That(Pe.MessageWithOutContext, Text.StartsWith(TagException.UnkownTag("h1").Message));
            }            
        }

        [Test]
        public void IgnoreResolve()
        {
            var model = new Hashtable();
            model.Add("small", 3);
            model.Add("large", 11);
            model.Add("text", "some text");
            model.Add("greet", "Hello");
            model.Add("to", "world");

            var list = new ArrayList(new[] { "H1", "H2", "H3" });
            model.Add("list", list);

            Formatter formatter = Formatter.FileBasedFormatter("bugsequencenoelement.htm",
                TagLibMode.IgnoreResolve,
                new TagLib(TagLibMode.IgnoreResolve, new Core(), new Format()));

            string randomFile = Path.GetRandomFileName();
            try
            {
                Encoding enc = Encoding.UTF8;
                formatter.FormatAndSave(model, randomFile, enc);
                string result = enc.GetString(File.ReadAllBytes(randomFile));
                File.WriteAllText("c:\\temp\\test.txt", result);
                string expected = enc.GetString(File.ReadAllBytes("bugsequencenoelement.txt"));
                Assert.That(result, Is.EqualTo(expected));
            }
            finally
            {
                //Console.WriteLine(randomFile);
                File.Delete(randomFile);
            }
        }

        [Test,Ignore]
        public void FileTemplateWithRelaxedResolveTags()
        {
            var model = new Hashtable();
            model.Add("small", 3);
            model.Add("large", 11);
            model.Add("text", "some text");
            model.Add("greet", "Hello");
            model.Add("to", "world");

            var list = new ArrayList(new[] { "H1", "H2", "H3" });
            model.Add("list", list);

            Formatter formatter = Formatter.FileBasedFormatter("templatewithrelaxedresolvetags.htm", TagLibMode.RelaxedResolve);
            string randomFile = Path.GetRandomFileName();
            try
            {
                Encoding enc = Encoding.UTF8;
                formatter.FormatAndSave(model, randomFile, enc);
                string result = enc.GetString(File.ReadAllBytes(randomFile));
                File.WriteAllText("c:\\temp\\test.txt", result);
                string expected = enc.GetString(File.ReadAllBytes("formattedwithrelaxedresolvetags.txt"));
                Assert.That(result, Is.EqualTo(expected));
            }
            finally
            {
                //Console.WriteLine(randomFile);
                File.Delete(randomFile);
            }
        }

        [Test]
        public void FileTemplateWithXmlTags()
        {
            var model = new Hashtable();
            model.Add("fileUrl", GetUrl("cd_catalog.xml"));

            Formatter formatter = Formatter.FileBasedFormatter("templatewithxmltags.htm");
            string randomFile = Path.GetRandomFileName();
            try
            {
                Encoding enc = Encoding.UTF8;
                formatter.FormatAndSave(model, randomFile, enc);
                string result = enc.GetString(File.ReadAllBytes(randomFile));
                string expected = enc.GetString(File.ReadAllBytes("formattedwithxmltags.htm"));
                Assert.That(result, Is.EqualTo(expected));
            }
            finally
            {
                File.Delete(randomFile);
            }
        }

        [Test]
        public void FileTemplateWithXmlTagsWithCachedSheet()
        {
            var model = new Hashtable();
            model.Add("fileUrl", GetUrl("cd_catalog.xml"));

            Formatter formatter = Formatter.FileBasedFormatter("templatewithxmltagscachedsheet.htm");
            string randomFile = Path.GetRandomFileName();
            try
            {
                Encoding enc = Encoding.UTF8;
                formatter.FormatAndSave(model, randomFile, enc);
                string result = enc.GetString(File.ReadAllBytes(randomFile));
                string expected = enc.GetString(File.ReadAllBytes("formattedwithxmltags.htm"));
                Assert.That(result, Is.EqualTo(expected));
            }
            finally
            {
                File.Delete(randomFile);
            }
        }

        [Test]
        public void NullObjectPropertiesShouldEvaluateInEmptyString()
        {
            const string TEMPLATE = "abcd${Text}efgh${Text}";
            const string FORMATTED = "abcdefgh";
            var model = new TestModel();
            model.Text = null;
            var formatter = new Formatter(TEMPLATE).Parse();
            string formatted = formatter.Format(model);
            Assert.That(formatted, Is.EqualTo(FORMATTED));
            Assert.That(formatter.ParsedTemplate.Count, Is.EqualTo(4));
        }

        [Test]
        public void ObjectPropertyTextInLiteralTemplateShouldBeChanged()
        {
            const string TEMPLATE = "abcd'${Text}'efgh${Text}";
            const string FORMATTED = "abcd'12345'efgh12345";
            var model = new TestModel();
            model.Text = "12345";
            var formatter = new Formatter(TEMPLATE).Parse();
            string formatted = formatter.Format(model);
            Assert.That(formatted, Is.EqualTo(FORMATTED));
        }

        [Test]
        public void CommentedObjectPropertyTextShouldNotBeChanged()
        {
            const string TEMPLATE = "abcd'\\$\\{Text\\}'efgh${Text}";
            const string FORMATTED = "abcd'${Text}'efgh12345";
            var model = new TestModel();
            model.Text = "12345";
            var formatter = new Formatter(TEMPLATE).Parse();
            string formatted = formatter.Format(model);
            Assert.That(formatted, Is.EqualTo(FORMATTED));
        }


        [Test]
        public void CommentedCodeShouldBeFilteredOut()
        {
            const string TEMPLATE = "abcd'<%--<mdc:test />--%>\\$\\{Text\\}'efgh${Text}";
            const string FORMATTED = "abcd'${Text}'efgh12345";
            var model = new TestModel();
            model.Text = "12345";
            var formatter = new Formatter(TEMPLATE).Parse();
            string formatted = formatter.Format(model);
            Assert.That(formatted, Is.EqualTo(FORMATTED));
        }

        [Test]
        public void ObjectPropertyTextTemplateAtTheBeginningOfTheInputShouldBeChanged()
        {
            const string TEMPLATE = "${Text}efgh";
            const string FORMATTED = "12345efgh";
            var model = new TestModel();
            model.Text = "12345";
            var formatter = new Formatter(TEMPLATE).Parse();
            string formatted = formatter.Format(model);
            Assert.That(formatted, Is.EqualTo(FORMATTED));
        }

        [Test]
        public void ObjectPropertyTextTemplateAtTheEndOfTheInputShouldBeChanged()
        {
            const string TEMPLATE = "abc${Text}";
            const string FORMATTED = "abc12345";
            var model = new TestModel();
            model.Text = "12345";
            var formatter = new Formatter(TEMPLATE).Parse();
            string formatted = formatter.Format(model);
            Assert.That(formatted, Is.EqualTo(FORMATTED));
        }

        [Test]
        public void ObjectPropertyTextTemplateShouldBeChanged()
        {
            const string TEMPLATE = "abcd${Text}efgh";
            const string FORMATTED = "abcd12345efgh";
            var model = new TestModel();
            model.Text = "12345";
            var formatter = new Formatter(TEMPLATE).Parse();
            string formatted = formatter.Format(model);
            Assert.That(formatted, Is.EqualTo(FORMATTED));
        }

        [Test]
        public void ParseFailureMissingCloseTagShouldThrowParseException()
        {
            const string TEMPLATE = "abcd${Text{slk";
            var model = new TestModel();
            model.Text = "12345";
            try
            {
                new Formatter(TEMPLATE).Parse();
            }
            catch (ParseException Fe)
            {
                Assert.That(Fe.MessageWithOutContext, Is.EqualTo(ParseException.ExpectedToken("}", "slk").Message));
            }
        }

        [Test]
        public void ParseFailureMissingCloseTagShouldThrowParseException2()
        {
            const string TEMPLATE = "abcd$Text{slk";
            var model = new TestModel();
            model.Text = "12345";
            try
            {
                new Formatter(TEMPLATE).Parse();
            }
            catch (ParseException Fe)
            {
                Assert.That(Fe.MessageWithOutContext, Is.EqualTo(ParseException.ExpectedToken("{").Message));
            }
        }

        [Test]
        public void ParseFailureMissingCloseTagShouldThrowParseException3()
        {
            const string TEMPLATE = "abcd{Text}slk";
            var model = new TestModel();
            model.Text = "12345";
            try
            {
                new Formatter(TEMPLATE).Parse();
            }
            catch (ParseException Fe)
            {
                Assert.That(Fe.Message, Is.EqualTo(ParseException.ExpectedToken("$").Message));
            }
        }

        [Test]
        public void ParseFailureOpenTagsAtEndOfString()
        {
            const string TEMPLATE = "abcd${Text";
            var model = new TestModel();
            model.Text = "12345";
            try
            {
                new Formatter(TEMPLATE).Parse();
            }
            catch (ParseException Fe)
            {
                Assert.That(Fe.MessageWithOutContext, Is.EqualTo(ParseException.ExpectedToken("}", "Text").Message));
            }
        }

        [Test]
        public void ParseOfContentBetweenNonCommandTagsShouldNotDistrubtFormatting()
        {
            const string TEMPLATE = "<a>bcd${Text}slk</a>";
            const string FORMATTED = "<a>bcd12345slk</a>";
            var model = new TestModel();
            model.Text = "12345";
            var formatter = new Formatter(TEMPLATE).Parse();
            string formatted = formatter.Format(model);
            Assert.That(formatted, Is.EqualTo(FORMATTED));
            Assert.That(formatter.ParsedTemplate.Count, Is.EqualTo(3));
        }

        [Test]
        public void ParseOfNonCommandTagsShouldNotDistrubtFormatting()
        {
            const string TEMPLATE = "<a/>bcd${Text}slk";
            const string FORMATTED = "<a/>bcd12345slk";
            var model = new TestModel();
            model.Text = "12345";
            var formatter = new Formatter(TEMPLATE).Parse();
            string formatted = formatter.Format(model);
            Assert.That(formatted, Is.EqualTo(FORMATTED));
            Assert.That(formatter.ParsedTemplate.Count, Is.EqualTo(3));
        }


        [Test]
        [Category("Performance")]
        public void PerformanceNoReUse()
        {
            const int RUN = 150;

            var model = new Hashtable();
            var list = new ArrayList();
            list.Add("This is a paragraph");
            list.Add("Some text");
            model.Add("paragraph", list);

            var table = new Hashtable();
            var header = new ArrayList(new[] {"H1", "H2", "H3"});
            var body = new ArrayList();
            body.Add(new[] {"", "X", ""});
            body.Add(new[] {"X", "", "X"});
            body.Add(new[] {"", "X", ""});
            table.Add("header", header);
            table.Add("body", body);
            model.Add("table", table);

            BenchMarkNoReUse("template.htm", model, RUN);
        }

        [Test]
        [Category("Performance")]
        public void PerformanceNoReUseAllResolve()
        {
            const int RUN = 175;

            var model = new Hashtable();
            var list = new ArrayList();
            list.Add("This is a paragraph");
            list.Add("Some text");
            model.Add("paragraph", list);

            var table = new Hashtable();
            var header = new ArrayList(new[] {"H1", "H2", "H3"});
            var body = new ArrayList();
            body.Add(new[] {"", "X", ""});
            body.Add(new[] {"X", "", "X"});
            body.Add(new[] {"", "X", ""});
            table.Add("header", header);
            table.Add("body", body);
            model.Add("table", table);

            BenchMarkNoReUse("templateallresolve.htm", model, RUN);
        }

        [Test]
        [Category("Performance")]
        public void PerformanceReUse()
        {
            const int RUN = 9500;

            var model = new Hashtable();
            var list = new ArrayList();
            list.Add("This is a paragraph");
            list.Add("Some text");
            model.Add("paragraph", list);

            var table = new Hashtable();
            var header = new ArrayList(new[] {"H1", "H2", "H3"});
            var body = new ArrayList();
            body.Add(new[] {"", "X", ""});
            body.Add(new[] {"X", "", "X"});
            body.Add(new[] {"", "X", ""});
            table.Add("header", header);
            table.Add("body", body);
            model.Add("table", table);

            BenchMarkReUse("template.htm", model, RUN);
        }

        [Test]
        [Category("Performance")]
        public void PerformanceReUseAllResolve()
        {
            const int RUN = 5000;

            var model = new Hashtable();
            var list = new ArrayList();
            list.Add("This is a paragraph");
            list.Add("Some text");
            model.Add("paragraph", list);

            var table = new Hashtable();
            var header = new ArrayList(new[] {"H1", "H2", "H3"});
            var body = new ArrayList();
            body.Add(new[] {"", "X", ""});
            body.Add(new[] {"X", "", "X"});
            body.Add(new[] {"", "X", ""});
            table.Add("header", header);
            table.Add("body", body);
            model.Add("table", table);

            BenchMarkReUse("templateallresolve.htm", model, RUN);
        }

        [Test]
        [Category("Performance")]
        public void PerformanceReUseAllResolveWithExpression()
        {
            const int RUN = 25000;

            var model = new Hashtable();
            model.Add("small", 3);
            model.Add("large", 11);
            model.Add("text", "some text");
            model.Add("greet", "Hello");
            model.Add("to", "world");


            BenchMarkReUse("templatewithexpressions.htm", model, RUN);
        }

        [Test]
        [Category("Performance")]
        public void PerformanceReUseAllResolveWithFormatTags()
        {
            const int RUN = 4500;

            var model = new Hashtable();

            BenchMarkReUse("templatewithformattags.htm", model, RUN);
        }

        [Test]
        [Category("Performance")]
        public void PerformanceReUseAllResolveWithTags()
        {
            const int RUN = 4500;

            var model = new Hashtable();
            model.Add("small", 3);
            model.Add("large", 11);
            model.Add("text", "some text");
            model.Add("greet", "Hello");
            model.Add("to", "world");

            var list = new ArrayList(new[] {"H1", "H2", "H3"});
            model.Add("list", list);

            BenchMarkReUse("templatewithtags.htm", model, RUN);
        }

        [Test]
        [Category("Performance")]
        public void PerformanceReUseAllResolveWithXmlTags()
        {
            const int RUN = 200;

            var model = new Hashtable();
            model.Add("fileUrl", GetUrl("cd_catalog.xml"));
            BenchMarkReUse("templatewithxmltags.htm", model, RUN);
        }

        [Test]
        [Category("Performance")]
        public void PerformanceReUseAllResolveWithXmlTagsCachedSheet()
        {
            const int RUN = 1400;
            var model = new Hashtable();
            model.Add("fileUrl", GetUrl("cd_catalog.xml"));
            BenchMarkReUse("templatewithxmltagscachedsheet.htm", model, RUN);
        }

        [Test]
        public void PlainTextTempalteShouldNotBeChanged()
        {
            const string TEMPLATE = "abcdefgh";
            var formatter = new Formatter(TEMPLATE).Parse();
            string formatted = formatter.Format(new object());
            Assert.That(formatted, Is.EqualTo(TEMPLATE));
        }

        [Test]
        public void Reuseable()
        {
            var model = new Hashtable();
            var list = new ArrayList();
            list.Add("This is a paragraph");
            list.Add("Some text");
            model.Add("paragraph", list);

            var table = new Hashtable();
            var header = new ArrayList(new[] {"H1", "H2", "H3"});
            var body = new ArrayList();
            body.Add(new[] {"", "X", ""});
            body.Add(new[] {"X", "", "X"});
            body.Add(new[] {"", "X", ""});
            table.Add("header", header);
            table.Add("body", body);
            model.Add("table", table);

            Formatter formatter = Formatter.FileBasedFormatter("template.htm");
            string first = formatter.Format(model);
            string second = formatter.Format(model);
            Assert.That(first, Is.EqualTo(second));
        }

        [Test]
        public void SimpleFileTemplate()
        {
            var model = new Hashtable();
            var list = new ArrayList();
            list.Add("This is a paragraph");
            list.Add("Some text");
            model.Add("paragraph", list);

            var table = new Hashtable();
            var header = new ArrayList(new[] {"H1", "H2", "H3"});
            var body = new ArrayList();
            body.Add(new[] {"", "X", ""});
            body.Add(new[] {"X", "", "X"});
            body.Add(new[] {"", "X", ""});
            table.Add("header", header);
            table.Add("body", body);
            model.Add("table", table);

            Formatter formatter = Formatter.FileBasedFormatter("template.htm");
            string randomFile = Path.GetRandomFileName();
            try
            {
                Encoding enc = Encoding.UTF8;
                formatter.FormatAndSave(model, randomFile, enc);
                string result = enc.GetString(File.ReadAllBytes(randomFile));
                string expected = enc.GetString(File.ReadAllBytes("formatted.htm"));
                Assert.That(result, Is.EqualTo(expected));
            }
            finally
            {
                File.Delete(randomFile);
            }
        }

        [Test]
        public void TwoObjectPropertiesTextTemplateShouldBeChanged()
        {
            const string TEMPLATE = "abcd${Text}efgh${Text}";
            const string FORMATTED = "abcd12345efgh12345";
            var model = new TestModel();
            model.Text = "12345";
            var formatter = new Formatter(TEMPLATE).Parse();
            string formatted = formatter.Format(model);
            Assert.That(formatted, Is.EqualTo(FORMATTED));
        }

        [Test]
        public void TestFSharpTemplate()
        {
            var raw = new Hashtable()
            {
                {"a", "B"},
                {"c", "D"},
                {"now", new DateTime(2012,11,5,17,30,0).ToString("dd-MM-yyyy hh:mm:yyyy")}
            };
            var formatter = new Formatter("<!-- This file is generated at ${now} -->\n${a} ==> ${c}").Parse();
            Assert.That(formatter.Format(new TagModel(raw)), Is.EqualTo("<!-- This file is generated at 05-11-2012 05:30:2012 -->\nB ==> D"));
        }

      


    }
}
