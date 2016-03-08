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
    public class Console : BaseCoreTag, ITag
    {
        public static readonly string NAME = "console";

        [TagDefaultProperty("Body")]
        public ITagAttribute Value { get; set; }

        [Internal]
        public ITagAttribute Body { get; set; }

       
        #region ITag Members

        public string TagName
        {
            get { return NAME; }
        }

        public string Evaluate(TagModel model)
        {
            string result = GetAutoValueAsString("Value", model);
            System.Console.WriteLine(result);
            return string.Empty;
        }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.Free; }
        }

        #endregion
    }
}
