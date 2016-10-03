/*
 * SharpTiles, R.Z. Slijp(2008), www.sharptiles.org
 *
 * This file is part of SharpTiles.
 * 
 * SharpTiles is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * SharpTiles is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with SharpTiles.  If not, see <http://www.gnu.org/licenses/>.
 */using System.Collections.Generic;
using System.Linq;
using org.SharpTiles.Common;
using org.SharpTiles.Expressions;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.Creators;

namespace org.SharpTiles.Templates
{
    public class RelaxedResolveTagLibParser : AbstractTagLibParser
    {
        public RelaxedResolveTagLibParser(TagLibForParsing lib, ExpressionLib expressionLib,ParseHelper helper, IResourceLocator locator, IResourceLocatorFactory factory, ITagValidator tagValidator)
            : base(lib, expressionLib, helper, locator,factory,tagValidator)
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
                throw TagException.UnkownTag(name.Contents, TagNames).Decorate(name.Context);
            }
            return hits.First();
        }

        protected override TagLibMode Mode => TagLibMode.RelaxedResolve;
    }
}
