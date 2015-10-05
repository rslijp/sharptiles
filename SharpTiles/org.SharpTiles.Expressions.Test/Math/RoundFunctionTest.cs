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
    public class RoundFunctionTest
    {
        
        [Test]
        public void Evaluate_Should_Return_6_on_input_6()
        {
            Assert.That(new RoundFunction().Evaluate(6m), Is.EqualTo(6m));
        }

        [Test]
        public void Evaluate_Should_Return_6_on_input_6_dot_1()
        {
            Assert.That(new RoundFunction().Evaluate(6.1m), Is.EqualTo(6m));
        }

        [Test]
        public void Evaluate_Should_Return_7_on_input_6_dot_6()
        {
            Assert.That(new RoundFunction().Evaluate(6.6m), Is.EqualTo(7m));
        }

        [Test]
        public void Evaluate_Should_Return_7_on_input_6_dot_66()
        {
            Assert.That(new RoundFunction().Evaluate(6.66m), Is.EqualTo(7m));
        }

       
        [Test]
        public void Evaluate_Should_Return_minus_7_on_input_minus_6_dot_1()
        {
            Assert.That(new RoundFunction().Evaluate(-6.1m), Is.EqualTo(-6m));
        }

        [Test]
        public void Evaluate_Should_Return_minus_7_on_string_input_minus_6_dot_1()
        {
            Assert.That(new RoundFunction().Evaluate((-6.1m).ToString(), 0), Is.EqualTo(-6m));
        }
    }
}
