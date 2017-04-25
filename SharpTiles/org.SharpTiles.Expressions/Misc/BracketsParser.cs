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
    public class BracketsParser : IExpressionParser
    {
        public static string BRACKETS_CLOSE = ")";
        public static string BRACKETS_COMMA = ",";
        public static string BRACKETS_OPEN = "(";

        private static ICollection<string> SIGNS = new HashSet<string>(new[] { BRACKETS_CLOSE, BRACKETS_COMMA });

    
        private readonly int _arguments;
        private readonly bool _functionArgument;
        private static readonly Type TYPE = typeof (Brackets);
        public BracketsParser() : this(false, 1)
        {
        }

        public BracketsParser(bool functionArgument, int arguments)
        {
            _functionArgument = functionArgument;
            _arguments = arguments;
        }

        #region IExpressionParser Members

        public Type[] ParsedTypes
        {
            get
            {
                return new[] {TYPE};
            }
        }

        public ExpressionOperatorSign DistinctToken
        {
            get { return new ExpressionOperatorSign(BRACKETS_OPEN, false); }
        }

        public ExpressionOperatorSign[] AdditionalTokens
        {
            get
            {
                return new[]
                           {
                               new ExpressionOperatorSign(BRACKETS_CLOSE, false),
                               new ExpressionOperatorSign(BRACKETS_COMMA, false),
                           };
            }
        }

        
        public void Parse(ExpressionParserHelper parseHelper)
        {
            if (parseHelper.At(BRACKETS_OPEN))
            {
                var token = parseHelper.Current;
                parseHelper.Expect(BRACKETS_OPEN);
                var brackets = new Brackets(_functionArgument, _arguments) {Token = token};
                parseHelper.Push(brackets);
                parseHelper.ParseExpression();
            }
            else
            {
                parseHelper.Reduce(TYPE);
                var nested = parseHelper.Pop();
                parseHelper.Expect(nameof(BracketsParser), SIGNS);
                var brackets = ((Brackets) parseHelper.Top);
                brackets.FillNext(nested);
                if (parseHelper.At(BRACKETS_CLOSE) && brackets.PartOfFunction)
                {
                    FunctionParser.FillArguments(parseHelper);
                }
            }
        }

        public bool Reduce(ExpressionParserHelper parseHelper, Expression you, int priorty)
        {
            return false;
        }

        #endregion

        public static void MunchEmptyBrackets(ExpressionParserHelper parseHelper)
        {
            parseHelper.Expect(BRACKETS_OPEN);
            parseHelper.Read(BRACKETS_CLOSE);

        }
    }
}
