using System;
using System.ComponentModel;
using org.SharpTiles.Common;

namespace org.SharpTiles.Expressions
{
    [Category("StringExpression")]
    public class ConcatWithSpace : TupleExpression
    {
        public ConcatWithSpace(Expression lhs) : base(lhs)
        {
        }

        public ConcatWithSpace(Expression lhs, Expression rhs)
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
            var lhs = _lhs?.Evaluate(model)?.ToString();
            var rhs = _rhs?.Evaluate(model)?.ToString();
            if (string.IsNullOrEmpty(lhs))
                return rhs ?? string.Empty;
            if (string.IsNullOrEmpty(rhs))
                return lhs;
            return lhs + " " + rhs;
        }
    }
}