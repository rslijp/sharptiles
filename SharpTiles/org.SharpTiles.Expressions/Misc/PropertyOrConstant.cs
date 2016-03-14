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
 using System.Globalization;
 using org.SharpTiles.Common;
using TypeConverter = org.SharpTiles.Common.TypeConverter;

namespace org.SharpTiles.Expressions
{
    [Category("OtherExpression")]
    public class PropertyOrConstant : Expression
    {
        private readonly string _name;
        private object _evaluated;
        private bool _constant = false;

        public PropertyOrConstant(string name)
        {
            _name = name;
            //Determine type is possible (ignoring I18N) This is done in the evaluate
            Evaluate(CultureInfo.InvariantCulture);
        }

        public bool IsConstant => _constant;

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
            return _evaluated!=null? _evaluated:model[_name];
        }

        private void Evaluate(CultureInfo culture)
        {
            _evaluated = TypeConverter.TryTo(_name, typeof (bool), culture) ??
                         TypeConverter.TryTo(_name, typeof (decimal), culture);
            _constant = _evaluated != null;
        }

        public override string ToString()
        {
            return _name;
        }

       

        /*
        public override void TypeCheck(IModel model)
        {
            return;
        }
        */
    }
}
