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
using org.SharpTiles.Tags.FormatTags;

namespace org.SharpTiles.Tags.Test.FormatTags
{
    [TestFixture]
    public class ParseNumberTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var model = new MockForCasting();
            _model = new TagModel(model);
            _model.Page[FormatConstants.LOCALE] = new CultureInfo("en-US");
        }

        #endregion

        private TagModel _model;

        public class MockForCasting
        {
            public int TheInt { get; set; }

            public float TheFloat { get; set; }

            public int TheNullableInt { get; set; }

            public decimal TheDecimal { get; set; }

            public object Number { get; set; }
        }

        [Test]
        public void CheckRequired()
        {
            var tag = new ParseNumber();
            RequiredAttribute.Check(tag);
            //no exceptions    
        }

        [Test]
        public void TestCastingToDecimal()
        {
            var tag = new ParseNumber();
            tag.Value = new MockAttribute(new StringConstant("1,345.67"));
            tag.Var = new MockAttribute(new StringConstant("TheDecimal"));
            tag.Scope = new MockAttribute(new StringConstant(VariableScope.Model.ToString()));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model.Model["TheDecimal"], Is.EqualTo(1345.67));
        }

        [Test]
        public void TestCastingToFloat()
        {
            var tag = new ParseNumber();
            tag.Value = new MockAttribute(new StringConstant("1,345.67"));
            tag.Var = new MockAttribute(new StringConstant("TheFloat"));
            tag.Scope = new MockAttribute(new StringConstant(VariableScope.Model.ToString()));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model.Model["TheFloat"], Is.EqualTo(1345.67f));
        }

        [Test]
        public void TestCastingToInt()
        {
            var tag = new ParseNumber();
            tag.Value = new MockAttribute(new StringConstant("1,345.67"));
            tag.Var = new MockAttribute(new StringConstant("TheInt"));
            tag.Scope = new MockAttribute(new StringConstant(VariableScope.Model.ToString()));
            tag.IntegerOnly = new MockAttribute(new StringConstant("True"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model.Model["TheInt"], Is.EqualTo(1345));
        }

        [Test]
        public void TestCastingToNullableInt()
        {
            var tag = new ParseNumber();
            tag.Value = new MockAttribute(new StringConstant("1,345.67"));
            tag.Var = new MockAttribute(new StringConstant("TheNullableInt"));
            tag.Scope = new MockAttribute(new StringConstant(VariableScope.Model.ToString()));
            tag.IntegerOnly = new MockAttribute(new StringConstant("True"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model.Model["TheNullableInt"], Is.EqualTo(1345));
        }

        [Test]
        public void TestFromValue()
        {
            var tag = new ParseNumber();
            tag.Value = new MockAttribute(new StringConstant("4"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(4m.ToString()));
        }

        [Test]
        public void TestFromValueDecimal()
        {
            var tag = new ParseNumber();
            tag.Value = new MockAttribute(new StringConstant("4.5"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(4.5m.ToString()));
        }

        [Test]
        public void TestFromValueDecimalToVar()
        {
            var tag = new ParseNumber();
            tag.Value = new MockAttribute(new StringConstant("4.5"));
            tag.Var = new MockAttribute(new StringConstant("Number"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model["Number"], Is.EqualTo(4.5m));
        }

        [Test]
        public void TestFromValueDecimalToVarIntegerOnly()
        {
            var tag = new ParseNumber();
            tag.Value = new MockAttribute(new StringConstant("4.5"));
            tag.Var = new MockAttribute(new StringConstant("Number"));
            tag.IntegerOnly = new MockAttribute(new StringConstant("True"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model["Number"], Is.EqualTo(4m));
        }

        [Test]
        public void TestFromValueDecimalToVarWithDecimalSeperator()
        {
            var tag = new ParseNumber();
            tag.Value = new MockAttribute(new StringConstant("1,345.67"));
            tag.Var = new MockAttribute(new StringConstant("Number"));
            tag.IntegerOnly = new MockAttribute(new StringConstant("False"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model["Number"], Is.EqualTo(1345.67m));
        }

        [Test]
        public void TestBug()
        {
            var tag = new ParseNumber();
            tag.Value = new MockAttribute(new StringConstant("0.42"));
            tag.Var = new MockAttribute(new StringConstant("Number"));
            tag.IntegerOnly = new MockAttribute(new StringConstant("False"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model["Number"], Is.EqualTo(0.42m));
        }

        [Test]
        public void TestFromValueDecimalToVarWithDecimalSeperatorNotAllowedInStyle()
        {
            var tag = new ParseNumber();
            tag.Value = new MockAttribute(new StringConstant("1,345.67"));
            tag.Var = new MockAttribute(new StringConstant("Number"));
            tag.Styles = new MockAttribute(new StringConstant(NumberStyles.AllowDecimalPoint.ToString()));
            try
            {
                tag.Evaluate(_model);
                Assert.Fail("Should be in incorrect format");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message, 
                    Is.EqualTo(TagException.ParseException("1,345.67", "Number").Message));
            }
        }



        [Test]
        public void TestFromValueDecimalToVarWithDecimalSeperatorWithStyles()
        {
            var tag = new ParseNumber();
            tag.Value = new MockAttribute(new StringConstant("1,345.67"));
            tag.Var = new MockAttribute(new StringConstant("Decimal"));
            tag.Styles =
                new MockAttribute(new StringConstant(NumberStyles.AllowDecimalPoint + "," + NumberStyles.AllowThousands));
            Console.WriteLine(new StringConstant(NumberStyles.AllowDecimalPoint + "," + NumberStyles.AllowThousands));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model["Decimal"], Is.EqualTo(1345.67m));
        }

        [Test]
        public void TestFromValueOnEmptyString()
        {
            var tag = new ParseNumber();
            tag.Value = new MockAttribute(new StringConstant(""));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(String.Empty));
        }

        [Test]
        public void TestFromValueOnNoInput()
        {
            var tag = new ParseNumber();
            Assert.That(tag.Evaluate(_model), Is.EqualTo(String.Empty));
        }

        [Test]
        public void TestFromValueToVar()
        {
            var tag = new ParseNumber();
            tag.Value = new MockAttribute(new StringConstant("4"));
            tag.Var = new MockAttribute(new StringConstant("Number"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo(String.Empty));
            Assert.That(_model["Number"], Is.EqualTo(4m));
        }

       
    }
}
