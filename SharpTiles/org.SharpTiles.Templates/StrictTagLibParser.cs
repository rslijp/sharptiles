using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;

namespace org.SharpTiles.Templates
{
    public class StrictTagLibParser : AbstractTagLibParser
    {
        public StrictTagLibParser(ParseHelper helper, IResourceLocator locator) : base(helper, locator)
        {

        }
        
        protected override ITag ParseTagType()
        {
            Token group = _helper.Read(TokenType.Regular);
            if (!_helper.IsAhead(TagLibConstants.GROUP_TAG_SEPERATOR))
            {
                return null;
            }
            _helper.Read(TagLibConstants.GROUP_TAG_SEPERATOR);
            Token name = _helper.Read(TokenType.Regular);

            if (_helper.IgnoreUnkownTag() && !TagLib.Exists(group.Contents))
            {
                return null;
            }
            ITagGroup tagGroup = TagLib.Libs.Get(group);
            return tagGroup.Get(name);
        }

        protected override TagLibMode Mode
        {
            get
            {
                return TagLibMode.Strict;  
            } 
        }
    }
}
