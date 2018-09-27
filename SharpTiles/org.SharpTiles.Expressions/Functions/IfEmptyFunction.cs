using System;
using System.Collections;

namespace org.SharpTiles.Expressions.Functions
{
    public class IfEmptyFunction : IFunctionDefinition
    {
        private static readonly FunctionArgument[]  ARGUMENTS =
        {
            new FunctionArgument { Type = typeof (object), Name = "source"},
            new FunctionArgument { Type = typeof (object), Name = "replacement"},
        };

        public string Name => "ifEmpty";

        public FunctionArgument[] Arguments => ARGUMENTS;

        public Type ReturnType => typeof(IList);

        public object Evaluate(params object[] parameters)
        {
            return EmptyFunction.IsEmpty(parameters[0]) ? parameters[1] : parameters[0];
        }
    }
}