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
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using MarkdownDeep;

namespace org.SharpTiles.Documentation.DocumentationAttributes
{
    [DataContract]

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, Inherited = true,
        AllowMultiple = false)]
    public class DescriptionAttribute : Attribute
    {

        public DescriptionAttribute(string value, string category=null)
        {
            Value = value;
            Category = category;
        }

        
        [DataMember]
        public string Value { get; private set; }
        [DataMember]
        public string Category { get; private set; }
        

        public static DescriptionValue Harvest(Type type)
        {
            var attr = type.GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().SingleOrDefault();
            if (attr == null) return null;
            return new DescriptionValue(attr);
        }

        
        internal static DescriptionValue Harvest(PropertyInfo property)
        {
            var attr = property.GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().SingleOrDefault();
            if (attr == null) return null;
            return new DescriptionValue(attr);
        }
        
    }
}
