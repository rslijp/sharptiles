using System.Collections.Generic;
using System.Linq;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;

namespace org.SharpTiles.Templates
{
    public class RelaxedResolveTagLibParser : AbstractTagLibParser
    {
        public RelaxedResolveTagLibParser(TagLibForParsing lib, ParseHelper helper, IResourceLocator locator) : base(lib,helper, locator)
        {
        }
        
        protected override ITag ParseTagType()
        {
            Token group;
            Token name;
            group = _helper.Read(TokenType.Regular);
            if (_helper.IsAhead(TagLibConstants.GROUP_TAG_SEPERATOR))
            {
                _helper.Read(TagLibConstants.GROUP_TAG_SEPERATOR);
                name = _helper.Read(TokenType.Regular);
                if (_helper.IgnoreUnkownTag() && !_lib.Exists(group.Contents))
                {
                    return null;
                }
                ITagGroup tagGroup = _lib.Get(group.Contents, group.Context);
                return tagGroup.Get(name);
            }
            
            name = group;
            var hits = new List<ITag>();
            var libs = new HashSet<ITagGroup>();
            foreach (var lib in _lib)
            {
                if (!lib.Exist(name)) continue;
                hits.Add(lib.Get(name));
                libs.Add(lib);
            }
            if (hits.Count==0)
            {
                if (_helper.IgnoreUnkownTag()) return null;
                throw TagException.UnkownTag(name.Contents).Decorate(name.Context);
            }
            return hits.First();
        }

        protected override TagLibMode Mode => TagLibMode.RelaxedResolve;
    }
}
