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
    public abstract class TernaryExpressionParser<T> : IExpressionParser where T : TernaryExpression
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
            get { return new[] {AdditionalToken}; }
        }

       
        public void Parse(ExpressionParserHelper parseHelper)
        {
            Token token = parseHelper.Current;
            if (token.Contents.Equals(AdditionalToken.Token))
            {
                if (!parseHelper.AnyOnStack(ParsedTypes))
                {
                    throw ParseException.ExpectedToken(DistinctToken.Token, token.Contents, $"TernaryExpressionParser<{typeof(T).Name}>").Decorate(token);
                }
                parseHelper.Expect(AdditionalToken.Token);
                parseHelper.Reduce(typeof(T));
                return;
            }
            if (ShouldReduce(parseHelper))
            {
                parseHelper.Reduce(typeof (T));
            }
            Expression first = parseHelper.Pop();
            parseHelper.Expect(DistinctToken.Token);
            TernaryExpression subResult = Create(first);
            subResult.Token = token;
            subResult.FirstSign = DistinctToken.Token;
            subResult.SecondSign = AdditionalToken.Token;
            parseHelper.Push(subResult);
            parseHelper.ParseExpression();
        }

        private bool ShouldReduce(ExpressionParserHelper parseHelper)
        {
            if (parseHelper.PeekOnStack == null) return true;
            var myPriority = OperatorPrecedence.Of(typeof (T));
            var stackPriority = OperatorPrecedence.Of(parseHelper.PeekOnStack.GetType());
            if (myPriority != stackPriority) return true;
            return false;
        }

        public bool Reduce(ExpressionParserHelper parseHelper, Expression you, int priority)
        {
            bool reduced = OperatorPrecedence.IsHigherThan(you.GetType(), priority);
            if (reduced)
            {
                var subResult = ((T) you);
                if (subResult.Second == null)
                {
                    subResult.FillInSecond(parseHelper.Pop());
                }
                else
                {
                    subResult.FillInThird(parseHelper.Pop());
                }
            }
            return reduced;
        }

        #endregion

        public abstract T Create(Expression lhs);
    }
}
