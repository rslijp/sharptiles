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
using TypeConverter=org.SharpTiles.Common.TypeConverter;

namespace org.SharpTiles.Expressions
{
    [Category("OtherExpression")]
    public class Constant : Expression
    {
        protected readonly string _value;
        private object _evaluated;

        public Constant(string value)
        {
            _value = value;
            //Determine type is possible (ignoring I18N) This is done in the evaluate
            Evaluate(CultureInfo.InvariantCulture);
        }

        private void Evaluate(CultureInfo culture)
        {
            _evaluated = TypeConverter.TryTo(_value, typeof (bool), culture) ??
                         TypeConverter.TryTo(_value, typeof (decimal), culture) ??
                         TryParseDateTime(_value, culture) ??
                         TryParseDate(_value, culture) ?? 
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

        public object TryParseDateTime(string raw, CultureInfo culture)
        {
            var value = default(DateTime);
            var r = DateTime.TryParseExact(raw, PatternStrings.DATETIME_FORMAT, culture, DateTimeStyles.None, out value);
            return r ? value : default(DateTime?);
        }

        public object TryParseDate(string raw, CultureInfo culture)
        {
            var value = default(DateTime);
            var r = DateTime.TryParseExact(raw, PatternStrings.DATE_FORMAT, culture, DateTimeStyles.None, out value);
            return r ? value : default(DateTime?);
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

        public override string AsParsable()
        {
            return "'" + _value + "'";
        }
    }
}
