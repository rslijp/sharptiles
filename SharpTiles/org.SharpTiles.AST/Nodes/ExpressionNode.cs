using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
            Value = expression.ToString();
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

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Type ReturnType { get; set; }

        [DataMember]
        public NodeType Type => NodeType.Expression;

        public bool Prune(AST.Options options)
        {
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
            foreach (var prop in expr.GetType().GetProperties())
            {
                if (!typeof(Expression).IsAssignableFrom(prop.PropertyType)) continue;
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
