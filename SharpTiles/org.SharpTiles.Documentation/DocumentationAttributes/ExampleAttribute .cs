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
using System.Text;
using System.Threading.Tasks;
using org.SharpTiles.Common;

namespace org.SharpTiles.Documentation.DocumentationAttributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ExampleAttribute : Attribute
    {
        public ExampleAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }

        public static bool Harvest(ResourceKeyStack messagePath, Type type)
        {
            var description = type.GetCustomAttributes(typeof(ExampleAttribute), false).Cast<ExampleAttribute>().SingleOrDefault();
            if (description == null) return false;
            var html=StringUtils.EscapeXml(description.Value);
            html = html.Replace("\n", "<br/>")
                .Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;")
                .Replace("{", "{{")
                .Replace("}", "}}");
            messagePath.AddExampleTranslation(html);
            return true;
        }
    }
}
