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
    [Category("OtherExpression")]
    public class Constant : Expression
    {
        protected readonly string _value;
        private readonly object _evaluated;
        public Constant(string value)
        {
            _value = value;
            _evaluated = TypeConverter.TryTo(_value, typeof(bool)) ??
                         TypeConverter.TryTo(_value, typeof(decimal)) ??
                        _value;
        }

        public string Value
        {
            get { return _value; }
        }

        public override Type ReturnType
        {
            get
            {
                return _evaluated!=null?_evaluated.GetType():null;
            }
        }

        public override void GuardTypeSafety()
        {
        }

        public override object Evaluate(IModel model)
        {
            return _evaluated;
        }

        public override string ToString()
        {
            return "'" + _value + "'";
        }

    }
}
