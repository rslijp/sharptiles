namespace org.SharpTiles.Expressions
{
    public class DefaultNumberParser : TupleExpressionParser<DefaultNumber>
    {
        private static string DEFAULT_SIGN = "??";

        public override ExpressionOperatorSign DistinctToken => new ExpressionOperatorSign(DEFAULT_SIGN, false);

        public override ExpressionOperatorSign AdditionalToken => null;

        public override DefaultNumber Create(Expression lhs)
        {
            return new DefaultNumber(lhs);
        }
    }
}