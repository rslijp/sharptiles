using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using org.SharpTiles.Common;
using org.SharpTiles.Expressions;
using org.SharpTiles.Templates;

namespace org.SharpTiles.AST.Nodes
{
    [DataContract]
    public class ExpressionNode : INode
    {

        protected List<ExpressionNode> _childs;

        public ExpressionNode()
        {
            _childs = new List<ExpressionNode>();
        }

        public ExpressionNode(ExpressionPart expressionPart) : this(expressionPart.Expression)
        {
           
        }

        public ExpressionNode(Expression expression) : this()
        {
            Context = expression.Token.Context;
            Value = expression.AsParsable();
            Yield(expression);
        }


        public ExpressionNode(string value, string name, Type returnType) : this()
        {
            Value = value?.Trim();
            Name = name;
            ReturnType = returnType;
        }
        

        [DataMember]
        public string Value { get; set; }

        public string Raw => $"${{{Value}}}";

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Type ReturnType { get; set; }

        [DataMember]
        public NodeType Type => NodeType.Expression;

        [DataMember(EmitDefaultValue = false)]
        public Context Context { get; private set; }

        public ExpressionNode At(int line, int index)
        {
            Context = new Context(line,index);
            return this;
        }

        public bool Prune(AST.Options options)
        {
            if (options.HasFlag(AST.Options.DontTrackContext)) Context=null;
            if (options.HasFlag(AST.Options.FlatExpression)) _childs.Clear();
            foreach (var expressionNode in _childs)
            {
                expressionNode.Prune(options);
            }
            return false;
        }

        [DataMember]
        public ExpressionNode[] Nodes => _childs.ToArray();

        public ExpressionNode Add(ExpressionNode node)
        {
            _childs.Add(node);
            return this;
        }


        private void Yield(Expression expr)
        {
            Name = expr.GetType().Name;
            HandlePropertyOrConstant(expr as PropertyOrConstant);
            ReturnType = expr.ReturnType;
            CollectLists(expr);
            CollectSingleExpression(expr);
        }

        private void CollectLists(Expression expr)
        {
            foreach (var prop in expr.GetType().GetProperties())
            {
                if (prop.GetCustomAttributes(typeof(InternalAttribute), true).Length > 0) continue;
                if (!typeof (IList<Expression>).IsAssignableFrom(prop.PropertyType)) continue;
                var values = prop.GetValue(expr) as IList<Expression>;
                if (values == null) continue;
                foreach (var value in values)
                {
                    Add(new ExpressionNode(value));
                }
            }
        }

        private void CollectSingleExpression(Expression expr)
        {
            foreach (var prop in expr.GetType().GetProperties())
            {
                if(prop.GetCustomAttributes(typeof(InternalAttribute), true).Length>0) continue;
                if (!typeof (Expression).IsAssignableFrom(prop.PropertyType)) continue;
                var value = prop.GetValue(expr) as Expression;
                if (value == null) continue;
                Add(new ExpressionNode(value));
            }
        }

        private void HandlePropertyOrConstant(PropertyOrConstant propertyOrConstant)
        {
            if (propertyOrConstant == null) return;
            if (propertyOrConstant.IsConstant)
            {
                Name = nameof(Constant);
            }
            else
            {
                Name = nameof(Property);
            }
        }

        

        public override string ToString()
        {
            var expr = "[" + Value + "/" + Name;
            if (Nodes.Any())
            {
                expr += (" " + string.Join(",", Nodes.Select(s => s.ToString())));
            }
            expr+="]";
            return expr;
        }
    }
}
