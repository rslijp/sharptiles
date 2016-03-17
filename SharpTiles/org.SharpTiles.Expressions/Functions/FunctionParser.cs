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

namespace org.SharpTiles.Expressions
{
    public class FunctionParser : IExpressionParser
    {
        private FunctionLib _lib;

        public FunctionParser(FunctionLib lib)
        {
            _lib = lib;
        }

        #region IExpressionParser Members

        public Type[] ParsedTypes
        {
            get { return _lib.Functions.Select(f=>f.GetType()).ToArray(); }
        }

        public ExpressionOperatorSign DistinctToken
        {
            get { return new ExpressionOperatorSign(FunctionSign, false); }
        }

        private string FunctionSign
        {
            get { return _lib.GroupName + ":"; }
        }

        public ExpressionOperatorSign[] AdditionalTokens
        {
            get { return null; }
        }


        public void Parse(ExpressionParserHelper parseHelper)
        {
            Token start = parseHelper.Current;
            parseHelper.Expect(FunctionSign);
            parseHelper.PushNewTokenConfiguration(true, true, '\\', _lib.Functions.Select(f=>f.Name).ToArray(), Expression.WHITESPACE_OPERANDS, null, ResetIndex.CurrentAndLookAhead);
            Token token = parseHelper.Next();
            var function = _lib.Obtain(token.Contents);
            parseHelper.PopTokenConfiguration(ResetIndex.MaintainPosition);
            parseHelper.Next();
            function.Token = start;
            function.PreFix = start.Contents;
            parseHelper.Push(function);
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

        public static void FillArguments(ExpressionParserHelper parseHelper)
        {
            var brackets = (Brackets) parseHelper.Pop();
            var function = (Function) parseHelper.Top;
            if (brackets.Nodes.Count < function.Arguments.Length)
            {
                throw ExpressionParseException.ExpectedMoreParameter(function, brackets.Nodes.Count,
                                                                     function.Arguments.Length).Decorate(
                    parseHelper.Lookahead ?? parseHelper.Current);
            }
            function.FillNested(brackets);
        }
    }
}
