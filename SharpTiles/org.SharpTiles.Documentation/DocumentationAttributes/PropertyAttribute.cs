using System;

namespace org.SharpTiles.Documentation.DocumentationAttributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PropertyAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }
        public bool Required { get; }
        public string DefaultValue { get; }
        public Type DeclaringType { get; }

        public PropertyAttribute(string name, string description, Type declaringType = null, bool required = false, string defaultValue = null)
        {
            Name = name;
            Description = description;
            Required = required;
            DefaultValue = defaultValue;
            DeclaringType = declaringType ?? typeof(string);
        }
    }
}