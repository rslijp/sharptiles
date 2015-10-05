using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Expressions.Math;

namespace org.SharpTiles.Expressions.Test.Math
{
    public class AbsFunctionTest
    {
        
        [Test]
        public void Evaluate_Should_Return_6_on_input_6()
        {
            Assert.That(new AbsFunction().Evaluate(6), Is.EqualTo(6m));
        }

        [Test]
        public void Evaluate_Should_Return_6_on_input_minus_6()
        {
            Assert.That(new AbsFunction().Evaluate(-6), Is.EqualTo(6m));
        }

        [Test]
        public void Evaluate_Should_Return_6_on_string_input_minus_6()
        {
            Assert.That(new AbsFunction().Evaluate((-6).ToString()), Is.EqualTo(6m));
        }
    }
}
