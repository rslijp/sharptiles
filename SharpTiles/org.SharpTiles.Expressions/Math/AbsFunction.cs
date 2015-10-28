using System;
using System.ComponentModel;
using org.SharpTiles.Expressions.Functions;
using TypeConverter = org.SharpTiles.Common.TypeConverter;

namespace org.SharpTiles.Expressions.Math
{
    [Category("MathExpression")]
    public class AbsFunction : IFunctionDefinition
    {
        private static readonly FunctionArgument[] ARGUMENTS = new[]
        {
            new FunctionArgument {Type = typeof (decimal), Name = "value"}
        };


        public string Name
        {
            get { return "abs"; }
        }

        public FunctionArgument[] Arguments
        {
            get { return ARGUMENTS; }
        }

        public Type ReturnType
        {
            get { return typeof (decimal); }
        }

        public object Evaluate(params object[] parameters)
        {
            var value = (decimal) TypeConverter.To(parameters[0], typeof (decimal));
            return System.Math.Abs(value);
        }
    }

}