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
using TypeConverter = org.SharpTiles.Common.TypeConverter;

namespace org.SharpTiles.Expressions
{
    [Category("OtherExpression")]
    public class Assignment : TupleExpression
    {
        public Assignment(Expression lhs)
            : base(lhs)
        {
        }

        public Assignment(Expression lhs, Expression rhs)
            : base(lhs)
        {
            _rhs = rhs;
        }

        public string LhsTyped(IModel model)
        {
            if (!(Lhs is Constant) && (model.Get(Lhs.AsParsable())?.Full??false)) return Lhs.AsParsable();
            return (string) TypeConverter.To(Lhs.Evaluate(model), typeof(string));
        }

        public override Type ParameterType => typeof(string);

        public override object InternalEvaluate(IModel model)
        {
            return model[LhsTyped(model)]=Rhs.Evaluate(model);
        }

        public override Type ReturnType => typeof(object);

        public override void GuardTypeSafety()
        {
            if (Lhs == null || Lhs.ReturnType == null) return;
            if (Lhs.ReturnType != typeof(string)) throw ConvertException.StaticTypeSafety(typeof(bool), Lhs.ReturnType, Lhs.ToString());
            Lhs.GuardTypeSafety();
            if (Rhs == null || Rhs.ReturnType == null) return;
            Rhs.GuardTypeSafety(); 
        }
    }
}
