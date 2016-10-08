using System;
using org.SharpTiles.Expressions;

namespace org.SharpTiles.Tags
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DatePropertyTypeAttribute : Attribute
    {
        public string Format { get; }

        public DatePropertyTypeAttribute(string format = PatternStrings.DATETIME_FORMAT)
        {
            Format = format;
        }
    }
}