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
using System.Text.RegularExpressions;
using MarkdownDeep;

namespace org.SharpTiles.Documentation.DocumentationAttributes
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Property|AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class DescriptionAttribute : Attribute
    {
        public static Regex INNER_CONTENT = new Regex("^<p>(.*?)</p>$");
        
        public DescriptionAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }

        public static void Harvest(ResourceKeyStack messagePath, Type type)
        {
            var description = type.GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().SingleOrDefault();
            Harvest(messagePath, description);
        }

        
        internal static void Harvest(ResourceKeyStack messagePath, PropertyInfo property)
        {
            var description = property.GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().SingleOrDefault();
            Harvest(messagePath, description);
        }

        private static void Harvest(ResourceKeyStack messagePath, DescriptionAttribute description)
        {
            if (description == null) return;
            messagePath.AddTranslation(AsHtml(description).Value);
        }

        public static DescriptionAttribute AsHtml(DescriptionAttribute description)
        {
            if (description == null)
                return null;

            var html = new Markdown { ExtraMode = true }.Transform(description.Value);
            html = html.Trim();
            if (INNER_CONTENT.IsMatch(html))
            {
                html = INNER_CONTENT.Match(html).Groups[1].Value;
            }
            return new DescriptionAttribute(html);
        }
    }
}
