using System;
using System.Collections.Generic;
using org.SharpTiles.Common;
using org.SharpTiles.Expressions;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Templates.MacroTags
{
    public class CallMacroTag : BaseCoreTagWithFreeFields, ITag
    {
        public static readonly string NAME = "call";

        #region ITag Members

        public string TagName
        {
            get { return NAME; }
        }

        [Required]
        public ITagAttribute Name { get; set; }
        

        public TagBodyMode TagBodyMode => TagBodyMode.Free;

        public string Evaluate(TagModel model)
        {
            var id = GetAutoValueAsString(nameof(Name), model);
            if (id == null)
            {
                throw ExpressionParseException.UnexpectedNullValue(nameof(Name)).Decorate(Name.Context);
            }
            id = LanguageHelper.CamelCaseAttribute(id);
            var raw = model.TryGet(id);
            if (raw == null)
            {
                throw MacroException.NotFound(id).Decorate(Name.Context);
            }
            var macro = raw as DefineMacroTag.MarcoDefinition;
            var function = raw as DefineFunctionTag.FunctionDefinition;
            if (macro == null && function==null)
            {
                throw MacroException.NoMacroOrFunction(id).Decorate(Name.Context);
            }
            if (macro != null)
            {
                return ExecuteMacro(model, id, macro);
            }
            return ExecuteFunction(model, function);
        }

        private string ExecuteFunction(TagModel model, DefineFunctionTag.FunctionDefinition function)
        {
            var callArguments = new Dictionary<string,object>();
            var freeFields = FreeFields(model);
            foreach (var argumentWithDefault in function.Arguments)
            {
                var argument = argumentWithDefault;
                var defaultPath = null as string;
                if (argumentWithDefault.Contains("(") && argumentWithDefault.EndsWith(")"))
                {
                    var split = argumentWithDefault.IndexOf("(", StringComparison.InvariantCulture);
                    argument = argumentWithDefault.Substring(0, split);
                    defaultPath = argumentWithDefault.Substring(split + 1, argumentWithDefault.Length-split-2);
                    // Console.WriteLine(defaultPath);
                }
                var value = CollectionUtils.SafeGet(freeFields,argument) ?? CollectionUtils.SafeGet(freeFields, LanguageHelper.CamelCaseProperty(argument));
                if (!string.IsNullOrEmpty(defaultPath) && value == null)
                {
                    value=model.TryGet(defaultPath);
                }
                if (function.IsStrict && value==null)
                {
                    throw MacroException.NullNotAllowed(argument).Decorate(Name.Context);
                }
                callArguments[argument] = value;
//                model.Tag[argument] = value;
            }
            model.PushTagStack();
            foreach (var argument in callArguments)
            {
                model.Tag[argument.Key] =argument.Value;
            }
            var result = function.Evaluate(model);
            model.PopTagStack();
            return TypeConverter.To<string>(result);
        }

        private string ExecuteMacro(TagModel model, string id, DefineMacroTag.MarcoDefinition macro)
        {
            if (FreeAttribs.Count > 0)
            {
                throw MacroException.AgrumentsNotAllowed(id).Decorate(Context);
            }
            return macro.Evaluate(model);
        }

        #endregion

      
    }
}
