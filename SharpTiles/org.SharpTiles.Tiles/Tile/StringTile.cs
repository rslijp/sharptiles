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
using org.SharpTiles.Tags;

namespace org.SharpTiles.Tiles.Tile
{
    public class StringTile : ITile
    {
        private readonly string _value;

        public StringTile(string value)
        {
            _value = value;
        }

        #region ITile Members

        public string Name
        {
            get { return null; }
        }

        public string Value
        {
            get { return _value; }
        }

        public string Render(TagModel model)
        {
            return _value;
        }

        public string Render(TagModel model, AttributeSet attributes)
        {
            return _value;
        }

        public AttributeSet Attributes
        {
            get { return new AttributeSet("anonymus string attribute"); }
        }

        #endregion
    }
}
