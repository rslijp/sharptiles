using System;
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
            model.PushTagStack();
            var freeFields = FreeFields(model);
            foreach (var argument in function.Arguments)
            {
                var value = CollectionUtils.SafeGet(freeFields,argument) ?? CollectionUtils.SafeGet(freeFields, LanguageHelper.CamelCaseProperty(argument));
                //accept null values;
                if (function.IsStrict && value==null)
                {
                    throw MacroException.NullNotAllowed(argument).Decorate(Name.Context);
                }
                model.Tag[argument] = value;
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
