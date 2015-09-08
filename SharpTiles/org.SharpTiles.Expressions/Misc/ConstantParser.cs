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
    public class ConstantParser : IExpressionParser
    {
        public static string CONSTANT_SIGN = "'";

        #region IExpressionParser Members

        public Type[] ParsedTypes
        {
            get { return new[] {typeof (Constant)}; }
        }

        public ExpressionOperatorSign DistinctToken
        {
            get { return new ExpressionOperatorSign(CONSTANT_SIGN, false); }
        }

        public ExpressionOperatorSign[] AdditionalTokens
        {
            get { return null; }
        }


        public void Parse(ExpressionParserHelper parseHelper)
        {
            Token token = parseHelper.Current;
            parseHelper.Expect(CONSTANT_SIGN);
            String constantStr = parseHelper.ReadUntil(TokenType.Seperator, CONSTANT_SIGN);
            parseHelper.Expect(CONSTANT_SIGN);
            var constant = new Constant(constantStr);
            constant.Token = token;
            parseHelper.Push(constant);
        }

        public bool Reduce(ExpressionParserHelper parseHelper, Expression you, int priorty)
        {
            return false;
        }

        #endregion
    }
}
