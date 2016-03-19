using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.SharpTiles.AST;
using org.SharpTiles.AST.Nodes;

public static class RawStringHelper
{
    public static string Build(IEnumerable<INode> nodes)
    {
        var sb = new StringBuilder();
        foreach (var node in nodes)
        {
            var tagNode = node as TagNode;
            var baseNode = node as BaseNode<TagNode>;
            if (tagNode != null)
            {
                sb.Append("<" + tagNode.Group.ToLower() + ":" + tagNode.Name.ToLower());
                foreach (var pair in tagNode.Attributes.Where(p => p.Value.IsPresent))
                {
                    sb.Append(" " + pair.Key.ToLower() + "='" + pair.Value.Raw + "'");
                }
                sb.Append(">");
            }
            sb.Append(node.Raw);

            if (baseNode != null)
            {
                var templateNodes = baseNode.Childs.Where(t => t.Type == NodeType.Template).Select(s => s.Raw);
                sb.Append(string.Join("", templateNodes));
            }
            if (tagNode != null)
            {
                sb.Append("</" + tagNode.Group.ToLower() + ":" + tagNode.Name.ToLower() + ">");
            }
        }
        return sb.Length > 0 ? sb.ToString() : null;
    }
}