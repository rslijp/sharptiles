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
using System.Collections;
using System.ComponentModel;
 using System.Linq;
 using org.SharpTiles.Common;

namespace org.SharpTiles.Tags.CoreTags
{
    [Category("Iterator"), HasExample]
    public class ForTokens : BaseIterationTag, ITag
    {
        public static readonly string NAME = "forTokens";


        [Required]
        public ITagAttribute Items { get; set; }

        [Required]
        public ITagAttribute Delims { get; set; }

        [TagDefaultValue(false)]
        public ITagAttribute Trim { get; set; }

        #region ITag Members

        public override string TagName
        {
            get { return NAME; }
        }


        public override TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.Free; }
        }

        #endregion

        public override IEnumerable GetIEnumerable(TagModel model)
        {
            string items = GetAsString(Items, model) ?? String.Empty;
            string delims = GetAsString(Delims, model);
            string[] tokens = items.Split(delims.ToCharArray());
            var trim = GetAutoValueAsBool(nameof(Trim), model);
            if (trim)
            {
                tokens = tokens.Select(t => t.Trim()).ToArray();
            }
            return new ArrayList(tokens);
        }
    }
}
