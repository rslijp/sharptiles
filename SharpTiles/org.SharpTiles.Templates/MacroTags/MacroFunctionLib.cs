using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.SharpTiles.Expressions.Functions;

namespace org.SharpTiles.Templates.MacroTags
{
    public class  MacroFunctionLib : FunctionLib
    {
        public MacroFunctionLib()
        {
            RegisterFunction(new CallFunction());           
        }

        public override string GroupName
        {
            get { return "macro"; }
        }
    }
}
