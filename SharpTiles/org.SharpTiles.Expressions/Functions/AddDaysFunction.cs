using System;
using org.SharpTiles.Common;

namespace org.SharpTiles.Expressions.Functions
{
    public class AddDaysFunction : IFunctionDefinition
    {
        private static readonly FunctionArgument[] ARGUMENTS = new[]
        {
            new FunctionArgument{ Type = typeof (DateTime?), Name = "source"},
            new FunctionArgument{ Type = typeof (double?), Name = "days"}
        };


        #region IFunctionDefinition Members

        public string Name => "addDays";

        public FunctionArgument[] Arguments => ARGUMENTS;

        public Type ReturnType => typeof(DateTime?);

        public object Evaluate(params object[] parameters)
        {
            var source = (DateTime?)parameters[0];
            var days = parameters[1] != null ? (double?)TypeConverter.To(parameters[1], typeof(double)) : null;
            return source != null && days != null ? source.Value.AddDays(days.Value) : source;
        }

        #endregion
    }
}