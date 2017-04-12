namespace org.SharpTiles.Expressions
{
    public class ConcatParser : TupleExpressionParser<Concat>
    {
        private static string CONCAT_SIGN = "|";

        public override ExpressionOperatorSign DistinctToken => new ExpressionOperatorSign(CONCAT_SIGN, false);

        public override ExpressionOperatorSign AdditionalToken => null;

        public override Concat Create(Expression lhs)
        {
            return new Concat(lhs);
        }
    }
}
