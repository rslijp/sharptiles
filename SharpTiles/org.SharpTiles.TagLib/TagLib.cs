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
 */
using System;
using System.Collections;
using System.Collections.Generic;
using org.SharpTiles.Common;
using org.SharpTiles.Tags.CoreTags;
using org.SharpTiles.Tags.FormatTags;
using org.SharpTiles.Tags.XmlTags;

namespace org.SharpTiles.Tags
{
    public class TagLib : ITagLib
    {
        private readonly IDictionary<string, ITagGroup> TAGS = new Dictionary<string, ITagGroup>();

        public TagLib()
        {
            Register(new Core());
            Register(new Format());
            Register(new Xml());
        }

        public TagLib(params ITagGroup[] groups)
        {
            foreach (var group in groups)
            {
                Register(group);
            }
        }

        #region IEnumerable<ITagGroup> Members

            IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ITagGroup>) this).GetEnumerator();
        }

        public IEnumerator<ITagGroup> GetEnumerator()
        {
            return TAGS.Values.GetEnumerator();
        }

        #endregion

        public ITagLib Register(ITagGroup group)
        {
            if (TAGS.ContainsKey(group.Name))
                throw new ArgumentException($"Group '{group.Name}' is already registered. Currently registered groups are: {string.Join(", ", TAGS.Keys)}.");
            TAGS.Add(group.Name, group);
            return this;
        }

 
        public ITagGroup Get(string group, ParseContext context=null)
        {
            if (TAGS.ContainsKey(group))
            {
                return TAGS[group];
            }
            if (context != null) {
                throw TagException.UnkownTagGroup(group).Decorate(context);
            }
            return null;;
        }

        public bool Exists(string group)
        {
            return TAGS.ContainsKey(group);
        }
    }
}
