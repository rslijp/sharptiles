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
    public  class BooleanTernaryExpression : TernaryExpression
    {
        public BooleanTernaryExpression(Expression first)
            : base(first)
        {
        }

        public override Type ReturnType => typeof(object);

        public override void GuardTypeSafety()
        {
            if (Condition?.ReturnType == null) return;
            if (Then?.ReturnType == null) return;
            if (Else?.ReturnType == null) return;
            if (Condition.ReturnType != typeof(bool)) throw ConvertException.StaticTypeSafety(typeof(bool), Condition.ReturnType, Condition.ToString());
            //            if (Rhs.ReturnType != typeof(bool)) throw ConvertException.StaticTypeSafety(typeof(bool), Rhs.ReturnType, Rhs.ToString());
            //            Lhs.GuardTypeSafety();
            //            Rhs.GuardTypeSafety();
        }

        public override Type ParameterType => ReturnType;

        public override object InternalEvaluate(IModel model)
        {
            if (ConditionTyped(model))
            {
                return Then.Evaluate(model);
            }
            return Else.Evaluate(model);
        }

        public Expression Condition=> First; 
        
        public Expression Then => Second; 
        
        public Expression Else => Third; 

        public bool ConditionTyped(IModel model)
        {
            return (bool) TypeConverter.To(Condition.Evaluate(model), ParameterType);
        }
        
    }
}
