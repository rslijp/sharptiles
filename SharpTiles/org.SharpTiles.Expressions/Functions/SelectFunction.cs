using System;
using System.Collections;
using System.Linq;
using org.SharpTiles.Common;

namespace org.SharpTiles.Expressions.Functions
{
    public class SelectFunction : IFunctionDefinition
    {
        private static readonly FunctionArgument[] ARGUMENTS = new[]
        {
            new FunctionArgument {Type = typeof(IEnumerable), Name = "enumerable"},
            new FunctionArgument {Type = typeof(string), Name = "key"},
            new FunctionArgument {Type = typeof(object), Name = "value"},
        };

        public object Evaluate(params object[] parameters)
        {
            object source = parameters != null ? parameters[0] : null;
            string key = (string)parameters[1];
            object condition = parameters[2];
            if (source == null)
            {
                return null;
            }
            if (key == null)
            {
                throw FunctionEvaluationException.WrongParameter(this, 1, source);

            }
            if (source is IEnumerable)
            {
                var linq = ((IEnumerable)source).Cast<object>();
                return linq.Where(
                    item =>
                    {
                        var t = new Reflection(item)[key];
                        if (t == null && condition == null) return true;
                        return Equals(TypeConverter.To(t, condition.GetType()), condition);
                    }
                ).ToArray();
            }
            throw FunctionEvaluationException.WrongParameter(this, 0, source);
        }

        public string Name => "select";

        public FunctionArgument[] Arguments => ARGUMENTS;

        public Type ReturnType => typeof(object[]);
    }
}
