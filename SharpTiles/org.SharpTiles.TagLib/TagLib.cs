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
    public class TagLib : IEnumerable<ITagGroup>
    {
        private static readonly TagLib lib = new TagLib();
        private static readonly IDictionary<string, ITagGroup> TAGS = new Dictionary<string, ITagGroup>();

        static TagLib()
        {
            Register(new Core());
            Register(new Format());
            Register(new Xml());
        }

        public static TagLib Libs
        {
            get { return lib; }
        }

        public static ITagGroup Get(String group)
        {
            if (TAGS.ContainsKey(group))
            {
                return TAGS[group];
            } 
            return null;
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

        public static void Register(ITagGroup group)
        {
                TAGS.Add(group.Name, group);
        }

        public ITagGroup Get(Token group)
        {
            return Get(group.Contents, group.Context);
        }

        public ITagGroup Get(string group, ParseContext context)
        {
            if (TAGS.ContainsKey(group))
            {
                return TAGS[group];
            }
            else
            {
                throw TagException.UnkownTagGroup(group).Decorate(context);
            }
        }

        public static bool Exists(string group)
        {
            return TAGS.ContainsKey(group);
        }
    }
}
