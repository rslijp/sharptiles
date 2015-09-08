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

namespace org.SharpTiles.Common
{
    public static class TypeConverter
    {
        private static readonly IDictionary<Type, ConvertTo> REGISTERED = new Dictionary<Type, ConvertTo>();
        private static readonly ICollection<Type> REGISTERED_NUMERICS = new HashSet<Type>();

        static TypeConverter()
        {
            REGISTERED.Add(typeof (int), source => (object) Convert.ToInt32(source));
            REGISTERED.Add(typeof (decimal), source => (object) Convert.ToDecimal(source));
            REGISTERED.Add(typeof (float), source => (float) Convert.ToDouble(source));
            REGISTERED.Add(typeof (double), source => (object) Convert.ToDouble(source));
            REGISTERED.Add(typeof (bool), source => (object) Convert.ToBoolean(source));
            REGISTERED.Add(typeof (int?),
                           source => source != null ? (object) Convert.ToInt32(source) : null);
            REGISTERED.Add(typeof (decimal?),
                           source => source != null ? (object) Convert.ToDecimal(source) : null);
            REGISTERED.Add(typeof (float?),
                           source => source != null ? (float?) Convert.ToDouble(source) : null);
            REGISTERED.Add(typeof (double?),
                           source => source != null ? (object) Convert.ToDouble(source) : null);
            REGISTERED.Add(typeof (bool?),
                           source => source != null ? (object) Convert.ToBoolean(source) : null);
            REGISTERED.Add(typeof (string),
                           source => source != null ? source.ToString() : null);
            
            REGISTERED_NUMERICS.Add(typeof (int));
            REGISTERED_NUMERICS.Add(typeof (int?));
            REGISTERED_NUMERICS.Add(typeof (decimal));
            REGISTERED_NUMERICS.Add(typeof (decimal?));
            REGISTERED_NUMERICS.Add(typeof (double));
            REGISTERED_NUMERICS.Add(typeof (double?));
            REGISTERED_NUMERICS.Add(typeof (float));
            REGISTERED_NUMERICS.Add(typeof (float?));
            
        }

        public static object To(object source, Type target)
        {
            try
            {
                if (source == null ||
                    Equals(target, typeof (object)) ||
                    Equals(source.GetType(), target) ||
                    source.GetType().GetInterfaces().Any(t => Equals(t, target)))
                {
                    return source;
                }
                return REGISTERED[target](source.ToString());
            }
            catch (FormatException)
            {
                throw ConvertException.CannotConvert(target, source);
            }
        }

        public static bool Possible(object source, Type target)
        {
            if (source == null && target.IsValueType) { return false; }
            if (source == null) return true;
            return Possible(source.GetType(), target);
        }


        public static bool Possible(Type source, Type target)
        {
            if (Equals(target, typeof(object)) ||
                Equals(source, target) ||
                source.GetInterfaces().Any(t => Equals(t, target)))
            {
                return true;
            }
            return REGISTERED.ContainsKey(target);
        }


        #region Nested type: ConvertTo

        private delegate object ConvertTo(object o);

        #endregion

        public static object TryTo(string value, Type type)
        {
            try
            {
                return To(value, type);
            } catch
            {
                return null;
            }
        }

        public static bool IsNumeric(object source)
        {
            return source!=null && REGISTERED_NUMERICS.Contains(source.GetType());
        }
    }
}
