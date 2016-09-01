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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using org.SharpTiles.Common;
using org.SharpTiles.Expressions.Functions;
using org.SharpTiles.Expressions.Math;

namespace org.SharpTiles.Expressions
{
    public class ExpressionLib
    {
        private bool TRY_RESOLVE_PROPERTY_INTO_CONSTANT = true;
        private string[] OPERANDS;

        private readonly IDictionary<string, IExpressionParser> PARSERS_BY_STR =
            new Dictionary<string, IExpressionParser>();

        private readonly IDictionary<Type, IExpressionParser> PARSERS_BY_TYPE =
            new Dictionary<Type, IExpressionParser>();

        internal static string[] WHITESPACE_OPERANDS;

        private IEnumerable<FunctionLib> _libs;

        public ExpressionLib(params FunctionLib[] libs)
        {
            _libs = libs?.ToList() ?? new List<FunctionLib>();
            Init();
        }
        

        public string[] WhiteSpaceOperands => WHITESPACE_OPERANDS;

        public void Clear()
        {
            OPERANDS = new String[0];
            WHITESPACE_OPERANDS = new String[0];
            PARSERS_BY_STR.Clear();
            PARSERS_BY_TYPE.Clear();            
        }

       
        private void Init()
        {
                var operands = new List<string>();
                var whiteSpaceOperands = new List<string>();
                

                Register(new BooleanTernaryExpressionParser(), operands, whiteSpaceOperands);
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
                foreach (var func in new MathFunctionLib().Functions)
                {
                    Register(new MathFunctionParser(func), operands, whiteSpaceOperands);
                }
                Register(new FunctionParser(new BaseFunctionLib()), operands, whiteSpaceOperands);
                foreach (var lib in _libs)
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

        
        protected void Register(IExpressionParser parser, ICollection<string> operands, ICollection<string> whiteSpaceOperands)
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

        private void AddParserByTypes(IEnumerable<Type> types, IExpressionParser parser)
        {
            foreach (var type in types)
            {
                if (PARSERS_BY_TYPE.ContainsKey(type))
                    throw new ArgumentException($"{parser}: PARSERS_BY_TYPE already contains key '{type.Name}' with value '{PARSERS_BY_TYPE[type]}'");
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

        public ICollection<IExpressionParser> GetRegisteredParsers()
        {
            return PARSERS_BY_TYPE.Values;
        }

        public Expression Parse(string expression, ParseContext offset = null)
        {
            var tokenizer = new Tokenizer(expression, true, '\\', OPERANDS, null, WHITESPACE_OPERANDS, offset).AddOffSet(offset);
            var parseHelper = new ExpressionParserHelper(this, tokenizer);
            parseHelper.Init();
            Parse(parseHelper);
            var result = parseHelper.Yield();
            result.GuardTypeSafety();
            return result;
        }


        internal void Parse(ExpressionParserHelper parseHelper)
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
                parser = PARSERS_BY_TYPE[typeof(PropertyOrConstant)];
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

        public object ParseAndEvaluate(string expression, IModel model)
        {
            var parsed = Parse(expression);
            // parsed.TypeCheck(model);
            return parsed.Evaluate(model);
        }

        public IExpressionParser GetParser()
        {
            return PARSERS_BY_TYPE[GetType()];
        }


        public IExpressionParser GetParser(Type type)
        {
            return PARSERS_BY_TYPE[type];
        }

        public IEnumerable<FunctionLib> FunctionLibs()
        {
            var libs = new List<FunctionLib>(_libs);
            libs.Insert(0, new MathFunctionLib());
            libs.Insert(0, new BaseFunctionLib());
            return libs;
        }
    }
}

