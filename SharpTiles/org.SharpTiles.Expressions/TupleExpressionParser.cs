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
    public abstract class TupleExpressionParser<T> : IExpressionParser where T : TupleExpression
    {
        public abstract ExpressionOperatorSign AdditionalToken { get; }

        #region IExpressionParser Members

        public Type[] ParsedTypes
        {
            get { return new[] {typeof (T)}; }
        }

        public abstract ExpressionOperatorSign DistinctToken { get; }

        public ExpressionOperatorSign[] AdditionalTokens
        {
            get { return AdditionalToken != null ? new[] {AdditionalToken} : null; }
        }

        public void Parse(ExpressionParserHelper parseHelper)
        {
            Token token = parseHelper.Current;
            parseHelper.Reduce(typeof (T));
            Expression lhs = parseHelper.Pop();
            if (AdditionalToken != null)
            {
                parseHelper.Expect(DistinctToken.Token, AdditionalToken.Token);
            }
            else
            {
                parseHelper.Expect(DistinctToken.Token);
            }
            TupleExpression subResult = Create(lhs);
            subResult.Token = token;
            parseHelper.Push(subResult);
            Expression.Parse(parseHelper);
            if (subResult.Rhs == null)
            {
                subResult.FillInRhs(parseHelper.Pop());
            }
        }

        public bool Reduce(ExpressionParserHelper parseHelper, Expression you, int priority)
        {
            bool reduced = OperatorPrecedence.IsHigherThan(you.GetType(), priority);
            if (reduced)
            {
                ((T) you).FillInRhs(parseHelper.Pop());
            }
            return reduced;
        }

        #endregion

        public abstract T Create(Expression lhs);
    }
}
