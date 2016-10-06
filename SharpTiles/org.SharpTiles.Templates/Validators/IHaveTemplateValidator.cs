using org.SharpTiles.Tags.Creators;

namespace org.SharpTiles.Templates.Validators
{
    public interface IHaveTemplateValidator
    {
        ITemplateValidator TemplateValidator { get; }
    }
}