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
using System.Linq;
using org.SharpTiles.Common;

namespace org.SharpTiles.Tags
{
    public abstract class BaseTagGroup<G> : ITagGroup
    {
        private readonly IDictionary<string, ITagFactory> REGISTERED_TAGS = new Dictionary<string, ITagFactory>();

        #region ITagGroup Members

        public abstract string Name { get; }

        public ITag Get(Token name)
        {
            return Get(name.Contents, name.Context);
        }

        public bool Exist(Token name)
        {
            return REGISTERED_TAGS.ContainsKey(name.Contents);
        }

        public string[] TagNames => REGISTERED_TAGS.Keys.OrderBy(tag => tag).ToArray();


        public ITag Get(string name, ParseContext context)
        {
            if (REGISTERED_TAGS.ContainsKey(name))
            {
                ITag tag = REGISTERED_TAGS[name].NewInstance();
                tag.Context = context;
                tag.Group = this;
                return tag;
            }
            throw TagException.UnkownTag(name, TagNames).Decorate(context);
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ITag>) this).GetEnumerator();
        }

        public IEnumerator<ITag> GetEnumerator()
        {
            IList<ITag> list = new List<ITag>();
            foreach (ITagFactory value in REGISTERED_TAGS.Values)
            {
                var t = value.NewInstance();
                t.Group = this;
                list.Add(t);
            }
            return list.GetEnumerator();
        }

        #endregion

        public void Register<T>() where T : ITag, new()
        {
            ITagFactory factory = new GenericTagFactory<T>();
            AddTag(factory.Name, factory);
        }

        public void Register(Type type)
        {
            ITagFactory factory = new GenericTagFactory(type);
            AddTag(factory.Name, factory);
        }

        public void Register(ITagFactory factory)
        {
            AddTag(factory.Name, factory);
        }

        private void AddTag(string name, ITagFactory factory)
        {
            if (REGISTERED_TAGS.ContainsKey(name))
                throw TagException.DuplicateTag(name, REGISTERED_TAGS.Keys.ToArray());
            REGISTERED_TAGS.Add(factory.Name, factory);
        }
    }
}
