using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.SharpTiles.Expressions.Functions
{
    public class NowFunction : IFunctionDefinition
    {
        private static readonly FunctionArgument[] ARGUMENTS = { };

        public string Name
        {
            get { return "now"; }
        }

        public FunctionArgument[] Arguments
        {
            get { return ARGUMENTS; }
        }

        public Type ReturnType
        {
            get { return typeof(DateTime); }
        }

        public object Evaluate(params object[] parameters)
        {
            return DateTime.Now;
        }

    }
}
