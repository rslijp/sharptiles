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
 using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Common;
 using org.SharpTiles.Expressions.Functions;

namespace org.SharpTiles.Expressions.Test
{
    [TestFixture]
    public class FunctionTest
    {
        public string Text
        {
            get { return "text"; }
        }

        public string SomeText
        {
            get { return "some text"; }
        }

        public object[] EmptyArray
        {
            get { return new object[0]; }
        }

        public object[] NonEmptyArray
        {
            get { return new object[1]; }
        }

        public string[] FilledString
        {
            get { return new[] {"one", "two", "three"}; }
        }

        public string Seperator
        {
            get { return "+"; }
        }

        public string HTML
        {
            get { return "<html><body>Hi 'Renzo'</body></html>"; }
        }

        public string HTMLEscaped
        {
            get { return "&lt;html&gt;&lt;body&gt;Hi &apos;Renzo&apos;&lt;/body&gt;&lt;/html&gt;"; }
        }

        [Test]
        public void TestChechkParsingWithArithmeticBothSide()
        {
            Expression exp = Expression.Parse("fn:length('aaa')+fn:length('aaa')");
            Assert.IsTrue(exp is Add);

            var add = (Add) exp;
            Assert.IsTrue(add.Lhs is Function);
            Assert.IsTrue(add.Rhs is Function);

            var funcLhs = (Function) add.Lhs;
            Assert.IsTrue(funcLhs.Nested is Brackets);

            var bracketsLhs = (Brackets) funcLhs.Nested;
            Assert.IsTrue(bracketsLhs.Nested is Constant);

            var funcRhs = (Function) add.Rhs;
            Assert.IsTrue(funcRhs.Nested is Brackets);

            var bracketsRhs = (Brackets) funcRhs.Nested;
            Assert.IsTrue(bracketsRhs.Nested is Constant);
        }

        [Test]
        public void TestChechkParsingWithArithmeticLeftHandSide()
        {
            Expression exp = Expression.Parse("'3'+fn:length('aaa')");
            Assert.IsTrue(exp is Add);

            var add = (Add) exp;
            Assert.IsTrue(add.Lhs is Constant);
            Assert.IsTrue(add.Rhs is Function);

            var func = (Function) add.Rhs;
            Assert.IsTrue(func.Nested is Brackets);

            var brackets = (Brackets) func.Nested;
            Assert.IsTrue(brackets.Nested is Constant);
        }


        [Test]
        public void TestChechkParsingWithArithmeticRightHandSide()
        {
            Expression exp = Expression.Parse("fn:length('aaa')+'3'");
            Assert.IsTrue(exp is Add);

            var add = (Add) exp;
            Assert.IsTrue(add.Lhs is Function);
            Assert.IsTrue(add.Rhs is Constant);

            var func = (Function) add.Lhs;
            Assert.IsTrue(func.Nested is Brackets);

            var brackets = (Brackets) func.Nested;
            Assert.IsTrue(brackets.Nested is Constant);
        }

        [Test]
        public void TestConcat()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:concat('abcd','ef')", new Reflection(this)),
                        Is.EqualTo("abcdef"));
        }

        [Test]
        public void TestUnkownFunction()
        {
            try
            {
                Expression.ParseAndEvaluate("fn:unkown('abcd','ef')", new Reflection(this));
                Assert.Fail("Expected exception");
            } catch (FunctionEvaluationException FEe)
            {
                Assert.That(FEe.MessageWithOutContext, Is.EqualTo(FunctionEvaluationException.UnkownFunction("fn:unkown('abcd','ef')").Message));
            }
        }

        [Test]
        public void TestContainsAndNestedJoin()
        {
            Assert.That(
                Expression.ParseAndEvaluate("fn:contains(fn:join(FilledString, Seperator), '-')", new Reflection(this)),
                Is.EqualTo(false));
            Assert.That(
                Expression.ParseAndEvaluate("fn:contains(fn:join(FilledString, Seperator), '+')", new Reflection(this)),
                Is.EqualTo(true));
        }

        [Test]
        public void TestContainsIgnoreCaseOnConstant()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:containsIgnoreCase('ABCDEF', 'd')", new Reflection(this)),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestContainsIgnoreCaseOnPropertySource()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:containsIgnoreCase(SomeText, 'TEXT')", new Reflection(this)),
                        Is.EqualTo(true));
        }


        [Test]
        public void TestContainsOnConstant()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:contains('abcdefg', 'd')", new Reflection(this)),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestContainsOnPropertySource()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:contains(SomeText, 'text')", new Reflection(this)),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestContainsOnPropertySourceAndPropertySubstring()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:contains(SomeText, Text)", new Reflection(this)),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestDeepNested()
        {
            Assert.That(
                Expression.ParseAndEvaluate(
                    "fn:concat(fn:concat(fn:concat('a','b'),fn:concat('c','d')),fn:concat('e','f'))",
                    new Reflection(this)),
                Is.EqualTo("abcdef"));
        }

        [Test]
        public void TestEmptyFunctionEmptWithBooleanExpressionBothSides()
        {
            Assert.That(
                Expression.ParseAndEvaluate("fn:empty(EmptyArray) && fn:empty(EmptyArray)", new Reflection(this)),
                Is.EqualTo(true));
        }

        [Test]
        public void TestEmptyFunctionEmptWithBooleanExpressionLeftHandSide()
        {
            Assert.That(Expression.ParseAndEvaluate("'true' && fn:empty(EmptyArray)", new Reflection(this)),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestEmptyFunctionEmptWithBooleanExpressionLeftRightSide()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:empty(EmptyArray) && 'true'", new Reflection(this)),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestEmptyFunctionEmptyArrayProperty()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:empty(EmptyArray)", new Reflection(this)),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestEmptyFunctionEmptyConstant()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:empty('')", new Reflection(this)),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestEmptyFunctionOnNonEmptyArrayProperty()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:empty(NonEmptyArray)", new Reflection(this)),
                        Is.EqualTo(false));
        }

        [Test]
        public void TestEmptyFunctionOnNonEmptyConstant()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:empty('a')", new Reflection(this)),
                        Is.EqualTo(false));
        }

        [Test]
        public void TestEndsWith()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:endsWith('abcdef', 'ef')", new Reflection(this)),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestEndsWithNotAtEnd()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:endsWith('abcdefg', 'ef')", new Reflection(this)),
                        Is.EqualTo(false));
        }

        [Test]
        public void TestEscapeXml()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:escapeXml(HTML)", new Reflection(this)),
                        Is.EqualTo(HTMLEscaped));
        }

        [Test]
        public void TestEscapeXmlOfPlainString()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:escapeXml('Text')", new Reflection(this)),
                        Is.EqualTo("Text"));
        }

        [Test]
        public void TestJoinAfterSplit()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:join(fn:split('a-b-c-d-e-f', '-'), '-')", new Reflection(this)),
                        Is.EqualTo("a-b-c-d-e-f"));
        }

        [Test]
        public void TestJoinOnPropertiesAndConstant()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:join(FilledString, '-')", new Reflection(this)),
                        Is.EqualTo("one-two-three"));
        }

        [Test]
        public void TestJoinOnTwoProperties()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:join(FilledString, Seperator)", new Reflection(this)),
                        Is.EqualTo("one+two+three"));
        }

        [Test]
        public void TestLengthOfEmptyConstant()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:length('')", new Reflection(this)),
                        Is.EqualTo(0));
        }

        [Test]
        public void TestLengthOfEmptyOneConstant()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:length('a')", new Reflection(this)),
                        Is.EqualTo(1));
        }

        [Test]
        public void TestLengthOfEmptyTwoLetterConstant()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:length('ab')", new Reflection(this)),
                        Is.EqualTo(2));
        }

        [Test]
        public void TestNestedFormula()
        {
            Assert.That(
                Expression.ParseAndEvaluate("fn:length(HTML)<fn:length(fn:escapeXml(HTML))", new Reflection(this)),
                Is.EqualTo(true));
        }

        [Test]
        public void TestNotContainsOnConstant()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:contains('abcefg', 'd')", new Reflection(this)),
                        Is.EqualTo(false));
        }

        [Test]
        public void TestNotEnoughArguments()
        {
            try
            {
                Expression.ParseAndEvaluate("fn:concat('abcd')", new Reflection(this));
                Assert.Fail("Expected exception");
            }
            catch (ExpressionParseException EPe)
            {
                Assert.That(EPe.MessageWithOutContext,
                            Is.EqualTo(
                                ExpressionParseException.ExpectedMoreParameter(new BaseFunctionLib().Obtain("concat"), 1, 2).Message));
            }
        }

        [Test]
        public void TestParseLengthFunction()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "fn:length('aaa')",
                            new Reflection(this)), Is.EqualTo(3)
                );
        }

        [Test]
        public void TestParseLengthFunctionOnBothSides()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "fn:length('aaa')+fn:length('aaa')",
                            new Reflection(this)), Is.EqualTo(6)
                );
        }

        [Test]
        public void TestParseLengthFunctionWithArithmeticLeftHandSide()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "'3'+fn:length('aaa')",
                            new Reflection(this)), Is.EqualTo(6)
                );
        }

        [Test]
        public void TestParseLengthFunctionWithArithmeticRightHandSide()
        {
            Assert.That(Expression.ParseAndEvaluate(
                            "fn:length('aaa')+'3'",
                            new Reflection(this)), Is.EqualTo(6)
                );
        }

        [Test]
        public void TestParsingEmptyFunctionEmptWithBooleanExpressionLeftRightSide()
        {
            Expression exp = Expression.Parse("fn:empty(EmptyArray) && 'true'");

            Assert.IsTrue(exp is And);

            var and = (And) exp;
            Assert.IsTrue(and.Lhs is Function);
            Assert.IsTrue(and.Rhs is Constant);

            var empty = (Function) and.Lhs;
            Assert.IsTrue(empty.Nested is Brackets);

            var brackets = (Brackets) empty.Nested;
            Assert.IsTrue(brackets.Nested is PropertyOrConstant);
        }

        [Test]
        public void TestParsingsEmptyFunctionEmptWithBooleanExpressionBothSides()
        {
            Expression exp = Expression.Parse("fn:empty(EmptyArray) && fn:empty(EmptyArray)");
            Assert.IsTrue(exp is And);

            var and = (And) exp;
            Assert.IsTrue(and.Lhs is Function);
            Assert.IsTrue(and.Rhs is Function);

            var emptyLhs = (Function) and.Lhs;
            Assert.IsTrue(emptyLhs.Nested is Brackets);

            var bracketsLhs = (Brackets) emptyLhs.Nested;
            Assert.IsTrue(bracketsLhs.Nested is PropertyOrConstant);

            var emptyRhs = (Function) and.Rhs;
            Assert.IsTrue(emptyLhs.Nested is Brackets);

            var bracketsRhs = (Brackets) emptyRhs.Nested;
            Assert.IsTrue(bracketsRhs.Nested is PropertyOrConstant);
        }

        [Test]
        public void TestReplace()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:replace('a-b-c-d-e-f', '-', '')", new Reflection(this)),
                        Is.EqualTo("abcdef"));
        }

        [Test]
        public void TestReplaceBachAndForth()
        {
            Assert.That(
                Expression.ParseAndEvaluate("fn:replace(fn:replace('a-b-c-d-e-f', '-', '+'), '+', '-')",
                                            new Reflection(this)),
                Is.EqualTo("a-b-c-d-e-f"));
        }

        [Test]
        public void TestSplit()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:split('a-b-c-d-e-f', '-')", new Reflection(this)),
                        Is.EqualTo(new[] {"a", "b", "c", "d", "e", "f"}));
        }

        [Test]
        public void TestSplitAfterJoin()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:split(fn:join(FilledString, '-'), '-')", new Reflection(this)),
                        Is.EqualTo(FilledString));
        }

        [Test]
        public void TestStartsWith()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:startsWith('bcdef', 'bc')", new Reflection(this)),
                        Is.EqualTo(true));
        }

        [Test]
        public void TestStartsWithNotAtEnd()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:startsWith('abcdefg', 'bc')", new Reflection(this)),
                        Is.EqualTo(false));
        }

        [Test]
        public void TestSubString()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:substring('abcdef', '2', '4')", new Reflection(this)),
                        Is.EqualTo("cd"));
        }

        [Test]
        public void TestSubStringAfter()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:substringAfter('abcdef', 'd')", new Reflection(this)),
                        Is.EqualTo("ef"));
        }

        [Test]
        public void TestSubStringBefore()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:substringBefore('abcdef', 'd')", new Reflection(this)),
                        Is.EqualTo("abc"));
        }

        [Test]
        public void TestSubStringsNested()
        {
            Assert.That(
                Expression.ParseAndEvaluate(
                    "fn:substring(fn:concat(fn:substringBefore('abcdef', 'd'), fn:substringAfter('abcdef', 'd')), '2', '4')",
                    new Reflection(this)),
                Is.EqualTo("ce"));
        }

        [Test]
        public void TestToLower()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:toLowerCase('ABCDEF')", new Reflection(this)),
                        Is.EqualTo("abcdef"));
        }

        [Test]
        public void TestToLowerNested()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:contains('ABCDEFG', 'abc')", new Reflection(this)),
                        Is.EqualTo(false));
            Assert.That(
                Expression.ParseAndEvaluate("fn:contains(fn:toLowerCase('ABCDEFG'), 'abc')", new Reflection(this)),
                Is.EqualTo(true));
        }

        [Test]
        public void TestTooManyArguments()
        {
            try
            {
                Expression.ParseAndEvaluate("fn:concat('abcd','ef', 'oops one extra')", new Reflection(this));
                Assert.Fail("Expected exception");
            }
            catch (ExpressionParseException EPe)
            {
                Assert.That(EPe.MessageWithOutContext,
                            Is.EqualTo(
                                ExpressionParseException.UnExpectedParameter(new Constant("oops one extra")).Message));
            }
        }

        [Test]
        public void TestToUpper()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:toUpperCase('abcdef')", new Reflection(this)),
                        Is.EqualTo("ABCDEF"));
        }

        [Test]
        public void TestToUpperNested()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:contains('abcdefg', 'ABC')", new Reflection(this)),
                        Is.EqualTo(false));
            Assert.That(
                Expression.ParseAndEvaluate("fn:contains(fn:toUpperCase('abcdefg'), 'ABC')", new Reflection(this)),
                Is.EqualTo(true));
        }

        [Test]
        public void TestTrim()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:trim(' abcdef ')", new Reflection(this)),
                        Is.EqualTo("abcdef"));
        }

        [Test]
        public void TestTrimNested()
        {
            Assert.That(Expression.ParseAndEvaluate("fn:length(' to trim ') eq '7'", new Reflection(this)),
                        Is.EqualTo(false));
            Assert.That(Expression.ParseAndEvaluate("fn:length(fn:trim(' to trim ')) eq '7'", new Reflection(this)),
                        Is.EqualTo(true));
        }
    }
}
