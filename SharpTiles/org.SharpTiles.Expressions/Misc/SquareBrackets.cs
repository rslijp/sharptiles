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
 using System.Linq;
 using org.SharpTiles.Common;

namespace org.SharpTiles.Expressions
{
    [Category("OtherExpression")] 
    public class SquareBrackets : Expression
    {
        private  Expression _node;
        

        public Expression Node
        {
            get { return _node; }
        }

        [Internal]
        public Expression Nested
        {
            get { return _node; }
        }

        public override Type ReturnType
        {
            get
            {
                return Nested.ReturnType;
            }
        }

        public override void GuardTypeSafety()
        {
            Nested?.GuardTypeSafety();
        }


        
        public override object Evaluate(IModel model)
        {
            return _node.Evaluate(model);
        }

        public override string ToString()
        {
            return AsParsable();
        }

        public override string AsParsable()
        {
            return "[" + _node.AsParsable() + "]";
        }

        public void Fill(Expression nested)
        {
            GuardNotSet();
            _node = nested;
        }

        private void GuardNotSet()
        {
            if (_node != null)
            {
                throw ExpressionParseException.NestedNodeAlreadySet().Decorate(Token);
            }
        }

    }
}
