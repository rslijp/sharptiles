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
using System.Collections.Generic;

namespace org.SharpTiles.Tags.CoreTags
{
    public abstract class BaseCoreTagWithFreeFields : BaseCoreTag
    {
        protected readonly Dictionary<string, ITagAttribute> FreeAttribs = new Dictionary<string, ITagAttribute>();

        public IDictionary<string, object> FreeFields(TagModel model)
        {
            if (FreeAttribs.Count == 0) return new Dictionary<string, object>();
            var freeFields = new Dictionary<string, object>();
            foreach (var attrib in FreeAttribs)
            {
                freeFields.Add(attrib.Key, attrib.Value.Evaluate(model));
            }
            return freeFields;
        }

        private ITagAttributeSetter _instance;

        public override ITagAttributeSetter AttributeSetter => _instance??(_instance=new FreeFieldsAttributeSetter(FreeAttribs, base.AttributeSetter));
     
        
    }

    public class FreeFieldsAttributeSetter : ITagAttributeSetter
    {
        private readonly IDictionary<string, ITagAttribute> _freeAttribs;
        private readonly ITagAttributeSetter _parent;

        public FreeFieldsAttributeSetter(IDictionary<string, ITagAttribute> freeAttribs, ITagAttributeSetter parent)
        {
            _freeAttribs = freeAttribs;
            _parent = parent;
        }
        public bool SupportNaturalLanguage => true;

        public bool HasAttribute(string property)
        {
            return true;
        }

        public void InitComplete()
        {
            _parent.InitComplete();
        }

        public ITagAttribute this[string property]
        {
            get
            {
                if (_parent.HasAttribute(property))
                {
                    return _parent[property];
                }
                return _freeAttribs.ContainsKey(property) ? _freeAttribs[property] : null;
            }
            set
            {
                if (_parent.HasAttribute(property))
                {
                    _parent[property] = value;
                }
                else
                {
                    _freeAttribs[property] = value;
                }
            }
        }
    }

}
