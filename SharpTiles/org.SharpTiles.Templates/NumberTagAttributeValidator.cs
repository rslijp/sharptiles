using System;
using System.Globalization;
using System.Reflection;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Templates.Validators;

namespace org.SharpTiles.Templates
{
    public class NumberTagAttributeValidator : TagAttributeValidator
    {
        protected override void ValidateProperty(ITag tag, PropertyInfo propertyInfo)
        {
            if (tag == null || propertyInfo == null)
                return;

            var numberType = propertyInfo.GetCustomAttribute<NumberPropertyTypeAttribute>();
            if (numberType == null)
                return;

            var value = propertyInfo.GetValue(tag) as ITagAttribute;
            if (!(value?.IsConstant ?? false))
                return;

            var text = value.ConstantValue.ToString();
            var result = TypeConverter.TryTo(text, typeof(decimal), CultureInfo.InvariantCulture);
            if (result == null)
                throw InvalidValue(propertyInfo.Name, text);
        }

        public static TagException InvalidValue(string propertyName, string value)
        {
            return new TagException($"{propertyName}: Can't parse '{value}' to a numeric value.");
        }
    }
}