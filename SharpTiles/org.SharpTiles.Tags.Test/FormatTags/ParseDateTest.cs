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
    public class ParseDateTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var model = new Hashtable();
            _model = new TagModel(model);
            _model.Page[FormatConstants.LOCALE] = new CultureInfo("en-US");
        }

        #endregion

        private TagModel _model;

        [Test]
        public void CheckRequired()
        {
            var tag = new ParseDate();
            RequiredAttribute.Check(tag);
            //no exceptions    
        }

        [Test]
        public void TestEmpty()
        {
            var date = new ParseDate();
            Assert.That(date.Evaluate(_model), Is.EqualTo(String.Empty));
        }

        [Test]
        public void TestSimpleFormatOfDateTimeExplicitBoth()
        {
            var date = new ParseDate();
            date.Value = new MockAttribute(new Constant("1/6/2008 04:13 AM"));
            date.Var = new MockAttribute(new Constant("result"));
            date.Type = new MockAttribute(new Constant(DateType.Both.ToString()));
            Assert.That(date.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model.Page["result"], Is.EqualTo(new DateTime(2008, 1, 6, 4, 13, 0)));
        }

        [Test]
        public void TestSimpleFormatOfDateTimeOnlyDate()
        {
            var date = new ParseDate();
            date.Value = new MockAttribute(new Constant("1/6/2008"));
            date.Exact = new MockAttribute(new Constant("true"));
            date.Var = new MockAttribute(new Constant("result"));
            date.Type = new MockAttribute(new Constant(DateType.Date.ToString()));
            Assert.That(date.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model.Page["result"], Is.EqualTo(new DateTime(2008, 1, 6, 0, 0, 0)));
        }

        [Test]
        public void TestSimpleFormatOfDateTimeOnlyTime()
        {
            var date = new ParseDate();
            date.Value = new MockAttribute(new Constant("4:12 AM"));
            date.Exact = new MockAttribute(new Constant("true"));
            date.Var = new MockAttribute(new Constant("result"));
            date.Type = new MockAttribute(new Constant(DateType.Time.ToString()));
            Assert.That(date.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model.Page["result"],
                        Is.EqualTo(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 4, 12, 0)));
        }

        [Test]
        public void TestSimpleFormatOfDifferentDateStyle_ExplicitDefault()
        {
            var date = new ParseDate();
            date.Value = new MockAttribute(new Constant("1/6/2008"));
            date.Var = new MockAttribute(new Constant("result"));
            date.Type = new MockAttribute(new Constant(DateType.Date.ToString()));
            date.DateStyle = new MockAttribute(new Constant(DateStyle.Default.ToString()));
            Assert.That(date.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model.Page["result"], Is.EqualTo(new DateTime(2008, 1, 6, 0, 0, 0)));
        }

        [Test]
        public void TestSimpleFormatOfDifferentDateStyle_Long()
        {
            var date = new ParseDate();
            date.Value = new MockAttribute(new Constant("Saturday, February 03, 2001"));
            date.Var = new MockAttribute(new Constant("result"));
            date.Type = new MockAttribute(new Constant(DateType.Date.ToString()));
            date.DateStyle = new MockAttribute(new Constant(DateStyle.Long.ToString()));
            Assert.That(date.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model.Page["result"], Is.EqualTo(new DateTime(2001, 2, 3, 0, 0, 0)));
        }

        [Test]
        public void TestSimpleFormatOfDifferentDateStyle_Short()
        {
            var date = new ParseDate();
            date.Value = new MockAttribute(new Constant("1/6/2008"));
            date.Var = new MockAttribute(new Constant("result"));
            date.Type = new MockAttribute(new Constant(DateType.Date.ToString()));
            date.DateStyle = new MockAttribute(new Constant(DateStyle.Short.ToString()));
            Assert.That(date.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model.Page["result"], Is.EqualTo(new DateTime(2008, 1, 6, 0, 0, 0)));
        }

        [Test]
        public void TestSimpleFormatOfDifferentDateStyle_ShortDifferentCulture()
        {
            _model.Page[FormatConstants.LOCALE] = new CultureInfo("nl-NL");
            var date = new ParseDate();
            date.Value = new MockAttribute(new Constant("6-1-2008"));
            date.Var = new MockAttribute(new Constant("result"));
            date.Type = new MockAttribute(new Constant(DateType.Date.ToString()));
            date.DateStyle = new MockAttribute(new Constant(DateStyle.Default.ToString()));
            Assert.That(date.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model.Page["result"], Is.EqualTo(new DateTime(2008, 1, 6, 0, 0, 0)));
        }


        [Test]
        public void TestSimpleFormatOfDifferentDateStyle_Wrong()
        {
            var date = new ParseDate();
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
            var date = new ParseDate();
            date.Value = new MockAttribute(new Constant("4:12 AM"));
            date.Var = new MockAttribute(new Constant("result"));
            date.Type = new MockAttribute(new Constant(DateType.Time.ToString()));
            date.TimeStyle = new MockAttribute(new Constant(TimeStyle.Default.ToString()));

            Assert.That(date.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model.Page["result"],
                        Is.EqualTo(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 4, 12, 0)));
        }

        [Test]
        public void TestSimpleFormatOfDifferentTimeStyle_Long()
        {
            var date = new ParseDate();
            date.Value = new MockAttribute(new Constant("4:05:06 AM"));
            date.Var = new MockAttribute(new Constant("result"));
            date.Type = new MockAttribute(new Constant(DateType.Time.ToString()));
            date.TimeStyle = new MockAttribute(new Constant(TimeStyle.Long.ToString()));

            Assert.That(date.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model.Page["result"],
                        Is.EqualTo(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 4, 5, 6)));
        }

        [Test]
        public void TestSimpleFormatOfDifferentTimeStyle_Short()
        {
            var date = new ParseDate();
            date.Value = new MockAttribute(new Constant("4:12 AM"));
            date.Var = new MockAttribute(new Constant("result"));
            date.Type = new MockAttribute(new Constant(DateType.Time.ToString()));
            date.TimeStyle = new MockAttribute(new Constant(TimeStyle.Default.ToString()));

            Assert.That(date.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model.Page["result"],
                        Is.EqualTo(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 4, 12, 0)));
        }

        [Test]
        public void TestSimpleFormatOfDifferentTimeStyle_ShortDifferentCulture()
        {
            _model.Page[FormatConstants.LOCALE] = new CultureInfo("nl-NL");
            var date = new ParseDate();
            date.Value = new MockAttribute(new Constant("16:12"));
            date.Var = new MockAttribute(new Constant("result"));
            date.Type = new MockAttribute(new Constant(DateType.Time.ToString()));
            date.TimeStyle = new MockAttribute(new Constant(TimeStyle.Default.ToString()));

            Assert.That(date.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model.Page["result"],
                        Is.EqualTo(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 16, 12, 0)));
        }

        [Test]
        public void TestSimpleFormatOfEmptyString()
        {
            var date = new ParseDate();
            date.Value = new MockAttribute(new Constant(""));

            Assert.That(date.Evaluate(_model), Is.EqualTo(String.Empty));
        }

        [Test]
        public void TestSimpleFormatOfWithBothLongFormat()
        {
            var date = new ParseDate();
            date.Value = new MockAttribute(new Constant("Saturday, February 03, 2001 4:05:06 AM"));
            date.TimeStyle = new MockAttribute(new Constant(TimeStyle.Long.ToString()));
            date.DateStyle = new MockAttribute(new Constant(DateStyle.Long.ToString()));
            date.Var = new MockAttribute(new Constant("result"));
            Assert.That(date.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model.Page["result"], Is.EqualTo(new DateTime(2001, 2, 3, 4, 5, 6)));
        }

        [Test]
        public void TestSimpleFormatOfWithBothLongFormatPatternOverride()
        {
            var date = new ParseDate();
            date.Value = new MockAttribute(new Constant("03 Feb 2001T04:05"));
            date.TimeStyle = new MockAttribute(new Constant(TimeStyle.Long.ToString()));
            date.DateStyle = new MockAttribute(new Constant(DateStyle.Long.ToString()));
            date.Pattern = new MockAttribute(new Constant("dd MMM yyyy'T'HH:mm"));
            date.Var = new MockAttribute(new Constant("result"));
            Assert.That(date.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model.Page["result"], Is.EqualTo(new DateTime(2001, 2, 3, 4, 5, 0)));
        }

        [Test]
        public void TestSimpleFormatOfWithCustomPattern()
        {
            var date = new ParseDate();
            date.Value = new MockAttribute(new Constant("03 Feb 2001T04:05"));
            date.Var = new MockAttribute(new Constant("result"));
            date.Pattern = new MockAttribute(new Constant("dd MMM yyyy'T'HH:mm"));
            Assert.That(date.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model.Page["result"], Is.EqualTo(new DateTime(2001, 2, 3, 4, 5, 0)));
        }

        [Test]
        public void TestSimpleFormatOfWithCustomPatternWithTimeZone()
        {
            var date = new ParseDate();
            date.Value = new MockAttribute(new Constant("04:05 +02:00"));
            date.Pattern = new MockAttribute(new Constant("HH:mm zzz"));
            date.Var = new MockAttribute(new Constant("result"));

            Assert.That(date.Evaluate(_model), Is.EqualTo(String.Empty));
            var dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 3, 5, 0);
            if (dt.IsDaylightSavingTime())
            {
                dt = dt.AddHours(1);
            }
            Assert.That(((DateTime) _model.Page["result"]), Is.EqualTo(dt));
        }

        [Test]
        public void TestSimpleParse()
        {
            var date = new ParseDate();
            date.Value = new MockAttribute(new Constant("1/6/2008 12:00 AM"));
            date.Var = new MockAttribute(new Constant("result"));
            Assert.That(date.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model.Page["result"], Is.EqualTo(new DateTime(2008, 1, 6, 0, 0, 0)));
        }

        [Test]
        public void TestSimpleParse_ExplicitExact()
        {
            var date = new ParseDate();
            date.Value = new MockAttribute(new Constant("1/6/2008 12:00 AM"));
            date.Exact = new MockAttribute(new Constant("true"));
            date.Var = new MockAttribute(new Constant("result"));
            Assert.That(date.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model.Page["result"], Is.EqualTo(new DateTime(2008, 1, 6, 0, 0, 0)));
        }

        [Test]
        public void TestSimpleParse_ExplicitExact_WrongFormat()
        {
            var date = new ParseDate();
            date.Value = new MockAttribute(new Constant("1/6/2008 12:00:00"));
            date.Exact = new MockAttribute(new Constant("true"));
            try
            {
                date.Evaluate(_model);
                Assert.Fail("Should fail");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message, Is.EqualTo(TagException.ParseException("1/6/2008 12:00:00", "Date").Message));
            }
        }

        [Test]
        public void TestSimpleParse_NonExact()
        {
            var date = new ParseDate();
            date.Value = new MockAttribute(new Constant("1/6/2008 12:00:00"));
            date.Exact = new MockAttribute(new Constant("false"));
            date.Var = new MockAttribute(new Constant("result"));
            Assert.That(date.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model.Page["result"], Is.EqualTo(new DateTime(2008, 1, 6, 12, 0, 0)));
        }
    }
}
