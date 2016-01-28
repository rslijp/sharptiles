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
 */using org.SharpTiles.Common;
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
