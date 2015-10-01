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

namespace org.SharpTiles.Expressions
{
    public class PropertyOrConstantParser : IExpressionParser
    {
        #region IExpressionParser Members

        public Type[] ParsedTypes
        {
            get { return new[] {typeof (PropertyOrConstant)}; }
        }

        public ExpressionOperatorSign DistinctToken
        {
            get { throw new NotImplementedException("PropertyOrConstant hasn't got a distinct token"); }
        }

        public ExpressionOperatorSign[] AdditionalTokens
        {
            get { return null; }
        }

        public void Parse(ExpressionParserHelper parseHelper)
        {
            var property = new PropertyOrConstant(parseHelper.CurrentTrimmed.Contents);
            property.Token = parseHelper.Current;
            parseHelper.Push(property);
        }

        public bool Reduce(ExpressionParserHelper parseHelper, Expression you, int priorty)
        {
            return false;
        }

        #endregion
    }
}
