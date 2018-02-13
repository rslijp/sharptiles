using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.SharpTiles.Expressions.Functions;

namespace org.SharpTiles.Expressions.Functions
{
    public static class FunctionArgumentsExtensions
    {
        public static bool IsParamsFunctions(this IFunctionDefinition func)
        {
            return IsParamsFunctions(func.Arguments);
        }

        public static bool IsParamsFunctions(this Function func)
        {
            return IsParamsFunctions(func.Arguments);
        }


        public static bool IsParamsFunctions(this FunctionArgument[] arguments)
        {
            return arguments.Length >= 1 && arguments.Last().Params;
//            return arguments.Length == 1 && arguments[0].Params;
        }
    }
}
