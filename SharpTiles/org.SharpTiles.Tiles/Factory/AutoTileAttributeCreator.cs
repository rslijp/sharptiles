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
using System.IO;
using org.SharpTiles.Tiles.Configuration;
using org.SharpTiles.Tiles.Tile;

namespace org.SharpTiles.Tiles.Factory
{
    public class AutoTileAttributeCreator : IAttributeCreator
    {
        #region IAttributeCreator Members

        public bool Applies(IAttributeEntry entry)
        {
            return !entry.TileType.HasValue;
        }

        public TileAttribute Create(IAttributeEntry entry, TilesFactory factory)
        {
            var template = factory.GetTemplate(entry, false);
            if (template!=null)
            {
                return new TileAttribute(entry.Name, new TemplateTile(null, template, null));
            }
            return new TileAttribute(entry.Name,
                                     new TileReference(entry.Value, factory.Map, new StringTile(entry.Value)));
        }

        #endregion
    }
}
