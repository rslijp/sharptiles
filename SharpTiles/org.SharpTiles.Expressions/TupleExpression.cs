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
    public abstract class TupleExpression : Expression
    {
        protected Expression _lhs;
        protected Expression _rhs;


        protected TupleExpression(Expression lhs)
        {
            _lhs = lhs;
        }

        public Expression Lhs
        {
            get { return _lhs; }
        }

        public Expression Rhs
        {
            get { return _rhs; }
        }

        public abstract Type ParameterType { get; }

        internal void FillInRhs(Expression rhs)
        {
            _rhs = rhs;
        }

        public override object Evaluate(IModel model)
        {
            if (_lhs == null)
            {
                throw ExpressionParseException.MissingExpression("lhs").Decorate(Token);
            }
            if (_rhs == null)
            {
                throw ExpressionParseException.MissingExpression("rhs").Decorate(Token);
            }
            return InternalEvaluate(model);
        }

        public abstract object InternalEvaluate(IModel model);

        public override string ToString()
        {
            return _lhs + " " + GetType().Name + " " + _rhs;
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
