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

namespace org.SharpTiles.Tags.CoreTags
{
    [Category("GeneralPurpose"), HasExample]
    public class Out : BaseCoreTag, ITag
    {
        public static readonly string NAME = "out";

        [TagDefaultProperty("Default")]
        public ITagAttribute Value { get; set; }

        [TagDefaultProperty("Body")]
        public ITagAttribute Default { get; set; }

        [Internal]
        public ITagAttribute Body { get; set; }

        [TagDefaultValue(true)]
        [EnumProperyType(typeof(BooleanEnum))]
        public ITagAttribute EscapeXml { get; set; }

        #region ITag Members

        public string TagName
        {
            get { return NAME; }
        }

        public string Evaluate(TagModel model)
        {
            string result = GetAutoValueAsString("Value", model);
            var escapeXml = GetAutoValueAsBool("EscapeXml", model);
            result = result ?? String.Empty;
            return escapeXml ? StringUtils.EscapeXml(result) : result;
        }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.Free; }
        }

        #endregion
    }
}
