using System;
using org.SharpTiles.Common;

namespace org.SharpTiles.Expressions.Functions
{
    public class AddMonthsFunction : IFunctionDefinition
    {
        private static readonly FunctionArgument[] ARGUMENTS = new[]
        {
            new FunctionArgument{ Type = typeof (DateTime?), Name = "source"},
            new FunctionArgument{ Type = typeof (int?), Name = "months"}
        };


        #region IFunctionDefinition Members

        public string Name => "addMonths";

        public FunctionArgument[] Arguments => ARGUMENTS;

        public Type ReturnType => typeof(DateTime?);

        public object Evaluate(params object[] parameters)
        {
            var source = (DateTime?)parameters[0];
            var months = parameters[1] != null ? (int?)TypeConverter.To(parameters[1], typeof(int)) : null;
            return source != null && months != null ? source.Value.AddMonths(months.Value) : source;
        }

        #endregion
    }
}