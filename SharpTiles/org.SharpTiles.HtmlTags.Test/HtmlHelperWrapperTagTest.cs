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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Common;
using org.SharpTiles.Connectors;
using org.SharpTiles.Expressions;
using org.SharpTiles.HtmlTags.Input;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.Creators;
using org.SharpTiles.Templates;

namespace org.SharpTiles.HtmlTags.Test
{
    [TestFixture]
    public class HtmlHelperWrapperTagTest
    {
        private class TestHtmlTag : HtmlHelperWrapperInputTag
        {
            public static readonly string NAME = "checkBox";

            public override string TagName
            {
                get { return NAME; }
            }

            public IEnumerable<MethodInfo> AllMethods
            {
                get { return _helper.AllMethods; }
            }

            public MethodInfo CandidateMethod()
            {
                return _helper.CandidateMethod();
            }

            public IEnumerable<MethodInfo> CandidateMethods()
            {
                return _helper.CandidateMethods();
            }

            public object[] AssembleParameters(MethodInfo method, HtmlHelper helper, TagModel model)
            {
                return _helper.AssembleParameters(method, helper, model);
            }
        }

        private static IEnumerable<MethodInfo> GetCandidates(params string[] parameters)
        {
            return
                Html.New<TestHtmlTag>(
                    parameters.ToList().ConvertAll(p => new ParameterValue {Name = p} as IParameterValue)).
                    CandidateMethods();
        }

        private static MethodInfo GetCandidate(params string[] parameters)
        {
            return
                Html.New<TestHtmlTag>(
                    parameters.ToList().ConvertAll(p => new ParameterValue {Name = p} as IParameterValue)).
                    CandidateMethod();
        }

        public class TestParameterValue : IParameterValue
        {
            public object TestValue { get; set; }

            #region IParameterValue Members

            public string Name { get; set; }

            public object Value(TagModel model)
            {
                return TestValue;
            }

            #endregion

            public override string ToString()
            {
                return Name + "=" + TestValue;
            }
        }

        private void CheckParameters(MethodInfo method, object[] values)
        {

            Assert.That(values.Length, Is.EqualTo(method.GetParameters().Count()));
            for (int i = 0; i < values.Length; i++)
            {
                Assert.That(TypeConverter.Possible(values[i].GetType(), method.GetParameters()[i].ParameterType));
            }
        }

        private static HtmlHelper GetHelper()
        {
            var controller = new TestController();
            var result = (ViewResult) controller.AlternativeViewData();
            ViewEngineResult engineResult = new TilesViewEngine().FindView(null, result.ViewName, null, false);
            var viewContext = new ViewContext(controller.ControllerContext, engineResult.View,
                                              result.ViewData,
                                              new TempDataDictionary());
            return new HtmlHelper(viewContext, new MockViewDataDictionary());
        }

        [Test]
        public void AllCandidatesShouldNarrowOnMoreSpecificInput()
        {
            IEnumerable<MethodInfo> candidates = GetCandidates("name");
            Assert.That(candidates.Count(), Is.GreaterThan(1));
            int count = candidates.Count(info => info.GetParameters().Any(p => p.Name.Equals("isChecked")));
            IEnumerable<MethodInfo> moreSpecificCandidates = GetCandidates("isChecked", "name");
            Assert.That(moreSpecificCandidates.Count(), Is.GreaterThan(0));
            Assert.That(moreSpecificCandidates.Count(), Is.LessThan(candidates.Count()));
            Assert.That(moreSpecificCandidates.Count(), Is.EqualTo(count));
            foreach (MethodInfo info in moreSpecificCandidates)
            {
                Assert.That(info.GetParameters().Any(p => p.Name.Equals("isChecked")));
            }
        }

        [Test]
        public void AllCandidatesShouldStillReturnMethodsWhenParameterNameIsntFound()
        {
            IEnumerable<MethodInfo> candidates = GetCandidates("wrong");
            Assert.That(candidates.Count(), Is.GreaterThan(0));
            foreach (MethodInfo info in candidates)
            {
                ParameterInfo htmlAttributes =
                    info.GetParameters().ToList().Find(p => p.Name.Equals("htmlAttributes"));
                Assert.That(htmlAttributes, Is.Not.Null);
                Assert.That(htmlAttributes.ParameterType, Is.EqualTo(typeof (IDictionary<string, object>)));
            }
        }


        [Test]
        public void AllCandidatesShoulReturnAllMethodsIfNoParametersAreGiven()
        {
            IEnumerable<MethodInfo> candidates = GetCandidates();
            Assert.That(candidates.Count(), Is.GreaterThan(0));
            Assert.That(candidates.All(m => m.Name.Equals("CheckBox")));

            IEnumerable<MethodInfo> all = new TestHtmlTag().AllMethods;
            Assert.That(candidates.Count(), Is.EqualTo(all.Count()));
            foreach (MethodInfo info in all)
            {
                Assert.That(candidates, Has.Member(info));
            }
        }

        [Test]
        public void AllCandidatesShoulReturnMethodsHavingTheExpectedParam()
        {
            IEnumerable<MethodInfo> candidates = GetCandidates("isChecked");
            List<MethodInfo> all = new TestHtmlTag().AllMethods.ToList();
            Assert.That(candidates.Count(), Is.GreaterThan(0));
            Assert.That(candidates.All(m => m.Name.Equals("CheckBox")));
            foreach (MethodInfo info in candidates)
            {
                Assert.That(all, Has.Member(info));
                Assert.That(info.GetParameters().Any(p => p.Name.Equals("isChecked")));
            }
        }

        [Test]
        public void AllCandidatesShoulReturnMethodsHavingTheExpectedParams()
        {
            var candidates = GetCandidates("isChecked", "name");
            var all = new TestHtmlTag().AllMethods.ToList();
            Assert.That(candidates.Count(), Is.GreaterThan(0));
            Assert.That(candidates.All(m => m.Name.Equals("CheckBox")));
            foreach (MethodInfo info in candidates)
            {
                Assert.That(all, Has.Member(info));
                Assert.That(info.GetParameters().Any(p => p.Name.Equals("isChecked")));
            }
        }


        [Test]
        public void AllMethodsShoulReturnMethodsHavingTheCheckBoxName()
        {
            IEnumerable<MethodInfo> all = new TestHtmlTag().AllMethods;
            Assert.That(all.Count(), Is.GreaterThan(0));
            Assert.That(all.All(m => m.Name.Equals("CheckBox")));
        }

        [Test]
        public void AssembleParametersShouldCorrectlyBuildParameterArray()
        {
            var tag = new TestHtmlTag
                          {
                              Parameters = new List<IParameterValue>
                                               {
                                                   new TestParameterValue {Name = "isChecked", TestValue = true},
                                                   new TestParameterValue {Name = "name", TestValue = "flag"}
                                               }
                          };
            MethodInfo method = tag.Method;
            object[] values = tag.AssembleParameters(method, GetHelper(), null);
            CheckParameters(method, values);
        }


        [Test]
        public void AssembleParametersShouldCorrectlyBuildParameterArrayMissingRequiredArgument()
        {
            var tag = new TestHtmlTag
                          {
                              Parameters = new List<IParameterValue>
                                               {
                                                   new TestParameterValue
                                                       {Name = "isChecked", TestValue = true}
                                               }
                          };
            try
            {
                tag.AssembleParameters(tag.Method, GetHelper(), null);
                Assert.Fail("Expected exception");
            }
            catch (HtmlHelperTagException HHTe)
            {
                Assert.That(HHTe.Message,
                            Is.EqualTo(HtmlHelperTagException.RequiredArgumentMissing("name", tag.Method).Message));
            }
        }

        [Test]
        public void AssembleParametersShouldTakeHtmlAttributesInAccount()
        {
            var tag = Html.New<TestHtmlTag>(new List<IParameterValue>
                                                {
                                                    new TestParameterValue {Name = "isChecked", TestValue = true},
                                                    new TestParameterValue {Name = "name", TestValue = "flag"},
                                                    new TestParameterValue {Name = "class", TestValue = "someClass"}
                                                }
                );
            var method = tag.Method;
            Assert.That(method.GetParameters().Last().Name, Is.EqualTo(Html.HTMLATTRIBUTES_PARAM_NAME));
            object[] values = tag.AssembleParameters(method, GetHelper(), null);
            CheckParameters(method, values);
        }

        [Test]
        public void CandidateMethodShouldReturnMatchWithMinimalParameters()
        {
            IEnumerable<MethodInfo> candidates = GetCandidates("name");
            Assert.That(candidates.Count(), Is.GreaterThan(1));
            int minParameters = candidates.Min(c => c.GetParameters().Length);
            MethodInfo candidate = GetCandidate("name");
            Assert.That(candidate, Is.Not.Null);
            Assert.That(candidate.GetParameters().Length, Is.EqualTo(minParameters));
        }

        [Test]
        public void CandidateMethodShouldReturnMatchWithMinimalParametersBasedOnTwoParameters()
        {
            IEnumerable<MethodInfo> candidates = GetCandidates("isChecked", "name");
            Assert.That(candidates.Count(), Is.GreaterThan(1));
            int minParameters = candidates.Min(c => c.GetParameters().Length);
            MethodInfo candidate = GetCandidate("isChecked", "name");
            Assert.That(candidate, Is.Not.Null);
            Assert.That(candidate.GetParameters().Length, Is.EqualTo(minParameters));
        }

        [Test]
        public void RenderShouldDelegateCallToHtmlHelper()
        {
            var tag = Html.New<TestHtmlTag>(new List<IParameterValue>
                                                {
                                                    new TestParameterValue {Name = "isChecked", TestValue = true},
                                                    new TestParameterValue {Name = "name", TestValue = "flag"},
                                                }
                );
            Assert.That(tag.Method, Is.Not.Null);
            var model = new TagModel(new object());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHelper();
            string result = tag.Evaluate(model);
            Assert.That(result.Contains("name=\"flag\""));
            Assert.That(result.Contains("checked=\"checked\""));
        }


        [Test]
        public void RenderShouldDelegateCallToHtmlHelperShouldUseAttributes()
        {
            var attribute = new TemplateAttribute(new ParsedTemplate(null, new TextPart("a", null)));
            var tag = Html.New<TestHtmlTag>(new List<IParameterValue>
                                                {
                                                    new TestParameterValue {Name = "isChecked", TestValue = true},
                                                    new ParameterValue {Name = "name", Attribute = attribute},
                                                }
                );
            Assert.That(tag.Method, Is.Not.Null);
            var model = new TagModel(new object());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHelper();
            string result = tag.Evaluate(model);
            Assert.That(result.Contains("name=\"a\""));
            Assert.That(result.Contains("checked=\"checked\""));
        }


        [Test]
        public void RenderShouldDelegateCallToHtmlHelperShouldUseAttributesAndConvert()
        {
            var attribute = new TemplateAttribute(new ParsedTemplate(null, new TextPart("a", null)));
            var tag = Html.New<TestHtmlTag>(new List<IParameterValue>
                                                {
                                                    new TestParameterValue {Name = "isChecked", TestValue = "true"},
                                                    new ParameterValue {Name = "name", Attribute = attribute},
                                                }
                );
            Assert.That(tag.Method, Is.Not.Null);
            var model = new TagModel(new object());
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHelper();
            string result = tag.Evaluate(model);
            Assert.That(result.Contains("name=\"a\""));
            Assert.That(result.Contains("checked=\"checked\""));
        }


        [Test]
        public void RenderShouldDelegateCallToHtmlHelperShouldUseAttributesAndProvideTagModel()
        {
            var attribute =
                new TemplateAttribute(new ParsedTemplate(null, new ExpressionPart(Expression.Parse("name"))));
            var tag = Html.New<TestHtmlTag>(new List<IParameterValue>
                                                {
                                                    new TestParameterValue {Name = "isChecked", TestValue = true},
                                                    new ParameterValue {Name = "name", Attribute = attribute},
                                                }
                );
            Assert.That(tag.Method, Is.Not.Null);
            var model = new TagModel(new Hashtable());
            model.Model["name"] = "a";
            model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE] = GetHelper();
            string result = tag.Evaluate(model);
            Assert.That(result.Contains("name=\"a\""));
            Assert.That(result.Contains("checked=\"checked\""));
        }

        [Test]
        public void SearchingOnCandidatesShouldBeCaseInsensitive()
        {
            IEnumerable<MethodInfo> candidates = GetCandidates("ISCHECKED");
            List<MethodInfo> all = new TestHtmlTag().AllMethods.ToList();
            Assert.That(candidates.Count(), Is.GreaterThan(0));
            Assert.That(candidates.All(m => m.Name.Equals("CheckBox")));
            foreach (MethodInfo info in candidates)
            {
                Assert.That(all, Has.Member(info));
                Assert.That(info.GetParameters().Any(p => p.Name.Equals("isChecked")));
            }
        }
    }
}