using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using org.SharpTiles.Templates;

namespace org.SharpTiles.AST
{
    [DataContract]
    public class TextNode : INode
    {
        public TextNode(TextPart textPart)
        {
            Context = textPart.Context;
            Value = textPart.ConstantValue.ToString();
        }

        public TextNode(string value)
        {
            Value = value;
        }

        [DataMember]
        public string Value { get; set; }

        public NodeType Type => NodeType.Text;
        public bool Prune(AST.Options options)
        {
            if (options.HasFlag(AST.Options.DontTrackContext)) Context = null;
            return PruneAll(options)||PruneEmpty(options);
        }

        [DataMember]
        public Context Context { get; internal set; }

   
        private bool PruneAll(AST.Options options)
        {
            return options.HasFlag(AST.Options.TrimAllTextNodes);
        }

        public bool PruneEmpty(AST.Options options)
        {
            if (!options.HasFlag(AST.Options.TrimEmptyTextNodes)) return false;
            if (Value.Trim().Length == 0) return true;
            return false;
        }

        public override string ToString()
        {
            return "'"+Value+"'";
        }

        public TextNode At(int line, int index)
        {
            Context=new Context(line,index);
            return this;
        }
    }
}
