using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Templates;

namespace org.SharpTiles.AST.Nodes
{
    public class TemplateContainerNode : TagNode
    {
        public TemplateContainerNode(TagPart tagPart) : base(tagPart)
        {
        }

        public TemplateContainerNode(string groupName, string tagName) : base(groupName, tagName) { }
    }
}