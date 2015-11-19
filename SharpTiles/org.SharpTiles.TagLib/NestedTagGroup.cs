using System;

namespace org.SharpTiles.Tags
{
    public class NestedTagGroup : BaseTagGroup<NestedTagGroup>, ITagGroup
    {
        public NestedTagGroup(string name, params Type[] tagTypes)
        {
            Name = name;
            foreach (var tagType in tagTypes)
            {
                Register(tagType);
            }
        }
         
        public override string Name { get; }
    }
}