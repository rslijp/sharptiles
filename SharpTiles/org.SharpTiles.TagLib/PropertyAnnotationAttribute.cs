using System;

namespace org.SharpTiles.Tags
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PropertyAnnotationAttribute:Attribute
    {
        public string Annotation { get; }

        public PropertyAnnotationAttribute(string annotation)
        {
            Annotation = annotation;
        }
    }
}
