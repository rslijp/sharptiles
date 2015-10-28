using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Common;
using org.SharpTiles.Expressions.Functions;

namespace org.SharpTiles.Expressions.Test
{
    public class TokenIsolationTest
    {
        
        public class TestMathFunctionLib : FunctionLib
        {
            public TestMathFunctionLib()
            {
                RegisterFunction(new TestRoundFunction());
            }

            public override string GroupName
            {
                get { return "tokenisolation"; }
            }
        }

        public class TestRoundFunction : IFunctionDefinition
        {
            private static readonly FunctionArgument[] ARGUMENTS = new[]
            {
            new FunctionArgument { Type = typeof (decimal), Name = "value"},
            new FunctionArgument { Type = typeof (decimal), Name = "decimals"},
        };


            public string Name
            {
                get { return "round"; }
            }

            public FunctionArgument[] Arguments
            {
                get { return ARGUMENTS; }
            }

            public Type ReturnType
            {
                get { return typeof(decimal); }
            }

            public object Evaluate(params object[] parameters)
            {
                var value = (decimal)TypeConverter.To(parameters[0], typeof(decimal));
                var decimals = (int)TypeConverter.To(parameters[1], typeof(int));
                return System.Math.Round(value, decimals, MidpointRounding.AwayFromZero);
            }

        }

        [SetUp]
        public void SetUp()
        {
            Expression.Clear();
            FunctionLib.Register(new TestMathFunctionLib());
            Expression.Init();
        }

        [TearDown]
        public void TearDown()
        {
            Expression.Clear();
            Expression.Init();
        }

        [Test]
        public void Should_Be_Parsed()
        {
            Assert.That(Expression.ParseAndEvaluate("tokenisolation:round('6.521','2')", new Reflection(new object())),
                Is.EqualTo(6.52m));
        }
    }

   
}
