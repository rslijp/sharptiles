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
using System.Globalization;
using System.Text;
using org.SharpTiles.Common;
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Tags.FormatTags
{
    public abstract class BaseDateTag : BaseCoreTagWithOptionalVariable, ITag
    {
        [EnumProperyType(typeof(DateType))]
        public ITagAttribute Type { get; set; }

        [EnumProperyType(typeof(DateStyle))]
        public ITagAttribute DateStyle { get; set; }

        [EnumProperyType(typeof(TimeStyle))]
        public ITagAttribute TimeStyle { get; set; }

        public ITagAttribute Pattern { get; set; }

        #region ITag Members

        public abstract string TagName { get; }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.Free; }
        }

        #endregion

        protected string GetPattern(TagModel model, DateTimeFormatInfo format)
        {
            DateType type = GetAs<DateType>(Type, model, DateType.Both).Value;
            DateStyle dateStyle = GetAs<DateStyle>(DateStyle, model, FormatTags.DateStyle.Default).Value;
            TimeStyle timeStyle = GetAs<TimeStyle>(TimeStyle, model, FormatTags.TimeStyle.Default).Value;
            return GetAsString(Pattern, model) ?? ConstructPattern(type, dateStyle, timeStyle, format);
        }

        private string ConstructPattern(DateType type, DateStyle dateStyle, TimeStyle timeStyle,
                                        DateTimeFormatInfo format)
        {
            var sb = new StringBuilder();
            if (type != DateType.Time)
            {
                sb.Append(dateStyle == FormatTags.DateStyle.Long ? format.LongDatePattern : format.ShortDatePattern);
            }
            if (type == DateType.Both)
            {
                sb.Append(" ");
            }
            if (type != DateType.Date)
            {
                sb.Append(timeStyle == FormatTags.TimeStyle.Long ? format.LongTimePattern : format.ShortTimePattern);
            }
            return sb.ToString();
        }

        public static T? GetAs<T>(ITagAttribute expressions, TagModel model, T? fallBack) where T : struct
        {
            string result = GetAsString(expressions, model);
            if(!String.IsNullOrEmpty(result))
            {
                return EnumParser<T>.Parse(result);
            }    
            return fallBack;
        }
    }
}
