using System;
using System.Reflection;

namespace org.SharpTiles.Documentation
{
    public class AttributeInfo
    {
        public string Name { get; }
        public Type DeclaringType { get; }

        public AttributeInfo(string name, Type declaringType)
        {
            Name = name;
            DeclaringType = declaringType;
        }

        public AttributeInfo(PropertyInfo propertyInfo)
        {
            Name = propertyInfo.Name;
            DeclaringType = propertyInfo.DeclaringType;
        }
    }
}