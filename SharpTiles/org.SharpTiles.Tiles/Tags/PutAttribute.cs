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
 */using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.CoreTags;
using org.SharpTiles.Tags.FormatTags;
using org.SharpTiles.Tiles.Configuration;
using org.SharpTiles.Tiles.Factory;
using org.SharpTiles.Tiles.Tile;

namespace org.SharpTiles.Tiles.Tags
{
    [HasExample]
    public class PutAttribute : BaseCoreTag, ITag
    {
        public static readonly string NAME = "putAttribute";

        
        [Required]
        public ITagAttribute Name { get; set; }

        [TagDefaultProperty("Body")]
        public ITagAttribute Value { get; set; }

        [EnumProperyType(typeof(TileType))]
        public ITagAttribute TileType { get; set; }

        [Internal]
        public ITagAttribute Body { get; set; }

        #region ITag Members

        public string TagName
        {
            get { return NAME; }
        }

        public string Evaluate(TagModel model)
        {
            throw TagException.NotAllowedShouldBePartOf(GetType(), typeof(InsertTemplate)).Decorate(Context);
        }

        public IAttributeEntry Yield(TagModel model)
        {
            try
            {
                 return new PutAttributeEntry
                    (
                        GetAutoValueAsString("Name", model),
                        GetAutoValueAsString("Value", model),
                        GetAs<TileType>(TileType, model)
                    );   
            }
            catch (TileException Te)
            {
                throw TileExceptionWithContext.ErrorInTile(Te, Context);
            }
        }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.Free; }
        }

        #endregion

    }

    public class PutAttributeEntry : IAttributeEntry
    {
        private readonly string _name;
        private readonly string _value;
        private readonly TileType? _type;

        public PutAttributeEntry(string name, string value, TileType? type)
        {
            _name = name;
            _value = value;
            _type = type;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Value
        {
            get { return _value; }
        }

        public TileType? TileType
        {
            get { return _type; }
        }
    }
}
