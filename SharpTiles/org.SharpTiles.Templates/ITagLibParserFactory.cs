using org.SharpTiles.Common;
using org.SharpTiles.Tags;

namespace org.SharpTiles.Templates
{
    public interface ITagLibParserFactory
    {
        ITagLibParser Construct(ParseHelper helper, IResourceLocator locator);
        ITag Parse(ParseHelper parser, IResourceLocator locator);
    }
}