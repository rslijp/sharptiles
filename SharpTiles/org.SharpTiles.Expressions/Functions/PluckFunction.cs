using System;
using System.Collections;
using System.Linq;
using org.SharpTiles.Common;

namespace org.SharpTiles.Expressions.Functions
{
    public class PluckFunction : IFunctionDefinition
    {
        private static readonly FunctionArgument[] ARGUMENTS = new[]
        {
            new FunctionArgument {Type = typeof(IEnumerable), Name = "enumerable"},
            new FunctionArgument {Type = typeof(string), Name = "key"},
        };

        public object Evaluate(params object[] parameters)
        {
            object source = parameters != null ? parameters[0] : null;
            string key = (string)parameters[1];
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
                return linq.Select(
                    item => new Reflection(item)[key]
                ).ToArray();
            }
            throw FunctionEvaluationException.WrongParameter(this, 0, source);
        }

        public string Name => "pluck";

        public FunctionArgument[] Arguments => ARGUMENTS;

        public Type ReturnType => typeof(object[]);
    }
}
