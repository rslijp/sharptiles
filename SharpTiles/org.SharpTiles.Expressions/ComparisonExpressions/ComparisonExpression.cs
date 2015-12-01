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
    [Category("Comparison")]
    public abstract class ComparisonExpression : TupleExpression
    {
        protected ComparisonExpression(Expression lhs)
            : base(lhs)
        {
        }

        public override Type ReturnType
        {
            get { return typeof(bool); }
        }

        public override void GuardTypeSafety()
        {
            if (Lhs == null || Rhs == null || Lhs.ReturnType == null || Rhs.ReturnType == null) return;
            if (Lhs.ReturnType != Rhs.ReturnType) throw ConvertException.StaticTypeSafety(Lhs.ReturnType, Rhs.ReturnType, Rhs.ToString());
            Lhs.GuardTypeSafety();
            Rhs.GuardTypeSafety();
        }


        public override Type ParameterType
        {
            get { return typeof(decimal); }
        }

        public IComparable LhsTyped(IModel model)
        {
            return (IComparable) Lhs.Evaluate(model);
        }

        public IComparable RhsTyped(IModel model)
        {
            return (IComparable) Rhs.Evaluate(model);
        }

        protected int Compare(IComparable lhs, IComparable rhs)
        {
            if (lhs == null || rhs == null)
                return lhs?.CompareTo(rhs) ?? (-1*rhs?.CompareTo(lhs) ?? 0);

            if(TypeConverter.IsNumeric(lhs) && TypeConverter.IsNumeric(rhs))
            {
                lhs = (decimal) TypeConverter.To(lhs, typeof(decimal));
                rhs = (decimal) TypeConverter.To(rhs, typeof(decimal));
            }
            if (!Equals(lhs.GetType(), rhs.GetType()))
            {
                throw ComparisionException.UnComparable(lhs.GetType(), rhs.GetType()).Decorate(Token);
            }
            return lhs.CompareTo(rhs);
        }

        protected bool IsEqual(object lhs, object rhs)
        {
            if (TypeConverter.IsNumeric(lhs) && TypeConverter.IsNumeric(rhs))
            {
                lhs = (decimal)TypeConverter.To(lhs, typeof(decimal));
                rhs = (decimal)TypeConverter.To(rhs, typeof(decimal));
            }
            return Equals(lhs, rhs);
        }
    }
}
