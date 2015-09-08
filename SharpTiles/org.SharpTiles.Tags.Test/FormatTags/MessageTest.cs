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
using System.Globalization;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Expressions;
 using org.SharpTiles.NUnit;
 using org.SharpTiles.Tags.CoreTags;
using org.SharpTiles.Tags.FormatTags;
using Param=org.SharpTiles.Tags.FormatTags.Param;

namespace org.SharpTiles.Tags.Test.FormatTags
{
    [TestFixture]
    public class MessageTest : RequiresEnglishCulture
    {
        [Test]
        public void CheckRequired()
        {
            var tag = new Message();
            try
            {
                RequiredAttribute.Check(tag);
                Assert.Fail("Expected Exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message,
                            Is.EqualTo(TagException.MissingRequiredAttribute(typeof (Message), "Key").Message));
            }
            tag.Key = new MockAttribute(new Constant("a"));
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void TestGetOfMessage()
        {
            var model = new TagModel(new object());
            model.PushTagStack();
            model.Tag[FormatConstants.BUNDLE] = new ResourceBundle("FormatTags/compiled", "");
            model.PushTagStack();
            model.Page[FormatConstants.LOCALE] = new CultureInfo("en-US");

            var tag = new Message();
            tag.Key = new MockAttribute(new Constant("c"));
            Assert.That(tag.Evaluate(model), Is.EqualTo("defaultC"));
        }

        [Test]
        public void TestGetOfMessageDifferentLocale()
        {
            var model = new TagModel(new object());
            model.PushTagStack();
            model.Tag[FormatConstants.BUNDLE] = new ResourceBundle("FormatTags/compiled", "");
            model.PushTagStack();
            model.Page[FormatConstants.LOCALE] = new CultureInfo("nl-NL");

            var tag = new Message();
            tag.Key = new MockAttribute(new Constant("c"));
            Assert.That(tag.Evaluate(model), Is.EqualTo("nederlandseC"));
        }


        [Test]
        public void TestGetOfMessageInVar()
        {
            var model = new TagModel(new object());
            model.PushTagStack();
            model.Tag[FormatConstants.BUNDLE] = new ResourceBundle("FormatTags/compiled", "");
            model.PushTagStack();
            model.Page[FormatConstants.LOCALE] = new CultureInfo("en-US");

            var tag = new Message();
            tag.Key = new MockAttribute(new Constant("c"));
            tag.Var = new MockAttribute(new Constant("myVar"));
            Assert.That(tag.Evaluate(model), Is.EqualTo(String.Empty));
            Assert.That(model.Page["myVar"], Is.EqualTo("defaultC"));
        }

        [Test]
        public void TestGetOfMessageInVarAndDifferentScope()
        {
            var model = new TagModel(new object(), new MockSessionState());
            model.PushTagStack();
            model.Tag[FormatConstants.BUNDLE] = new ResourceBundle("FormatTags/compiled", "");
            model.PushTagStack();
            model.Page[FormatConstants.LOCALE] = new CultureInfo("en-US");

            var tag = new Message();
            tag.Key = new MockAttribute(new Constant("c"));
            tag.Var = new MockAttribute(new Constant("myVar"));
            tag.Scope = new MockAttribute(new Constant(VariableScope.Session.ToString()));
            Assert.That(tag.Evaluate(model), Is.EqualTo(String.Empty));
            Assert.That(model.Page["myVar"], Is.Null);
            Assert.That(model.Session["myVar"], Is.EqualTo("defaultC"));
        }

        [Test]
        public void TestGetOfMessageNonExisting()
        {
            var model = new TagModel(new object());
            model.PushTagStack();
            model.Tag[FormatConstants.BUNDLE] = new ResourceBundle("FormatTags/complex", "");
            model.PushTagStack();
            model.Page[FormatConstants.LOCALE] = new CultureInfo("en-US");

            var tag = new Message();
            tag.Key = new MockAttribute(new Constant("missing"));
            Assert.That(tag.Evaluate(model), Is.EqualTo("?missing?"));
        }

        [Test]
        public void TestGetOfMessageWithBundleWithPrefix()
        {
            var model = new TagModel(new object());
            model.PushTagStack();
            model.Tag[FormatConstants.BUNDLE] = new ResourceBundle("FormatTags/compiled", "pre_");
            model.PushTagStack();
            model.Page[FormatConstants.LOCALE] = new CultureInfo("en-US");

            var tag = new Message();
            tag.Key = new MockAttribute(new Constant("c"));
            Assert.That(tag.Evaluate(model), Is.EqualTo("prefixedC"));
        }

        [Test]
        public void TestGetOfMessageWithNamedBundle()
        {
            var model = new TagModel(new object(), new MockSessionState());
            model.Session["myBundle"] = new ResourceBundle("FormatTags/compiled", "");

            var tag = new Message();
            tag.Key = new MockAttribute(new Constant("c"));
            tag.Bundle = new MockAttribute(new Constant("myBundle"));
            Assert.That(tag.Evaluate(model), Is.EqualTo("defaultC"));
        }

        [Test]
        public void TestGetOfMessageWithOneParam()
        {
            var model = new TagModel(new object());
            model.PushTagStack();
            model.Tag[FormatConstants.BUNDLE] = new ResourceBundle("FormatTags/complex", "");
            model.PushTagStack();
            model.Page[FormatConstants.LOCALE] = new CultureInfo("en-US");

            var tag = new Message();
            tag.Key = new MockAttribute(new Constant("onevar"));

            var param1 = new Param();
            param1.Body = new MockAttribute(new Constant("#1"));

            tag.AddNestedTag(param1);

            Assert.That(tag.Evaluate(model), Is.EqualTo("the #1 var"));
        }

        [Test]
        public void TestGetOfMessageInBundleGlobalScope()
        {
            var model = new TagModel(new object(), new MockSessionState());
            try
            {
                model.Global[FormatConstants.BUNDLE] = new ResourceBundle("FormatTags/compiled", "");

                var tag = new Message();
                tag.Key = new MockAttribute(new Constant("c"));
                Assert.That(tag.Evaluate(model), Is.EqualTo("defaultC"));
            } finally
            {
                TagModel.GlobalModel[FormatConstants.BUNDLE] = null;
            }
        }

        [Test]
        public void TestGetOfMessageNoBundle()
        {
            var model = new TagModel(new object(), new MockSessionState());
            var tag = new Message();
            tag.Key = new MockAttribute(new Constant("c"));
            try
            {
                tag.Evaluate(model);
                Assert.Fail("Expected exception");
            } catch (TagException Te)
            {
                Assert.That(TagException.NoResourceBundleFoundInTagScope().Message,
                              Is.EqualTo(Te.MessageWithOutContext));
            }
        }

        [Test]
        public void TestGetOfMessageWithTwoParams()
        {
            var model = new TagModel(new object());
            model.PushTagStack();
            model.Tag[FormatConstants.BUNDLE] = new ResourceBundle("FormatTags/complex", "");
            model.PushTagStack();
            model.Page[FormatConstants.LOCALE] = new CultureInfo("en-US");

            var tag = new Message();
            tag.Key = new MockAttribute(new Constant("twovars"));

            var param1 = new Param();
            param1.Body = new MockAttribute(new Constant("#1"));
            tag.AddNestedTag(param1);

            var param2 = new Param();
            param2.Body = new MockAttribute(new Constant("#2"));
            tag.AddNestedTag(param2);


            Assert.That(tag.Evaluate(model), Is.EqualTo("two #1, #2 vars"));
        }

        [Test]
        public void TestGetOfMessageWithWrongNestedType()
        {
            var model = new TagModel(new object());
            model.PushTagStack();
            model.Tag[FormatConstants.BUNDLE] = new ResourceBundle("FormatTags/compiled", "");
            model.Page[FormatConstants.LOCALE] = new CultureInfo("en-US");

            var tag = new Message();
            tag.Key = new MockAttribute(new Constant("c"));
            try
            {
                tag.AddNestedTag(new Out());
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message,
                            Is.EqualTo(TagException.OnlyNestedTagsOfTypeAllowed(typeof (Out), typeof (Param)).Message));
            }
        }
    }
}
