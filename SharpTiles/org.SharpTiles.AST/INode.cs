using org.SharpTiles.Common;

namespace org.SharpTiles.AST
{
    public interface INode
    {
        NodeType Type { get; }

        Context Context { get; }
        bool Prune(AST.Options options);

        string Raw { get; }
    }
}