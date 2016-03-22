using System;
using System.Collections.Generic;
using System.Linq;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.Creators;
using org.SharpTiles.Templates;

namespace org.SharpTiles.AST.Nodes
{
    public abstract class BaseNode<T>  where T : BaseNode<T>
    {
        protected List<INode> _children;
        public BaseNode()
        {
            _children = new List<INode>();
        }

        internal INode[] Childs=> _children.ToArray();

        public T Add(INode child)
        {
            _children.Add(child);
            return (T) this;
        }

        protected INode Harvest(ITemplatePart templatePart)
        {
            var node = Yield(templatePart);
            if (node != null)
            {
                Add(node);
            }
            return node;
        }

        protected INode Yield(ITemplatePart templatePart)
        {
            var node = YieldTag(templatePart as TagPart) ??
                       YieldText(templatePart as TextPart) ??
                       YieldExpression(templatePart as ExpressionPart);
            return node;
        }

        private INode YieldTag(TagPart tagPart)
        {
            if (tagPart == null) return null;
            return tagPart.Tag is ITagWithInnerTemplate
                ? new TemplateContainerNode(tagPart) : 
                  new TagNode(tagPart);
        }

        private INode YieldText(TextPart textPart)
        {
            if (textPart == null) return null;
            return  new TextNode(textPart);
        }

        private INode YieldExpression(ExpressionPart expressionPart)
        {
            if (expressionPart == null) return null;
            var node = new ExpressionNode(expressionPart);
            return node;
        }

        public virtual bool Prune(AST.Options options)
        {
            if (options.HasFlag(AST.Options.InlineTemplates))
            {
                var expanded = new List<INode>();
                foreach (var child in _children)
                {
                    var templateChild = child as TemplateNode;
                    if (templateChild!=null) expanded.AddRange(templateChild.Nodes);
                    else expanded.Add(child);
                }
                _children = expanded;
            }
            _children = _children.Where(c => !c.Prune(options)).ToList();

            if (options.HasFlag(AST.Options.ExcludeTemplateContainers))
            {
                var expanded = new List<INode>();
                foreach (var child in _children)
                {
                    var containerChild = child as TemplateContainerNode;
                    if (containerChild != null)
                        expanded.AddRange(containerChild.Nodes);
                    else
                        expanded.Add(child);
                }
                _children = expanded;
            }
            return _children.Count == 0;
        }

        
    }
}