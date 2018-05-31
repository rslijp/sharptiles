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
 using System.Runtime.Serialization;
 using org.SharpTiles.Documentation.DocumentationAttributes;
 using org.SharpTiles.Tags;
using org.SharpTiles.Tags.DefaultPropertyValues;

namespace org.SharpTiles.Documentation
{
    [DataContract]
    public enum TagAttributeDataType
    {
        Text,
        Number,
        Boolean,
        Enum
    }

    [DataContract]
    public class PropertyDocumentation : IDescriptionElement
    {
        private readonly ResourceKeyStack _messagePath;
        private readonly PropertyInfo _property;
        private readonly string _propertyName;
        private bool _required;
        private string _default;
        private DescriptionAttribute _description;
        private EnumValue[] _enumValues;

        public PropertyDocumentation(ResourceKeyStack messagePath, PropertyInfo property)
        {
            _property = property;
            _messagePath = messagePath.BranchFor(property);
            IsRequired(property);
            DetermineDefault(property);
            _description = DescriptionAttribute.Harvest(property)??_messagePath.Description;
            _enumValues = GetEnumValues(_property.GetCustomAttribute<EnumProperyTypeAttribute>(false)?.EnumType);
            DataType = DetermineDataType(_property);
            Annotations = GetAnnotations(_property);
        }

        private string[] GetAnnotations(PropertyInfo property)
        {
            return property.GetCustomAttributes<PropertyAnnotationAttribute>().Select(a => a.Annotation).ToArray();
        }

        [DataMember]
        public TagAttributeDataType DataType { get; }

        [DataMember]
        public string[] Annotations { get; }

        private TagAttributeDataType DetermineDataType(PropertyInfo property)
        {
            if (property.GetCustomAttribute<EnumProperyTypeAttribute>() != null)
            {
                return TagAttributeDataType.Enum;
            }
            if (property.GetCustomAttribute<NumberPropertyTypeAttribute>() != null)
            {
                return TagAttributeDataType.Number;
            }
            if (property.GetCustomAttribute<BooleanPropertyTypeAttribute>() != null)
            {
                return TagAttributeDataType.Boolean;
            }
            return TagAttributeDataType.Text;
        }

        public PropertyDocumentation(ResourceKeyStack messagePath, PropertyAttribute property)
        {
            _propertyName = property.Name;
            _messagePath = messagePath.BranchFor(new AttributeInfo(property.Name, property.DeclaringType));
            _required = property.Required;
            _default = property.DefaultValue;
            _description = property.Description != null ? new DescriptionAttribute(property.Description) : _messagePath.Description;
            _enumValues = property.DeclaringType.IsEnum ? GetEnumValues(property.DeclaringType) : null;
        }

        [DataMember]
        public bool Required
        {
            get { return _required; }
        }

        [DataMember]
        public string Default
        {
            get { return _default; }
        }

        #region IDescriptionElement Members

        [DataMember]
        public string Id
        {
            get { return _messagePath.Id; }
        }

        [DataMember]
        public DescriptionAttribute Description => _description;
        public string DescriptionKey => _messagePath.DescriptionKey;

        [DataMember]
        public string Name
        {
            get { return _property.Name; }
        }

        #endregion

        private void IsRequired(PropertyInfo property)
        {
            foreach (object attribute in property.GetCustomAttributes(false))
            {
                _required |= attribute is RequiredAttribute;
            }
        }

        private void DetermineDefault(PropertyInfo property)
        {
            var fallBack = (IDefaultPropertyValue)property.GetCustomAttributes(typeof(IDefaultPropertyValue), false).FirstOrDefault();
            if(fallBack!=null)
            {
                _default = fallBack.ToString();
            }
            ;
        }

        [DataMember]
        public EnumValue[] EnumValues => _enumValues;

        private EnumValue[] GetEnumValues(Type enumType)
        {

            return enumType != null ? Enum.GetValues(enumType)
                .Cast<object>()
                .Select(v => EnumValue.Create(enumType, v))
                .ToArray() : null;
        }

        [DataContract]
        public class EnumValue
        {
            [DataMember]
            public object Value { get; }
            [DataMember]
            public DescriptionAttribute Description { get; }

            public EnumValue(object value, DescriptionAttribute description)
            {
                Value = value;
                Description = description;
            }

            public static EnumValue Create(Type enumType, object value)
            {
                var member = GetMember(enumType, value);
                var description = member.GetCustomAttribute<DescriptionAttribute>(false);
                return new EnumValue(value, description);
            }

            private static MemberInfo GetMember(Type enumType, object value)
            {
                return enumType.GetField(Enum.GetName(enumType, value));
            }

        }

    }
}
