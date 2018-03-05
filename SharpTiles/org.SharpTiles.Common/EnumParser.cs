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

namespace org.SharpTiles.Common
{
    public class EnumParser<T>
    {
        private static readonly IDictionary<string, T> CACHE = new Dictionary<string, T>();

        static EnumParser()
        {
            foreach (T enumValue in Enum.GetValues(typeof (T)))
            {
                var key = enumValue.ToString();
                if (!CACHE.ContainsKey(key))
                    CACHE.Add(key, enumValue);

                key = enumValue.ToString().ToLowerInvariant();
                if (!CACHE.ContainsKey(key))
                    CACHE.Add(key, enumValue);
            }
        }

        public static T Parse(string value)
        {
            try
            {
                return CACHE[value];
            }
            catch (KeyNotFoundException)
            {
                String msg = String.Format("{0}.{1} is not defined", typeof (T).Name, value);
                throw new ArgumentException(msg);
            }
        }
    }
}
