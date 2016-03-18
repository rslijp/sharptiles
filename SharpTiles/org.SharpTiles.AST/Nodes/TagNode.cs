using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Templates;

namespace org.SharpTiles.AST.Nodes
{
    [DataContract]
    public class TagNode : BaseNode<TagNode>, INode
    {
        private IDictionary<string, TagAttributeNode> _attributes = new SortedDictionary<string, TagAttributeNode>();

        public TagNode(string groupName, string tagName)
        {
            Group = groupName;
            Name = tagName;
        }

        public TagNode(ITag tag, ParseContext context)
        {
            Group = tag.Group.Name;
            Name = tag.TagName;
            YieldAttributes(tag);
            YieldBody(tag);
            YieldNestedTags(tag as ITagWithNestedTags);
            YieldInnerTemplate(tag as ITagWithInnerTemplate);
            Context = context;
        }

        public TagNode(TagPart tagPart) : this(tagPart.Tag, tagPart.Context)
        {
        }

        private void YieldNestedTags(ITagWithNestedTags tagWithNestedTags)
        {
            if (tagWithNestedTags != null)
            {
                foreach (var nestedTag in tagWithNestedTags.NestedTags)
                {
                    Add(new TagNode(nestedTag, nestedTag.Context));
                }
            }
        }

        private void YieldInnerTemplate(ITagWithInnerTemplate tagWithInnerTemplate)
        {
            if (tagWithInnerTemplate != null && tagWithInnerTemplate.Template != null)
            {
                Add(new TemplateNode(tagWithInnerTemplate.Template));
            }
        }


        [DataMember]
        public string Group { get; private set; }

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public IDictionary<string, TagAttributeNode> Attributes => new ReadOnlyDictionary<string, TagAttributeNode>(_attributes);

        [DataMember]
        public INode[] Nodes => _children.ToArray();


        [DataMember]
        public NodeType Type => NodeType.Tag;

        [DataMember(EmitDefaultValue = false)]
        public string Raw { get; private set; }

        [DataMember(EmitDefaultValue = false)]
        public Context Context { get; private set; }

        public TagNode At(int line, int index)
        {
            Context = new Context(line, index);
            return this;
        }

        public TagNode With(string name, params INode[] attributes)
        {
            _attributes.Add(name,new TagAttributeNode(name, attributes));
            return this;
        }

        public TagNode With(string name, string value, Context context=null)
        {
            return With(name, new TextNode(value) {Context = context});
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
            var childs = new List<INode>();
            foreach (var templatePart in value.TemplateParsed)
            {
                childs.Add(Harvest(templatePart));
            }
            Raw = RawStringHelper.Build(childs);

        }
        

        public override bool Prune(AST.Options options)
        {
            PruneRawText(options);
            PruneNaturalLanguage(options);
            PruneContext(options);
            //Empty tag before prune. Not wise to remove
            if (_attributes.Count == 0 && _children.Count == 0) return false;
            var prune = base.Prune(options);
            foreach (var attribute in _attributes.Keys.ToList())
            {
                var c = _attributes[attribute].Nodes.All(a => a.Prune(options));
                if (c) _attributes.Remove(attribute);
            }
            return prune && _attributes.Count==0;
        }

        private void PruneContext(AST.Options options)
        {
            if (options.HasFlag(AST.Options.DontTrackContext))
            {
                foreach (var attribute in Attributes.Values.SelectMany(x => x.Nodes))
                {
                    attribute.Prune(AST.Options.DontTrackContext);
                }
                Context = null;
            }
        }

        private void PruneRawText(AST.Options options)
        {
            if (options.HasFlag(AST.Options.PruneRawTexts))
            {
                Raw = null;
                foreach (var attr in _attributes.Values)
                {
                    attr.Raw = null;
                }
            }
        }

        private void PruneNaturalLanguage(AST.Options options)
        {
            if (options.HasFlag(AST.Options.NaturalLanguage))
            {
                Name = LanguageHelper.DashProperty(Name);
                Group = LanguageHelper.DashProperty(Name);
                var mapped = new Dictionary<string, TagAttributeNode>();
                foreach (var pair in _attributes)
                {
                    mapped[LanguageHelper.DashProperty(pair.Key)] = pair.Value;
                }
                _attributes = mapped;
            }
        }

        public override string ToString()
        {
            var title = $"##{Group}:{Name}";
            string attributes = "";
            string nodes = "";

            if (Attributes.Any()) {
                attributes = "\r\n###Attributes\r\n" +
                             string.Join("\r\n",Attributes.Select(a => a.Key + ":" + string.Join(">>",a.Value.Nodes.Select(n => n.ToString().Replace("\r\n", "\r\n\t")))));
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
