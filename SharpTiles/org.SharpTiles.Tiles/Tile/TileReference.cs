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
using System.Text;
using org.SharpTiles.Tags;

namespace org.SharpTiles.Tiles.Tile
{
    public class TileReference : ITile
    {
        private readonly string _name = null;
        private TilesMap _map = null;
        private ITile _tile;
        private ITile _fallBack;

        public TileReference(string name, TilesMap map)
        {
            _name = name;
            _map = map;
        }

        public TileReference(string name, TilesMap map, ITile fallBack) : this (name, map)
        {
            _fallBack = fallBack;
        }

        public string Name
        {
            get { return _name;  }
        }

        public ITile FallBack
        {
            get { return _fallBack; }
        }

        public ITile Tile
        {
            get { return _tile; }
        }

        public string Render(TagModel model)
        {
           return Render(model, Attributes);
        }

        public string Render(TagModel model, AttributeSet attributes)
        {
            GuardInit();
            return _tile.Render(model, attributes);
        }

        public AttributeSet Attributes
        {
            get
            {
                GuardInit();
                return _tile.Attributes;
            }
        }

        public void GuardInit()
        {   
            if(_tile==null)
            {
                Init();
            }
        }

        private void Init()
        {
            if (_fallBack != null)
            {
                InitWithFallBack();
            }
            else
            {
                _tile = _map[Name];
            }
        }

        private void InitWithFallBack()
        {
            _tile = _map.SafeGet(Name);
            if (_tile == null)
            {
                _tile = _fallBack;
            }
        }
    }
}
