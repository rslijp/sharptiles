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
 using System.Globalization;
 using System.Linq;
 using System.Reflection;

namespace org.SharpTiles.Common
{
    public static class TypeConverter
    {
        private static readonly IDictionary<Type, ConvertTo> REGISTERED = new Dictionary<Type, ConvertTo>();
        private static readonly IDictionary<Type, CanConvertTo> REGISTER_CAN = new Dictionary<Type, CanConvertTo>();
        private static readonly ICollection<Type> REGISTERED_NUMERICS = new HashSet<Type>();

        public static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            var types = new List<Type>();
            var c = type;
            while (c.BaseType != null && c.BaseType != typeof(object))
            {
                c=c.BaseType;
                types.Add(c);
            }
            return types;
        }

        static TypeConverter()
        {
            REGISTERED.Add(typeof (int), (source, culture) => (object) Convert.ToInt32(source));
            REGISTERED.Add(typeof(long), (source, culture) => (object)Convert.ToInt64(source));
            REGISTERED.Add(typeof (decimal), (source, culture) => (object) Convert.ToDecimal(source, culture.NumberFormat));
            REGISTERED.Add(typeof (float), (source, culture) => (float) Convert.ToDouble(source, culture.NumberFormat));
            REGISTERED.Add(typeof (double), (source, culture) => (object) Convert.ToDouble(source, culture.NumberFormat));
            REGISTERED.Add(typeof (bool), (source, culture) => (object) Convert.ToBoolean(source));
            REGISTERED.Add(typeof (Guid), (source, culture) => (object) Guid.ParseExact(source,"D"));
            REGISTERED.Add(typeof(DateTime), (source, culture) => (object) ParseUniversalDate(source, culture.DateTimeFormat));
            REGISTERED.Add(typeof (int?),
                           (source, culture) => source != null ? (object) Convert.ToInt32(source) : null);
            REGISTERED.Add(typeof(long?),
                (source, culture) => source != null ? (object)Convert.ToInt64(source) : null);
            REGISTERED.Add(typeof (decimal?),
                           (source, culture) => source != null ? (object) Convert.ToDecimal(source, culture.NumberFormat) : null);
            REGISTERED.Add(typeof (float?),
                           (source, culture) => source != null ? (float?) Convert.ToDouble(source, culture.NumberFormat) : null);
            REGISTERED.Add(typeof (double?),
                           (source, culture) => source != null ? (object) Convert.ToDouble(source, culture.NumberFormat) : null);
            REGISTERED.Add(typeof (bool?),
                           (source, culture) => source != null ? (object) Convert.ToBoolean(source) : null);
            REGISTERED.Add(typeof(Guid?),
                (source, culture) => source != null ? (object)Guid.ParseExact(source, "D") : null);
            REGISTERED.Add(typeof(DateTime?), (source, culture) => !string.IsNullOrEmpty(source) ?  (DateTime?) ParseUniversalDate(source,culture) : null);
            REGISTERED.Add(typeof (string),
                           (source, culture) => source != null ? source.ToString() : null);

            REGISTER_CAN.Add(typeof(int), (source, culture) =>
            {
                int result;
                if (source == null || "".Equals(source)) return false;
                return int.TryParse(source.ToString(), NumberStyles.Any, culture.NumberFormat, out result);
            });
            REGISTER_CAN.Add(typeof(decimal), (source, culture) =>
            {
                decimal result;
                if (source == null || "".Equals(source)) return false;
                return decimal.TryParse(source.ToString(), NumberStyles.Any, culture.NumberFormat, out result);
            });
            REGISTER_CAN.Add(typeof(float), (source, culture) =>
            {
                float result;
                if (source == null) return false;
                return float.TryParse(source.ToString(), NumberStyles.Any, culture.NumberFormat, out result);
            });
            REGISTER_CAN.Add(typeof(double), (source, culture) =>
            {
                double result;
                if (source == null) return false;
                return double.TryParse(source.ToString(), NumberStyles.Any, culture.NumberFormat, out result);
            });
            REGISTER_CAN.Add(typeof(bool), (source, culture) =>
            {
                bool result;
                if (source == null || "".Equals(source)) return false;
                return bool.TryParse(source.ToString(), out result);
            });
            REGISTER_CAN.Add(typeof(Guid), (source, culture) =>
            {
                Guid result;
                if (source == null || "".Equals(source)) return false;
                return Guid.TryParse(source.ToString(), out result);
            });
            REGISTER_CAN.Add(typeof(DateTime), (source, culture) =>
            {
                if (source == null || "".Equals(source)) return false;
                try
                {
                    ParseUniversalDate(source.ToString(), culture);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
            REGISTER_CAN.Add(typeof(int?), (source, culture) =>
            {
                int result;
                if (source == null || "".Equals(source)) return true;
                return int.TryParse(source.ToString(), NumberStyles.Any, culture.NumberFormat, out result);
            });
            REGISTER_CAN.Add(typeof(decimal?), (source, culture) =>
            {
                decimal result;
                if (source == null || "".Equals(source)) return true;
                return decimal.TryParse(source.ToString(), NumberStyles.Any, culture.NumberFormat, out result);
            });
            REGISTER_CAN.Add(typeof(float?), (source, culture) =>
            {
                float result;
                if (source == null || "".Equals(source)) return true;
                return float.TryParse(source.ToString(), NumberStyles.Any, culture.NumberFormat, out result);
            });
            REGISTER_CAN.Add(typeof(double?), (source, culture) =>
            {
                double result;
                if (source == null || "".Equals(source)) return true;
                return double.TryParse(source.ToString(), NumberStyles.Any, culture.NumberFormat, out result);
            });
            REGISTER_CAN.Add(typeof(bool?), (source, culture) =>
            {
                bool result;
                if (source == null || "".Equals(source)) return true;
                return bool.TryParse(source.ToString(), out result);
            });
            REGISTER_CAN.Add(typeof(Guid?), (source, culture) =>
            {
                Guid result;
                if (source == null || "".Equals(source)) return true;
                return Guid.TryParse(source.ToString(), out result);
            });
            REGISTER_CAN.Add(typeof(DateTime?), (source, culture) =>
            {
                if (source == null || "".Equals(source)) return true;
                try
                {
                    ParseUniversalDate(source.ToString(), culture);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
            REGISTERED_NUMERICS.Add(typeof (int));
            REGISTERED_NUMERICS.Add(typeof (int?));
            REGISTERED_NUMERICS.Add(typeof (decimal));
            REGISTERED_NUMERICS.Add(typeof (decimal?));
            REGISTERED_NUMERICS.Add(typeof (double));
            REGISTERED_NUMERICS.Add(typeof (double?));
            REGISTERED_NUMERICS.Add(typeof (float));
            REGISTERED_NUMERICS.Add(typeof (float?));
            
        }

        private const string DATE_ONLY = "yyyy-MM-dd";
        private const string DATE_TIME = "yyyy-MM-dd HH:mm:ss";
        private const string DATE_TIME2 = "yyyy-MM-dd'T'HH:mm:ss";

        private static DateTime? ParseUniversalDate(string source, IFormatProvider formatProvider)
        {
            if (source.Length.Equals(DATE_ONLY.Length)) return DateTime.ParseExact(source, DATE_ONLY, formatProvider).ToUniversalTime();
            if (source[10]=='T') return DateTime.ParseExact(source, DATE_TIME2, formatProvider).ToUniversalTime();
            return DateTime.ParseExact(source, DATE_TIME, formatProvider).ToUniversalTime();
        }
        public static T To<T>(object source)
        {
            return (T) To(source,typeof(T), CultureInfo.CurrentCulture);
        }


        public static object To(object source, Type target)
        {
            return To(source, target, CultureInfo.CurrentCulture);
        }

        public static object To(object source, Type target, CultureInfo culture)
        {
            try
            {
                if (source == null ||
                    target == typeof (object) ||
                    source.GetType() == target ||
                    source.GetType().GetInterfaces().Any(t => t == target) ||
                    source.GetType().GetBaseTypes().Any(t => t == target))
                {
                    return source;
                }
                if (target.IsGenericType && target.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var realTarget = target.GetGenericArguments()[0];
                    if (source.GetType() == realTarget ||
                        source.GetType().GetInterfaces().Any(t => t == realTarget))
                    {
                        return source;
                    }
                    if (realTarget.IsEnum)
                    {
                        return ToEnum(source, realTarget, culture);
                    }
                }
                if (target.IsEnum)
                {
                    return ToEnum(source, target,culture);
                }
                if (target.IsArray)
                {
                    return TryArray(source, target);
                }
                var formattable = source as IFormattable;
                var text = formattable?.ToString(null, culture) ?? source.ToString();
                ConvertTo convertTo = null;
                if(!REGISTERED.TryGetValue(target, out convertTo)) throw ConvertException.CannotConvert(target, source);
                return convertTo(text, culture);
            }
            catch (FormatException)
            {
                throw ConvertException.CannotConvert(target, source);
            }
        }

        private static object TryArray(object source, Type target)
        {
           var elementType = target.GetElementType();
           var castMethod = typeof(Enumerable).GetMethod("Cast")
               .MakeGenericMethod(new System.Type[] {elementType});
           var toArrayMethod = typeof(Enumerable).GetMethod("ToArray")
               .MakeGenericMethod(new System.Type[] {elementType});

            try
            {
                var castedObjectEnum = castMethod.Invoke(null, new[] {source});
                var castedObject = toArrayMethod.Invoke(null, new[] {castedObjectEnum});
                return castedObject;
            } catch
            {
                throw ConvertException.CannotConvert(target, source);
            } 
            
        }

        private static object ToEnum(object source, Type target, CultureInfo culture)
        {
            try
            {
                if (source?.GetType() == typeof(string))
                {
                    return Enum.Parse(target, (string) source);
                }
                var intValue = REGISTERED[target](source.ToString(), culture);
                return Enum.ToObject(target, intValue);
            }
            catch (ArgumentException)
            {
                throw ConvertException.CannotConvertValue(target, source);
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

        private delegate object ConvertTo(string o, CultureInfo c);

        private delegate bool CanConvertTo(object o, CultureInfo c);
        

        #endregion

        public static object TryTo(object source, Type target, CultureInfo culture)
        {
            try
            {
                if (source == null ||
                    Equals(target, typeof(object)) ||
                    Equals(source.GetType(), target) ||
                    source.GetType().GetInterfaces().Any(t => Equals(t, target)))
                {
                    return source;
                }
                if (REGISTER_CAN[target](source, culture))
                {
                    return REGISTERED[target](source.ToString(), culture);
                }
                return null;
            }
            catch (FormatException)
            {
                return null;
            }
        }

        public static object TryTo(string value, Type type)
        {
            return TryTo(value, type, CultureInfo.CurrentCulture);
        }

        public static bool IsNumeric(object source)
        {
            return source!=null && REGISTERED_NUMERICS.Contains(source.GetType());
        }

        public static bool IsEnum(object source)
        {
            return source?.GetType().IsEnum ?? false;
        }
    }
}
