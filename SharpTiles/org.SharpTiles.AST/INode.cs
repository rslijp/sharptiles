namespace org.SharpTiles.AST
{
    public interface INode
    {
        NodeType Type { get; }

        bool Prune(AST.Options options);
    }
}