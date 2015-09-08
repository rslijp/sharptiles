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
    public class FormatDate : BaseDateTag
    {
        public const string NAME = "formatDate";

        [TagDefaultProperty("Body")]
        public ITagAttribute Value { get; set; }

        [Internal]
        public ITagAttribute Body { get; set; }


        public override string TagName
        {
            get { return NAME; }
        }

        public override object InternalEvaluate(TagModel model)
        {
            object value = GetAutoValue("Value", model);
            DateTime? date = null;
            if (value != null && !"".Equals(value))
            {
                date = (value is String) ? DateTime.Parse((String) value) : (DateTime) value;
            }
            var culture = (CultureInfo) model[FormatConstants.LOCALE];
            DateTimeFormatInfo format = DateTimeFormatInfo.GetInstance(culture.DateTimeFormat);
            string pattern = GetAsString(Pattern, model) ?? GetPattern(model, format);
            return date.HasValue ? date.Value.ToString(pattern, format) : null;
        }
    }
}
