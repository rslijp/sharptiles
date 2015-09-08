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
 using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Common;
 using org.SharpTiles.Expressions.Functions;

namespace org.SharpTiles.Expressions.Test
{
    [TestFixture]
    public class EmptyFunctionTest
    {
        /*
         * True if the operand is null, an empty String,
         * empty array, empty Map, or empty List; false,
         * otherwise.
         */

        public object NullProperty
        {
            get { return null; }
        }

        public object NotNullProperty
        {
            get { return new object(); }
        }

        public string EmptyString
        {
            get { return ""; }
        }

        public string NonEmptyString
        {
            get { return "abc"; }
        }

        public object[] EmptyArray
        {
            get { return new object[0]; }
        }

        public object[] NonEmptyArray
        {
            get { return new object[1]; }
        }

        public IList<object> EmptyList
        {
            get { return new List<object>(); }
        }

        public IList<object> NonEmptyList
        {
            get
            {
                var list = new List<object>();
                list.Add(new object());
                return list;
            }
        }

        [Test]
        public void OperandIsEmptyArray()
        {
            var empty = new BaseFunctionLib().Obtain("empty");
            empty.FillNested(new Brackets(new Property("EmptyArray")));
            Assert.That(empty.Evaluate(new Reflection(this)), Is.True);
        }

        [Test]
        public void OperandIsEmptyList()
        {
            var empty = new BaseFunctionLib().Obtain("empty");
            empty.FillNested(new Brackets(new Property("EmptyList")));
            Assert.That(empty.Evaluate(new Reflection(this)), Is.True);
        }

        [Test]
        public void OperandIsEmptyString()
        {
            var empty = new BaseFunctionLib().Obtain("empty");
            empty.FillNested(new Brackets(new Property("EmptyString")));
            Assert.That(empty.Evaluate(new Reflection(this)), Is.True);
        }

        [Test]
        public void OperandIsFilledArray()
        {
            var empty = new BaseFunctionLib().Obtain("empty");
            empty.FillNested(new Brackets(new Property("NonEmptyArray")));
            Assert.That(empty.Evaluate(new Reflection(this)), Is.False);
        }

        [Test]
        public void OperandIsFilledList()
        {
            var empty = new BaseFunctionLib().Obtain("empty");
            empty.FillNested(new Brackets(new Property("NonEmptyList")));
            Assert.That(empty.Evaluate(new Reflection(this)), Is.False);
        }

        [Test]
        public void OperandIsFilledString()
        {
            var empty = new BaseFunctionLib().Obtain("empty");
            empty.FillNested(new Brackets(new Property("NonEmptyString")));
            Assert.That(empty.Evaluate(new Reflection(this)), Is.False);
        }

        [Test]
        public void OperandIsNotNull()
        {
            var empty = new BaseFunctionLib().Obtain("empty");
            empty.FillNested(new Brackets(new Property("NotNullProperty")));
            Assert.That(empty.Evaluate(new Reflection(this)), Is.False);
        }

        [Test]
        public void OperandIsNull()
        {
            var empty = new BaseFunctionLib().Obtain("empty");
            empty.FillNested(new Brackets(new Property("NullProperty")));
            Assert.That(empty.Evaluate(new Reflection(this)), Is.True);
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
            Assert.IsTrue(brackets.Nested is Property);
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
            Assert.IsTrue(bracketsLhs.Nested is Property);

            var emptyRhs = (Function) and.Rhs;
            Assert.IsTrue(emptyLhs.Nested is Brackets);

            var bracketsRhs = (Brackets) emptyRhs.Nested;
            Assert.IsTrue(bracketsRhs.Nested is Property);
        }
    }
}
