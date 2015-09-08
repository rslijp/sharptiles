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
 using org.SharpTiles.NUnit;
 using org.SharpTiles.Tags.FormatTags;

namespace org.SharpTiles.Tags.Test.FormatTags
{
    [TestFixture]
    public class SetBundleTest : RequiresEnglishCulture
    {
        [Test]
        public void CheckLoadingOfSetBundle()
        {
            var model = new TagModel(new Hashtable());
            var tag = new SetBundle();
            tag.BaseName = new MockAttribute(new Constant("FormatTags/test"));
            tag.Var = new MockAttribute(new Constant("myBundle"));
            Assert.That(tag.Evaluate(model), Is.EqualTo(String.Empty));
            Assert.That(model.Page["myBundle"], Is.Not.Null);
            Assert.That(model.Page["myBundle"] is ResourceBundle, Is.True);
            var bundle = (ResourceBundle) model.Page["myBundle"];
            Assert.That(bundle.BaseName, Is.EqualTo("FormatTags/test"));
        }

        [Test]
        public void CheckRequired()
        {
            var tag = new SetBundle();
            try
            {
                RequiredAttribute.Check(tag);
                Assert.Fail("Expected Exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message,
                            Is.EqualTo(
                                TagException.MissingRequiredAttribute(typeof (SetBundle), "BaseName", "Var").Message));
            }
            tag.BaseName = new MockAttribute(new Constant("a"));
            tag.Var = new MockAttribute(new Constant("a"));
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void TestSettingOfScope()
        {
            var model = new TagModel(new Hashtable(), new MockSessionState());
            var tag = new SetBundle();
            tag.BaseName = new MockAttribute(new Constant("FormatTags/test"));
            tag.Var = new MockAttribute(new Constant("myBundle"));
            tag.Scope = new MockAttribute(new Constant(VariableScope.Session.ToString()));
            Assert.That(tag.Evaluate(model), Is.EqualTo(String.Empty));
            Assert.That(model.Page["myBundle"], Is.Null);
            Assert.That(model.Session["myBundle"], Is.Not.Null);
            Assert.That(model.Session["myBundle"] is ResourceBundle, Is.True);
            var bundle = (ResourceBundle) model.Session["myBundle"];
            Assert.That(bundle.BaseName, Is.EqualTo("FormatTags/test"));
        }
    }
}

