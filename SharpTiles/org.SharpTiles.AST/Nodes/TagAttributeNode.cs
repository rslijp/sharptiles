using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using org.SharpTiles.Common;
using org.SharpTiles.Tags.DefaultPropertyValues;

namespace org.SharpTiles.AST.Nodes
{
    [DataContract]
    public class TagAttributeNode
    {
        public TagAttributeNode()
        {
            
        }
        public TagAttributeNode(string name, params INode[] nodes)
        {
            Name = name;
            Nodes = nodes;
            Raw= RawStringHelper.Build(nodes);
        }
        public TagAttributeNode(params INode[] nodes):this(null, nodes) { }
        public TagAttributeNode(IEnumerable<INode> nodes):this(null, nodes.ToArray()) { }

        public TagAttributeNode(string name, TagDefaultValue defaultValue)
        {
            Name = name;
            Default = defaultValue?.Value.ToString();
        }

        public TagAttributeNode(string name, TagDefaultProperty defaultProperty)
        {
            Name = name;
            Default = "${"+defaultProperty?.PropertyName+"}";
        }

        [DataMember]
        public string Name { get; internal set; }

        [DataMember]
        public string Raw { get; internal set; }

        [DataMember]
        public INode[] Nodes { get; } = new INode[0];

        [DataMember]
        public string Default { get; }

        public bool IsPresent => Nodes.Length > 0;
    }
}
