using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Templates.SharpTags
{
    public class DefineMacro : BaseCoreTagWithVariable, ITag
    {
        public static readonly string NAME = "defineMacro";

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
            return new LazyFunction(() => self.GetAutoValueAsString(nameof(Body), model));
        }

        public TagBodyMode TagBodyMode => TagBodyMode.Free;

        #endregion

        public class LazyFunction
        {
            private readonly Func<string> _lazyFunction;

            public LazyFunction(Func<string> lazyFunction)
            {
                _lazyFunction = lazyFunction;
            }

            public string Evaluate()
            {
                return _lazyFunction();
            }
        }
    }
}
