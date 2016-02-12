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
 */using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.Creators;

namespace org.SharpTiles.Templates
{
    public class StrictResolveTagLibParser : IgnoreResolveTagLibParser
    {
        public StrictResolveTagLibParser(TagLibForParsing lib, ParseHelper helper, IResourceLocator locator, IResourceLocatorFactory factory) : base(lib, helper, locator, factory)
        {
        }
        
        protected override ITag ParseTagType()
        {
            var tag=base.ParseTagType();
            if (tag == null)
            {
                throw TagException.UnkownTag(_helper.Current.Contents).Decorate(_helper.Current);
            }
            return tag;
        }

        protected override TagLibMode Mode
        {
            get
            {
                return TagLibMode.StrictResolve;  
            } 
        }
    }
}
