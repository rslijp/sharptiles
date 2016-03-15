using System;
using System.Collections.Generic;
using System.Linq;
using org.SharpTiles.Tags.Creators;
using org.SharpTiles.Templates;

namespace org.SharpTiles.AST.Nodes
{
    public abstract class BaseNode<T>  where T : BaseNode<T>
    {
        protected List<INode> _childs;
        public BaseNode()
        {
            _childs = new List<INode>();
        }

        public T Add(INode child)
        {
            _childs.Add(child);
            return (T) this;
        }

        protected void Harvest(ITemplatePart templatePart)
        {
            var node = Yield(templatePart);
            if (node != null)
            {
                Add(node);
            }
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
            return new TagNode(tagPart);
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
            _childs = _childs.Where(c => !c.Prune(options)).ToList();
            return _childs.Count == 0;

        }

        
    }
}