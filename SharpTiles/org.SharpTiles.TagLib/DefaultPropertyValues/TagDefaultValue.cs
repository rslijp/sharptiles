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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.CoreTags;
using org.SharpTiles.Tags.DefaultPropertyValues;

namespace org.SharpTiles.Common
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class TagDefaultValue : Attribute, IDefaultPropertyValue
    {
        private object _value;


        public TagDefaultValue(object value)
        {
            _value = value;
        }

        public object Value
        {
            get { return _value; }
        }

        public object GetValue(BaseCoreTag source, TagModel model)
        {
            return _value;
        }

        public override string ToString()
        {
            return String.Format("'{0}'", _value);
        }
    }
}

