using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Templates;

namespace org.SharpTiles.AST.Nodes
{
    [DataContract]
    public class TagNode : BaseNode<TagNode>, INode
    {
        private readonly IDictionary<string, INode[]> _attributes;

        public TagNode(string groupName, string tagName) : this()
        {
            Group = groupName;
            Name = tagName;
        }

        public TagNode(TagPart tagPart) : this()
        {
            var tag = tagPart.Tag;
            Group = tag.Group.Name;
            Name = tag.TagName;
            YieldAttributes(tag);
            YieldBody(tag);
            Context = tagPart.Context;
        }

       
        public TagNode() : base()
        {
            _attributes=new SortedDictionary<string, INode[]>();
        }

        [DataMember]
        public string Group { get; private set; }

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public IDictionary<string, INode[]> Attributes => new ReadOnlyDictionary<string, INode[]>(_attributes);

        [DataMember]
        public INode[] Nodes => _childs.ToArray();


        [DataMember]
        public NodeType Type => NodeType.Tag;

        [DataMember]
        public Context Context { get; private set; }

        public TagNode At(int line, int index)
        {
            Context = new Context(line, index);
            return this;
        }

        public TagNode With(string name, params INode[] value)
        {
            _attributes.Add(name,value);
            return this;
        }

        public TagNode With(string name, string value, Context context=null)
        {
            _attributes.Add(name, new INode[] { new TextNode(value) {Context = context} });
            return this;
        }

        private void YieldAttributes(ITag tag)
        {
            foreach (var prop in tag.GetType().GetProperties())
            {
                if (!typeof(ITagAttribute).IsAssignableFrom(prop.PropertyType)) continue;
                var value = prop.GetValue(tag) as ITagAttribute;
                if (value == null) continue;
                if (prop.Name.Equals("Body")) continue;
                HandleConstant(prop.Name, value as ConstantAttribute);
                HandleExpression(prop.Name, value as TemplateAttribute);
            }
        }

        private void HandleConstant(string name, ConstantAttribute value)
        {
            if (value == null) return;
            With(name, value.ConstantValue.ToString(), value.Context);
        }

        private void HandleExpression(string name, TemplateAttribute value)
        {
            var nodes=value.
                TemplateParsed.
                Select(Yield).
                Where(n=>n!=null).
                ToArray();
            With(name, nodes);
        }

        private void YieldBody(ITag tag)
        {
            var body = tag.GetType().GetProperty("Body");
            if (body == null) return;
            if (!typeof(ITagAttribute).IsAssignableFrom(body.PropertyType)) return;
            var value = body.GetValue(tag) as TemplateAttribute;
            if (value == null) return;
            foreach (var templatePart in value.TemplateParsed)
            {
                Harvest(templatePart);
            }
        }

        public override bool Prune(AST.Options options)
        {
            if (options.HasFlag(AST.Options.DontTrackContext))
            {
                foreach (var attribute in Attributes.Values.SelectMany(x=>x))
                {
                    attribute.Prune(AST.Options.DontTrackContext);
                }
                Context = null;
            }
            var prune =base.Prune(options);
            foreach (var attribute in _attributes.Keys.ToList())
            {
                var c = _attributes[attribute].All(a => a.Prune(options));
                if (c) _attributes.Remove(attribute);
            }
            return prune && _attributes.Count==0;
        }

        public override string ToString()
        {
            var title = $"##{Group}:{Name}";
            string attributes = "";
            string nodes = "";

            if (Attributes.Any()) {
                attributes = "\r\n###Attributes\r\n" +
                             string.Join("\r\n",Attributes.Select(a => a.Key + ":" + string.Join(">>",a.Value.Select(n => n.ToString().Replace("\r\n", "\r\n\t")))));
            }
            if (Nodes.Any())
            {
                if (Attributes.Any())
                {
                    attributes += "\r\n";
                }
                nodes = "\r\n###Nodes\r\n" + string.Join("\r\n", Nodes.Select(n => n.ToString().Replace("\r\n", "\r\n\t")));
            }
            return title + attributes + nodes;
        }
    }
}
