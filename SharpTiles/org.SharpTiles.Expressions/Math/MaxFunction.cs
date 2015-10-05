using System;
using org.SharpTiles.Common;
using org.SharpTiles.Expressions.Functions;

namespace org.SharpTiles.Expressions.Math
{
    public class MaxFunction : IFunctionDefinition
    {
        private static readonly FunctionArgument[] ARGUMENTS = new[]
        {
            new FunctionArgument { Type = typeof (decimal), Name = "lhs"},
            new FunctionArgument { Type = typeof (decimal), Name = "rhs"},
        };

        public string Name
        {
            get { return "max"; }
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
            var lhs = (decimal)TypeConverter.To(parameters[0], typeof(decimal));
            var rhs = (decimal)TypeConverter.To(parameters[1], typeof(decimal));
            return System.Math.Max(lhs, rhs);
        }

    }

}