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
            Assert.That(Expression.ParseAndEvaluate(
                            "('3'+'2')",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(5)
                );
        }

        public class SampleModel
        {
            public string Name
            {
                get { return "NAME VALUE"; }
            }

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
        }

        [Test]
        public void TestAdditionalDownGradeInComplexPrecendence()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "'2'+'3'^'2'*'2'+'2'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(22)
                );
        }

        [Test]
        public void TestAdditionalNameForAnd()
        {
            var reflection = new Reflection(new SampleModel());
            Assert.That(Expression.ParseAndEvaluate("'true' and 'false'", reflection),
                        Is.EqualTo(Expression.ParseAndEvaluate("'true'&&'false'", reflection)));
        }

        [Test]
        public void TestAdditionalNameForDivide()
        {
            var reflection = new Reflection(new SampleModel());
            Assert.That(Expression.ParseAndEvaluate("'6' div '2'", reflection),
                        Is.EqualTo(Expression.ParseAndEvaluate("'6'/'2'", reflection)));
        }

        [Test]
        public void TestAdditionalNameForEqualTo()
        {
            var reflection = new Reflection(new SampleModel());
            Assert.That(Expression.ParseAndEvaluate("'6' eq '2'", reflection),
                        Is.EqualTo(Expression.ParseAndEvaluate("'6'=='2'", reflection)));
        }

        [Test]
        public void TestAdditionalNameForGreaterThan()
        {
            var reflection = new Reflection(new SampleModel());
            Assert.That(Expression.ParseAndEvaluate("'6' gt '2'", reflection),
                        Is.EqualTo(Expression.ParseAndEvaluate("'6'>'2'", reflection)));
        }

        [Test]
        public void TestAdditionalNameForGreaterThanOrEqual()
        {
            var reflection = new Reflection(new SampleModel());
            Assert.That(Expression.ParseAndEvaluate("'6' ge '2'", reflection),
                        Is.EqualTo(Expression.ParseAndEvaluate("'6'>='2'", reflection)));
        }

        [Test]
        public void TestAdditionalNameForLessThan()
        {
            var reflection = new Reflection(new SampleModel());
            Assert.That(Expression.ParseAndEvaluate("'6' lt '2'", reflection),
                        Is.EqualTo(Expression.ParseAndEvaluate("'6'<'2'", reflection)));
        }

        [Test]
        public void TestAdditionalNameForLessThanOrEqual()
        {
            var reflection = new Reflection(new SampleModel());
            Assert.That(Expression.ParseAndEvaluate("'6' le '2'", reflection),
                        Is.EqualTo(Expression.ParseAndEvaluate("'6'<='2'", reflection)));
        }

        [Test]
        public void TestAdditionalNameForModulo()
        {
            var reflection = new Reflection(new SampleModel());
            Assert.That(Expression.ParseAndEvaluate("'6' mod '2'", reflection),
                        Is.EqualTo(Expression.ParseAndEvaluate("'6'%'2'", reflection)));
        }

        [Test]
        public void TestAdditionalNameForNot()
        {
            var reflection = new Reflection(new SampleModel());
            Assert.That(Expression.ParseAndEvaluate("not 'true'", reflection),
                        Is.EqualTo(Expression.ParseAndEvaluate("!'true'", reflection)));
        }

        [Test]
        public void TestAdditionalNameForNotEqualTo()
        {
            var reflection = new Reflection(new SampleModel());
            Assert.That(Expression.ParseAndEvaluate("'6' ne '2'", reflection),
                        Is.EqualTo(Expression.ParseAndEvaluate("'6'!='2'", reflection)));
        }

        [Test]
        public void TestAdditionalNameForOr()
        {
            var reflection = new Reflection(new SampleModel());
            Assert.That(Expression.ParseAndEvaluate("'true' or 'false'", reflection),
                        Is.EqualTo(Expression.ParseAndEvaluate("'true'||'false'", reflection)));
        }

        [Test]
        public void EagerShouldFailOnUnparsedEnd()
        {
            var reflection = new Reflection(new SampleModel());
            try
            {
                var e = Expression.Parse("'true' or false'");
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
            var e = Expression.Parse("Name == ''");
            Assert.That(e.Evaluate(reflection), Is.False);

        }

        [Test]
        public void ShouldHaveReturnTypeOfConstants_bool()
        {
            var reflection = new Reflection(new SampleModel());
            var e = Expression.Parse("'false'");
            Assert.That(e.ReturnType, Is.Not.Null);
            Assert.That(e.ReturnType, Is.EqualTo(typeof(bool)));

        }

        [Test]
        public void ShouldHaveReturnTypeOfExpression_int()
        {
            var reflection = new Reflection(new SampleModel());
            var e = Expression.Parse("'6' + '7'");
            Assert.That(e.ReturnType, Is.Not.Null);
            Assert.That(e.ReturnType, Is.EqualTo(typeof(decimal)));

        }

        [Test]
        public void ShouldHaveReturnTypeOfExpression_bool()
        {
            try
            {
                var e = Expression.Parse("Name=='6' || '7'");
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
                var e = Expression.Parse("Name=='NAME VALUE' || '7'");
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
            Assert.That(Expression.ParseAndEvaluate("'6' pow '2'", reflection),
                        Is.EqualTo(Expression.ParseAndEvaluate("'6'^'2'", reflection)));
        }


        [Test]
        public void TestAdditionalNamesForOperators()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "'2' pow '3' mod '3'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(2)
                );
        }

        [Test]
        public void TestAdditionalNestedComplexPrecendence()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "'2'+'2'*'3'^'2'+'2'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(22)
                );
        }

        [Test]
        public void TestAdditionalTwoPrecendenceParts()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "'2'+'3'*'2'+'2'^'2'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(12)
                );
        }

        [Test]
        public void TestAdditionProperty()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "FirstInt+SecondInt",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(79)
                );
        }

        [Test]
        public void TestAdditionWithConstant()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "'1'+'2'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(3)
                );
        }


        [Test]
        public void TestArithmeticTypeCheckWrongBothSides()
        {
            Expression exp = Expression.Parse("Bool+Bool");
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
                Expression exp = Expression.Parse("'true'+'true'");
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
            Expression exp = Expression.Parse("Bool+FirstInt");
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
                Expression exp = Expression.Parse("'true'+'8'");
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
            Expression exp = Expression.Parse("FirstInt+Bool");
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
                Expression exp = Expression.Parse("'8'+'true'");
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
            Expression exp = Expression.Parse("FirstInt&&FirstInt");
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
                Expression exp = Expression.Parse("'9'&&'9'");
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
            Expression exp = Expression.Parse("FirstInt&&Bool");
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
                Expression exp = Expression.Parse("'9'&&'true'");
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
            Expression exp = Expression.Parse("Bool&&FirstInt");
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
                Expression exp = Expression.Parse("'true'&&'9'");
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
            Assert.That(Expression.ParseAndEvaluate(
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
                Expression.Parse("('2', '3')");
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
            Expression exp = Expression.Parse("!(FirstInt)");
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
            Expression exp = Expression.Parse("'true'<'false'");
            Assert.That(exp.Evaluate(new Reflection(new SampleModel())), Is.False);
            exp = Expression.Parse("'false'<'true'");
            Assert.That(exp.Evaluate(new Reflection(new SampleModel())), Is.True);
        }

        [Test]
        public void TestComparisonTypeForcedStringsOnBothSides()
        {
            Expression exp = Expression.Parse("@'8'<@'false'");
            Assert.That(exp.Evaluate(new Reflection(new SampleModel())), Is.True);
            exp = Expression.Parse("@'false'<@'8'");
            Assert.That(exp.Evaluate(new Reflection(new SampleModel())), Is.False);
        }

        [Test]
        public void TestComparisonTypeCheckWrongLeftHandSide()
        {
            Expression exp = Expression.Parse("Bool<FirstInt");
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
                Expression exp = Expression.Parse("'true'<'8'");
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
            Expression exp = Expression.Parse("FirstInt<Bool");
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
                Expression exp = Expression.Parse("'8'<'true'");
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
                Expression exp = Expression.Parse("@'8'<'true'");
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
            Assert.That(Expression.ParseAndEvaluate(
                            "('6'+FirstInt*'3'^('2'%'1'))<('9'+SecondInt^('4'/'2'))",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestConstant()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "'a constant'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo("a constant")
                );
        }

        [Test]
        public void TestDivisionProperty()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "'6'/'3'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(2)
                );
        }

        [Test]
        public void TestIntProperty()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "FirstInt",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(42)
                );
        }

        [Test]
        public void TestModuloProperty()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "'13'%'5'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(3)
                );
        }

        [Test]
        public void TestMultipliProperty()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "FirstInt*SecondInt",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(1554)
                );
        }

        [Test]
        public void TestNoAdditionalNameStartOrEndsWithAWhiteSpace()
        {
            foreach (IExpressionParser parser in Expression.GetRegisteredParsers())
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
            Expression exp = Expression.Parse("!FirstInt");
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
                Expression exp = Expression.Parse("!'9'");
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
            Expression exp = Expression.Parse("!FirstInt");
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
            Assert.That(Expression.ParseAndEvaluate(
                            "'false'&&'true'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(false));
            Assert.That(Expression.ParseAndEvaluate(
                            "'true'&&'true'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestParseOfBooleanComparisionAndArithmeticExpressionNestedAndOnBothSides()
        {
            Console.WriteLine(
                Expression.ParseAndEvaluate("'13' + FirstInt / '2' >= SecondInt - SecondInt % '8'",
                                            new Reflection(new SampleModel())));
            Assert.That(Expression.ParseAndEvaluate(
                            "'13' + FirstInt / '2' >= SecondInt - SecondInt % '8' && '7' == FirstInt / '6'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestParseOfBooleanLogicExpression()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "'false'||!'true'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(false));
        }

        [Test]
        public void TestParseOfBooleanLogicExpressionMoreFormulas()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "!'false' and !'false'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestParseNumberWhenValueIsInt()
        {
            Assert.That(Expression.ParseAndEvaluate("9",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(9));
        }

        [Test]
        public void TestParseNumberWhenValueIsDecimal()
        {

            Assert.That(Expression.ParseAndEvaluate("9.6",
                            new Reflection(new SampleModel() {I18NLocale = new CultureInfo("en-GB")})),
                        Is.EqualTo(9.6m));
        }

        [Test]
        public void TestParseNumberWhenValueIsBool()
        {
            Assert.That(Expression.ParseAndEvaluate("true",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestParseNumberWhenValueIsDateTime()
        {
            Assert.That(Expression.ParseAndEvaluate("'1979-10-02T16:22:33'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(new DateTime(1979, 10, 2, 16, 22, 33)));
        }

        [Test]
        public void TestParseNumberWhenValueIsDate()
        {
            Assert.That(Expression.ParseAndEvaluate("'1979-10-02'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(new DateTime(1979, 10, 2)));
        }

        [Test]
        public void TestParsePropertyWhenNoneMatch()
        {
            Assert.That(Expression.ParseAndEvaluate("Name",
                            new Reflection(new SampleModel())),
                        Is.EqualTo("NAME VALUE"));
        }

        [Test]
        public void TestParseOfEqualThanOrEqualExpression()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "'9'=='9'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }


        [Test]
        public void TestParseOfGreaterThanExpression()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "'9'>'9'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(false));
        }

        [Test]
        public void TestParseOfGreaterThanOrEqualExpression()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "'9'>='9'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestParseOfLessThanExpression()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "'6'<'9'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestParseOfLessThanExpressionWithLeftFormula()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "'6'+FirstInt<'9'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(false));
        }

        [Test]
        public void TestParseOfLessThanExpressionWithRightFormula()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "'6'<'9'+SecondInt",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestParseOfLessThanOrEqualExpression()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "'9'<='9'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestParseOfNestedBooleanLogicExpressionBothSides()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "!'false' and !'false' && 'false' || 'false'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(false));
        }

        [Test]
        public void TestParseOfNestedBooleanLogicExpressionLeftSide()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "!'false' and !'false' || 'false'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestParseOfNotEqualThanOrEqualExpression()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "'9'!='9'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(false));
        }

        [Test]
        public void TestParseOfNotExpression()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "!'false'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
            Assert.That(Expression.ParseAndEvaluate(
                            "!'true'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(false));
        }

        [Test]
        public void TestParseOfOrExpression()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "'false'||'false'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(false));
            Assert.That(Expression.ParseAndEvaluate(
                            "'true'||'false'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestPowerProperty()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "'2'^'8'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(256)
                );
        }

        [Test]
        public void TestPrecende()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "'2'+'3'*'3'+'2'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(13)
                );
        }

        [Test]
        public void TestPrecendeMultipleAboveAdd()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "'2'+'2'*'3'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(8)
                );
        }

        [Test]
        public void TestPrecendeMultipleAboveAddDifferentOrder()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "'2'*'3'+'2'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(8)
                );
        }

        [Test]
        public void TestPrecendeOverridenByBrackets()
        {
            Expression exp = Expression.Parse("('2'+'3')*'3'+'2'");

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
            Expression exp = Expression.Parse("'2'+('3'*'7')+'2'");
            Assert.That(exp.Evaluate(new Reflection(new SampleModel())),
                        Is.EqualTo(25)
                );
        }

        [Test]
        public void TestPrecendeOverridenByBracketsMultipleAboveAdd()
        {
            Expression exp = Expression.Parse("('2'+'2')*'3'");
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
            Expression exp = Expression.Parse("'3'*('3'+'2')");
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
            Assert.That(Expression.ParseAndEvaluate(
                            "('6' + FirstInt * '3' ^ ('2' % '1')) < ('9' + SecondInt ^ ('4' / '2'))",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestSimpleBracketWithModel()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "(FirstInt)",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(42)
                );
        }

        [Test]
        public void TestSimpleBracketWithOutModel()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "('2')",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(2m)
                );
        }

        [Test]
        public void TestSimpleBracketWithOutModelWithForcedString()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "(@'2')",
                            new Reflection(new SampleModel())),
                        Is.EqualTo("2")
                );
        }

        [Test]
        public void TestSimpleDoubleNestedBracketWithModel()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "(((FirstInt)))",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(42)
                );
        }

        [Test]
        public void TestSimpleNestedBracketWithModel()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "((FirstInt))",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(42)
                );
        }

        [Test]
        public void TestSpacesAllowed()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "'2' + '3' * '3' + '2'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(13)
                );
        }

        [Test]
        public void TestSpacesShouldBeAtTheEndOfAnExpressionShouldBeIgnored()
        {
            var add = (Add) Expression.Parse("'2' + '3' ");
            Assert.That(((Constant) add.Lhs).Value, Is.EqualTo("2"));
            Assert.That(((Constant) add.Rhs).Value, Is.EqualTo("3"));
        }

        [Test]
        public void TestSpacesShouldBeAtTheStartOfAnExpressionShouldBeIgnored()
        {
            var add = (Add) Expression.Parse(" '2' + '3'");
            Assert.That(((Constant) add.Lhs).Value, Is.EqualTo("2"));
            Assert.That(((Constant) add.Rhs).Value, Is.EqualTo("3"));
        }

        [Test]
        public void TestSpacesShouldBeIngnoredSurroundingSeperators()
        {
            var add = (Add) Expression.Parse("'2' + '3'");
            Assert.That(((Constant) add.Lhs).Value, Is.EqualTo("2"));
            Assert.That(((Constant) add.Rhs).Value, Is.EqualTo("3"));
        }

        [Test]
        public void TestSpacesShouldBeTrimmedInProperties()
        {
            Assert.That(((PropertyOrConstant) Expression.Parse("Property ")).Name, Is.EqualTo("Property"));
        }

        [Test]
        public void TestSpacesShouldBeTrimmedInSeperators()
        {
            var add = (Add) Expression.Parse("'2' + '3'");
            Assert.That(((Constant) add.Lhs).Value, Is.EqualTo("2"));
            Assert.That(((Constant) add.Rhs).Value, Is.EqualTo("3"));
        }

        [Test]
        public void TestSpacesShouldNotBeTrimmedInConstants()
        {
            Assert.That(((Constant) Expression.Parse("'2 '")).Value, Is.EqualTo("2 "));
        }

        [Test]
        public void TestStringProperty()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "Name",
                            new Reflection(new SampleModel())),
                        Is.EqualTo("NAME VALUE")
                );
        }

        [Test]
        public void TestSubstractionProperty()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "FirstInt-SecondInt",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(5)
                );
        }

        [Test]
        public void TestThreeAdditions()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "FirstInt+FirstInt+FirstInt",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(126)
                );
        }

        [Test]
        public void TestTwoBracketsInOneExpressionSameLevel()
        {
            Expression exp = Expression.Parse("('14'+'6')/('2'*'5')");
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
            Assert.That(Expression.ParseAndEvaluate(
                            "('14'+'6')/'2'^'2'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(5)
                );
            Assert.That(Expression.ParseAndEvaluate(
                            "(('14'+'6')/'2')",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(10)
                );
            Assert.That(Expression.ParseAndEvaluate(
                            "(('14'+'6')/'2')^'2'",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(100)
                );
        }

        [Test]
        public void Should_Handle_Function_With_Zero_Arguments()
        {
            var reflection = new Reflection(new SampleModel());
            var e = Expression.Parse("fn:now()");
            Assert.That(e.ReturnType, Is.Not.Null);
            Assert.That(e.ReturnType, Is.EqualTo(typeof(DateTime)));

        }

        [Test]
        public void Should_Handle_Functions_With_Zero_Arguments()
        {
            var reflection = new Reflection(new SampleModel());
            var e = Expression.Parse("fn:now() le fn:now()");
            Assert.That(e.ReturnType, Is.Not.Null);
            Assert.That(e.ReturnType, Is.EqualTo(typeof(bool)));

        }

        [Test]
        public void TestMathMaxFunction()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "max('3','2')",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(3)
                );
        }
        [Test]
        public void TestMathMaxMinimalSyntaxFunction()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "max(3,2)",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(3)
                );
        }
        [Test]
        public void TestMathMinMinimalSyntaxFunction()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "min(3,2)",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(2)
                );
        }
        [Test]
        public void TestMathAbsMinimalSyntaxFunction()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "abs('-3')",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(3)
                );
        }
        [Test]
        public void TestMatRoundMinimalSyntaxFunction()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "round(3.2)",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(3)
                );
        }
        [Test]
        public void TestMatFloorMinimalSyntaxFunction()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "floor(3.2)",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(3)
                );
        }
        [Test]
        public void TestMatCeilMinimalSyntaxFunction()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "ceil(3.2)",
                            new Reflection(new SampleModel())),
                        Is.EqualTo(4)
                );
        }
        [Test]
        public void TestMathMaxFunctionWithProperty()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "max(3,FirstInt)",
                            new Reflection(new SampleModel() )),
                        Is.EqualTo(42)
                );
        }
    }
}
