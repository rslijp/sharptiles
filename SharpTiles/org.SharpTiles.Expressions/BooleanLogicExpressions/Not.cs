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
using System.ComponentModel;
using org.SharpTiles.Common;
using TypeConverter=org.SharpTiles.Common.TypeConverter;

namespace org.SharpTiles.Expressions
{
    [Category("LogicExpression")]
    public class Not : Expression
    {
        private Expression _nested;

        protected Not()
        {
        }

        public Not(Expression nested)
        {
            _nested = nested;
        }

        public Expression Nested
        {
            get { return _nested; }
        }

        public override Type ReturnType
        {
            get { return typeof (bool); }
        }

        public override void GuardTypeSafety()
        {
            if (Nested == null || Nested.ReturnType == null) return;
            if (Nested.ReturnType != typeof(bool)) throw ConvertException.StaticTypeSafety(typeof(bool), Nested.ReturnType, Nested.ToString());
            Nested.GuardTypeSafety();
        }

        internal void FillNested(Expression exp)
        {
            _nested = exp;
        }

        public override object Evaluate(IModel model)
        {
            var nestedValue = (bool) TypeConverter.To(Nested.Evaluate(model), typeof (bool));
            return !nestedValue;
        }

        public override string ToString()
        {
            return "!" + _nested;
        }

        public override string AsParsable()
        {
            return "!" + _nested.AsParsable();
        }

        /*
        public override void TypeCheck(IModel model)
        {
            _nested.TypeCheck(model);
            PerformTypeCheck(this, _nested, ReturnType, model, "nested");
        }
         */
    }
}
