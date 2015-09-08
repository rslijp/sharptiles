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
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Expressions;
using org.SharpTiles.Tags.FormatTags;

namespace org.SharpTiles.Tags.Test.FormatTags
{
    [TestFixture]
    public class SetLocaleTest : RequiresEnglishCulture
    {
        [Test]
        public void CheckParsingOfLocale()
        {
            var model = new TagModel(new Hashtable());
            var tag = new SetLocale();
            Assert.That(model[FormatConstants.LOCALE], Is.EqualTo(Thread.CurrentThread.CurrentCulture));
            tag.Value = new MockAttribute(new Constant("nl-NL"));
            Assert.That(tag.Evaluate(model), Is.EqualTo(String.Empty));
            Assert.That(model[FormatConstants.LOCALE], Is.EqualTo(new CultureInfo("nl-NL")));
        }

        [Test]
        public void CheckParsingOfLocaleDefautScope()
        {
            var model = new TagModel(this);
            var tag = new SetLocale();
            tag.Value = new MockAttribute(new Constant("nl-NL"));
            Assert.That(tag.Evaluate(model), Is.EqualTo(String.Empty));
            Assert.That(model.Page[FormatConstants.LOCALE], Is.EqualTo(new CultureInfo("nl-NL")));
        }

        [Test]
        public void CheckParsingOfWrongCulture()
        {
            var model = new TagModel(this);
            var tag = new SetLocale();
            tag.Value = new MockAttribute(new Constant("wrong"));
            try
            {
                tag.Evaluate(model);
                Assert.Fail("Expected exception");
            }
            catch (ArgumentException Ae)
            {
                Assert.That(Ae.Message.Contains("wrong"), Is.True);
            }
        }

        [Test]
        public void CheckRequired()
        {
            var tag = new SetLocale();
            try
            {
                RequiredAttribute.Check(tag);
                Assert.Fail("Expected Exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message,
                            Is.EqualTo(TagException.MissingRequiredAttribute(typeof (SetLocale), "Value").Message));
            }
            tag.Value = new MockAttribute(new Constant("a"));
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void CheckStoringOfLocaleInDifferentScope()
        {
            var model = new Hashtable();
            var tagModel = new TagModel(model);
            var tag = new SetLocale();
            tag.Value = new MockAttribute(new Constant("nl-NL"));
            tag.Scope = new MockAttribute(new Constant("Model"));
            Assert.That(tag.Evaluate(tagModel), Is.EqualTo(String.Empty));
            Assert.That(tagModel[FormatConstants.LOCALE], Is.EqualTo(new CultureInfo("nl-NL")));
            Assert.That(tagModel.Page[FormatConstants.LOCALE], Is.Null);
            Assert.That(tagModel.Model[FormatConstants.LOCALE], Is.EqualTo(new CultureInfo("nl-NL")));
        }
    }
}
