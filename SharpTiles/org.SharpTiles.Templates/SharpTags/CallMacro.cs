using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Templates.SharpTags
{
    public class CallMacro : BaseCoreTag, ITag
    {
        public static readonly string NAME = "callMacro";

        #region ITag Members

        public string TagName
        {
            get { return NAME; }
        }

        [Required]
        public ITagAttribute Macro { get; set; }
        

        public TagBodyMode TagBodyMode => TagBodyMode.Free;
        public string Evaluate(TagModel model)
        {
            throw new NotImplementedException();
        }

        #endregion

      
    }
}
