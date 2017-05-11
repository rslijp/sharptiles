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
        private static readonly string[] _predefinedChars = { "&", "<", ">", "\"", "'" };
        private static readonly string[] _predefinedEntities = { "&amp;", "&lt;", "&gt;", "&quot;", "&apos;" };

        public static string EscapeXml(string text)
        {
            var result = text;
            for (int i = 0; i < _predefinedChars.Length; i++)
                result = result.Replace(_predefinedChars[i], _predefinedEntities[i]);
            return result;
        }

        public static string UnescapeXml(string text)
        {
            var result = text;
            for (int i = _predefinedEntities.Length - 1; i >= 0; i--)
                result = result.Replace(_predefinedEntities[i], _predefinedChars[i]);
            return result;
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
