using System;
using System.ComponentModel;
using org.SharpTiles.Expressions.Functions;
using TypeConverter = org.SharpTiles.Common.TypeConverter;


namespace org.SharpTiles.Expressions.Math
{
    [Category("MathExpression")]
    public class FloorFunction : IFunctionDefinition
    {
        private static readonly FunctionArgument[] ARGUMENTS = new[]
        {
            new FunctionArgument{ Type = typeof (decimal), Name = "value"},
        };


        #region IFunctionDefinition Members

        public string Name
        {
            get { return "floor"; }
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
            return (int) System.Math.Floor(value);
        }

        #endregion        
    }

}