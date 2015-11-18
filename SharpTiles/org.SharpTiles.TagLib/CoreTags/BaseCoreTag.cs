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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
 using System.Globalization;
 using org.SharpTiles.Common;
 using org.SharpTiles.Expressions;
 using org.SharpTiles.Tags.DefaultPropertyValues;
 using org.SharpTiles.Tags.FormatTags;

namespace org.SharpTiles.Tags.CoreTags
{
    public abstract class BaseCoreTag
    {
        private IDictionary<string, PropertyInfo> PROPERTY_CACHE;
        private IDictionary<string, IDefaultPropertyValue> DEFAULT_CACHE;
        protected ITagAttribute _id;
        protected TagState _state;

        protected BaseCoreTag()
        {
                InitCache();
        }

        private void InitCache()
        {
            PROPERTY_CACHE = new Dictionary<string, PropertyInfo>();
            DEFAULT_CACHE = new Dictionary<string, IDefaultPropertyValue>();
            var type = GetType();
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
            {
                PROPERTY_CACHE[property.Name] = property;
                var fallBack = (IDefaultPropertyValue)property.GetCustomAttributes(typeof(IDefaultPropertyValue), false).FirstOrDefault();
                if(fallBack!=null)
                {
                    DEFAULT_CACHE[property.Name] = fallBack;
                }
            }
        }

        [Internal]
        public virtual ITagAttribute Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public TagState State
        {
            get { return _state; }
            set { _state = value; }
        }


        public virtual ITagAttributeSetter AttributeSetter
        {
            get { return new ReflectionAttributeSetter(this); }
        }

        public ParseContext Context { get; set; }

        protected VariableScope ToScope(string valueStr)
        {
            return EnumParser<VariableScope>.Parse(valueStr);
        }

        public string GetAutoValueAsString(string propertyName, TagModel model)
        {
            object result = GetAutoValue(propertyName, model);
            return result != null ? ValueOfWithi18N(model, result.ToString()) : null;

        }

        public static string ValueOfWithi18N(TagModel model, object raw)
        {
            if (raw == null) return String.Empty;
            var value = raw.ToString();
            if (raw is DateTime)
            {
                value = ((DateTime)raw).ToString(ResourceBundle.GetLocale(model).DateTimeFormat);
            }
            else if (raw is decimal)
            {
                value = ((decimal)raw).ToString(ResourceBundle.GetLocale(model).NumberFormat);
            }
            return value;
        }

        public int? GetAutoValueAsInt(string propertyName, TagModel model)
        {
            object result = GetAutoValue(propertyName, model);
            return (int?) TypeConverter.To(result, typeof (int?));
        }

        public decimal? GetAutoValueAsDecimal(string propertyName, TagModel model)
        {
            object result = GetAutoValue(propertyName, model);
            return (decimal?)TypeConverter.To(result, typeof(decimal?));
        }

        public DateTime? GetAutoValueAsDateTime(string propertyName, TagModel model)
        {
            return ResolveToDate(propertyName, PatternStrings.DATETIME_FORMAT, model);
        }

        private DateTime? ResolveToDate(string propertyName, string pattern, TagModel model)
        {
            object result = GetAutoValue(propertyName, model);
            if (result == null) return default(DateTime?);
            if ((result as DateTime?) != null) return (DateTime?) result;
            var dateStr = result.ToString();
            if (string.IsNullOrEmpty(dateStr))
            {
                return default(DateTime?);
            }
            try
            {
                return DateTime.ParseExact(dateStr, pattern, CultureInfo.CurrentCulture);
            }
            catch (FormatException Fe)
            {
                throw new FormatException($"Can't parse '{dateStr}' to date using pattern {pattern}.",Fe);
            }
        }

        public DateTime? GetAutoValueAsDate(string propertyName, TagModel model)
        {
            return ResolveToDate(propertyName, PatternStrings.DATE_FORMAT, model);
        }

        public bool GetAutoValueAsBool(string propertyName, TagModel model)
        {
            object result = GetAutoValue(propertyName, model);
            return (bool)TypeConverter.To(result, typeof(bool));
        }

        public T? GetAutoValueAs<T>(string propertyName, TagModel model) where T : struct
        {
            string result = GetAutoValueAsString(propertyName, model);
            return result != null ? EnumParser<T>.Parse(result) : default(T?);
        }


        public object GetAutoValue(string propertyName, TagModel model)
        {
            PropertyInfo property = CollectionUtils.SafeGet(PROPERTY_CACHE, propertyName);
            var attribute = (ITagAttribute)property.GetValue(this, null);
            object value = attribute != null ? attribute.Evaluate(model) : null;
            if (value == null)
            {
                IDefaultPropertyValue fallBack = CollectionUtils.SafeGet(DEFAULT_CACHE, propertyName);
                value = fallBack != null ? fallBack.GetValue(this, model): null;
            }
            return value;
        }

        public static string GetAsString(ITagAttribute expressions, TagModel model)
        {
            object result = Get(expressions, model);
            return result != null ? result.ToString() : null;
        }

        public static int? GetAsInt(ITagAttribute expressions, TagModel model)
        {
            object result = Get(expressions, model);
            return (int?) TypeConverter.To(result, typeof (int?));
        }

        protected bool? GetAsBool(ITagAttribute attribute, TagModel model)
        {
            return (bool?) TypeConverter.To(Get(attribute, model), typeof (bool?));
        }

        public static object Get(ITagAttribute expressions, TagModel model)
        {
            return expressions != null ? expressions.Evaluate(model) : null;
        }

        public static T? GetAs<T>(ITagAttribute expressions, TagModel model) where T : struct
        {
            string result = GetAsString(expressions, model);
            return result != null ? EnumParser<T>.Parse(result) : default(T?);
        }

    }
}
