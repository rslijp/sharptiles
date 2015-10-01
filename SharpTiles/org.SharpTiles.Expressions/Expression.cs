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
 using System.Diagnostics;
 using org.SharpTiles.Common;
 using org.SharpTiles.Expressions.Functions;

namespace org.SharpTiles.Expressions
{
    public abstract class Expression
    {
        private static bool TRY_RESOLVE_PROPERTY_INTO_CONSTANT=true;
        private static string[] OPERANDS;

        private static readonly IDictionary<string, IExpressionParser> PARSERS_BY_STR =
            new Dictionary<string, IExpressionParser>();

        private static readonly IDictionary<Type, IExpressionParser> PARSERS_BY_TYPE =
            new Dictionary<Type, IExpressionParser>();

        private static string[] WHITESPACE_OPERANDS;

        static Expression()
        {
            Init();
        }

        protected static void Clear()
        {
            OPERANDS = new String[0];
            WHITESPACE_OPERANDS = new String[0];
            PARSERS_BY_STR.Clear();
            PARSERS_BY_TYPE.Clear();
            FunctionLib.Clear();
        }

        protected static void Init()
        {
            var operands = new List<string>();
            var whiteSpaceOperands = new List<string>();
            FunctionLib.Register(new BaseFunctionLib());

            Register(new AddParser(), operands, whiteSpaceOperands);
            Register(new MinusParser(), operands, whiteSpaceOperands);
            Register(new MultiplyParser(), operands, whiteSpaceOperands);
            Register(new DivideParser(), operands, whiteSpaceOperands);
            Register(new ModuloParser(), operands, whiteSpaceOperands);
            Register(new PowerParser(), operands, whiteSpaceOperands);
            Register(new LessThanParser(), operands, whiteSpaceOperands);
            Register(new GreaterThanParser(), operands, whiteSpaceOperands);
            Register(new LessThanOrEqualParser(), operands, whiteSpaceOperands);
            Register(new GreaterThanOrEqualParser(), operands, whiteSpaceOperands);
            Register(new EqualToParser(), operands, whiteSpaceOperands);
            Register(new NotEqualToParser(), operands, whiteSpaceOperands);
            Register(new AndParser(), operands, whiteSpaceOperands);
            Register(new OrParser(), operands, whiteSpaceOperands);
            Register(new NotParser(), operands, whiteSpaceOperands);
            Register(new BracketsParser(), operands, whiteSpaceOperands);
            foreach (var lib in FunctionLib.Libs())
            {
                Register(new FunctionParser(lib), operands, whiteSpaceOperands);
            }
            Register(new StringConstantParser(), operands, whiteSpaceOperands);
            Register(new ConstantParser(), operands, whiteSpaceOperands);
            if (TRY_RESOLVE_PROPERTY_INTO_CONSTANT)
            {
                AddParserByTypes(new PropertyOrConstantParser().ParsedTypes, new PropertyOrConstantParser());
            }
            else
            {
                AddParserByTypes(new PropertyParser().ParsedTypes, new PropertyParser());
            }
     
            OPERANDS = operands.ToArray();
            WHITESPACE_OPERANDS = whiteSpaceOperands.ToArray();
        }

        public abstract Type ReturnType { get; }

        public abstract void GuardTypeSafety();

        public Token Token { get; protected internal set; }

        protected static void Register(IExpressionParser parser, ICollection<string> operands, ICollection<string> whiteSpaceOperands)
        {
            AddParserByTypes(parser.ParsedTypes, parser);
            PARSERS_BY_STR.Add(parser.DistinctToken.Token, parser);
            RegisterToken(parser.DistinctToken, operands, whiteSpaceOperands);
            if (parser.AdditionalTokens != null)
            {
                foreach (var sign in parser.AdditionalTokens)
                {
                    PARSERS_BY_STR.Add(sign.Token, parser);
                    RegisterToken(sign, operands, whiteSpaceOperands);
                }
            }
        }

        private static void AddParserByTypes(IEnumerable<Type> types, IExpressionParser parser)
        {
            foreach (var type in types)
            {
                PARSERS_BY_TYPE.Add(type, parser);
            }
        }

        private static void RegisterToken(ExpressionOperatorSign token, 
                                          ICollection<string> operands,
                                          ICollection<string> whiteSpaceOperands)
        {
            if (!token.SurroundedWithWhiteSpace)
            {
                operands.Add(token.Token);
            }
            else
            {
                whiteSpaceOperands.Add(token.Token);
            }
        }

        public static ICollection<IExpressionParser> GetRegisteredParsers()
        {
            return PARSERS_BY_TYPE.Values;
        }

        public static Expression Parse(string expression)
        {
            var tokenizer = new Tokenizer(expression, true, '\\', OPERANDS, null, WHITESPACE_OPERANDS);
            var parseHelper = new ExpressionParserHelper(tokenizer);
            parseHelper.Init();
            Parse(parseHelper);
            var result= parseHelper.Yield();
            result.GuardTypeSafety();
            return result;
        }

        
        internal static void Parse(ExpressionParserHelper parseHelper)
        {
            IgnoreSpaces(parseHelper);
            if (!parseHelper.HasMore())
            {
                return;
            }
            parseHelper.Next();
            Token current = parseHelper.Current;
            IExpressionParser parser = null;
            if (current.Type == TokenType.Seperator)
            {
                parser = PARSERS_BY_STR[current.Contents];
            }
            else
            {
                parser = PARSERS_BY_TYPE[typeof (PropertyOrConstant)];
            }
            parser.Parse(parseHelper);
            if (parseHelper.HasMore())
            {
                Parse(parseHelper);
            }
        }

        private static void IgnoreSpaces(ExpressionParserHelper parseHelper)
        {
            while (
                parseHelper.HasMore() &&
                parseHelper.Lookahead.Contents.Trim().Length == 0)
            {
                parseHelper.Next();
            }
        }

        public static object ParseAndEvaluate(string expression, IModel model)
        {
            Expression parsed = Parse(expression);
            // parsed.TypeCheck(model);
            return parsed.Evaluate(model);
        }

        public IExpressionParser GetParser()
        {
            return PARSERS_BY_TYPE[GetType()];
        }


        public static IExpressionParser GetParser(Type type)
        {
            return PARSERS_BY_TYPE[type];
        }

        public abstract object Evaluate(IModel model);
    }
}
