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

       
        [Test]
        public void Should_Be_Parsed()
        {
            Assert.That(new ExpressionLib(new TestMathFunctionLib()).ParseAndEvaluate("tokenisolation:round('6.521','2')", new Reflection(new object())),
                Is.EqualTo(6.52m));
        }
    }

   
}
