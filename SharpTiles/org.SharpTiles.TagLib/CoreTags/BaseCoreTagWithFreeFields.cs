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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
 using System.Globalization;
 using org.SharpTiles.Common;
 using org.SharpTiles.Expressions;
 using org.SharpTiles.Tags.Creators;
 using org.SharpTiles.Tags.DefaultPropertyValues;
 using org.SharpTiles.Tags.FormatTags;

namespace org.SharpTiles.Tags.CoreTags
{
    public abstract class BaseCoreTagWithFreeFields : BaseCoreTag, ITagAttributeSetter
    {
        protected readonly Dictionary<string, ITagAttribute> FreeAttribs = new Dictionary<string, ITagAttribute>();

        public IDictionary<string, object> FreeFields(TagModel model)
        {
            if (FreeAttribs.Count == 0) return null;
            var freeFields = new Dictionary<string, object>();
            foreach (var attrib in FreeAttribs)
            {
                freeFields.Add(attrib.Key, attrib.Value.Evaluate(model));
            }
            return freeFields;
        }

        public bool HasAttribute(string property)
        {
            return true;
        }

        public void InitComplete()
        {
            base.AttributeSetter.InitComplete();
        }

        public override ITagAttributeSetter AttributeSetter => this;

        public bool SupportNaturalLanguage => true;

        public ITagAttribute this[string property]
        {
            get
            {
                if (base.AttributeSetter.HasAttribute(property))
                {
                    return base.AttributeSetter[property];
                }
                return FreeAttribs.ContainsKey(property) ? FreeAttribs[property] : null;
            }
            set
            {
                if (base.AttributeSetter.HasAttribute(property))
                {
                    base.AttributeSetter[property] = value;
                }
                else
                {
                    FreeAttribs[property] = value;
                }
            }
        }
        
    }
}
