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
    public class FormatNumberTest : RequiresEnglishCulture
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
            var tag = new FormatNumber();
            RequiredAttribute.Check(tag);
            //no exceptions    
        }

        [Test]
        public void TestDecimalsMaximalFractionDigits()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Property("anumber"));
            number.MaxFractionDigits = new MockAttribute(new Constant("5"));
            _model.Model["anumber"] = 1.23456789m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("1.23457"));
            _model.Model["anumber"] = 1.23m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("1.23"));
        }

        [Test]
        public void TestDecimalsMaximalIntegerDigits()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Property("anumber"));
            number.MaxIntegerDigits = new MockAttribute(new Constant("10"));
            number.GroupingUsed = new MockAttribute(new Constant("false"));
            number.MaxFractionDigits = new MockAttribute(new Constant("0"));
            _model.Model["anumber"] = 12345m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("12345"));
            _model.Model["anumber"] = 1234567890123m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("4567890123"));
        }

        [Test]
        public void TestDecimalsMinimalAndMaximalFractionDigits()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Property("anumber"));
            number.MinFractionDigits = new MockAttribute(new Constant("3"));
            number.MaxFractionDigits = new MockAttribute(new Constant("5"));
            _model.Model["anumber"] = 1.23m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("1.230"));
            _model.Model["anumber"] = 1.234m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("1.234"));
            _model.Model["anumber"] = 1.2345m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("1.2345"));
            _model.Model["anumber"] = 1.23456m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("1.23456"));
            _model.Model["anumber"] = 1.234567m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("1.23457"));
        }

        [Test]
        public void TestDecimalsMinimalAndMaximalFractionDigitsNegative()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Property("anumber"));
            number.MinFractionDigits = new MockAttribute(new Constant("3"));
            number.MaxFractionDigits = new MockAttribute(new Constant("5"));
            _model.Model["anumber"] = -1.23m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("-1.230"));
            _model.Model["anumber"] = -1.234m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("-1.234"));
            _model.Model["anumber"] = -1.2345m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("-1.2345"));
            _model.Model["anumber"] = -1.23456m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("-1.23456"));
            _model.Model["anumber"] = -1.234567m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("-1.23457"));
        }

        [Test]
        public void TestDecimalsMinimalAndMaximalIntegerDigits()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Property("anumber"));
            number.MinIntegerDigits = new MockAttribute(new Constant("3"));
            number.MaxIntegerDigits = new MockAttribute(new Constant("5"));
            number.GroupingUsed = new MockAttribute(new Constant("false"));
            number.MaxFractionDigits = new MockAttribute(new Constant("0"));
            _model.Model["anumber"] = 12m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("012"));
            _model.Model["anumber"] = 123m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("123"));
            _model.Model["anumber"] = 1234m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("1234"));
            _model.Model["anumber"] = 12345m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("12345"));
            _model.Model["anumber"] = 123456m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("23456"));
        }

        [Test]
        public void TestDecimalsMinimalFractionDigits()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Property("anumber"));
            number.MinFractionDigits = new MockAttribute(new Constant("5"));
            _model.Model["anumber"] = 1.23456789m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("1.23457"));
            _model.Model["anumber"] = 1.23m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("1.23000"));
        }

        [Test]
        public void TestDecimalsMinimalIntegerDigits()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Property("anumber"));
            number.MinIntegerDigits = new MockAttribute(new Constant("10"));
            number.GroupingUsed = new MockAttribute(new Constant("false"));
            number.MaxFractionDigits = new MockAttribute(new Constant("0"));
            _model.Model["anumber"] = 12345m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("0000012345"));
            _model.Model["anumber"] = 1234567890123m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("1234567890123"));
        }

        [Test]
        public void TestDecimalsMinimalIntegerDigitsWithDefault()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Property("anumber"));
            number.MinIntegerDigits = new MockAttribute(new Constant("10"));
            _model.Model["anumber"] = 12345m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("0,000,012,345.00"));
            _model.Model["anumber"] = 1234567890123m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("1,234,567,890,123.00"));
        }

        [Test]
        public void TestDecimalsMinimalIntegerDigitsWithGroupingAndFractions()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Property("anumber"));
            number.MinIntegerDigits = new MockAttribute(new Constant("10"));
            number.GroupingUsed = new MockAttribute(new Constant("true"));
            number.MinFractionDigits = new MockAttribute(new Constant("2"));
            _model.Model["anumber"] = 12345m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("0,000,012,345.00"));
            _model.Model["anumber"] = 1234567890123m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("1,234,567,890,123.00"));
        }

        [Test]
        public void TestDecimalsRoundingDown()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Constant("1.23456789"));
            Assert.That(number.Evaluate(_model), Is.EqualTo("1.23"));
        }

        [Test]
        public void TestDecimalsRoundingUp()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Constant("1.26789"));
            Assert.That(number.Evaluate(_model), Is.EqualTo("1.27"));
        }

        [Test]
        public void TestEmpty()
        {
            var number = new FormatNumber();
            Assert.That(number.Evaluate(_model), Is.EqualTo(String.Empty));
        }

        [Test]
        public void TestFormatOfCurrency()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Property("anumber"));
            number.Type = new MockAttribute(new Constant("Currency"));
            _model.Model["anumber"] = 12m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("$12.00"));
        }

        [Test]
        public void TestFormatOfCurrencyWithDifferentCurrencyCode()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Property("anumber"));
            number.Type = new MockAttribute(new Constant("Currency"));
            number.CurrencyCode = new MockAttribute(new Constant("RESL"));
            _model.Model["anumber"] = 12m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("RESL12.00"));
        }

        [Test]
        public void TestFormatOfCurrencyWithDifferentCurrencySymbol()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Property("anumber"));
            number.Type = new MockAttribute(new Constant("Currency"));
            number.CurrencySymbol = new MockAttribute(new Constant("@"));
            _model.Model["anumber"] = 12m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("@12.00"));
        }

        [Test]
        public void TestFormatOfCurrencyWithDifferentCurrencySymbolOverridesCode()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Property("anumber"));
            number.Type = new MockAttribute(new Constant("Currency"));
            number.CurrencySymbol = new MockAttribute(new Constant("@"));
            number.CurrencyCode = new MockAttribute(new Constant("RESL"));
            _model.Model["anumber"] = 12m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("@12.00"));
        }

        [Test]
        public void TestFormatOfCurrencyWithDifferentCurrencySymbolOverridesCodeWithAlternativePattern()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Property("anumber"));
            number.Type = new MockAttribute(new Constant("Currency"));
            number.CurrencySymbol = new MockAttribute(new Constant("@"));
            number.CurrencyCode = new MockAttribute(new Constant("RESL"));
            number.MinFractionDigits = new MockAttribute(new Constant("6"));
            _model.Model["anumber"] = 12m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("@12.000000"));
        }

        [Test]
        public void TestFormatOfNegativeCurrency()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Property("anumber"));
            number.Type = new MockAttribute(new Constant("Currency"));
            _model.Model["anumber"] = -12m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("($12.00)")); //Stupid us formatting
        }

        [Test]
        public void TestFormatOfNumberExplicit()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Property("anumber"));
            number.Type = new MockAttribute(new Constant("Number"));
            _model.Model["anumber"] = 12;
            Assert.That(number.Evaluate(_model), Is.EqualTo("12.00"));
        }

        [Test]
        public void TestFormatOfNumberWithCustomPattern()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Property("anumber"));
            number.Pattern = new MockAttribute(new Constant("###,'abc',###.00000"));
            _model.Model["anumber"] = 123456.789;
            Assert.That(number.Evaluate(_model), Is.EqualTo("123,abc456.78900"));
        }

        [Test]
        public void TestFormatOfPercentage()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Property("anumber"));
            number.Type = new MockAttribute(new Constant("Percentage"));
            _model.Model["anumber"] = 0.12m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("12.00 %"));
        }

        [Test]
        public void TestFormatOfPercentageNegative()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Property("anumber"));
            number.Type = new MockAttribute(new Constant("Percentage"));
            _model.Model["anumber"] = -0.12m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("-12.00 %"));
        }

        [Test]
        public void TestFormatOfPercentageWithAlternativePattern()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Property("anumber"));
            number.Type = new MockAttribute(new Constant("Percentage"));
            number.MinFractionDigits = new MockAttribute(new Constant("6"));
            _model.Model["anumber"] = 0.12;
            Assert.That(number.Evaluate(_model), Is.EqualTo("12.000000 %"));
        }

        [Test]
        public void TestFormatOfPercentageWithDecimals()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Property("anumber"));
            number.Type = new MockAttribute(new Constant("Percentage"));
            _model.Model["anumber"] = 0.012m;
            Assert.That(number.Evaluate(_model), Is.EqualTo("1.20 %"));
        }

        [Test]
        public void TestSimpleFormatOfEmptyString()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Constant(""));

            Assert.That(number.Evaluate(_model), Is.EqualTo(String.Empty));
        }

        [Test]
        public void TestSimpleFormatOfInt()
        {
            _model.Model["anumber"] = 1;
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Property("anumber"));
            Assert.That(number.Evaluate(_model), Is.EqualTo("1.00"));
        }

        [Test]
        public void TestSimpleFormatOfIntWithGroupingDefault()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Constant("1000"));
            Assert.That(number.Evaluate(_model), Is.EqualTo("1,000.00"));
        }

        [Test]
        public void TestSimpleFormatOfIntWithGroupingExplicitFalse()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Constant("1000"));
            number.GroupingUsed = new MockAttribute(new Constant("false"));
            Assert.That(number.Evaluate(_model), Is.EqualTo("1000.00"));
        }

        [Test]
        public void TestSimpleFormatOfIntWithGroupingExplicitFalseBigNumber()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Constant("123456789"));
            number.GroupingUsed = new MockAttribute(new Constant("false"));
            Assert.That(number.Evaluate(_model), Is.EqualTo("123456789.00"));
        }

        [Test]
        public void TestSimpleFormatOfIntWithGroupingExplicitFalseBigNumberWithDecimals()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Constant("123456789.876543"));
            number.GroupingUsed = new MockAttribute(new Constant("false"));
            Assert.That(number.Evaluate(_model), Is.EqualTo("123456789.88"));
        }

        [Test]
        public void TestSimpleFormatOfIntWithGroupingExplicitTrue()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Constant("1000"));
            number.GroupingUsed = new MockAttribute(new Constant("true"));
            Assert.That(number.Evaluate(_model), Is.EqualTo("1,000.00"));
        }

        [Test]
        public void TestSimpleFormatOfIntWithGroupingExplicitTrueBigNumber()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Constant("123456789"));
            number.GroupingUsed = new MockAttribute(new Constant("true"));
            Assert.That(number.Evaluate(_model), Is.EqualTo("123,456,789.00"));
        }

        [Test]
        public void TestSimpleFormatOfIntWithGroupingExplicitTrueBigNumberWithDecimals()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Constant("123456789.876543"));
            number.GroupingUsed = new MockAttribute(new Constant("true"));
            Assert.That(number.Evaluate(_model), Is.EqualTo("123,456,789.88"));
        }

        [Test]
        public void TestSimpleFormatOfNegativeInt()
        {
            _model.Model["anumber"] = -1;
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Property("anumber"));
            Assert.That(number.Evaluate(_model), Is.EqualTo("-1.00"));
        }

        [Test]
        public void TestSimpleFormatOfString()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Constant("1"));

            Assert.That(number.Evaluate(_model), Is.EqualTo("1.00"));
        }

        [Test]
        public void TestSimpleFormatOfStringDifferentCultureInPageModel()
        {
            var number = new FormatNumber();
            number.Value = new MockAttribute(new Constant("1"));
            _model.Page[FormatConstants.LOCALE] = new CultureInfo("nl-NL");
            Assert.That(number.Evaluate(_model), Is.EqualTo("1,00"));
        }
    }
}
