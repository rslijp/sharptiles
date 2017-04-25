namespace org.SharpTiles.Expressions
{
    public class ConcatWithSpaceParser : TupleExpressionParser<ConcatWithSpace>
    {
        private static string CONCAT_SIGN = "~";

        public override ExpressionOperatorSign DistinctToken => new ExpressionOperatorSign(CONCAT_SIGN, false);

        public override ExpressionOperatorSign AdditionalToken => null;

        public override ConcatWithSpace Create(Expression lhs)
        {
            return new ConcatWithSpace(lhs);
        }
    }
}
