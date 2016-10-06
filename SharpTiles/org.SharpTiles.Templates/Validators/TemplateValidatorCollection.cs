using System.Linq;
using org.SharpTiles.Tags.Creators;

namespace org.SharpTiles.Templates.Validators
{
    public class TemplateValidatorCollection : ITemplateValidator
    {
        private readonly ITemplateValidator[] _validators;

        public TemplateValidatorCollection(params ITemplateValidator[] validators)
        {
            _validators = validators ?? new ITemplateValidator[0];
        }

        public void Validate(ParsedTemplate template)
        {
            foreach (var validator in _validators)
            {
                validator?.Validate(template);
            }   
        }

        public override string ToString() => $"TemplateValidatorCollection[{string.Join(",",_validators.Select(v => v?.GetType().Name))}]";
    }
}