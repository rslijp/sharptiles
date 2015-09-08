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
using System.ComponentModel;
using org.SharpTiles.Common;

namespace org.SharpTiles.Expressions
{
    [Category("OtherExpression")] 
    public class Brackets : Expression
    {
        private readonly int _arguments;
        private readonly List<Expression> _nodes;
        private readonly bool _partOfFunction;

        public Brackets()
        {
        }

        public Brackets(bool partOfFunction, int arguments)
        {
            _partOfFunction = partOfFunction;
            _arguments = arguments;
            _nodes = new List<Expression>();
        }

        public Brackets(Expression nested) : this(false, 1)
        {
            FillNext(nested);
        }

        public IList<Expression> Nodes
        {
            get { return _nodes; }
        }

        public Expression Nested
        {
            get
            {
                GuardNotPartOfFunction("Nested");
                return _nodes[0];
            }
        }

        public override Type ReturnType
        {
            get
            {
                GuardNotPartOfFunction("ReturnType");
                return _nodes[0].ReturnType;
            }
        }

        public override void GuardTypeSafety()
        {
            if (_nodes != null && _nodes.Count >= 1)
            {
                _nodes[0].GuardTypeSafety();
            }
        }

        public bool PartOfFunction
        {
            get { return _partOfFunction; }
        }

        public bool AllFilled
        {
            get { return _nodes.Count == _arguments; }
        }

        internal void FillNext(Expression exp)
        {
            if (!PartOfFunction && _nodes.Count > 1)
            {
                throw ExpressionParseException.BracketsAreNotUsedInFunction("Adding second parameter").Decorate(Token);
            }
            if (AllFilled)
            {
                throw ExpressionParseException.UnExpectedParameter(exp).Decorate(Token);
            }
            _nodes.Add(exp);
        }

        private void GuardNotPartOfFunction(string method)
        {
            if (_arguments == 0)
            {
                ExpressionParseException.BracketsAreUsedInFunction(method);
            }
        }

        public override object Evaluate(IModel model)
        {
            GuardNotPartOfFunction("Evaluate");
            return _nodes[0].Evaluate(model);
        }

        public override string ToString()
        {
            return "(" + _nodes + ")";
        }

        /*
        public override void TypeCheck(IModel model)
        {
            foreach (Expression expression in _nodes)
            {
                expression.TypeCheck(model);    
            }
        }
        */
    }
}
