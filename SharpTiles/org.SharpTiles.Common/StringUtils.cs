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

namespace org.SharpTiles.Common
{
    public static class StringUtils
    {
        public static string EscapeXml(string text)
        {
            return text == null
                       ? null
                       :
                           text.Replace("&", "&amp;").
                               Replace("<", "&lt;").
                               Replace(">", "&gt;").
                               Replace("\"", "&quot;").
                               Replace("'", "&apos;");
        }

        public static string FormatAsProperty(string property)
        {
            return property.Substring(0, 1).ToUpperInvariant() + property.Substring(1);
        }

        public static string FormatAsMethod(string property)
        {
            return property.Substring(0, 1).ToUpperInvariant() + property.Substring(1);
        }

        public static object Reverse(string str)
        {
            char[] charArray = str.ToCharArray();
            charArray = (char[]) CollectionUtils.Reverse(charArray);
            return new String(charArray);
        }

    }
}
