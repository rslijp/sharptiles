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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;

namespace org.SharpTiles.Templates
{
    public class TagLibForParsing : ITagLib
    {
        private readonly Stack<ITagLib> _stack = new Stack<ITagLib>();
        private ITagLib _top;

        public TagLibForParsing(ITagLib tagLib)
        {
            _top = tagLib;
            _stack.Push(tagLib);
        }

        #region IEnumerable<ITagGroup> Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _stack.SelectMany(l=>l.ToList()).GetEnumerator();
        }

        public IEnumerator<ITagGroup> GetEnumerator()
        {
            return _stack.SelectMany(l => l.ToList()).GetEnumerator();
        }

        #endregion

        public ITagGroup Get(string group, ParseContext context = null)
        {
            var matches = _stack.Where(l => l.Exists(group)).ToList();
            if (matches.Count == 1) return matches.Single().Get(group, context);
            if (matches.Count > 0)
            {
                var groups = matches.Select(lib => lib.Get(group,context)).ToList();
                var tags = groups.SelectMany(grp => grp.ToList()).ToArray();
                return new CombinedTagGroup(group, tags);
            } 
            if (context != null)
            {
                throw TagException.UnkownTagGroup(group).Decorate(context);
            }
            return null;
        }

        public ITagLib Register(ITagGroup group)
        {
            return _top.Register(group);
        }

        public bool Exists(string group)
        {
            foreach (var lib in _stack)
            {
                if (lib.Exists(group)) return true;
            }
            return false;
        }

        public TagLibMode Mode => _top.Mode;

        public void Push(params ITagGroup[] extension)
        {
            var dish = new TagLib(Mode,extension);
            _stack.Push(dish);
        }

        public void Pop()
        {
            _stack.Pop();
        }

        public override string ToString() => $"TagLibForParsing[Mode={Mode},Tags={string.Join(",", _stack)}]";


        public class CombinedTagGroup : ITagGroup
        {
            private ITag[] _tags;

            public CombinedTagGroup(string name, params ITag[] tags)
            {
                Name = name;
                _tags = tags;
            }
            public IEnumerator<ITag> GetEnumerator()
            {
                return ((IEnumerable<ITag>)this).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _tags.GetEnumerator();
            }

            public string Name { get; }

            public ITag Get(Token name)
            {
                return Get(name.Contents, name.Context);
            }

            public bool Exist(Token name)
            {
                return _tags.Any(t => t.TagName.Equals(name));
            }

            public string[] TagNames => _tags.Select(tag => tag.TagName).OrderBy(t => t).ToArray();

            public ITag Get(string name, ParseContext context)
            {
               var tag = _tags.FirstOrDefault(t => t.TagName.Equals(name));
               if (tag != null) return tag;
               throw TagException.UnkownTag(name).Decorate(context);
            }
        }
    }
}
