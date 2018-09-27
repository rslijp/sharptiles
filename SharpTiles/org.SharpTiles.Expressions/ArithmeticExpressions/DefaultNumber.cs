using org.SharpTiles.Common;

namespace org.SharpTiles.Expressions
{
    public class DefaultNumber : ArithmeticExpression
    {
        public DefaultNumber(Expression lhs)
            : base(lhs)
        {
        }

        public DefaultNumber(Expression lhs, Expression rhs)
            : base(lhs)
        {
            _rhs = rhs;
        }

        public override object InternalEvaluate(IModel model)
        {
            return Lhs?.Evaluate(model) ?? RhsTyped(model);
        }
    }
}