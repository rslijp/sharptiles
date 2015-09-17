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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Connectors;
using org.SharpTiles.HtmlTags.Input;
using org.SharpTiles.Tags;
using org.SharpTiles.Templates;

namespace org.SharpTiles.HtmlTags.Test
{
    [TestFixture]
    public class HtmlParserTest
    {

        public const long BENCHMARK_FIX = 400000;
        public static double BENCHMARK_RATIO;
        private static bool _benchMarkSet = false;

        static HtmlParserTest()
        {
            TagLib.Register(new Html());
        }

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
                BENCHMARK_RATIO = (i / (double)BENCHMARK_FIX);
                Console.WriteLine("BENCHMARK RATIO {0} ({1}/{2}:", BENCHMARK_RATIO, i, BENCHMARK_FIX);
                _benchMarkSet = true;
            }
        }

        
        public HtmlHelper GetHelperWithValidation()
        {
            HtmlHelper helper = GetHtmlHelper();
            var state = new ModelState { Value = new ValueProviderResult("x", "x", null)};
            state.Errors.Add("error");
            helper.ViewData.ModelState.Add("errorName", state);
            helper.ViewData.ModelState.Add("modelName", new ModelState { Value=new ValueProviderResult("x", "x", null) });
            return helper;
        }

        [Test]
        public void TestDoubleAttribute()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHtmlHelper();
            model.Model["name"] = "name";
            try
            {
                TagLibParser.Parse("<html:textBox name=\"${Model.name}\" name=\"${Model.name}\"/>");
                Assert.Fail("Expected exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.MessageWithOutContext, Is.EqualTo(TagException.PropertyAlReadySet("name").Message));
            }
        }


        [Test]
        public void TestMissingAttribute()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHtmlHelper();
            model.Model["name"] = "name";
            try
            {
                ITag tag = TagLibParser.Parse("<html:radioButton name=\"${Model.name}\"/>");
                tag.Evaluate(model);
                Assert.Fail("Expected exception");
            }
            catch (HtmlHelperTagException HHTe)
            {
                Assert.That(HHTe.MessageWithOutContext, Is.EqualTo(
                                                            HtmlHelperTagException.RequiredArgumentMissing(
                                                                "value",
                                                                CandateMethod(
                                                                Html.New<RadioButtonTag>(new List<IParameterValue>
                                                                                             {
                                                                                                 new ParameterValue
                                                                                                     {Name = "name"}
                                                                                             })
                                                                    )).Message)
                    );
            }
        }

        private static MethodInfo CandateMethod(HtmlHelperWrapperTag tag)
        {
            return new HtmlReflectionHelper(tag.WrappedType, tag.MethodName)
                       {
                           Parameters = tag.Parameters
                       }.CandidateMethod();
        }


        [Test]
        public void TestParseOfCheckBoxTag()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHtmlHelper();
            model.Model["name"] = "name";
            model.Model["value"] = "false";
            ITag tag =
                TagLibParser.Parse(
                    "<html:checkBox name=\"${Model.name}\" isChecked=\"${Model.value}\"/>");
            Assert.That(tag.Evaluate(model), Is.EqualTo(
                                                 "<input id=\"name\" name=\"name\" type=\"checkbox\" value=\"true\" /><input name=\"name\" type=\"hidden\" value=\"false\" />"
                                                 ));
        }

        [Test]
        public void TestParseOfCheckBoxTagNameOnly()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHtmlHelper();
            model.Model["name"] = "name";
            ITag tag =
                TagLibParser.Parse(
                    "<html:checkBox name=\"${Model.name}\" isChecked=\"true\"/>");
            Assert.That(tag.Evaluate(model), Is.EqualTo(
                                                 "<input checked=\"checked\" id=\"name\" name=\"name\" type=\"checkbox\" value=\"true\" /><input name=\"name\" type=\"hidden\" value=\"false\" />"
                                                 ));
        }

        [Test]
        public void TestParseOfCheckBoxTagWithHtmlAttribute()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHtmlHelper();
            model.Model["name"] = "name";
            model.Model["value"] = "true";
            model.Model["style"] = "nice";
            ITag tag =
                TagLibParser.Parse(
                    "<html:checkBox name=\"${Model.name}\" isChecked=\"${Model.value}\" class=\"${Model.style}\"/>");
            Assert.That(tag.Evaluate(model),
                        Is.EqualTo(
                            "<input checked=\"checked\" class=\"nice\" id=\"name\" name=\"name\" type=\"checkbox\" value=\"true\" /><input name=\"name\" type=\"hidden\" value=\"false\" />"
                            ));
        }

        [Test]
        public void TestParseOfHiddenTag()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHtmlHelper();
            model.Model["name"] = "name";
            model.Model["value"] = "value";
            ITag tag = TagLibParser.Parse("<html:hidden name=\"${Model.name}\" value=\"${Model.value}\"/>");
            Assert.That(tag.Evaluate(model), Is.EqualTo(
                                                 "<input id=\"name\" name=\"name\" type=\"hidden\" value=\"value\" />"
                                                 ));
        }

        [Test]
        public void TestParseOfHiddenTagNameOnly()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHtmlHelper();
            model.Model["name"] = "name";
            ITag tag = TagLibParser.Parse("<html:hidden name=\"${Model.name}\"/>");
            Assert.That(tag.Evaluate(model),
                        Is.EqualTo("<input id=\"name\" name=\"name\" type=\"hidden\" value=\"\" />"));
        }

        [Test]
        public void TestParseOfHiddenTagWithHtmlAttribute()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHtmlHelper();
            model.Model["name"] = "name";
            model.Model["value"] = "true";
            model.Model["style"] = "nice";

            ITag tag =
                TagLibParser.Parse(
                    "<html:hidden name=\"${Model.name}\" value=\"${Model.value}\" class=\"${Model.style}\"/>");
            Assert.That(tag.Evaluate(model),
                        Is.EqualTo("<input class=\"nice\" id=\"name\" name=\"name\" type=\"hidden\" value=\"true\" />"));
        }

        [Test]
        public void TestParseOfPasswordTag()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHtmlHelper();
            model.Model["name"] = "name";
            model.Model["value"] = "value";
            ITag tag = TagLibParser.Parse("<html:password name=\"${Model.name}\" value=\"${Model.value}\"/>");
            Assert.That(tag.Evaluate(model), Is.EqualTo(
                                                 "<input id=\"name\" name=\"name\" type=\"password\" value=\"value\" />"
                                                 ));
        }

        [Test]
        public void TestParseOfPasswordTagNameOnly()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHtmlHelper();
            model.Model["name"] = "name";
            ITag tag = TagLibParser.Parse("<html:password name=\"${Model.name}\"/>");
            Assert.That(tag.Evaluate(model),
                        Is.EqualTo("<input id=\"name\" name=\"name\" type=\"password\" />"));
        }

        [Test]
        public void TestParseOfPasswordTagWithHtmlAttribute()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHtmlHelper();
            model.Model["name"] = "name";
            model.Model["value"] = "true";
            model.Model["style"] = "nice";

            ITag tag =
                TagLibParser.Parse(
                    "<html:password name=\"${Model.name}\" value=\"${Model.value}\" class=\"${Model.style}\"/>");
            Assert.That(tag.Evaluate(model),
                        Is.EqualTo("<input class=\"nice\" id=\"name\" name=\"name\" type=\"password\" value=\"true\" />"));
        }

        [Test]
        public void TestParseOfRadioButtonTag()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHtmlHelper();
            model.Model["name"] = "name";
            model.Model["value"] = "value";
            model.Model["isChecked"] = "false";
            ITag tag =
                TagLibParser.Parse(
                    "<html:radioButton name=\"${Model.name}\" value=\"${Model.value}\" isChecked=\"${Model.isChecked}\"/>");
            Assert.That(tag.Evaluate(model),
                        Is.EqualTo("<input id=\"name\" name=\"name\" type=\"radio\" value=\"value\" />"));
        }

        [Test]
        public void TestParseOfRadioButtonTagNameAndValueOnly()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHtmlHelper();
            model.Model["name"] = "name";
            model.Model["value"] = "value";
            ITag tag = TagLibParser.Parse("<html:radioButton name=\"${Model.name}\" value=\"${Model.value}\"/>");
            Assert.That(tag.Evaluate(model),
                        Is.EqualTo("<input id=\"name\" name=\"name\" type=\"radio\" value=\"value\" />"));
        }

        [Test]
        public void TestParseOfRadioButtonTagWithHtmlAttribute()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHtmlHelper();
            model.Model["name"] = "name";
            model.Model["value"] = "true";
            model.Model["style"] = "nice";

            ITag tag =
                TagLibParser.Parse(
                    "<html:radioButton name=\"${Model.name}\" value=\"${Model.value}\" class=\"${Model.style}\"/>");
            //Console.WriteLine(tag.Evaluate(model).Replace("\"", "\\\""));
            Assert.That(tag.Evaluate(model),
                        Is.EqualTo("<input class=\"nice\" id=\"name\" name=\"name\" type=\"radio\" value=\"true\" />"));
        }

        [Test]
        public void TestParseOfTextAreaTag()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHtmlHelper();
            model.Model["name"] = "name";
            model.Model["value"] = "value";
            model.Model["rows"] = "80";
            model.Model["columns"] = "60";
            model.Model["isChecked"] = "false";

            ITag tag =
                TagLibParser.Parse(
                    "<html:textArea name=\"${Model.name}\" value=\"${Model.value}\" columns=\"${Model.columns}\" rows=\"${Model.rows}\" isChecked=\"${Model.isChecked}\"/>");
            Assert.That(tag.Evaluate(model),
                        Is.EqualTo(
                            "<textarea cols=\"60\" id=\"name\" isChecked=\"false\" name=\"name\" rows=\"80\">\r\nvalue</textarea>"));
        }

        [Test]
        public void TestParseOfTextAreaTagNameOnly()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHtmlHelper();
            model.Model["name"] = "name";
            ITag tag = TagLibParser.Parse("<html:textArea name=\"${Model.name}\"/>");
            Assert.That(tag.Evaluate(model),
                        Is.EqualTo("<textarea cols=\"20\" id=\"name\" name=\"name\" rows=\"2\">\r\n</textarea>"));
        }

        [Test]
        public void TestParseOfTextAreaTagWithHtmlAttribute()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHtmlHelper();
            model.Model["name"] = "name";
            model.Model["value"] = "true";
            model.Model["style"] = "nice";

            ITag tag =
                TagLibParser.Parse(
                    "<html:textArea name=\"${Model.name}\" value=\"${Model.value}\" class=\"${Model.style}\"/>");
            Assert.That(tag.Evaluate(model),
                        Is.EqualTo(
                            "<textarea class=\"nice\" cols=\"20\" id=\"name\" name=\"name\" rows=\"2\">\r\ntrue</textarea>"));
        }

        [Test]
        public void TestParseOfTextBoxTag()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHtmlHelper();
            model.Model["name"] = "name";
            model.Model["value"] = "value";
            ITag tag = TagLibParser.Parse("<html:textBox name=\"${Model.name}\" value=\"${Model.value}\"/>");
            Assert.That(tag.Evaluate(model), Is.EqualTo(
                                                 "<input id=\"name\" name=\"name\" type=\"text\" value=\"value\" />"
                                                 ));
        }

        [Test]
        public void TestParseOfTextBoxTagNameOnly()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHtmlHelper();
            model.Model["name"] = "name";
            ITag tag = TagLibParser.Parse("<html:textBox name=\"${Model.name}\"/>");
            Assert.That(tag.Evaluate(model), Is.EqualTo("<input id=\"name\" name=\"name\" type=\"text\" value=\"\" />"));
        }

        [Test]
        public void TestParseOfTextBoxTagWithHtmlAttribute()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHtmlHelper();
            model.Model["name"] = "name";
            model.Model["value"] = "true";
            model.Model["style"] = "nice";

            ITag tag =
                TagLibParser.Parse(
                    "<html:textBox name=\"${Model.name}\" value=\"${Model.value}\" class=\"${Model.style}\"/>");
            Assert.That(tag.Evaluate(model),
                        Is.EqualTo("<input class=\"nice\" id=\"name\" name=\"name\" type=\"text\" value=\"true\" />"));
        }

        [Test]
        public void TestParseOfValidationMessageTagCorrectValue()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHelperWithValidation();
            model.Model["modelName"] = "modelName";
            model.Model["validationMessage"] = "message";
            ITag tag =
                TagLibParser.Parse(
                    "<html:validationMessage modelName=\"${Model.modelName}\" validationMessage=\"${Model.validationMessage}\"/>");
            Assert.That(tag.Evaluate(model), Is.Null);
        }

        [Test]
        public void TestParseOfValidationMessageTagNameAndMessage()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHelperWithValidation();
            model.Model["modelName"] = "errorName";
            model.Model["validationMessage"] = "alternative message";
            ITag tag =
                TagLibParser.Parse(
                    "<html:validationMessage modelName=\"${Model.modelName}\" validationMessage=\"${Model.validationMessage}\"/>");
            Assert.That(tag.Evaluate(model),
                        Is.EqualTo("<span class=\"field-validation-error\">alternative message</span>"));
        }

        [Test]
        public void TestParseOfValidationMessageTagNameOnly()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHelperWithValidation();
            model.Model["modelName"] = "errorName";
            ITag tag = TagLibParser.Parse("<html:validationMessage modelName=\"${Model.modelName}\"/>");
            Assert.That(tag.Evaluate(model), Is.EqualTo("<span class=\"field-validation-error\">error</span>"));
        }


        [Test]
        public void TestParseOfValidationMessageTagNoValue()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHelperWithValidation();
            model.Model["modelName"] = "name";
            model.Model["validationMessage"] = "message";
            ITag tag =
                TagLibParser.Parse(
                    "<html:validationMessage modelName=\"${Model.modelName}\" validationMessage=\"${Model.validationMessage}\"/>");
            Assert.That(tag.Evaluate(model), Is.Null);
        }

        [Test]
        public void TestParseOfValidationMessageTagWithHtmlAttribute()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHelperWithValidation();
            model.Model["name"] = "errorName";
            model.Model["value"] = "true";
            model.Model["style"] = "nice";

            ITag tag =
                TagLibParser.Parse(
                    "<html:validationMessage modelName=\"${Model.name}\" value=\"${Model.value}\" class=\"${Model.style}\"/>");
            Assert.That(tag.Evaluate(model), Is.EqualTo("<span class=\"nice\" value=\"true\">error</span>"));
        }


        [Test]
        public void TestParseOfValidationSummaryTag()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHelperWithValidation();
            ITag tag =
                TagLibParser.Parse(
                    "<html:validationSummary/>");
            Assert.That(tag.Evaluate(model), Is.EqualTo("<ul class=\"validation-summary-errors\"><li>error</li>\r\n</ul>"));
        }

        [Test]
        public void TestParseOfValidationSummaryTagWithHtmlAttribute()
        {
            var model = new TagModel(new Hashtable());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHelperWithValidation();
            model.Model["class"] = "nice";
            ITag tag =
                TagLibParser.Parse(
                    "<html:validationSummary message=\"msg\" class=\"${Model.class}\"/>");
            
            Assert.That(tag.Evaluate(model), Is.EqualTo("<ul class=\"nice\" message=\"msg\"><li>error</li>\r\n</ul>"));
        }

        [Test]
        public void TestParseOfListBoxTagNameOnly()
        {
            var model = new TagModel(new Hashtable());
            HtmlHelper helper = GetHtmlHelper();
            helper.ViewData.Add("name", new MultiSelectList(new []{"a", "b", "c", "d"}, null, null, new []{"a", "c"}));
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = helper;
            model.Model["name"] = "name";
            ITag tag =
                TagLibParser.Parse(
                    "<html:listBox name=\"${Model.name}\"/>");
            Assert.That(tag.Evaluate(model), Is.EqualTo(
                "<select id=\"name\" multiple=\"multiple\" name=\"name\"><option selected=\"selected\">a</option>\r\n<option>b</option>\r\n<option selected=\"selected\">c</option>\r\n<option>d</option>\r\n</select>"));
        }


        [Test, Ignore]
        public void TestParseOfListBoxTagWithHtmlAttribute()
        {
            var model = new TagModel(new Hashtable());
            HtmlHelper helper = GetHtmlHelper();
            helper.ViewData.Add("name", new MultiSelectList(new[] { "a", "b", "c", "d" }, null, null, new[] { "a", "c" }));
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = helper;
            model.Model["name"] = "name";
            model.Model["class"] = "nice";
            ITag tag =
                TagLibParser.Parse(
                    "<html:listBox class=\"${Model.class}\" name=\"${Model.name}\"/>");
            Assert.That(tag.Evaluate(model), Is.EqualTo(
                "<select class=\"nice\" id=\"name\" multiple=\"multiple\" name=\"name\"><option selected=\"selected\">a</option>\r\n<option>b</option>\r\n<option selected=\"selected\">c</option>\r\n<option>d</option>\r\n</select>"));
        }

        [Test]
        public void TestParseOfListBoxTag()
        {
            var model = new TagModel(new Hashtable());
            HtmlHelper helper = GetHtmlHelper();
            helper.ViewData.Add("name", new MultiSelectList(new[] { "a", "b", "c", "d" }, null, null, new[] { "a", "c" }));
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = helper;
            model.Model["name"] = "name";
            model.Model["list"] = new MultiSelectList(new[] { "a", "b", "c", "d" }, null, null, new[] { "a", "c" });
            
           ITag tag =
                TagLibParser.Parse(
                    "<html:listBox name=\"${Model.name}\" selectList=\"${Model.list}\"/>");
            Assert.That(tag.Evaluate(model), Is.EqualTo(
                "<select id=\"name\" multiple=\"multiple\" name=\"name\"><option>a</option>\r\n<option>b</option>\r\n<option>c</option>\r\n<option>d</option>\r\n</select>"));
        }

        [Test]
        public void TestParseOfDropDownListTagNameOnly()
        {
            var model = new TagModel(new Hashtable());
            HtmlHelper helper = GetHtmlHelper();
            helper.ViewData.Add("name", new SelectList(new[] { "a", "b", "c", "d" }, null, null, "c"));
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = helper;
            model.Model["name"] = "name";
            ITag tag =
                TagLibParser.Parse(
                    "<html:dropDownList name=\"${Model.name}\"/>");
            Assert.That(tag.Evaluate(model), Is.EqualTo(
                "<select id=\"name\" name=\"name\"><option>a</option>\r\n<option>b</option>\r\n<option selected=\"selected\">c</option>\r\n<option>d</option>\r\n</select>"));
        }


        [Test, Ignore]
        public void TestParseOfDropDownListTagWithHtmlAttribute()
        {
            var model = new TagModel(new Hashtable());
            HtmlHelper helper = GetHtmlHelper();
            helper.ViewData.Add("name", new SelectList(new[] { "a", "b", "c", "d" }, null, null, "c"));
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = helper;
            model.Model["name"] = "name";
            model.Model["class"] = "nice";
            ITag tag =
                TagLibParser.Parse(
                    "<html:dropDownList class=\"${Model.class}\" name=\"${Model.name}\"/>");
            Assert.That(tag.Evaluate(model), Is.EqualTo(
                "<select class=\"nice\" id=\"name\" name=\"name\"><option>a</option>\r\n<option>b</option>\r\n<option selected=\"selected\">c</option>\r\n<option>d</option>\r\n</select>"));
        }

        [Test]
        public void TestActionTagWithActionAndController()
        {
            var model = new TagModel(new Hashtable());
            UrlHelper helper = GetUrlHelper("Welcome");
            model.Page[Html.PAGE_MODEL_URLHELPER_INSTANCE] = helper;
            model.Model["action"] = "Bye";
            model.Model["controller"] = "LogOut";

            ITag tag =
                 TagLibParser.Parse(
                     "<html:action action=\"${Model.action}\" controller=\"${Model.controller}\"/>");
            Assert.That(tag.Evaluate(model), Is.EqualTo("/SharpTiles/LogOut/Bye"));
        }

        [Test]
        public void TestFormTagWithActionAndSameController()
        {
            var model = new TagModel(new Hashtable());
            UrlHelper helper = GetUrlHelper("Welcome");
            model.Page[Html.PAGE_MODEL_URLHELPER_INSTANCE] = helper;
            model.Model["action"] = "Bye";
            model.Model["controller"] = "Welcome";

            ITag tag =
                 TagLibParser.Parse(
                     "<html:form action=\"${Model.action}\" controller=\"${Model.controller}\" method=\"post\">abc</html:form>");
            Assert.That(tag.Evaluate(model), Is.EqualTo("<form action=\"/SharpTiles/Welcome/Bye\" method=\"post\">abc</form>"));
        }

        [Test]
        public void TestActionTagWithActionOnlyNoMethodNoControllerNoBody()
        {
            var model = new TagModel(new Hashtable());
            UrlHelper helper = GetUrlHelper("Welcome");
            model.Page[Html.PAGE_MODEL_URLHELPER_INSTANCE] = helper;
            model.Model["action"] = "Bye";
            
            ITag tag =
                 TagLibParser.Parse(
                     "<html:form action=\"${Model.action}\"/>");
            Assert.That(tag.Evaluate(model), Is.EqualTo("<form action=\"/SharpTiles/Welcome/Bye\" method=\"post\" />"));
        }

        [Test]
        public void TestActionTagWithOutActionOnlyNoMethodNoControllerNoBody()
        {
            var model = new TagModel(new Hashtable());
            UrlHelper helper = GetUrlHelper("Welcome");
            model.Page[Html.PAGE_MODEL_URLHELPER_INSTANCE] = helper;
            model.Model["action"] = "Bye";

            ITag tag =
                 TagLibParser.Parse(
                     "<html:form/>");
            Assert.That(tag.Evaluate(model), Is.EqualTo("<form method=\"post\" />"));
        }


        [Test]
        public void TestActionTagWithActionAndNoController()
        {
            var model = new TagModel(new Hashtable());
            UrlHelper helper = GetUrlHelper("Welcome");
            model.Page[Html.PAGE_MODEL_URLHELPER_INSTANCE] = helper;
            model.Model["action"] = "Bye";
        
            ITag tag =
                 TagLibParser.Parse(
                     "<html:action action=\"${Model.action}\" controller=\"${Model.controller}\"/>");
            Assert.That(tag.Evaluate(model), Is.EqualTo("/SharpTiles/Welcome/Bye"));
        }

        [Test]
        public void TestFormTagWithActionAndController()
        {
            var model = new TagModel(new Hashtable());
            UrlHelper helper = GetUrlHelper("Welcome");
            model.Page[Html.PAGE_MODEL_URLHELPER_INSTANCE] = helper;
            model.Model["action"] = "Bye";
            model.Model["controller"] = "LogOut";

            ITag tag =
                 TagLibParser.Parse(
                     "<html:action action=\"${Model.action}\" controller=\"${Model.controller}\"/>");
            Assert.That(tag.Evaluate(model), Is.EqualTo("/SharpTiles/LogOut/Bye"));
        }

        private static HtmlHelper GetHtmlHelper()
        {
            var controller = new TestController();
            var result = (ViewResult)controller.AlternativeViewData();
            ViewEngineResult engineResult = new TilesViewEngine().FindView(null, result.ViewName, null, false);
            var viewContext = new ViewContext(controller.ControllerContext, engineResult.View,
                                              result.ViewData,
                                              new TempDataDictionary());
            return new HtmlHelper(viewContext, new MockViewDataDictionary());
        }


        private static UrlHelper GetUrlHelper(string currentController)
        {
            GetHtmlHelper();
            var data = new RouteData();
            data.Values.Add("controller", currentController);
            var collection = new RouteCollection(new MockVirtualPathProvider());
            collection.Add(new MockRouteBase());
            var helper = new UrlHelper(new RequestContext(new MockContextBase(), data), collection);
            return helper;
        }

        

        [Test]
        public void TestParseOfDropDownListTag()
        {
            var model = new TagModel(new Hashtable());
            HtmlHelper helper = GetHtmlHelper();
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = helper;
            model.Model["name"] = "name";
            model.Model["list"] = new SelectList(new[] { "a", "b", "c", "d" }, null, null, "c");

            ITag tag =
                 TagLibParser.Parse(
                     "<html:dropDownList name=\"${Model.name}\" selectList=\"${Model.list}\"/>");
            Assert.That(tag.Evaluate(model), Is.EqualTo(
                "<select id=\"name\" name=\"name\"><option>a</option>\r\n<option>b</option>\r\n<option selected=\"selected\">c</option>\r\n<option>d</option>\r\n</select>"));
        }

        [Test]
        public void FileTemplateWithHtmlTags()
        {
            var model = GetModel();

            Formatter formatter = Formatter.FileBasedFormatter("templatehtml.htm");
            string randomFile = Path.GetRandomFileName();
            try
            {
                Encoding enc = Encoding.UTF8;
                formatter.FormatAndSave(model, randomFile, enc);
                var result = enc.GetString(File.ReadAllBytes(randomFile));
                var expected = enc.GetString(File.ReadAllBytes("formattedhtml.htm"));
                Assert.That(result, Is.EqualTo(expected));
            }
            finally
            {
                File.Delete(randomFile);
            }
        }

        [Test]
        [Category("Performance")]
        public void PerformanceHtmlTagsNoReUse()
        {
            const int RUN = 100;
            BenchMarkNoReUse("templatehtml.htm", GetModel(), RUN);
        }

        private static TagModel GetModel()
        {
            var model = new TagModel(new Hashtable());
            model.Model["style"] = "sharptiles";
            model.Model["textBoxName"] = "textBox";
            model.Model["radioButtonName"] = "radioButton";
            model.Model["checkBoxName"] = "checkBoxName";
            model.Model["hiddenName"] = "hiddenName";
            model.Model["passwordName"] = "passwordName";
            model.Model["textAreaName"] = "textArea";
            model.Model["modelName"] = "someProperty";
            model.Model["listBoxName"] = "Model.listBox";
            model.Model["dropDownListName"] = "Model.dropDownList";
            model.Model["textBoxValue"] = "textBox";
            model.Model["checkBoxValue"] = "true";
            model.Model["hiddenValue"] = "hidden";
            model.Model["passwordValue"] = "password";
            model.Model["textAreaValue"] = "textArea";
            model.Model["radioButtonValue"] = "radio";
            model.Model["rows"] = "25";
            model.Model["columns"] = "80";
            model.Model["validationMessage"] = "message";
            model.Model["mutliList"] = new MultiSelectList(new[] { "a", "b", "c", "d"}, null, null, new[] { "a", "c" });
            model.Model["selectList"] = new SelectList(new[] { "a", "b", "c", "d" }, null, null, "c");
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHtmlHelper();
            model.Page[Html.PAGE_MODEL_URLHELPER_INSTANCE] = GetUrlHelper("Welcome");
            model.Model["action"] = "Bye";
            model.Model["controller"] = "LogOut";
            return model;
        }

        [Test]
        [Category("Performance")]
        public void PerformanceHtmlTagseReUse()
        {
            const int RUN = 160;
            BenchMarkReUse("templatehtml.htm", GetModel(), RUN);
        }


        private static void BenchMarkReUse(string fileNameTemplate, TagModel model, int run)
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
            double avg = (run / (time.TotalMilliseconds / 1000.0));
            //the number in run should be able to be processed in a second
            //the benchmark ratio is to cope with individual performance of different systems
            Assert.That(avg, Is.GreaterThanOrEqualTo(BENCHMARK_RATIO * run));
            Console.WriteLine(fileNameTemplate + ": " + avg + " average formats per second no one parse");
        }

        private static void BenchMarkNoReUse(string fileNameTemplate, TagModel model, int run)
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
            double avg = (run / (time.TotalMilliseconds / 1000.0));
            //the number in run should be able to be processed in a second
            //the benchmark ratio is to cope with individual performance of different systems
            Assert.That(avg, Is.GreaterThanOrEqualTo(BENCHMARK_RATIO * run));
            Console.WriteLine(fileNameTemplate + ": " + avg + " average formats per second no one parse");
        }

    }

    internal class MockVirtualPathProvider : VirtualPathProvider
    {
    }

    internal class MockRouteBase : RouteBase
    {
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var bye = new RouteData();
            bye.DataTokens.Add("Bye", new object());
            
            var route = new RouteData();
            route.DataTokens.Add("LogOut", bye);
            return route;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
           return new VirtualPathData(this, values["controller"]+"/"+values["action"]);
        }
    }
}