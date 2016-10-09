using System.Globalization;
using System.Reflection;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;

namespace org.SharpTiles.Templates.Validators
{
    public class BooleanTagAttributeValidator : TagAttributeValidator
    {
        protected override void ValidateProperty(ITag tag, PropertyInfo propertyInfo)
        {
            if (tag == null || propertyInfo == null)
                return;

            var numberType = propertyInfo.GetCustomAttribute<BooleanPropertyTypeAttribute>();
            if (numberType == null)
                return;

            var value = propertyInfo.GetValue(tag) as ITagAttribute;
            if (!(value?.IsConstant ?? false))
                return;

            var text = value.ConstantValue.ToString();
            var result = TypeConverter.TryTo(text, typeof(bool), CultureInfo.InvariantCulture);
            if (result == null)
                throw InvalidValue(propertyInfo.Name, text);
        }

        public static TagException InvalidValue(string propertyName, string value)
        {
            return new TagException($"{propertyName}: Can't parse '{value}' to a boolean value.");
        }
    }
}