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
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Tags.FormatTags
{
    [Category("Formatting"), HasExample]
    public class ParseNumber : BaseCoreTagWithOptionalVariable, ITag
    {
        public const bool INTEGER_ONLY = false;
        public static readonly string NAME = "parseNumber";

        [Internal]
        public ITagAttribute Body { get; set; }

        [TagDefaultProperty("Body")]
        public ITagAttribute Value { get; set; }

        [EnumProperyType(typeof(NumberType))]
        [TagDefaultValue(NumberType.Number)]
        public ITagAttribute Type { get; set; }

        [EnumProperyType(typeof(NumberStyles), Multiple = true)]
        public ITagAttribute Styles { get; set; }

        public ITagAttribute ParseLocale { get; set; }

        [EnumProperyType(typeof(BooleanEnum))]
        [TagDefaultValue(INTEGER_ONLY)]
        public ITagAttribute IntegerOnly { get; set; }

        #region ITag Members

        public string TagName
        {
            get { return NAME; }
        }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.Free; }
        }

        #endregion

        public override object InternalEvaluate(TagModel model)
        {
            string value = GetAutoValueAsString("Value", model);
            NumberType type = GetAutoValueAs<NumberType>("Type", model).Value;

            decimal? result = null;

            if (!String.IsNullOrEmpty(value))
            {
                result = Parse(model, value, type);
            }

            return result;
        }

        private decimal Parse(TagModel model, string number, NumberType type)
        {
            try
            {
                decimal result;
                CultureInfo culture = ParseLocale != null
                                          ? new CultureInfo(GetAsString(ParseLocale, model))
                                          : (CultureInfo) model[FormatConstants.LOCALE];
                var format = (NumberFormatInfo) NumberFormatInfo.GetInstance(culture.NumberFormat).Clone();
                bool integerOnly = INTEGER_ONLY;
                if (IntegerOnly != null)
                {
                    integerOnly = GetAutoValueAsBool("IntegerOnly", model);
                }
                NumberStyles style = FormatConstants.NUMBERSTYLES[type];
                style = GetAsNumberStyle(Styles, model, style);
                result = InternalParse(format, number, style);
                return integerOnly ? Decimal.Floor(result) : result;
            } catch (FormatException)
            {
                throw TagException.ParseException(number, type.ToString()).Decorate(Context);
            }
        }

        private NumberStyles GetAsNumberStyle(ITagAttribute styles, TagModel model, NumberStyles style)
        {
            if (Styles == null)
            {
                return style;
            }
            string stylesAsString = GetAsString(Styles, model);
            if (stylesAsString != null)
            {
                bool first = true;
                string[] stylesStr = stylesAsString.Split(',');
                foreach (string styleStr in stylesStr)
                {
                    if (first)
                    {
                        style = EnumParser<NumberStyles>.Parse(styleStr);
                    }
                    else
                    {
                        style = style | EnumParser<NumberStyles>.Parse(styleStr);
                    }
                    first = false;
                }
            }
            return style;
        }

        private decimal InternalParse(NumberFormatInfo format, string number, NumberStyles style)
        {
            return Decimal.Parse(number, style, format);
        }
    }
}
