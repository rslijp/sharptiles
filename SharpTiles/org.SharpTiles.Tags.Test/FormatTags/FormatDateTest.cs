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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Expressions;
using org.SharpTiles.Tags.FormatTags;

namespace org.SharpTiles.Tags.Test.FormatTags
{
    [TestFixture]
    public class FormatDateTest : RequiresEnglishCulture
    {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            var model = new Hashtable();
            _model = new TagModel(model);
            _model.Page[FormatConstants.LOCALE] = new CultureInfo("en-US");
        }

        #endregion

        private TagModel _model;

        [Test]
        public void CheckRequired()
        {
            var tag = new FormatDate();
            RequiredAttribute.Check(tag);
            //no exceptions    
        }

        [Test]
        public void TestEmpty()
        {
            var date = new FormatDate();
            Assert.That(date.Evaluate(_model), Is.EqualTo(String.Empty));
        }

        [Test]
        public void TestSimpleFormatOfDateTime()
        {
            var date = new FormatDate();
            _model.Model["DateValue"] = new DateTime(2001, 2, 3, 4, 5, 6);
            date.Value = new MockAttribute(new Property("DateValue"));
            Assert.That(date.Evaluate(_model), Is.EqualTo("2/3/2001 4:05 AM"));
        }

        [Test]
        public void TestSimpleFormatOfDateTimeExplicitBoth()
        {
            var date = new FormatDate();
            _model.Model["DateValue"] = new DateTime(2001, 2, 3, 4, 5, 6);
            date.Value = new MockAttribute(new Property("DateValue"));
            date.Type = new MockAttribute(new Constant(DateType.Both.ToString()));
            Assert.That(date.Evaluate(_model), Is.EqualTo("2/3/2001 4:05 AM"));
        }

        [Test]
        public void TestSimpleFormatOfDateTimeOnlyDate()
        {
            var date = new FormatDate();
            _model.Model["DateValue"] = new DateTime(2001, 2, 3, 4, 5, 6);
            date.Value = new MockAttribute(new Property("DateValue"));
            date.Type = new MockAttribute(new Constant(DateType.Date.ToString()));
            Assert.That(date.Evaluate(_model), Is.EqualTo("2/3/2001"));
        }

        [Test]
        public void TestSimpleFormatOfDateTimeOnlyTime()
        {
            var date = new FormatDate();
            _model.Model["DateValue"] = new DateTime(2001, 2, 3, 4, 5, 6);
            date.Value = new MockAttribute(new Property("DateValue"));
            date.Type = new MockAttribute(new Constant(DateType.Time.ToString()));
            Assert.That(date.Evaluate(_model), Is.EqualTo("4:05 AM"));
        }

        [Test]
        public void TestSimpleFormatOfDifferentDateStyle_ExplicitDefault()
        {
            var date = new FormatDate();
            _model.Model["DateValue"] = new DateTime(2001, 2, 3, 4, 5, 6);
            date.Value = new MockAttribute(new Property("DateValue"));
            date.Type = new MockAttribute(new Constant(DateType.Date.ToString()));
            date.DateStyle = new MockAttribute(new Constant(DateStyle.Default.ToString()));
            Assert.That(date.Evaluate(_model), Is.EqualTo("2/3/2001"));
        }

        [Test]
        public void TestSimpleFormatOfDifferentDateStyle_Long()
        {
            var date = new FormatDate();
            _model.Model["DateValue"] = new DateTime(2001, 2, 3, 4, 5, 6);
            date.Value = new MockAttribute(new Property("DateValue"));
            date.Type = new MockAttribute(new Constant(DateType.Date.ToString()));
            date.DateStyle = new MockAttribute(new Constant(DateStyle.Long.ToString()));
            Assert.That(date.Evaluate(_model), Is.EqualTo("Saturday, February 03, 2001"));
        }

        [Test]
        public void TestSimpleFormatOfDifferentDateStyle_Short()
        {
            var date = new FormatDate();
            _model.Model["DateValue"] = new DateTime(2001, 2, 3, 4, 5, 6);
            date.Value = new MockAttribute(new Property("DateValue"));
            date.Type = new MockAttribute(new Constant(DateType.Date.ToString()));
            date.DateStyle = new MockAttribute(new Constant(DateStyle.Short.ToString()));
            Assert.That(date.Evaluate(_model), Is.EqualTo("2/3/2001"));
        }

        [Test]
        public void TestSimpleFormatOfDifferentDateStyle_ShortDifferentCulture()
        {
            var date = new FormatDate();
            _model.Model["DateValue"] = new DateTime(2001, 2, 3, 4, 5, 6);
            _model.Page[FormatConstants.LOCALE] = new CultureInfo("nl-NL");
            date.Value = new MockAttribute(new Property("DateValue"));
            date.Type = new MockAttribute(new Constant(DateType.Date.ToString()));
            date.DateStyle = new MockAttribute(new Constant(DateStyle.Short.ToString()));
            Assert.That(date.Evaluate(_model), Is.EqualTo("3-2-2001"));
        }


        [Test]
        public void TestSimpleFormatOfDifferentDateStyle_Wrong()
        {
            var date = new FormatDate();
            _model.Model["DateValue"] = new DateTime(2001, 2, 3, 4, 5, 6);
            date.Value = new MockAttribute(new Property("DateValue"));
            date.Type = new MockAttribute(new Constant(DateType.Date.ToString()));
            date.DateStyle = new MockAttribute(new Constant("Wrong"));
            try
            {
                date.Evaluate(_model);
                Assert.Fail("Expected failure");
            }
            catch (ArgumentException Ae)
            {
                Assert.That(Ae.Message.Contains("Wrong"));
            }
        }

        [Test]
        public void TestSimpleFormatOfDifferentTimeStyle_ExplicitDefault()
        {
            var date = new FormatDate();
            _model.Model["DateValue"] = new DateTime(2001, 2, 3, 4, 5, 6);
            date.Value = new MockAttribute(new Property("DateValue"));
            date.Type = new MockAttribute(new Constant(DateType.Time.ToString()));
            date.TimeStyle = new MockAttribute(new Constant(TimeStyle.Default.ToString()));
            Assert.That(date.Evaluate(_model), Is.EqualTo("4:05 AM"));
        }

        [Test]
        public void TestSimpleFormatOfDifferentTimeStyle_Long()
        {
            var date = new FormatDate();
            _model.Model["DateValue"] = new DateTime(2001, 2, 3, 4, 5, 6);
            date.Value = new MockAttribute(new Property("DateValue"));
            date.Type = new MockAttribute(new Constant(DateType.Time.ToString()));
            date.TimeStyle = new MockAttribute(new Constant(TimeStyle.Long.ToString()));
            Assert.That(date.Evaluate(_model), Is.EqualTo("4:05:06 AM"));
        }

        [Test]
        public void TestSimpleFormatOfDifferentTimeStyle_Short()
        {
            var date = new FormatDate();
            _model.Model["DateValue"] = new DateTime(2001, 2, 3, 14, 5, 6);
            date.Value = new MockAttribute(new Property("DateValue"));
            date.Type = new MockAttribute(new Constant(DateType.Time.ToString()));
            date.TimeStyle = new MockAttribute(new Constant(TimeStyle.Short.ToString()));
            Assert.That(date.Evaluate(_model), Is.EqualTo("2:05 PM"));
        }

        [Test]
        public void TestSimpleFormatOfDifferentTimeStyle_ShortDifferentCulture()
        {
            var date = new FormatDate();
            _model.Model["DateValue"] = new DateTime(2001, 2, 3, 14, 5, 6);
            _model.Page[FormatConstants.LOCALE] = new CultureInfo("nl-NL");
            date.Value = new MockAttribute(new Property("DateValue"));
            date.Type = new MockAttribute(new Constant(DateType.Time.ToString()));
            date.TimeStyle = new MockAttribute(new Constant(TimeStyle.Short.ToString()));
            Assert.That(date.Evaluate(_model), Is.EqualTo("14:05"));
        }

        [Test]
        public void TestSimpleFormatOfEmptyString()
        {
            var date = new FormatDate();
            date.Value = new MockAttribute(new Constant(""));

            Assert.That(date.Evaluate(_model), Is.EqualTo(String.Empty));
        }

        [Test]
        public void TestSimpleFormatOfString()
        {
            var date = new FormatDate();
            date.Value = new MockAttribute(new Constant("1/6/2008 12:00:00"));
            Assert.That(date.Evaluate(_model), Is.EqualTo("1/6/2008 12:00 PM"));
        }

        [Test]
        public void TestSimpleFormatOfWithBothLongFormat()
        {
            var date = new FormatDate();
            _model.Model["DateValue"] = new DateTime(2001, 2, 3, 4, 5, 6);
            date.Value = new MockAttribute(new Property("DateValue"));
            date.TimeStyle = new MockAttribute(new Constant(TimeStyle.Long.ToString()));
            date.DateStyle = new MockAttribute(new Constant(DateStyle.Long.ToString()));
            Assert.That(date.Evaluate(_model), Is.EqualTo("Saturday, February 03, 2001 4:05:06 AM"));
        }

        [Test]
        public void TestSimpleFormatOfWithBothLongFormatPatternOverride()
        {
            var date = new FormatDate();
            _model.Model["DateValue"] = new DateTime(2001, 2, 3, 4, 5, 6);
            date.Value = new MockAttribute(new Property("DateValue"));
            date.TimeStyle = new MockAttribute(new Constant(TimeStyle.Long.ToString()));
            date.DateStyle = new MockAttribute(new Constant(DateStyle.Long.ToString()));
            date.Pattern = new MockAttribute(new Constant("dd MMM yyyy'T'HH:mm"));
            Assert.That(date.Evaluate(_model), Is.EqualTo("03 Feb 2001T04:05"));
        }

        [Test]
        public void TestSimpleFormatOfWithCustomPattern()
        {
            var date = new FormatDate();
            _model.Model["DateValue"] = new DateTime(2001, 2, 3, 4, 5, 6);
            date.Value = new MockAttribute(new Property("DateValue"));
            date.Pattern = new MockAttribute(new Constant("dd MMM yyyy'T'HH:mm"));
            Assert.That(date.Evaluate(_model), Is.EqualTo("03 Feb 2001T04:05"));
        }

        [Test]
        public void TestSimpleFormatOfWithCustomPatternWithTimeZone()
        {
            var date = new FormatDate();
            _model.Model["DateValue"] = new DateTime(2001, 2, 3, 4, 5, 6);
            date.Value = new MockAttribute(new Property("DateValue"));
            date.Pattern = new MockAttribute(new Constant("HH:mm zzz"));
            Assert.That(date.Evaluate(_model), Is.EqualTo("04:05 +01:00"));
        }
    }
}
