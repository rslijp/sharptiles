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
            ITagAttribute value = null;
            EnumProperyTypeAttribute enumType;
            try
            {
                if (tag == null || propertyInfo == null)
                    return;

                enumType = propertyInfo.GetCustomAttribute<EnumProperyTypeAttribute>();
                if (enumType == null)
                    return;

                value = propertyInfo.GetValue(tag) as ITagAttribute;
                if (!(value?.IsConstant ?? false))
                    return;

                if (enumType.EnumValues.Cast<object>().Select(v => v.ToString().ToLowerInvariant()).ToList().Contains(value.ConstantValue.ToString().ToLowerInvariant()))
                    return;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e); // Sweep under the carpet...
                return;
            }
            throw InvalidValueException(value, enumType.EnumValues);
        }

        public static TagException InvalidValueException(object value, Array possibleValues)
        {
            return new TagException($"Value '{value}' is not allowed. Possible values are: {string.Join(", ", possibleValues.Cast<object>().Select(v => v.ToString()))}");
        }
    }
}