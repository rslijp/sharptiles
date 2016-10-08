using System;
using System.Globalization;
using System.Reflection;
using org.SharpTiles.Tags;

namespace org.SharpTiles.Templates.Validators
{
    public class DateTagAttributeValidator : TagAttributeValidator
    {
        protected override void ValidateProperty(ITag tag, PropertyInfo propertyInfo)
        {
            if (tag == null || propertyInfo == null)
                return;

            var dateType = propertyInfo.GetCustomAttribute<DatePropertyTypeAttribute>();
            if (dateType == null)
                return;

            var value = propertyInfo.GetValue(tag) as ITagAttribute;
            if (!(value?.IsConstant ?? false))
                return;

            var text = value.ConstantValue.ToString();
            try
            {
                DateTime.ParseExact(text, dateType.Format, CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            {
                throw InvalidValue(propertyInfo.Name, text, dateType.Format);
            }
        }

        public static TagException InvalidValue(string propertyName, string value, string dateFormat)
        {
            return new TagException($"{propertyName}: Can't parse '{value}' to date using pattern {dateFormat}.");
        }
    }
}