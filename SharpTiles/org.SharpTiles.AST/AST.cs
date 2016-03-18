using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using org.SharpTiles.AST.Nodes;
using org.SharpTiles.Tags.Creators;

namespace org.SharpTiles.AST
{
    [DataContract]
    public class AST : BaseNode<AST>, INode
    {
        [Flags]
        public enum Options
        {
            None = 0,
            TrimEmptyTextNodes = 1,
            TrimAllTextNodes = 2,
            FlatExpression = 4,
            DontTrackContext = 8,
            PruneTemplates = 16,
            InlineTemplates = 32,
            NaturalLanguage = 64,
            PruneRawTexts = 64
        }


        public AST()
        {
        }

        public AST(ParsedTemplate source, Options options=Options.None, string name = null) : this()
        {
            Name = name;
            Context = new Context(1, 1);
            Yield(this,source);
            Prune(options);
        }

        public override bool Prune(Options options)
        {
            if (options.HasFlag(Options.DontTrackContext))
            {
                Context = null;
            }
            return base.Prune(options);
        }

        public string Raw => null;

     
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
        public string Name { get; }
       
        [DataMember]
        public NodeType Type => NodeType.Root;

        [DataMember(EmitDefaultValue = false)]
        public Context Context { get; private set; }
        public AST At(int line, int index)
        {
            Context = new Context(line, index);
            return this;
        }
        [DataMember]
        public INode[] Nodes => _children.ToArray();

        public override string ToString()
        {
            return "#AST\r\n"+string.Join("\r\n",Nodes.Select(n=>n.ToString().Replace("\r\n", "\r\n\t")));
        }

       
    }
}
