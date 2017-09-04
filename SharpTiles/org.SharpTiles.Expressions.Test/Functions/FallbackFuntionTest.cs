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
    public class FallbackFunctionTest
    {
        public class FallbackSubject {
            public string A { get; set; }
            public string B { get; set; }
            public string C { get; set; }
        }

        [Test]
        public void FallBackTo_First()
        {
            var fb = new BaseFunctionLib().Obtain("fallback");
            var brackets = new Brackets(true,3);
            brackets.Nodes.Add(new Property("A"));
            brackets.Nodes.Add(new Property("B"));
            brackets.Nodes.Add(new Property("C"));
            fb.FillNested(brackets);
            Assert.That(fb.Evaluate(new Reflection(new FallbackSubject {A="A"})), Is.EqualTo("A"));
        }

        [Test]
        public void FallBackTo_Second()
        {
            var fb = new BaseFunctionLib().Obtain("fallback");
            var brackets = new Brackets(true, 3);
            brackets.Nodes.Add(new Property("A"));
            brackets.Nodes.Add(new Property("B"));
            brackets.Nodes.Add(new Property("C"));
            fb.FillNested(brackets);
            Assert.That(fb.Evaluate(new Reflection(new FallbackSubject { B = "B", C="C" })), Is.EqualTo("B"));
        }

        [Test]
        public void FallBackTo_Third()
        {
            var fb = new BaseFunctionLib().Obtain("fallback");
            var brackets = new Brackets(true, 3);
            brackets.Nodes.Add(new Property("A"));
            brackets.Nodes.Add(new Property("B"));
            brackets.Nodes.Add(new Property("C"));
            fb.FillNested(brackets);
            Assert.That(fb.Evaluate(new Reflection(new FallbackSubject { C = "C" })), Is.EqualTo("C"));
        }


        [Test]
        public void FallBackTo_Null_Is_All_Else_Fails()
        {
            var fb = new BaseFunctionLib().Obtain("fallback");
            var brackets = new Brackets(true, 3);
            brackets.Nodes.Add(new Property("A"));
            brackets.Nodes.Add(new Property("B"));
            brackets.Nodes.Add(new Property("C"));
            fb.FillNested(brackets);
            Assert.That(fb.Evaluate(new Reflection(new FallbackSubject())), Is.Null);
        }

        [Test]
        public void ParseOfParams()
        {
            Assert.That(
                new ExpressionLib().ParseAndEvaluate("fn:fallback(A,B,C)", new Reflection(new FallbackSubject { C = "C" })),
                Is.EqualTo("C"));
        }

        [Test]
        public void ParseOfOneParam()
        {
            Assert.That(
                new ExpressionLib().ParseAndEvaluate("fn:fallback(A)", new Reflection(new FallbackSubject { C = "C" })),
                Is.Null);
        }

        [Test]
        public void ParseOfOneParamWithConstant()
        {
            Assert.That(
                new ExpressionLib().ParseAndEvaluate("fn:fallback(A,'constant')", new Reflection(new FallbackSubject { C = "C" })),
                Is.EqualTo("constant"));
        }

        [Test]
        public void ParseNoParams()
        {
            Assert.That(
                new ExpressionLib().ParseAndEvaluate("fn:fallback()", new Reflection(new FallbackSubject { C = "C" })),
                Is.Null);
        }
        [Test]
        public void ParseOfALotOfParams()
        {
            Assert.That(
                new ExpressionLib().ParseAndEvaluate("fn:fallback(A,B,C,A,B,C,A,B,C,A,B,C,A,B,C,A,B,C)", new Reflection(new FallbackSubject { C = "C" })),
                Is.EqualTo("C"));
        }
    }
}
