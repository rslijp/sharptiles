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
    public abstract class TernaryExpression : Expression
    {
        protected Expression _first;
        protected Expression _second;
        protected Expression _third;


        protected TernaryExpression(Expression first)
        {
            _first = first;
        }

        public Expression First
        {
            get { return _first; }
        }

        public Expression Second
        {
            get { return _second; }
        }

        public Expression Third
        {
            get { return _third; }
        }

        public abstract Type ParameterType { get; }

        public TernaryExpression FillInSecond(Expression second)
        {
            _second = second;
            return this;
        }

        public TernaryExpression FillInThird(Expression third)
        {
            _third = third;
            return this;
        }

        public override object Evaluate(IModel model)
        {
            if (_first == null)
            {
                throw ExpressionParseException.MissingExpression("first").Decorate(Token);
            }
            if (_second == null)
            {
                throw ExpressionParseException.MissingExpression("second").Decorate(Token);
            }
            return InternalEvaluate(model);
        }

        public abstract object InternalEvaluate(IModel model);

        public override string ToString()
        {
            return _first + " " + GetType().Name + " " + _second;
        }

        /*
        public override void TypeCheck(IModel model)
        {
            _lhs.TypeCheck(model);
            _rhs.TypeCheck(model);
            PerformTypeCheck(this, _lhs, ParameterType, model, "left hand side");
            PerformTypeCheck(this, _rhs, ParameterType, model, "right hand side");
        }
        */
    }
}
