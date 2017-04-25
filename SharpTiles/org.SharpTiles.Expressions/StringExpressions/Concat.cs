using System;
using System.ComponentModel;
using org.SharpTiles.Common;

namespace org.SharpTiles.Expressions
{
    [Category("StringExpression")]
    public class Concat : TupleExpression
    {
        public Concat(Expression lhs) : base(lhs)
        {
        }

        public Concat(Expression lhs, Expression rhs)
            : base(lhs)
        {
            _rhs = rhs;
        }

        public override Type ReturnType => typeof(string);

        public override void GuardTypeSafety()
        {
        }

        public override Type ParameterType => typeof(object);

        public override object InternalEvaluate(IModel model)
        {
            return _lhs?.Evaluate(model)?.ToString() + _rhs?.Evaluate(model);
        }
    }
}