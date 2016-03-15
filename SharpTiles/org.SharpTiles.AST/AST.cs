using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using org.SharpTiles.AST.Nodes;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.Creators;
using org.SharpTiles.Templates;

namespace org.SharpTiles.AST
{
    [DataContract]
    public class AST : BaseNode<AST>, INode
    {
        public enum Options
        {
            None = 0,
            TrimEmptyTextNodes = 1,
            TrimAllTextNodes = 2,
            FlatExpression = 4,
            DontTrackContext = 8
        }


        public AST()
        {
        }

        public AST(ParsedTemplate source, Options options=Options.None) : this()
        {
            if (!options.HasFlag(Options.DontTrackContext))
            {
                Context=new Context(1,1);
            }
            Yield(this,source);
            Prune(options);
        }

        private static void Yield(AST ast,ParsedTemplate source)
        {
            if (source == null) return;
            ast.Yield(source.TemplateParsed);            
        }

        private void Yield(IList<ITemplatePart> parts)
        {
            foreach (var templatePart in parts)
            {
                Harvest(templatePart);
            }
        }

       
        [DataMember]
        public NodeType Type => NodeType.Root;

        [DataMember]
        public Context Context { get; private set; }
        public AST At(int line, int index)
        {
            Context = new Context(line, index);
            return this;
        }
        [DataMember]
        public INode[] Nodes => _childs.ToArray();

        public override string ToString()
        {
            return "#AST\r\n"+string.Join("\r\n",Nodes.Select(n=>n.ToString().Replace("\r\n", "\r\n\t")));
        }

       
    }
}
