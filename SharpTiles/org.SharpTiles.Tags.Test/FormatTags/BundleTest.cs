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
using org.SharpTiles.Tags.FormatTags;

namespace org.SharpTiles.Tags.Test.FormatTags
{
    [TestFixture]
    public class BundleTest : RequiresEnglishCulture
    {
        [Test]
        public void CheckLoadingOfBundle()
        {
            var model = new TagModel(new Hashtable());
            var tag = new Bundle();
            tag.BaseName = new MockAttribute(new Constant("FormatTags/test"));
            Assert.That(tag.Evaluate(model), Is.EqualTo(String.Empty));
            Assert.That(model.SearchInTagScope(FormatConstants.BUNDLE), Is.Not.Null);
            Assert.That(model.SearchInTagScope(FormatConstants.BUNDLE) is ResourceBundle, Is.True);
            var bundle = (ResourceBundle) model.SearchInTagScope(FormatConstants.BUNDLE);
            Assert.That(bundle.BaseName, Is.EqualTo("FormatTags/test"));
        }

        [Test]
        public void CheckRequired()
        {
            var tag = new Bundle();
            try
            {
                RequiredAttribute.Check(tag);
                Assert.Fail("Expected Exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message,
                            Is.EqualTo(TagException.MissingRequiredAttribute(typeof (Bundle), "BaseName").Message));
            }
            tag.BaseName = new MockAttribute(new Constant("a"));
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void TestSettingOfPrefix()
        {
            var model = new TagModel(new Hashtable());
            var tag = new Bundle();
            tag.BaseName = new MockAttribute(new Constant("FormatTags/test"));
            tag.Prefix = new MockAttribute(new Constant("pre_"));
            Assert.That(tag.Evaluate(model), Is.EqualTo(String.Empty));
            var bundle = (ResourceBundle) model.SearchInTagScope(FormatConstants.BUNDLE);
            Assert.That(bundle.Prefix, Is.EqualTo("pre_"));
        }
    }
}

