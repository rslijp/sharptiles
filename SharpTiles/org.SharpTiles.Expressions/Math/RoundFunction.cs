using System;
using org.SharpTiles.Common;
using org.SharpTiles.Expressions.Functions;


namespace org.SharpTiles.Expressions.Math
{
    public class RoundFunction : IFunctionDefinition
    {
        private static readonly FunctionArgument[] ARGUMENTS = new[]
        {
            new FunctionArgument { Type = typeof (decimal), Name = "value"},
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
            return System.Math.Round(value, 0, MidpointRounding.AwayFromZero);
        }

    }
    
}