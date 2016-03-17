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
    public class TemplateNode : BaseNode<TemplateNode>, INode
    {
        public TemplateNode()
        {
            
        }

        public TemplateNode(ITemplate source)
        {
            Context = new Context(1,1);
            Yield(source);
        }

        private void Yield(ITemplate source)
        {
            if (source == null) return;
            Yield(source.Template.TemplateParsed);            
        }

        private void Yield(IList<ITemplatePart> parts)
        {
            foreach (var templatePart in parts)
            {
                Harvest(templatePart);
            }
        }

        public override bool Prune(AST.Options options)
        {
            if (options.HasFlag(AST.Options.DontTrackContext)) Context = null;
            if (options.HasFlag(AST.Options.PruneTemplates)) return true;
            base.Prune(options);
            return false;
        }

        public string Raw => RawStringHelper.Build(_childs);


        [DataMember]
        public NodeType Type => NodeType.Template;

        [DataMember(EmitDefaultValue = false)]
        public Context Context { get; private set; }
        
        [DataMember]
        public INode[] Nodes => _childs.ToArray();

        public override string ToString()
        {
            return "#Template\r\n"+string.Join("\r\n",Nodes.Select(n=>n.ToString().Replace("\r\n", "\r\n\t")));
        }

       
    }
}
