using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;

namespace org.SharpTiles.Templates
{
    public class RelaxedResolveTagLibParser : AbstractTagLibParser
    {
        public RelaxedResolveTagLibParser(ParseHelper helper, IResourceLocator locator) : base(helper, locator)
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
                if (_helper.IgnoreUnkownTag() && !TagLib.Exists(group.Contents))
                {
                    return null;
                }
                ITagGroup tagGroup = TagLib.Libs.Get(group);
                return tagGroup.Get(name);
            }
            
            name = group;
            var hits = new List<ITag>();
            var libs = new HashSet<ITagGroup>();
            foreach (var lib in TagLib.Libs)
            {
                if (!lib.Exist(name)) continue;
                hits.Add(lib.Get(name));
                libs.Add(lib);
            }
            if (hits.Count==0)
            {
                return null;
            }
            return hits.First();
        }

        protected override TagLibMode Mode
        {
            get
            {
                return TagLibMode.RelaxedResolve;  
            } 
        }
    }
}
