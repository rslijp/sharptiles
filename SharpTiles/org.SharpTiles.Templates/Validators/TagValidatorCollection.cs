using org.SharpTiles.Tags;

namespace org.SharpTiles.Templates.Validators
{
    public class TagValidatorCollection : ITagValidator
    {
        private readonly ITagValidator[] _validators;

        public TagValidatorCollection(params ITagValidator[] validators)
        {
            _validators = validators ?? new ITagValidator[0];
        }

        public void Validate(ITag tag)
        {
            foreach (var validator in _validators)
            {
                validator?.Validate(tag);
            }
        }
    }
}