namespace org.SharpTiles.Common
{
    public interface IHaveIndexer
    {
        object this[string property] { get; set; }
    }
}