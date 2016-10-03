using System.Reflection;
using org.SharpTiles.Tags;

namespace org.SharpTiles.Templates.Validators
{
    public abstract class TagAttributeValidator : ITagValidator
    {
        public virtual void Validate(ITag tag)
        {
            if (tag == null)
                return;

            var type = tag.GetType();
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy))
            {
                ValidateProperty(tag, propertyInfo);
            }
        }

        protected abstract void ValidateProperty(ITag tag, PropertyInfo propertyInfo);
    }
}