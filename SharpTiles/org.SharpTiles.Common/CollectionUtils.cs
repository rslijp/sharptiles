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
using System.Reflection;
using System.Text;

namespace org.SharpTiles.Common
{
    public class CollectionUtils
    {
        public static string ToString(IEnumerable values, PropertyInfo info)
        {
            bool first = true;
            var builder = new StringBuilder();
            foreach (object o in values)
            {
                if (!first)
                {
                    builder.Append(", ");
                }
                builder.Append(info.GetValue(o, null).ToString());
                first = false;
            }
            return builder.ToString();
        }

        public static string ToString(IEnumerable values)
        {
            bool first = true;
            var builder = new StringBuilder();
            foreach (object o in values)
            {
                if (!first)
                {
                    builder.Append(", ");
                }
                builder.Append(o);
                first = false;
            }
            return builder.ToString();
        }

        public static Array Reverse(Array array)
        {
            Array clone = (Array) array.Clone();
            int length = array.Length-1;
            int halfTheWay = (int) Math.Ceiling(array.Length/2m);
            for (int i = 0; i < halfTheWay; i++)
            {
                object tmp = array.GetValue(i);
                clone.SetValue(array.GetValue(length - i), i);
                clone.SetValue(tmp, length - i); 
            }
            return clone;
        }

        public static V SafeGet<K,V>(IDictionary<K, V> cache, K key)
        {
            V result;
            cache.TryGetValue(key, out result);
            return result;
        }
    }
}
