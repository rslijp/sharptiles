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
using System.Linq;
using org.SharpTiles.Common;
using org.SharpTiles.Expressions.Functions;
using org.SharpTiles.Expressions.Math;

namespace org.SharpTiles.Expressions
{
    public class MathFunctionParser : IExpressionParser
    {
        private IFunctionDefinition _func;

        public MathFunctionParser(IFunctionDefinition func)
        {
            _func = func;
        }

        #region IExpressionParser Members

        public Type[] ParsedTypes
        {
            get { return new[] {_func.GetType()}; }
        }

        public ExpressionOperatorSign DistinctToken
        {
            get { return new ExpressionOperatorSign(FunctionSign, false); }
        }

        private string FunctionSign
        {
            get { return _func.Name+BracketsParser.BRACKETS_OPEN; }
        }

        public ExpressionOperatorSign[] AdditionalTokens
        {
            get { return null; }
        }


        public void Parse(ExpressionParserHelper parseHelper)
        {
            Token start = parseHelper.Current;
            parseHelper.PushNewTokenConfiguration(true, true,'\\', new string[] {_func.Name}, Expression.WHITESPACE_OPERANDS, null, ResetIndex.CurrentAndLookAhead);
            parseHelper.Expect(_func.Name);
            parseHelper.PopTokenConfiguration(ResetIndex.LookAhead);
            var function = new Function(_func);
            function.Token = start;
            parseHelper.Push(function);
            parseHelper.Next();
            if (function.Arguments.Length == 0)
            {
                BracketsParser.MunchEmptyBrackets(parseHelper);
            }
            else
            {
                var bp = new BracketsParser(true, function.Arguments.Length);
                bp.Parse(parseHelper);
            }
        }

        public bool Reduce(ExpressionParserHelper parseHelper, Expression you, int priorty)
        {
            return false;
        }

        #endregion
        
    }
}
