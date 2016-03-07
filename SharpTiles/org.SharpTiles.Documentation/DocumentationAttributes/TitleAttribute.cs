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
using System.Threading.Tasks;

namespace org.SharpTiles.Documentation.DocumentationAttributes
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Property|AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class TitleAttribute : Attribute
    {
        public TitleAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }

        public static void Harvest(ResourceKeyStack messagePath, Type type)
        {
            var description = type.GetCustomAttributes(typeof(TitleAttribute), false).Cast<TitleAttribute>().SingleOrDefault();
            if (description != null) messagePath.AddTitleTranslation(description.Value);
        }
        
    }
}
