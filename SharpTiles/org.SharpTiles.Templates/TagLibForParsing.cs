using System;
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
        private Stack<ITagLib> stack = new Stack<ITagLib>();
        private ITagLib _top;

        public TagLibForParsing(ITagLib tagLib)
        {
            _top = tagLib;
            stack.Push(tagLib);
        }

        #region IEnumerable<ITagGroup> Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ITagGroup>)stack.SelectMany(l=>l.ToList())).GetEnumerator();
        }

        public IEnumerator<ITagGroup> GetEnumerator()
        {
            return stack.SelectMany(l => l.ToList()).GetEnumerator();
        }

        #endregion

        public ITagGroup Get(string group, ParseContext context = null)
        {
            var matches = stack.Where(l => l.Exists(group)).ToList();
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
            foreach (var lib in stack)
            {
                if (lib.Exists(group)) return true;
            }
            return false;
        }

        public void Push(params ITagGroup[] extension)
        {
            var dish = new TagLib(extension);
            stack.Push(dish);
        }

        public void Pop()
        {
            stack.Pop();
        }

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

            public ITag Get(string name, ParseContext context)
            {
               var tag = _tags.FirstOrDefault(t => t.TagName.Equals(name));
               if (tag != null) return tag;
               throw TagException.UnkownTag(name).Decorate(context);
            }
        }
    }
}
