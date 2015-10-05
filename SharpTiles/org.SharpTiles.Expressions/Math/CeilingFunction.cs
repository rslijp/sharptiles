using System;
using org.SharpTiles.Common;
using org.SharpTiles.Expressions.Functions;

namespace org.SharpTiles.Expressions.Math
{
    public class CeilingFunction : IFunctionDefinition
    {
        private static readonly FunctionArgument[] ARGUMENTS = new[]
        {
            new FunctionArgument{ Type = typeof (decimal), Name = "value"},
        };


        #region IFunctionDefinition Members

        public string Name
        {
            get { return "ceil"; }
        }

        public FunctionArgument[] Arguments
        {
            get { return ARGUMENTS; }
        }

        public Type ReturnType
        {
            get { return typeof(int); }
        }

        public object Evaluate(params object[] parameters)
        {
            var value = (decimal)TypeConverter.To(parameters[0], typeof(decimal));
            return (int) System.Math.Ceiling(value);
        }

        #endregion        
    }

}