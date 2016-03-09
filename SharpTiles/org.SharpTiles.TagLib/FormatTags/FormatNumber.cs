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
using System.Text;
using org.SharpTiles.Common;
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Tags.FormatTags
{
    [Category("Formatting"), HasExample]
    public class FormatNumber : BaseCoreTagWithOptionalVariable, ITag
    {
        public const int MIN_FRACTIONS = 2;

        public static readonly string NAME = "formatNumber";
        public const bool USE_GROUPING = true;

        [Internal]
        public ITagAttribute Body { get; set; }

        [TagDefaultProperty("Body")]
        public ITagAttribute Value { get; set; }

        [EnumProperyType(typeof(NumberType))]
        [TagDefaultValue(NumberType.Number)]
        public ITagAttribute Type { get; set; }

        public ITagAttribute Pattern { get; set; }

        public ITagAttribute CurrencyCode { get; set; }

        [TagDefaultProperty("CurrencyCode")]
        public ITagAttribute CurrencySymbol { get; set; }

        [EnumProperyType(typeof(BooleanEnum))]
        [TagDefaultValue(USE_GROUPING)]
        public ITagAttribute GroupingUsed { get; set; }

        [TagDefaultValue(MIN_FRACTIONS)]
        public ITagAttribute MinFractionDigits { get; set; }

        public ITagAttribute MaxFractionDigits { get; set; }

        public ITagAttribute MinIntegerDigits { get; set; }

        public ITagAttribute MaxIntegerDigits { get; set; }

        #region ITag Members

        public string TagName
        {
            get { return NAME; }
        }

        #endregion

        public override object InternalEvaluate(TagModel model)
        {
            object value = GetAutoValue("Value", model);
            NumberType type = GetAutoValueAs<NumberType>("Type", model).Value;

            decimal? number = null;
            if (value != null && !"".Equals(value))
            {
                number = (value is String) ? decimal.Parse((String) value) : Convert.ToDecimal(value);
            }
            string result = String.Empty;

            if (number.HasValue)
            {
                result = Format(model, number.Value, type);
            }

            return result;
        }

        private string Format(TagModel model, decimal number, NumberType type)
        {
            var culture = (CultureInfo) model[FormatConstants.LOCALE];
            var format = (NumberFormatInfo) NumberFormatInfo.GetInstance(culture.NumberFormat).Clone();
            if (type == NumberType.Currency)
            {
                string currencyCode = GetAutoValueAsString("CurrencySymbol", model) ?? format.CurrencySymbol;
                format.CurrencySymbol = currencyCode;
            }
            string pattern = ConstructPattern(model, type, format) ?? FormatConstants.NUMBERPATTERNS[type];
            pattern = GetAsString(Pattern, model) ?? pattern;

            number = ClipNumber(model, number);

            return number.ToString(pattern, format);
        }

        private decimal ClipNumber(TagModel model, decimal number)
        {
            if (MaxIntegerDigits == null)
            {
                return number;
            }
            int? maxIntegers = GetAsInt(MaxIntegerDigits, model);
            if (maxIntegers.HasValue)
            {
                decimal integers = Decimal.Truncate(number);
                decimal fractions = number - integers;
                string intAsString = integers.ToString();
                int cutOff = Math.Max(intAsString.Length - maxIntegers.Value, 0);
                intAsString = intAsString.Substring(cutOff);
                integers = Decimal.Parse(intAsString);
                number = integers + fractions;
            }
            return number;
        }

        #region construction of pattern

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.Free; }
        }

        private string ConstructPattern(TagModel model, NumberType type, NumberFormatInfo format)
        {
            string constructedPattern = null;
            if (GroupingUsed != null ||
                MinFractionDigits != null ||
                MaxFractionDigits != null ||
                MinIntegerDigits != null ||
                MaxIntegerDigits != null
                )
            {
                bool groupingUsed = GetAutoValueAsBool("GroupingUsed", model);
                int? minIntegers = GetAsInt(MinIntegerDigits, model);
                int? minFractions = GetAutoValueAsInt("MinFractionDigits", model);
                int? maxFractions = GetAsInt(MaxFractionDigits, model);
                var pattern = new StringBuilder();
                if (type == NumberType.Currency)
                {
                    pattern.Append("'" + format.CurrencySymbol + "'");
                }
                pattern.Append(ConstructNumberPattern(minIntegers, groupingUsed));
                pattern.Append(".");
                pattern.Append(ConstructFractionString(minFractions, maxFractions));
                if (type == NumberType.Percentage)
                {
                    pattern.Append(" %");
                }
                constructedPattern = pattern.ToString();
            }
            return constructedPattern;
        }

        private static string ConstructNumberPattern(int? minDigits, bool groupingUsed)
        {
            int count = 0;
            int optional = 4;
            var pattern = new StringBuilder();
            int total = (minDigits ?? 0) + optional;
            for (int i = 0; i < optional - (minDigits ?? 0); i++)
            {
                pattern.Append("#");
                count++;
                PlaceSeperator(count, total, pattern, groupingUsed);
            }
            if (minDigits.HasValue)
            {
                for (int i = 0; i < minDigits; i++)
                {
                    pattern.Append("0");
                    count++;
                    PlaceSeperator(count, total, pattern, groupingUsed);
                }
            }
            return pattern.ToString();
        }

        private static void PlaceSeperator(int count, int total, StringBuilder pattern, bool groupingUsed)
        {
            int placeFromPoint = total - count;
            if (groupingUsed && placeFromPoint > 0 && placeFromPoint%3 == 0)
            {
                pattern.Append(",");
            }
        }

        private static string ConstructFractionString(int? minFractions, int? maxFractions)
        {
            var pattern = new StringBuilder();
            if (minFractions.HasValue &&
                maxFractions.HasValue)
            {
                minFractions = Math.Min(minFractions.Value, maxFractions.Value);
            }
            minFractions = minFractions ?? 0;
            for (int i = 0; i < minFractions; i++)
            {
                pattern.Append("0");
            }
            if (maxFractions.HasValue)
            {
                for (int i = 0; i < maxFractions.Value - minFractions; i++)
                {
                    pattern.Append("#");
                }
            }
            return pattern.ToString();
        }

        #endregion
    }
}
