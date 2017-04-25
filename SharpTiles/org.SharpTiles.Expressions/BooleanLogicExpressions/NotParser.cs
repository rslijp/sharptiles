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
 using System.Collections.Generic;
 using org.SharpTiles.Common;

namespace org.SharpTiles.Expressions
{
    public class NotParser : IExpressionParser
    {
        private static string ADDITIONAL_SIGN = "not";
        private static string NOT_SIGN = "!";
        private static ICollection<string> SIGNS =new HashSet<string>(new[] {NOT_SIGN, ADDITIONAL_SIGN});

        #region IExpressionParser Members

        public Type[] ParsedTypes
        {
            get { return new[] {typeof (Not)}; }
        }

        public ExpressionOperatorSign DistinctToken
        {
            get { return new ExpressionOperatorSign(NOT_SIGN, false); }
        }

        public ExpressionOperatorSign[] AdditionalTokens
        {
            get
            {
                return new[]
                           {
                               new ExpressionOperatorSign(ADDITIONAL_SIGN, true)
                           };
            }
        }


        public void Parse(ExpressionParserHelper parseHelper)
        {
            Token token = parseHelper.Current;
            parseHelper.Expect(nameof(NotParser), SIGNS);
            parseHelper.ParseExpression();
             var not = new Not(parseHelper.Pop());
            not.Token = token;
            parseHelper.Push(not);
        }

        public bool Reduce(ExpressionParserHelper parseHelper, Expression you, int priorty)
        {
            return false;
        }

        #endregion
    }
}
