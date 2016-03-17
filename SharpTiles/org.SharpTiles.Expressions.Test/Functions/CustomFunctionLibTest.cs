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
        static CustomFunctionLibTest(){ 
            new PatchedExpressions().ReInit(); //Hack
        }

        [Test]
        public void TestTrimNested()
        {
            Assert.That(Expression.ParseAndEvaluate("math:fibonacci('8')", new Reflection(this)),
                        Is.EqualTo(34));
            Assert.That(Expression.ParseAndEvaluate("math:faculty('8')", new Reflection(this)),
                        Is.EqualTo(40320));
        }
    }
}

public class PatchedExpressions : Expression
{
    public void ReInit()
    {
        Clear();
        FunctionLib.Register(new MathLib());
        Init();
    }

    public override void GuardTypeSafety()
    {
        
    }

    public override Type ReturnType
    {
        get { throw new System.NotImplementedException(); }
    }

    public override object Evaluate(IModel model)
    {
        throw new System.NotImplementedException();
    }

    public override string AsParsable()
    {
        throw new NotImplementedException();
    }
}

public class MathLib : FunctionLib
{
    public MathLib()
    {
        RegisterFunction(new FacultyFunction());
        RegisterFunction(new FibonacciFunction());
    }


    public override string GroupName
    {
        get { return "math"; }
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
