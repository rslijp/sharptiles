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
 using System.Linq;
 using System.Runtime.InteropServices.WindowsRuntime;
 using org.SharpTiles.Common;

namespace org.SharpTiles.Expressions
{
    public class ExpressionParserHelper : ParseHelper
    {
        private readonly Stack<Expression> _stack = new Stack<Expression>();
        private Expression _top;
        private ExpressionLib _expressionLib;

        public ExpressionParserHelper(ExpressionLib expressionLib, Tokenizer tokenizer) : base(tokenizer)
        {
            _expressionLib = expressionLib;
        }

        public Token Expand()
        {
            var current = Current;
            while (Lookahead!=null&& Lookahead.Type!=TokenType.Seperator&& (Lookahead.IsTextOrSpace))
            {
                var part = Lookahead.Contents;
                current.Append(part);
                Next();
            }
            return current;
        }

        public Expression Top
        {
            get { return _top; }
        }

        public Expression PeekOnStack 
        {
            get { return _stack.Count>0?_stack.Peek():null; }
        }
        private int Count
        {
            get
            {
                int count = _stack.Count;
                if (_top != null)
                {
                    count++;
                }
                return count;
            }
        }

        public int CurrentPresendence { get; set; }
        public ExpressionLib ExpressionLib => _expressionLib;

        public void Push(Expression exp)
        {
            if (_top != null)
            {
                _stack.Push(_top);
            }
            _top = exp;
        }

       

        public Expression Pop()
        {
            Expression popped = _top;
            if (_stack.Count > 0)
            {
                _top = _stack.Pop();
            }
            else
            {
                _top = null;
            }
            return popped;
        }

        public void Reduce(Type expression)
        {
            if (OperatorPrecedence.Applicable(expression))
            {
                Reduce(OperatorPrecedence.Of(expression));
            }
        }


        private void Reduce(int priority)
        {
            if (Count > 1)
            {
                Expression subtop = _stack.Peek();
                IExpressionParser parser = _expressionLib.GetParser(subtop.GetType());
                if (parser != null && parser.Reduce(this, subtop, priority))
                {
                    Reduce(priority);
                }
            }
        }

        public Expression Yield()
        {
            Reduce(-1);
            Expression result = Pop();
            if (Count > 0)
            {
                throw ExpressionParseException.ExpressionStackNotEmptyOnYield(Top).Decorate(_current);
            }
            return result;
        }

        public bool AnyOnStack(Type[] parsedTypes)
        {
            return _stack.Any(partial =>
            {
                return parsedTypes.Contains(partial.GetType());
            });
        }

        public void ParseExpression()
        {
            _expressionLib.Parse(this);
        }
    }
}
