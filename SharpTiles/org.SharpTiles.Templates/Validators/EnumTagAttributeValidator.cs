using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using org.SharpTiles.Tags;

namespace org.SharpTiles.Templates.Validators
{
    public class EnumTagAttributeValidator : TagAttributeValidator
    {
        protected override void ValidateProperty(ITag tag, PropertyInfo propertyInfo)
        {
            if (tag == null || propertyInfo == null)
                return;

            var enumType = propertyInfo.GetCustomAttribute<EnumProperyTypeAttribute>();
            if (enumType == null)
                return;

            var value = propertyInfo.GetValue(tag) as ITagAttribute;
            if (!(value?.IsConstant ?? false))
                return;

            var enumValues = enumType.EnumValues.Cast<object>().Select(v => v.ToString().ToLowerInvariant()).ToList();

            var text = value.ConstantValue.ToString();
            var names = enumType.Multiple ? text.Split(enumType.Separator) : new[] { text };
            foreach (var name in names.Select(n => n?.Trim()))
            {
                if (name == enumType.Wildcard)
                    continue;

                if (!enumValues.Contains(name.ToLowerInvariant()))
                    throw InvalidValueException(name, enumType.EnumValues);
            }
        }

        public static TagException InvalidValueException(object value, Array possibleValues)
        {
            return new TagException($"Value '{value}' is not allowed. Possible values are: {string.Join(", ", possibleValues.Cast<object>().Select(v => v.ToString()))}");
        }
    }
}