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

namespace org.SharpTiles.Tags.FormatTags
{
    [Category("Formatting"), HasExample]
    public class ParseDate : BaseDateTag
    {
        public const string NAME = "parseDate";

        [TagDefaultProperty("Body")]
        public ITagAttribute Value { get; set; }

        [Internal]
        public ITagAttribute Body { get; set; }

        public ITagAttribute ParseLocale { get; set; }

        [TagDefaultValue(true)]
        public ITagAttribute Exact { get; set; }


        public override string TagName
        {
            get { return NAME; }
        }

        public override object InternalEvaluate(TagModel model)
        {
            string dateStr = GetAutoValueAsString("Value", model);
            CultureInfo culture = ParseLocale != null
                                      ? new CultureInfo(GetAsString(ParseLocale, model))
                                      : (CultureInfo) model[FormatConstants.LOCALE];
            var format =
                (DateTimeFormatInfo) DateTimeFormatInfo.GetInstance(culture.DateTimeFormat).Clone();
            DateTime? result = null;
            if (!String.IsNullOrEmpty(dateStr))
            {
                if (GetAutoValueAsBool("Exact", model))
                {
                    string pattern = GetAsString(Pattern, model) ?? GetPattern(model, format);
                    try
                    {
                        result = DateTime.ParseExact(dateStr, pattern, format);
                    } catch (FormatException)
                    {
                        throw TagException.ParseException(dateStr, "Date").Decorate(Context);
                    }
                }
                else
                {
                    result = DateTime.Parse(dateStr, format);
                }
            }
            return result;
        }
    }
}
