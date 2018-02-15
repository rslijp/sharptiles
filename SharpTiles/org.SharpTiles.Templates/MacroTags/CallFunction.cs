using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.SharpTiles.Expressions;
using org.SharpTiles.Expressions.Functions;
using org.SharpTiles.Tags;

namespace org.SharpTiles.Templates.MacroTags
{
    public class CallFunction : IFunctionDefinition
    {
        private static readonly FunctionArgument[] ArgumentsDefinition = {
            new FunctionArgument{ Type = typeof (DefineFunctionTag.FunctionDefinition), Name = "function", Params = false},
            new FunctionArgument{ Type = typeof (object), Name = "args", Params = true}
        };

        public object Evaluate(params object[] parameters)
        {
            var def = parameters.Cast<DefineFunctionTag.FunctionDefinition>().First();
            var args = parameters.Skip(1).ToArray();
            if (args.Length != def.Arguments.Length)
            {
                throw MacroException.ArgumentIndexOutOfBounds(args.Length, def.Arguments.Length);
            }
            var model = new Dictionary<string, object>();
            for (var i=0; i<def.Arguments.Length; i++)
            {
                var argument = def.Arguments[i];
                model[argument] = args[i];
            }
            return def.Evaluate(new TagModel(model));
        }

        public string Name => "call";

        public FunctionArgument[] Arguments => ArgumentsDefinition;
        public Type ReturnType => typeof(object);

    }
}
