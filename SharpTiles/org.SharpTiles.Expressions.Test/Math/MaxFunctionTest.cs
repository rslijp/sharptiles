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
    public class MaxFunctionTest
    {
        
        [Test]
        public void Evaluate_Should_Return_7_dot_2_on_input_6_dot_1_and_7_dot_2()
        {
            Assert.That(new MaxFunction().Evaluate(6.1m, 7.2m), Is.EqualTo(7.2m));
        }

        [Test]
        public void Evaluate_Should_Return_7_dot_2_on_string_input_6_dot_1_and_7_dot_2()
        {
            Assert.That(new MaxFunction().Evaluate(6.1m.ToString(), 7.2m.ToString()), Is.EqualTo(7.2m));
        }
    }
}
