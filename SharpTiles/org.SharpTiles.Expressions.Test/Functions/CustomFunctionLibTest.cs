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
 */using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Common;
using org.SharpTiles.Expressions;
using org.SharpTiles.Expressions.Functions;

namespace org.SharpTiles.Expressions.Test.Functions
{
    [TestFixture]
    public class CustomFunctionLibTest
    {

        [Test]
        public void Should_Reject_Faulthy_Params_Function()
        {
            var lib = new MathLib();
            try
            {
                lib.PublicRegisterFunction(new MathLib.WhoopsyFunction());
            }
            catch (Exception e)
            {
                Assert.That(e.Message, Is.EqualTo("Only last argument of a function can be a params argument. Argument whoopsy of whoopsy is in violation."));
            }
        }

        [Test]
        public void TestTrimNested()
        {
            Assert.That(new ExpressionLib(new MathLib()).ParseAndEvaluate("math:fibonacci('8')", new Reflection(this)),
                        Is.EqualTo(34));
            Assert.That(new ExpressionLib(new MathLib()).ParseAndEvaluate("math:faculty('8')", new Reflection(this)),
                        Is.EqualTo(40320));
        }
    }
}


public class MathLib : FunctionLib
{
    public MathLib()
    {
        RegisterFunction(new FacultyFunction());
        RegisterFunction(new FibonacciFunction());
    }

    public void PublicRegisterFunction(IFunctionDefinition function)
    {
        RegisterFunction(function);
    }

    public override string GroupName
    {
        get { return "math"; }
    }

    public class WhoopsyFunction : IFunctionDefinition
    {
        private static readonly FunctionArgument[] ARGUMENTS = new[]
        {
            new FunctionArgument{ Type = typeof (decimal), Name = "whoopsy",Params = true},
            new FunctionArgument{ Type = typeof (decimal), Name = "number" }
        };

        public object Evaluate(params object[] parameters)
        {
            return null;
        }

      
        public string Name => "whoopsy";

        public FunctionArgument[] Arguments => ARGUMENTS;

        public Type ReturnType => typeof(decimal);
    }

    public class FibonacciFunction : IFunctionDefinition
    {
        private static readonly FunctionArgument[] ARGUMENTS = new[]
                             {
                                 new FunctionArgument{ Type = typeof (decimal), Name = "number" }
                             };

        public object Evaluate(params object[] parameters)
        {
            int seed = (int)TypeConverter.To(parameters[0], typeof(int));

            return fib(seed);
        }

        private int fib(int seed)
        {
            if (seed == 0 || seed == 1) return 1;
            return fib(seed - 1) + fib(seed - 2);
        }

        public string Name
        {
            get { return "fibonacci"; }
        }

        public FunctionArgument[] Arguments
        {
            get { return ARGUMENTS; }
        }

        public Type ReturnType
        {
            get { return typeof(decimal); }
        }
    }

    public class FacultyFunction : IFunctionDefinition
    {
        private static readonly FunctionArgument[] ARGUMENTS = new[]
                             {
                                 new FunctionArgument{ Type = typeof (decimal), Name = "number" }
                             };

        public object Evaluate(params object[] parameters)
        {
            int seed = (int)TypeConverter.To(parameters[0], typeof(int));
            
            return fac(seed);
        }

        private int fac(int seed)
        {
            if (seed == 1) return 1;
            return seed * fac(seed - 1);
        }

        public string Name
        {
            get { return "faculty"; }
        }

        public FunctionArgument[] Arguments
        {
            get { return ARGUMENTS; }
        }

        public Type ReturnType
        {
            get { return typeof(decimal); }
        }
    }


}
