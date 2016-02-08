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
    [Category("ArithmeticExpression")] 
    public abstract class ArithmeticExpression : TupleExpression
    {
        protected ArithmeticExpression(Expression lhs) : base(lhs)
        {
        }

        public override Type ReturnType
        {
            get { return typeof (decimal); }
        }

        public override void GuardTypeSafety()
        {
            if (Lhs == null || Rhs == null || Lhs.ReturnType == null || Rhs.ReturnType == null) return;
            if (Lhs.ReturnType != typeof(decimal)) throw ConvertException.StaticTypeSafety(typeof(decimal), Lhs.ReturnType, Lhs.ToString());
            if (Rhs.ReturnType != typeof(decimal)) throw ConvertException.StaticTypeSafety(typeof(decimal), Rhs.ReturnType, Rhs.ToString());
            Lhs.GuardTypeSafety();
            Rhs.GuardTypeSafety();
        }

        public override Type ParameterType
        {
            get { return ReturnType; }
        }

        public decimal LhsTyped(IModel model)
        {
            if (Lhs == null)
                throw ExpressionParseException.MissingExpression("lhs").Decorate(Token);

            return (decimal) TypeConverter.To(Lhs.Evaluate(model), ParameterType);
        }

        public decimal RhsTyped(IModel model)
        {
            if (Rhs == null)
                throw ExpressionParseException.MissingExpression("rhs").Decorate(Token);
            return (decimal) TypeConverter.To(Rhs.Evaluate(model), ParameterType);
        }
    }
}
