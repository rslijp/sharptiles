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
    public class Property : Expression
    {
        protected readonly string _name;
        protected List<Expression> _bracketIndexers;

        public Property(string name)
        {
            _name = name;
        }


        public string Name
        {
            get { return _name; }
        }

        public override Type ReturnType
        {
            get { return null; }
        }

        public override void GuardTypeSafety()
        {
        }

        public override object Evaluate(IModel model)
        {
            if(_bracketIndexers==null) return model[_name];
            var t = model[_name];
            foreach (var expression in _bracketIndexers)
            {
                t=new Reflection(t).GetDirectProperty(expression.Evaluate(model)?.ToString());
            }
            return t;
        }

        public override string ToString()
        {
            return _name;
        }

        public override string AsParsable()
        {
            return _name;
        }

        /*
        public override void TypeCheck(IModel model)
        {
            return;
        }
        */

        public virtual void AddBracketIndexer(Expression expr)
        {
            if(_bracketIndexers==null) _bracketIndexers=new List<Expression>();
            _bracketIndexers.Add(expr);
        }
    }
}
