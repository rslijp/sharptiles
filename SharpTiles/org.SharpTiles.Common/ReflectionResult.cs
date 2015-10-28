namespace org.SharpTiles.Common
{
    public class ReflectionResult
    {
        public ReflectionException ReflectionException { get; set; }
        public object Result { get; set; }

        public bool Partial { get; set; }

        public bool Full { get; set; }
    }
}