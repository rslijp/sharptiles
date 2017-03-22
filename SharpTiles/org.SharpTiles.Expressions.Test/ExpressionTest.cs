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
 using System.Globalization;
 using System.Linq;
 using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Common;

namespace org.SharpTiles.Expressions.Test
{
    [TestFixture]
    public class ExpressionTest
    {
        [Test]
        public void TestSimpleFormulaNestedBracketWithModel()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "('3'+'2')",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(5)
                );
        }

        [Test]
        public void MinusOne()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'-1'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(-1)
                );
        }

        public class SampleModel
        {
            public SampleModel()
            {
                Name = "NAME VALUE";
            }

            public string Name { get; set; }

            public CultureInfo I18NLocale { get; set; }


            public bool Bool
            {
                get { return true; }
            }

            public decimal FirstInt
            {
                get { return 42; }
            }

            public decimal SecondInt
            {
                get { return 37; }
            }

            public object[] EmptyArray
            {
                get { return new object[0]; }
            }

            public object[] NonEmptyArray
            {
                get { return new object[1]; }
            }

            public string Value { get; set; }

            public object Nested  {get;set;}
        }

        [Test]
        public void TestAdditionalDownGradeInComplexPrecendence()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'2'+'3'^'2'*'2'+'2'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(22)
                );
        }

        [Test]
        public void TestAdditionalNameForAnd()
        {
            var reflection = new Reflection(new SampleModel());
            Assert.That(new ExpressionLib().ParseAndEvaluate("'true' and 'false'", reflection),
                        Is.EqualTo(new ExpressionLib().ParseAndEvaluate("'true'&&'false'", reflection)));
        }

        [Test]
        public void TestAdditionalNameForDivide()
        {
            var reflection = new Reflection(new SampleModel());
            Assert.That(new ExpressionLib().ParseAndEvaluate("'6' div '2'", reflection),
                        Is.EqualTo(new ExpressionLib().ParseAndEvaluate("'6'/'2'", reflection)));
        }

        [Test]
        public void TestAdditionalNameForEqualTo()
        {
            var reflection = new Reflection(new SampleModel());
            Assert.That(new ExpressionLib().ParseAndEvaluate("'6' eq '2'", reflection),
                        Is.EqualTo(new ExpressionLib().ParseAndEvaluate("'6'=='2'", reflection)));
        }

        [Test]
        public void TestAdditionalNameForGreaterThan()
        {
            var reflection = new Reflection(new SampleModel());
            Assert.That(new ExpressionLib().ParseAndEvaluate("'6' gt '2'", reflection),
                        Is.EqualTo(new ExpressionLib().ParseAndEvaluate("'6'>'2'", reflection)));
        }

        [Test]
        public void TestAdditionalNameForGreaterThanOrEqual()
        {
            var reflection = new Reflection(new SampleModel());
            Assert.That(new ExpressionLib().ParseAndEvaluate("'6' ge '2'", reflection),
                        Is.EqualTo(new ExpressionLib().ParseAndEvaluate("'6'>='2'", reflection)));
        }

        [Test]
        public void TestAdditionalNameForLessThan()
        {
            var reflection = new Reflection(new SampleModel());
            Assert.That(new ExpressionLib().ParseAndEvaluate("'6' lt '2'", reflection),
                        Is.EqualTo(new ExpressionLib().ParseAndEvaluate("'6'<'2'", reflection)));
        }

        [Test]
        public void TestAdditionalNameForLessThanOrEqual()
        {
            var reflection = new Reflection(new SampleModel());
            Assert.That(new ExpressionLib().ParseAndEvaluate("'6' le '2'", reflection),
                        Is.EqualTo(new ExpressionLib().ParseAndEvaluate("'6'<='2'", reflection)));
        }

        [Test]
        public void TestAdditionalNameForModulo()
        {
            var reflection = new Reflection(new SampleModel());
            Assert.That(new ExpressionLib().ParseAndEvaluate("'6' mod '2'", reflection),
                        Is.EqualTo(new ExpressionLib().ParseAndEvaluate("'6'%'2'", reflection)));
        }

        [Test]
        public void TestAdditionalNameForNot()
        {
            var reflection = new Reflection(new SampleModel());
            Assert.That(new ExpressionLib().ParseAndEvaluate("not 'true'", reflection),
                        Is.EqualTo(new ExpressionLib().ParseAndEvaluate("!'true'", reflection)));
        }

        [Test]
        public void TestAdditionalNameForNotEqualTo()
        {
            var reflection = new Reflection(new SampleModel());
            Assert.That(new ExpressionLib().ParseAndEvaluate("'6' ne '2'", reflection),
                        Is.EqualTo(new ExpressionLib().ParseAndEvaluate("'6'!='2'", reflection)));
        }

        [Test]
        public void TestAdditionalNameForOr()
        {
            var reflection = new Reflection(new SampleModel());
            Assert.That(new ExpressionLib().ParseAndEvaluate("'true' or 'false'", reflection),
                        Is.EqualTo(new ExpressionLib().ParseAndEvaluate("'true'||'false'", reflection)));
        }

        [Test]
        public void EagerShouldFailOnUnparsedEnd()
        {
            var reflection = new Reflection(new SampleModel());
            try
            {
                var e = new ExpressionLib().Parse("'true' or false'");
                Assert.Fail("Parse error expected");
            }
            catch (ParseException  EPe)
            {
                Assert.That(EPe.MessageWithOutContext, Is.EqualTo(ParseException.ExpectedToken().Message));
            }
            
        }

        [Test]
        public void EagerShouldAllowForEmptyString()
        {
            var reflection = new Reflection(new SampleModel());
            var e = new ExpressionLib().Parse("Name == ''");
            Assert.That(e.Evaluate(reflection), Is.False);

        }

        [Test]
        public void ShouldHaveReturnTypeOfConstants_bool()
        {
            var reflection = new Reflection(new SampleModel());
            var e = new ExpressionLib().Parse("'false'");
            Assert.That(e.ReturnType, Is.Not.Null);
            Assert.That(e.ReturnType, Is.EqualTo(typeof(bool)));

        }

        [Test]
        public void ShouldHaveReturnTypeOfExpression_int()
        {
            var reflection = new Reflection(new SampleModel());
            var e = new ExpressionLib().Parse("'6' + '7'");
            Assert.That(e.ReturnType, Is.Not.Null);
            Assert.That(e.ReturnType, Is.EqualTo(typeof(decimal)));

        }

        [Test]
        public void ShouldHaveReturnTypeOfExpression_bool()
        {
            try
            {
                var e = new ExpressionLib().Parse("Name=='6' || '7'");
            }
            catch (ConvertException Pe)
            {
                Assert.That(Pe.Message, Is.EqualTo(ConvertException.StaticTypeSafety(typeof(bool), typeof(decimal), "'7'").Message));
            }
        }

        [Test]
        public void ShouldHaveReturnTypeOfExpression_bool2()
        {
            try
            {
                var e = new ExpressionLib().Parse("Name=='NAME VALUE' || '7'");
            }
            catch (ConvertException Pe)
            {
                Assert.That(Pe.Message, Is.EqualTo(ConvertException.StaticTypeSafety(typeof(bool), typeof(decimal), "'7'").Message));
            }
        }

        [Test]
        public void TestAdditionalNameForPower()
        {
            var reflection = new Reflection(new SampleModel());
            Assert.That(new ExpressionLib().ParseAndEvaluate("'6' pow '2'", reflection),
                        Is.EqualTo(new ExpressionLib().ParseAndEvaluate("'6'^'2'", reflection)));
        }


        [Test]
        public void TestAdditionalNamesForOperators()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'2' pow '3' mod '3'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(2)
                );
        }

        [Test]
        public void TestAdditionalNestedComplexPrecendence()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'2'+'2'*'3'^'2'+'2'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(22)
                );
        }

        [Test]
        public void TestAdditionalTwoPrecendenceParts()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'2'+'3'*'2'+'2'^'2'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(12)
                );
        }

        [Test]
        public void TestAdditionProperty()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "FirstInt+SecondInt",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(79)
                );
        }

        [Test]
        public void TestAdditionWithConstant()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'1'+'2'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(3)
                );
        }


        [Test]
        public void TestArithmeticTypeCheckWrongBothSides()
        {
            Expression exp = new ExpressionLib().Parse("Bool+Bool");
            try
            {
                exp.Evaluate(new Reflection(new SampleModel()));
                Assert.Fail("Expected type error");
            }
            catch (ConvertException EPe)
            {
                Assert.That(EPe.Message, Is
                                             .EqualTo(ConvertException.CannotConvert(
                                                          typeof(decimal),
                                                          true.ToString()).Message));
            }
        }

        [Test]
        public void TestArithmeticTypeCheckWrongBothSides_Static()
        {
            try
            {
                Expression exp = new ExpressionLib().Parse("'true'+'true'");
                Assert.Fail("Expected type error");
            }
            catch (ConvertException EPe)
            {
                Assert.That(EPe.Message, Is
                                             .EqualTo(ConvertException.StaticTypeSafety(
                                                          typeof(decimal),
                                                          typeof(bool),
                                                          "'true'").Message));
            }
        }

        [Test]
        public void TestArithmeticTypeCheckWrongLeftHandSide()
        {
            Expression exp = new ExpressionLib().Parse("Bool+FirstInt");
            try
            {
                exp.Evaluate(new Reflection(new SampleModel()));
                Assert.Fail("Expected type error");
            }
            catch (ConvertException EPe)
            {
                Assert.That(EPe.Message, Is
                                             .EqualTo(ConvertException.CannotConvert(
                                                          typeof (decimal),
                                                          true.ToString()).Message));
            }
        }

        [Test]
        public void TestArithmeticTypeCheckWrongLeftHandSide_Static()
        {
            try
            {
                Expression exp = new ExpressionLib().Parse("'true'+'8'");
                Assert.Fail("Expected type error");
            }
            catch (ConvertException EPe)
            {
                Assert.That(EPe.Message, Is
                                             .EqualTo(ConvertException.StaticTypeSafety(
                                                          typeof(decimal),
                                                          typeof(bool),
                                                          "'true'").Message));
            }
        }

        [Test]
        public void TestArithmeticTypeCheckWrongRightHandSide()
        {
            Expression exp = new ExpressionLib().Parse("FirstInt+Bool");
            try
            {
                exp.Evaluate(new Reflection(new SampleModel()));
                Assert.Fail("Expected type error");
            }
            catch (ConvertException EPe)
            {
                Assert.That(EPe.Message, Is
                                             .EqualTo(ConvertException.CannotConvert(
                                                          typeof (decimal),
                                                          true.ToString()).Message));
            }
        }

        [Test]
        public void TestArithmeticTypeCheckWrongRightHandSide_Static()
        {
            try
            {
                Expression exp = new ExpressionLib().Parse("'8'+'true'");
                Assert.Fail("Expected type error");
            }
            catch (ConvertException EPe)
            {
                Assert.That(EPe.Message, Is
                                             .EqualTo(ConvertException.StaticTypeSafety(
                                                          typeof(decimal),
                                                          typeof(bool),
                                                          "'true'").Message));
            }
        }

        [Test]
        public void TestBooleanTupleTypeCheckWrongBothSides()
        {
            Expression exp = new ExpressionLib().Parse("FirstInt&&FirstInt");
            try
            {
                exp.Evaluate(new Reflection(new SampleModel()));
                Assert.Fail("Expected type error");
            }
            catch (ConvertException EPe)
            {
                Assert.That(EPe.Message, Is
                                             .EqualTo(ConvertException.CannotConvert(
                                                          typeof (bool),
                                                          "42").Message));
            }
        }

        [Test]
        public void TestBooleanTupleTypeCheckWrongBothSides_Static()
        {
            try
            {
                Expression exp = new ExpressionLib().Parse("'9'&&'9'");
                Assert.Fail("Expected type error");
            }
            catch (ConvertException EPe)
            {
                Assert.That(EPe.Message, Is
                                             .EqualTo(ConvertException.StaticTypeSafety(
                                                          typeof(bool),
                                                          typeof(decimal),
                                                          "'9'").Message));
            }
        }

        [Test]
        public void TestBooleanTupleTypeCheckWrongLeftHandSide()
        {
            Expression exp = new ExpressionLib().Parse("FirstInt&&Bool");
            try
            {
                exp.Evaluate(new Reflection(new SampleModel()));
                Assert.Fail("Expected type error");
            }
            catch (ConvertException EPe)
            {
                Assert.That(EPe.Message, Is
                                             .EqualTo(ConvertException.CannotConvert(
                                                          typeof (bool),
                                                          "42").Message));
            }
        }

        [Test]
        public void TestBooleanTupleTypeCheckWrongLeftHandSide_Static()
        {
            try
            {
                Expression exp = new ExpressionLib().Parse("'9'&&'true'");
                Assert.Fail("Expected type error");
            }
            catch (ConvertException EPe)
            {
                Assert.That(EPe.Message, Is
                                             .EqualTo(ConvertException.StaticTypeSafety(
                                                          typeof(bool),
                                                          typeof(decimal),
                                                          "'9'").Message));
            }
        }

        [Test]
        public void TestBooleanTupleTypeCheckWrongRightHandSide()
        {
            Expression exp = new ExpressionLib().Parse("Bool&&FirstInt");
            try
            {
                exp.Evaluate(new Reflection(new SampleModel()));
                Assert.Fail("Expected type error");
            }
            catch (ConvertException EPe)
            {
                Assert.That(EPe.Message, Is
                                             .EqualTo(ConvertException.CannotConvert(
                                                          typeof (bool),
                                                          "42").Message));
            }
        }

        [Test]
        public void TestBooleanTupleTypeCheckWrongRightHandSide_Static()
        {
            try
            {
                Expression exp = new ExpressionLib().Parse("'true'&&'9'");
                Assert.Fail("Expected type error");
            }
            catch (ConvertException EPe)
            {
                Assert.That(EPe.Message, Is
                                             .EqualTo(ConvertException.StaticTypeSafety(
                                                          typeof(bool),
                                                          typeof(decimal),
                                                          "'9'").Message));
            }
        }

        [Test]
        public void TestBoolProperty()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "Bool",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true)
                );
        }

        [Test]
        public void TestBracketsWithParametersShouldThrowException()
        {
            try
            {
                new ExpressionLib().Parse("('2', '3')");
                Assert.Fail("Exception expected");
            }
            catch (ExpressionParseException EPe)
            {
                String msg = ExpressionParseException.UnExpectedParameter(new Constant("3")).Message;
                Assert.That(EPe.MessageWithOutContext, Is.EqualTo(msg));
            }
        }

        [Test]
        public void TestBracketTypeCheckWrongNestedOfProperty()
        {
            Expression exp = new ExpressionLib().Parse("!(FirstInt)");
            try
            {
                exp.Evaluate(new Reflection(new SampleModel()));
                Assert.Fail("Expected type error");
            }
            catch (ConvertException EPe)
            {
                Assert.That(EPe.Message, Is
                                             .EqualTo(ConvertException.CannotConvert(
                                                          typeof (bool),
                                                          "42").Message));
            }
        }

        [Test]
        public void TestComparisonTypeBooleansOnBothSides()
        {
            Expression exp = new ExpressionLib().Parse("'true'<'false'");
            Assert.That(exp.Evaluate(new Reflection(new SampleModel())), Is.False);
            exp = new ExpressionLib().Parse("'false'<'true'");
            Assert.That(exp.Evaluate(new Reflection(new SampleModel())), Is.True);
        }

        [Test]
        public void TestComparisonTypeForcedStringsOnBothSides()
        {
            Expression exp = new ExpressionLib().Parse("@'8'<@'false'");
            Assert.That(exp.Evaluate(new Reflection(new SampleModel())), Is.True);
            exp = new ExpressionLib().Parse("@'false'<@'8'");
            Assert.That(exp.Evaluate(new Reflection(new SampleModel())), Is.False);
        }

        [Test]
        public void TestComparisonTypeCheckWrongLeftHandSide()
        {
            Expression exp = new ExpressionLib().Parse("Bool<FirstInt");
            try
            {
                exp.Evaluate(new Reflection(new SampleModel()));
                Assert.Fail("Expected type error");
            }
            catch (ComparisionException CPe)
            {
                Assert.That(CPe.MessageWithOutContext, Is
                                             .EqualTo(ComparisionException.UnComparable(
                                                          typeof(bool),
                                                          typeof(decimal)
                                                         ).Message));
            }
        }

        [Test]
        public void TestComparisonTypeCheckWrongLeftHandSide_Static()
        {
            try
            {
                Expression exp = new ExpressionLib().Parse("'true'<'8'");
                Assert.Fail("Expected type error");
            }
            catch (ConvertException CPe)
            {
                Assert.That(CPe.Message, Is
                                             .EqualTo(ConvertException.StaticTypeSafety(
                                                          typeof(bool),
                                                          typeof(decimal),
                                                          "'8'"
                                                         ).Message));
            }
        }

        [Test]
        public void TestComparisonTypeCheckWrongRightHandSide()
        {
            Expression exp = new ExpressionLib().Parse("FirstInt<Bool");
            try
            {
                exp.Evaluate(new Reflection(new SampleModel()));
                Assert.Fail("Expected type error");
            }
            catch (ComparisionException CPe)
            {
                Assert.That(CPe.MessageWithOutContext, Is
                                             .EqualTo(ComparisionException.UnComparable(
                                                          typeof(decimal),
                                                          typeof(bool)
                                                         ).Message));
            }
        }

        [Test]
        public void TestComparisonTypeCheckWrongRightHandSide_Static()
        {
            try
            {
                Expression exp = new ExpressionLib().Parse("'8'<'true'");
                Assert.Fail("Expected type error");
            }
            catch (ConvertException CPe)
            {
                Assert.That(CPe.Message, Is
                                             .EqualTo(ConvertException.StaticTypeSafety(
                                                          typeof(decimal),
                                                          typeof(bool),
                                                          "'true'"
                                                         ).Message));
            }
        }

       

        [Test]
        public void TestComparisonTypeCheckWrongRightHandSideWithForcedString_Static()
        {
           try
            {
                Expression exp = new ExpressionLib().Parse("@'8'<'true'");
                Assert.Fail("Expected type error");
            }
            catch (ConvertException CPe)
            {
                Assert.That(CPe.Message, Is
                                             .EqualTo(ConvertException.StaticTypeSafety(
                                                          typeof(string),
                                                          typeof(bool),
                                                          "'true'"
                                                         ).Message));
            }
        }



        [Test]
        public void TestComplexNestedFormula()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "('6'+FirstInt*'3'^('2'%'1'))<('9'+SecondInt^('4'/'2'))",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestConstant()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'a constant'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo("a constant")
                );
        }

        [Test]
        public void TestDivisionProperty()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'6'/'3'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(2)
                );
        }

        [Test]
        public void TestIntProperty()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "FirstInt",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(42)
                );
        }

        [Test]
        public void TestModuloProperty()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'13'%'5'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(3)
                );
        }

        [Test]
        public void TestMultipliProperty()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "FirstInt*SecondInt",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(1554)
                );
        }

        [Test]
        public void TestNoAdditionalNameStartOrEndsWithAWhiteSpace()
        {
            foreach (IExpressionParser parser in new ExpressionLib().GetRegisteredParsers())
            {
                if (parser.AdditionalTokens != null)
                {
                    foreach (ExpressionOperatorSign sign in parser.AdditionalTokens)
                    {
                        string token = sign.Token;
                        Assert.That(token.Trim().Length, Is.EqualTo(token.Length),
                                    "Additional token of " + parser.ParsedTypes.First().Name +
                                    " contains whitespaces at begin or end");
                    }
                }
            }
        }

        [Test]
        public void TestNotTypeCheckWrongNested()
        {
            Expression exp = new ExpressionLib().Parse("!FirstInt");
            try
            {
                exp.Evaluate(new Reflection(new SampleModel()));
                Assert.Fail("Expected type error");
            }
            catch (ConvertException EPe)
            {
                Assert.That(EPe.Message, Is
                                             .EqualTo(ConvertException.CannotConvert(
                                                          typeof (bool),
                                                          "42").Message));
            }
        }

        [Test]
        public void TestNotTypeCheckWrongNested_Static()
        {
            try
            {
                Expression exp = new ExpressionLib().Parse("!'9'");
                Assert.Fail("Expected type error");
            }
            catch (ConvertException EPe)
            {
                Assert.That(EPe.Message, Is
                                             .EqualTo(ConvertException.StaticTypeSafety(
                                                          typeof(bool),
                                                          typeof(decimal),
                                                          "'9'").Message));
            }
        }

        [Test]
        public void TestNotTypeCheckWrongNestedOfProperty()
        {
            Expression exp = new ExpressionLib().Parse("!FirstInt");
            try
            {
                exp.Evaluate(new Reflection(new SampleModel()));
                Assert.Fail("Expected type error");
            }
            catch (ConvertException EPe)
            {
                Assert.That(EPe.Message, Is
                                             .EqualTo(ConvertException.CannotConvert(
                                                          typeof (bool),
                                                          "42").Message));
            }
        }

        [Test]
        public void TestParseOfAndExpression()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'false'&&'true'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(false));
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'true'&&'true'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestParseOfBooleanComparisionAndArithmeticExpressionNestedAndOnBothSides()
        {
            Console.WriteLine(
                new ExpressionLib().ParseAndEvaluate("'13' + FirstInt / '2' >= SecondInt - SecondInt % '8'",
                                            new Reflection(new SampleModel())));
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'13' + FirstInt / '2' >= SecondInt - SecondInt % '8' && '7' == FirstInt / '6'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestParseOfBooleanLogicExpression()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'false'||!'true'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(false));
        }

        [Test]
        public void TestParseOfBooleanLogicExpressionMoreFormulas()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "!'false' and !'false'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestParseNumberWhenValueIsInt()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate("9",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(9));
        }

        [Test]
        public void TestParseNumberWhenValueIsDecimal()
        {

            Assert.That(new ExpressionLib().ParseAndEvaluate("9.6",
                            new Reflection(new SampleModel() {I18NLocale = new CultureInfo("en-GB")})),
                        Is.EqualTo(9.6m));
        }

        [Test]
        public void TestParseNumberWhenValueIsBool()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate("true",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestParseNumberWhenValueIsDateTime()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate("'1979-10-02T16:22:33'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(new DateTime(1979, 10, 2, 16, 22, 33)));
        }

        [Test]
        public void TestParseNumberWhenValueIsDate()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate("'1979-10-02'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(new DateTime(1979, 10, 2)));
        }

        [Test]
        public void TestParsePropertyWhenNoneMatch()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate("Name",
                            new Reflection(new SampleModel())),
                        Is.EqualTo("NAME VALUE"));
        }

        [Test]
        public void TestParseOfEqualThanOrEqualExpression()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'9'=='9'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }


        [Test]
        public void TestParseOfGreaterThanExpression()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'9'>'9'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(false));
        }

        [Test]
        public void TestParseOfGreaterThanOrEqualExpression()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'9'>='9'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestParseOfLessThanExpression()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'6'<'9'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestParseOfLessThanExpressionWithLeftFormula()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'6'+FirstInt<'9'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(false));
        }

        [Test]
        public void TestParseOfLessThanExpressionWithRightFormula()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'6'<'9'+SecondInt",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestParseOfLessThanOrEqualExpression()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'9'<='9'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestParseOfNestedBooleanLogicExpressionBothSides()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "!'false' and !'false' && 'false' || 'false'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(false));
        }

        [Test]
        public void TestParseOfNestedBooleanLogicExpressionLeftSide()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "!'false' and !'false' || 'false'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestParseOfNotEqualThanOrEqualExpression()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'9'!='9'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(false));
        }

        [Test]
        public void TestParseOfNotExpression()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "!'false'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "!'true'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(false));
        }

        [Test]
        public void TestParseOfOrExpression()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'false'||'false'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(false));
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'true'||'false'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestPowerProperty()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'2'^'8'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(256)
                );
        }

        [Test]
        public void TestPrecende()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'2'+'3'*'3'+'2'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(13)
                );
        }

        [Test]
        public void TestPrecendeMultipleAboveAdd()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'2'+'2'*'3'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(8)
                );
        }

        [Test]
        public void TestPrecendeMultipleAboveAddDifferentOrder()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'2'*'3'+'2'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(8)
                );
        }

        [Test]
        public void TestPrecendeOverridenByBrackets()
        {
            Expression exp = new ExpressionLib().Parse("('2'+'3')*'3'+'2'");

            Assert.That(exp.GetType(), Is.EqualTo(typeof (Add)));

            var add = (Add) exp;
            Assert.That(add.Lhs.GetType(), Is.EqualTo(typeof (Multiply)));
            Assert.That(add.Rhs.GetType(), Is.EqualTo(typeof (Constant)));

            var multipy = (Multiply) add.Lhs;
            Assert.That(multipy.Lhs.GetType(), Is.EqualTo(typeof (Brackets)));
            Assert.That(multipy.Rhs.GetType(), Is.EqualTo(typeof (Constant)));


            Assert.That(exp.Evaluate(new Reflection(new SampleModel())),
                        Is.EqualTo(17)
                );
        }

        [Test]
        public void TestPrecendeOverridenByBracketsBeforeAndAfterNormal()
        {
            Expression exp = new ExpressionLib().Parse("'2'+('3'*'7')+'2'");
            Assert.That(exp.Evaluate(new Reflection(new SampleModel())),
                        Is.EqualTo(25)
                );
        }

        [Test]
        public void TestPrecendeOverridenByBracketsMultipleAboveAdd()
        {
            Expression exp = new ExpressionLib().Parse("('2'+'2')*'3'");
            Assert.That(exp.GetType(), Is.EqualTo(typeof (Multiply)));
            var multipy = (Multiply) exp;
            Assert.That(multipy.Lhs.GetType(), Is.EqualTo(typeof (Brackets)));
            Assert.That(multipy.Rhs.GetType(), Is.EqualTo(typeof (Constant)));
            var brackets = (Brackets) multipy.Lhs;
            Assert.That(brackets.Nested.GetType(), Is.EqualTo(typeof (Add)));
            var add = (Add) brackets.Nested;
            Assert.That(add.Lhs.GetType(), Is.EqualTo(typeof (Constant)));
            Assert.That(add.Rhs.GetType(), Is.EqualTo(typeof (Constant)));

            Assert.That(exp.Evaluate(new Reflection(new SampleModel())),
                        Is.EqualTo(12)
                );
        }


        [Test]
        public void TestPrecendeOverridenByBracketsMultipleAboveAddDifferentOrder()
        {
            Expression exp = new ExpressionLib().Parse("'3'*('3'+'2')");
            Assert.That(exp.GetType(), Is.EqualTo(typeof (Multiply)));
            var multipy = (Multiply) exp;
            Assert.That(multipy.Lhs.GetType(), Is.EqualTo(typeof (Constant)));
            Assert.That(multipy.Rhs.GetType(), Is.EqualTo(typeof (Brackets)));
            var brackets = (Brackets) multipy.Rhs;
            Assert.That(brackets.Nested.GetType(), Is.EqualTo(typeof (Add)));
            var add = (Add) brackets.Nested;
            Assert.That(add.Lhs.GetType(), Is.EqualTo(typeof (Constant)));
            Assert.That(add.Rhs.GetType(), Is.EqualTo(typeof (Constant)));

            Assert.That(exp.Evaluate(new Reflection(new SampleModel())),
                        Is.EqualTo(15)
                );
        }

        [Test]
        public void TestSameComplexNestedFormulaWithSpaces()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "('6' + FirstInt * '3' ^ ('2' % '1')) < ('9' + SecondInt ^ ('4' / '2'))",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestSimpleBracketWithModel()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "(FirstInt)",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(42)
                );
        }

        [Test]
        public void TestSimpleBracketWithOutModel()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "('2')",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(2m)
                );
        }

        [Test]
        public void TestSimpleBracketWithOutModelWithForcedString()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "(@'2')",
                            new Reflection(new SampleModel())),
                        Is.EqualTo("2")
                );
        }

        [Test]
        public void TestSimpleDoubleNestedBracketWithModel()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "(((FirstInt)))",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(42)
                );
        }

        [Test]
        public void TestSimpleNestedBracketWithModel()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "((FirstInt))",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(42)
                );
        }

        [Test]
        public void TestSpacesAllowed()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'2' + '3' * '3' + '2'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(13)
                );
        }

        [Test]
        public void TestSpacesShouldBeAtTheEndOfAnExpressionShouldBeIgnored()
        {
            var add = (Add) new ExpressionLib().Parse("'2' + '3' ");
            Assert.That(((Constant) add.Lhs).Value, Is.EqualTo("2"));
            Assert.That(((Constant) add.Rhs).Value, Is.EqualTo("3"));
        }

        [Test]
        public void TestSpacesShouldBeAtTheStartOfAnExpressionShouldBeIgnored()
        {
            var add = (Add) new ExpressionLib().Parse(" '2' + '3'");
            Assert.That(((Constant) add.Lhs).Value, Is.EqualTo("2"));
            Assert.That(((Constant) add.Rhs).Value, Is.EqualTo("3"));
        }

        [Test]
        public void TestSpacesShouldBeIngnoredSurroundingSeperators()
        {
            var add = (Add) new ExpressionLib().Parse("'2' + '3'");
            Assert.That(((Constant) add.Lhs).Value, Is.EqualTo("2"));
            Assert.That(((Constant) add.Rhs).Value, Is.EqualTo("3"));
        }

        [Test]
        public void TestSpacesShouldBeTrimmedInProperties()
        {
            Assert.That(((PropertyOrConstant) new ExpressionLib().Parse("Property ")).Name, Is.EqualTo("Property"));
        }

        [Test]
        public void TestSpacesShouldBeTrimmedInSeperators()
        {
            var add = (Add) new ExpressionLib().Parse("'2' + '3'");
            Assert.That(((Constant) add.Lhs).Value, Is.EqualTo("2"));
            Assert.That(((Constant) add.Rhs).Value, Is.EqualTo("3"));
        }

        [Test]
        public void TestSpacesShouldNotBeTrimmedInConstants()
        {
            Assert.That(((Constant) new ExpressionLib().Parse("'2 '")).Value, Is.EqualTo("2 "));
        }

        [Test]
        public void TestStringProperty()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "Name",
                            new Reflection(new SampleModel())),
                        Is.EqualTo("NAME VALUE")
                );
        }

        [Test]
        public void TestNestedProperty_Dot_Notation()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "Nested.Name",
                            new Reflection(new SampleModel() {Nested = new SampleModel {Name = "NESTED VALUE"} })),
                        Is.EqualTo("NESTED VALUE")
                );
        }

        [Test]
        public void TestNestedProperty_Constant_Bracket_Notation()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "Nested['Name']",
                            new Reflection(new SampleModel() { Nested = new SampleModel { Name = "NESTED VALUE" } })),
                        Is.EqualTo("NESTED VALUE")
                );
        }

        [Test]
        public void TestNestedProperty_DictionaryAccess_Bracket_Notation()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "Nested['A']",
                            new Reflection(new SampleModel() { Nested = new Dictionary<string, string> { { "A", "Found me" } } })),
                        Is.EqualTo("Found me")
                );
        }

        public void TestNestedProperty_Bracket_Notation_Using_A_Function()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "Nested[fn:substring('AB',1)]",
                            new Reflection(new SampleModel() { Nested = new Dictionary<string, string> { { "A", "Found me" } } })),
                        Is.EqualTo("Found me")
                );
        }

        [Test]
        public void TestNestedProperty_WithDots_Bracket_Notation()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "Nested['A.B']",
                            new Reflection(new SampleModel() { Nested = new Dictionary<string, string> { {"A.B", "Found me" } } })),
                        Is.EqualTo("Found me")
                );
        }

        [Test]
        public void TestNestedProperty_Bracket_Notation()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "Nested[Name]",
                            new Reflection(new SampleModel() { Nested = new SampleModel { Name = "Wrong" }, Name="FirstInt" })),
                        Is.EqualTo(42)
                );
        }

        [Test]
        public void TestDoubleNestedProperty_Bracket_Notation()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "Nested['Nested']['Name']",
                            new Reflection(new SampleModel() { Nested = new SampleModel { Nested = new SampleModel { Name = "DEEPER NESTED VALUE" } } })),
                        Is.EqualTo("DEEPER NESTED VALUE")
                );
        }

        [Test]
        public void TestNestedEvaluateProperty_Bracket_Notation()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "Nested[Nested['Name']]",
                            new Reflection(new SampleModel() { Nested = new SampleModel { Name= "FirstInt" }})),
                        Is.EqualTo(42)
                );
        }

        [Test]
        public void TestSubstractionProperty()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "FirstInt-SecondInt",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(5)
                );
        }

        [Test]
        public void TestThreeAdditions()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "FirstInt+FirstInt+FirstInt",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(126)
                );
        }

        [Test]
        public void TestTwoBracketsInOneExpressionSameLevel()
        {
            Expression exp = new ExpressionLib().Parse("('14'+'6')/('2'*'5')");
            Assert.That(exp.GetType(), Is.EqualTo(typeof (Divide)));

            var divide = (Divide) exp;

            Assert.That(divide.Lhs.GetType(), Is.EqualTo(typeof (Brackets)));
            Assert.That(divide.Rhs.GetType(), Is.EqualTo(typeof (Brackets)));

            Assert.That(exp.Evaluate(new Reflection(new SampleModel())),
                        Is.EqualTo(2)
                );
        }

        [Test]
        public void TestTwoNestedBracketsInOneExpression()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "('14'+'6')/'2'^'2'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(5)
                );
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "(('14'+'6')/'2')",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(10)
                );
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "(('14'+'6')/'2')^'2'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(100)
                );
        }

        [Test]
        public void Should_Handle_Function_With_Zero_Arguments()
        {
            var reflection = new Reflection(new SampleModel());
            var e = new ExpressionLib().Parse("fn:now()");
            Assert.That(e.ReturnType, Is.Not.Null);
            Assert.That(e.ReturnType, Is.EqualTo(typeof(DateTime)));

        }

        [Test]
        public void Should_Handle_Functions_With_Zero_Arguments()
        {
            var reflection = new Reflection(new SampleModel());
            var e = new ExpressionLib().Parse("fn:now() le fn:now()");
            Assert.That(e.ReturnType, Is.Not.Null);
            Assert.That(e.ReturnType, Is.EqualTo(typeof(bool)));

        }

        [Test]
        public void TestMathMaxFunction()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "max('3','2')",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(3)
                );
        }
        [Test]
        public void TestMathMaxMinimalSyntaxFunction()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "max(3,2)",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(3)
                );
        }
        [Test]
        public void TestMathMinMinimalSyntaxFunction()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "min(3,2)",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(2)
                );
        }
        [Test]
        public void TestMathAbsMinimalSyntaxFunction()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "abs('-3')",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(3)
                );
        }
        [Test]
        public void TestMatRoundMinimalSyntaxFunction()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "round(3.2)",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(3)
                );
        }
        [Test]
        public void Should_Slobber_Up_Whole_PropertyName()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "Administrator",
                            new Reflection(new Hashtable() {{"Administrator", true}})),
                        Is.EqualTo(true)
                );
        }

        [Test]
        public void Should_Slobber_Up_Whole_PropertyName_LowerCase()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "administrator",
                            new Reflection(new Hashtable() { { "administrator", true } })),
                        Is.EqualTo(true)
                );
        }

        [Test]
        public void Should_Not_Slobber_Up_Operators()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "administrator eq 'piet'",
                            new Reflection(new Hashtable() { { "administrator", "piet" } })),
                        Is.EqualTo(true)
                );
        }

        [Test]
        public void Should_FallBackTo_Property_If_Min_Misses_Bracket()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "minPrice",
                            new Reflection(new Hashtable() { { "minPrice", true } })),
                        Is.EqualTo(true)
                );
        }

        [Test]
        public void Should_Slobber_Up_Whole_PropertyName_With_Spaces()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "as administrator op in",
                            new Reflection(new Hashtable() { { "as administrator op in", true } })),
                        Is.EqualTo(true)
                );
        }
        [Test]
        public void TestMatFloorMinimalSyntaxFunction()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "floor(3.2)",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(3)
                );
        }
        [Test]
        public void TestMatCeilMinimalSyntaxFunction()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "ceil(3.2)",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(4)
                );
        }
        [Test]
        public void TestNestedMathFunctions()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "max(3.2, min(2.0, 4.0))",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(3.2m)
                );
        }
        
        [Test]
        public void TestMathMaxFunctionWithProperty()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "max(3,FirstInt)",
                            new Reflection(new SampleModel() )),
                        Is.EqualTo(42)
                );
        }

        [Test]
        public void TestSimpleTernaryExpression()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "'true'?'then':'else'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo("then")
                );
        }

        [Test]
        public void TestSimpleTernaryExpressionWithoutQuotes()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "true?'then':'else'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo("then")
                );
        }

        [Test]
        public void TestSimpleTernaryExpressionWithoutQuotesWithNestedExpressionsInCondition()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "true&&false?'then':'else'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo("else")
                );
        }

        [Test]
        public void TestSimpleTernaryExpressionWithoutQuotesWithNestedComplexExpressionsInCondition()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "true||(true&&false)?'then':'else'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo("then")
                );
        }

        [Test]
        public void TestSimpleTernaryExpressionWithoutExpressionInThen()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "true?fn:trim(' then'):'else'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo("then")
                );
        }


        [Test]
        public void TestSimpleTernaryExpressionWithoutExpressionInElse()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "false?fn:trim(' then'):1+1",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(2)
                );
        }

        [Test]
        public void Bug()
        {
            var input = "fn:replace(Name,'\\\\','/')";
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            input,
                            new Reflection(new SampleModel {Name="C:\\Temp\\Bla"})),
                        Is.EqualTo("C:/Temp/Bla")
                );
        }

        [Test]
        public void TestNestedTernaryExpressions()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                            "true ? true?'then':'no then' :'else'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo("then")
                );
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                           "false ? true?'then':'no then' :false?'no else':'else'",
                           new Reflection(new SampleModel())),
                       Is.EqualTo("else")
               );
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                          "false ? true?'then':'no then' :true?false:true?'no else':'else'",
                          new Reflection(new SampleModel())),
                      Is.EqualTo("else")
              );
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                          "false ? (true?'then':'no then') :((true?false:true)?'no else':'else')",
                          new Reflection(new SampleModel())),
                      Is.EqualTo("else")
              );
        }

        [Test]
        public void ComplexPattern()
        {
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                        "fn:regExReplace(Value, 'a', 'A')",
                        new Reflection(new SampleModel() { Value = "aBC" })),
                    Is.EqualTo("ABC")
            );
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                @"fn:regExReplace(Value, '(^\\+)|(^00)|(\ |\\(|\\)|\-)', '')",
                new Reflection(new SampleModel() {Value = "555-1234"})),
                Is.EqualTo("5551234")
            );
            Assert.That(new ExpressionLib().ParseAndEvaluate(
               @"fn:regExReplace(Value, '(^\\+)|(^00)|(\ |\\(|\\)|\-)', '')",
               new Reflection(new SampleModel() { Value = "555-001234" })),
               Is.EqualTo("555001234")
           );
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                        @"fn:regExReplace(Value, '(^\\+)|(^00)|(\ |\\(|\\)|\-)', '')",
                        new Reflection(new SampleModel() { Value = "(06) 14 66 49 54" })),
                    Is.EqualTo("0614664954")
            );
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                       @"fn:regExReplace(Value, '(^\\+)|(^00)|(\ |\\(|\\)|\-)', '')",
                       new Reflection(new SampleModel() { Value = "+31614664954" })),
                   Is.EqualTo("31614664954")
           );
            Assert.That(new ExpressionLib().ParseAndEvaluate(
                       @"fn:regExReplace(Value, '(^\\+)|(^00)|(\ |\\(|\\)|\-)', '')",
                       new Reflection(new SampleModel() { Value = "061+4664954" })),
                   Is.EqualTo("061+4664954")
           );
            //            var function = new RegExReplaceFunction();
            //            var pattern = @"(^\+)|(^00)|(\ |\(|\)|\-)";
            //            Assert.That(function.Evaluate("0614664954", pattern, ""), Is.EqualTo("0614664954"));
            //            Assert.That(function.Evaluate("+31614664954", pattern, ""), Is.EqualTo("31614664954"));
            //            Assert.That(function.Evaluate("061+4664954", pattern, ""), Is.EqualTo("061+4664954"));
            //            Assert.That(function.Evaluate("0031614664954", pattern, ""), Is.EqualTo("31614664954"));
            //            Assert.That(function.Evaluate("0031614664954", pattern, ""), Is.EqualTo("31614664954"));
            //            Assert.That(function.Evaluate("06 14 66 49 54", pattern, ""), Is.EqualTo("0614664954"));
            //            Assert.That(function.Evaluate("(06) 14 66 49 54", pattern, ""), Is.EqualTo("0614664954"));
            //            Assert.That(function.Evaluate("06-14664954", pattern, ""), Is.EqualTo("0614664954"));
        }
    }
}
