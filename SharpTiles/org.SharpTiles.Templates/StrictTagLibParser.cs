using org.SharpTiles.Common;
using org.SharpTiles.Tags;

namespace org.SharpTiles.Templates
{
    public class StrictTagLibParser : AbstractTagLibParser
    {

        public StrictTagLibParser(TagLibForParsing lib,ParseHelper helper, IResourceLocator locator) : base(lib,helper, locator)
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

            if (_helper.IgnoreUnkownTag() && !_lib.Exists(group.Contents))
            {
                return null;
            }
            ITagGroup tagGroup = _lib.Get(group.Contents, group.Context);
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
