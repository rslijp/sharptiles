/*
 * SharpTiles, R.Z. Slijp(2008), www.sharptiles.org
 *
 * This file is part of SharpTiles.
 * 
 * SharpTiles is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * SharpTiles is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with SharpTiles.  If not, see <http://www.gnu.org/licenses/>.
 */
 using System;
using org.SharpTiles.Common;

namespace org.SharpTiles.Expressions
{
    public class ExpressionParseException : ParseException
    {
        public ExpressionParseException(string msg) : base(msg)
        {
        }

        public static PartialExceptionWithContext<ExpressionParseException> ExpressionStackNotEmptyOnYield(
            Expression expression)
        {
            String msg = String.Format("Expression stack not empty on yield, '{0}'", expression.GetType());
            return MakePartial(new ExpressionParseException(msg));
        }

        public static PartialExceptionWithContext<ExpressionParseException> MissingExpression(string part)
        {
            String msg = String.Format("Can't evaluate {0} is missing", part);
            return MakePartial(new ExpressionParseException(msg));
        }

        public static PartialExceptionWithContext<ExpressionParseException> UnexpectedNullValue(string part)
        {
            String msg = String.Format("{0} results in an unexpected null value.", part);
            return MakePartial(new ExpressionParseException(msg));
        }

        public static PartialExceptionWithContext<ExpressionParseException> ExpressionEndedButTokensLeft()
        {
            return MakePartial(new ExpressionParseException("Expression ended but more tokens left to parse"));
        }

        public static PartialExceptionWithContext<ExpressionParseException> BracketsAreNotUsedInFunction(string method)
        {
            String msg = String.Format("'{0}' not available. Brackes are not used in a function.", method);
            return MakePartial(new ExpressionParseException(msg));
        }


        public static PartialExceptionWithContext<ExpressionParseException> BracketsAreUsedInFunction(string method)
        {
            String msg = String.Format("Method '{0}' not available. Brackes are used in a function.", method);
            return MakePartial(new ExpressionParseException(msg));
        }

        public static PartialExceptionWithContext<ExpressionParseException> NestedNodeAlreadySet()
        {
            return MakePartial(new ExpressionParseException("The nested node is already set."));
        }

        public static PartialExceptionWithContext<ExpressionParseException> SquareBracketsAreNotUsedInPropertyAccess()
        {
            return MakePartial(new ExpressionParseException("The bracket expression can only be used in combination with property access."));
        }
        

        public static PartialExceptionWithContext<ExpressionParseException> UnExpectedParameter(Expression exp)
        {
            String msg = String.Format("Unexpected expression {0}; all parameters alread set.", exp);
            return MakePartial(new ExpressionParseException(msg));
        }

        public static PartialExceptionWithContext<ExpressionParseException> ExpectedMoreParameter(Expression exp,
                                                                                                  int
                                                                                                      currentNumberOfArguments,
                                                                                                  int
                                                                                                      expectedNumberOfArguments)
        {
            String msg = String.Format("Expression {0}; Requires {0} more parameters.", exp);
            return MakePartial(new ExpressionParseException(msg));
        }
    }
}
