using System.Reflection;
using org.SharpTiles.Common;
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
                try
                {
                    ValidateProperty(tag, propertyInfo);
                }
                catch (TagException e)
                {
                    throw ExceptionWithContext.MakePartial(e).Decorate(tag.Context);
                }
            }
        }

        protected abstract void ValidateProperty(ITag tag, PropertyInfo propertyInfo);
    }
}