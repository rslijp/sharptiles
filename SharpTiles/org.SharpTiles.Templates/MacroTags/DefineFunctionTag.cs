using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using org.SharpTiles.Common;
using org.SharpTiles.Expressions;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Templates.MacroTags
{
    public class DefineFunctionTag : BaseCoreTagWithVariable, ITag, ITagAttributeSetter
    {
        public static readonly string NAME = "function";

        public DefineFunctionTag()
        {
            Arguments=new List<ITagAttribute>();
        }

        #region ITag Members

        public string TagName => NAME;

        public ITagAttribute Result { get; private set; }

        private List<ITagAttribute> Arguments { get; }

        [TagDefaultValue(true)]
        [EnumProperyType(typeof(BooleanEnum))]
        public ITagAttribute IsStrict { get; set; }

        [Internal]
        public ITagAttribute Body { get; set; }

        public override object InternalEvaluate(TagModel model)
        {
            var self = this;
            var def = new FunctionDefinition((callingModel) => self.GetAutoValueAsString(nameof(Body), callingModel));
            foreach (var argument in Arguments)
            {
                def.RegisterArgument(TypeConverter.To<string>(argument.Evaluate(model)));
            }
            if (Result != null)
            {
                def.Result = TypeConverter.To<string>(Result.Evaluate(model));
            }
            if (GetAutoValueAsBool(nameof(IsStrict),model))
            {
                def.MakeStrict();
            }
            return def;
        }

        public TagBodyMode TagBodyMode => TagBodyMode.Free;

        public bool SupportNaturalLanguage => base.AttributeSetter.SupportNaturalLanguage;

        public ITagAttribute this[string property]
        {
            get
            {
                if (nameof(Result).Equals(property)) return Result;
                var argumentIndex = ParseArgumentsProperty(property);
                if (!argumentIndex.HasValue) return base.AttributeSetter[property];
                if (argumentIndex >= Arguments.Count) return null;
                return Arguments[argumentIndex.Value - 1];

            }
            set {
                if (nameof(Result).Equals(property))
                {
                    Result = value;
                    return;
                }
                var argumentIndex = ParseArgumentsProperty(property);
                if (!argumentIndex.HasValue)
                {
                    base.AttributeSetter[property] = value;
                    return;
                }
                if (argumentIndex != Arguments.Count+1) throw MacroException.ExcpectedArgumentIndex(argumentIndex.Value, Arguments.Count).Decorate(value.Context);
                Arguments.Add(value);
            }
        }

        public bool HasAttribute(string propery)
        {
            if(ParseArgumentsProperty(propery).HasValue) return true;
            return base.AttributeSetter.HasAttribute(propery);
        }

        public override ITagAttributeSetter AttributeSetter => this;

        private int? ParseArgumentsProperty(string property)
        {
            var matcher = new Regex($"^Argument([0-9]*)$");
            if (!matcher.IsMatch(property)) return default(int?);
            var match = matcher.Match(property);
            if (match.Groups.Count == 1 || match.Groups[1].Value.Length==0) return 1;
            return int.Parse(match.Groups[1].Value);
        }

        public void InitComplete() => base.AttributeSetter.InitComplete();

        #endregion

        public class FunctionDefinition
        {
            private readonly Func<TagModel, string> _lazyFunction;
            private readonly List<string> _arguments;
            

            public FunctionDefinition(Func<TagModel, string> lazyFunction)
            {
                _lazyFunction = lazyFunction;
                _arguments = new List<string>();
            }

            public void RegisterArgument(string name)
            {
                _arguments.Add(name);
            }

            public string Result { get; set; }

            public string[] Arguments => _arguments.ToArray();

            public bool IsStrict { get; private set; }

            public object Evaluate(TagModel model)
            {
                var output = _lazyFunction(model);
                if (string.IsNullOrEmpty(Result)) return output;
                return model.TryGet(Result);
            }

            public void MakeStrict()
            {
                IsStrict = true;
            }
        }

       
    }
}
