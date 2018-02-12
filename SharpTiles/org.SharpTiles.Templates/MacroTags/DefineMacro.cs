using System;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Templates.MacroTags
{
    public class DefineMacro : BaseCoreTagWithVariable, ITag
    {
        public static readonly string NAME = "define";

        #region ITag Members

        public string TagName
        {
            get { return NAME; }
        }

        [Internal]
        public ITagAttribute Body { get; set; }

        public override object InternalEvaluate(TagModel model)
        {
            var self = this;
            return new MarcoDefinition((callingModel) => self.GetAutoValueAsString(nameof(Body), callingModel));
        }

        public TagBodyMode TagBodyMode => TagBodyMode.Free;

        #endregion

        public class MarcoDefinition
        {
            private readonly Func<TagModel, string> _lazyFunction;

            public MarcoDefinition(Func<TagModel, string> lazyFunction)
            {
                _lazyFunction = lazyFunction;
            }

            public string Evaluate(TagModel model)
            {
                return _lazyFunction(model);
            }
        }
    }
}
