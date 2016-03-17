using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace org.SharpTiles.AST.Nodes
{
    [DataContract]
    public class TagAttributeNode : List<INode>
    {
        public TagAttributeNode() : base()
        {
            
        }
        public TagAttributeNode(params INode[] nodes) : base(nodes)
        {
            Raw= RawStringHelper.Build(this);
        }
        public TagAttributeNode(IEnumerable<INode> nodes):base(nodes)
        {
            Raw = RawStringHelper.Build(this);
        }
        [DataMember]
        public string Raw { get; internal set; }

        
    }
}
