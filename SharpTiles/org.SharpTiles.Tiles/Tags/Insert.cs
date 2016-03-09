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
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.CoreTags;
using org.SharpTiles.Tiles.Tile;

namespace org.SharpTiles.Tiles.Tags
{
    [HasExample]
    //[HasNote]
    public class Insert : BaseCoreTag, ITag
    {
        public static readonly string NAME = "insert";

        [Required]
        public ITagAttribute Name { get; set; }

        [EnumProperyType(typeof(BooleanEnum))]
        [TagDefaultValue(true)]
        public ITagAttribute Mandatory { get; set; }


        #region ITag Members

        public string TagName
        {
            get { return NAME; }
        }

        public string Evaluate(TagModel model)
        {
            try
            {
                string tileName = GetAutoValueAsString("Name", model);
                bool mandatory = GetAutoValueAsBool("Mandatory", model);
                using (var stack = new TagModelAttributeStack(model))
                {
                    if (!mandatory && !stack.Current.HasDefinitionFor(tileName)) return String.Empty;
                    string result = stack.Current[tileName].Value.Render(model);
                    return result ?? String.Empty;
                }
            } catch (TileException Te)
            {
                throw TileExceptionWithContext.ErrorInTile(Te, Context);
            }
        }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.None; }
        }

        #endregion
    }
}