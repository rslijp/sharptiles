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
using System.Reflection;

namespace org.SharpTiles.Tags
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class RequiredAttribute : Attribute
    {
        private static readonly IDictionary<Type, IDictionary<PropertyInfo, RequiredAttribute>> CACHE =
            new Dictionary<Type, IDictionary<PropertyInfo, RequiredAttribute>>();

        public bool IsSet(PropertyInfo property, Object obj)
        {
            Object value = property.GetValue(obj, null);
            return !(value == null || (value is String && String.IsNullOrEmpty((String) value)));
        }

        public static void Check(ITag tag)
        {
            IDictionary<PropertyInfo, RequiredAttribute> requiredProperties = GetRequiredProperties(tag);
            List<string> missingProperties = CollectMissingProperties(requiredProperties, tag);
            if (missingProperties.Count > 0)
            {
                throw TagException.MissingRequiredAttribute(tag.GetType(), missingProperties.ToArray()).Decorate(
                    tag.Context);
            }
        }

        private static List<string> CollectMissingProperties(
            IDictionary<PropertyInfo, RequiredAttribute> requiredProperties, ITag tag)
        {
            var missingProperties = new List<string>();
            foreach (var pair in requiredProperties)
            {
                if (!pair.Value.IsSet(pair.Key, tag))
                {
                    missingProperties.Add(pair.Key.Name);
                }
            }
            return missingProperties;
        }

        private static IDictionary<PropertyInfo, RequiredAttribute> GetRequiredProperties(ITag tag)
        {
            Type type = tag.GetType();
            if (!CACHE.ContainsKey(type))
            {
                CACHE.Add(type, AssembleProperties(tag));
            }
            return CACHE[type];
        }

        private static IDictionary<PropertyInfo, RequiredAttribute> AssembleProperties(ITag tag)
        {
            Type tagType = tag.GetType();
            IDictionary<PropertyInfo, RequiredAttribute> requiredProperties =
                new Dictionary<PropertyInfo, RequiredAttribute>();
            foreach (PropertyInfo property in tagType.GetProperties())
            {
                foreach (Object attribute in property.GetCustomAttributes(false))
                {
                    var requiredValidator = attribute as RequiredAttribute;
                    if (requiredValidator != null)
                    {
                        requiredProperties.Add(property, requiredValidator);
                    }
                }
            }
            return requiredProperties;
        }
    }
}
