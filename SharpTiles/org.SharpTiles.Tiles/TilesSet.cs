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
using System.Collections.Generic;
using org.SharpTiles.Common;
 using org.SharpTiles.Tags;
 using org.SharpTiles.Tiles.Configuration;
 using org.SharpTiles.Tiles.Factory;

namespace org.SharpTiles.Tiles
{
    public class TilesSet : RefreshableResource
    {
//        static TilesSet()
//        {
//            if (!TagLib.Exists(Tags.Tiles.TILES_GROUP_NAME)) TagLib.Register(new Tags.Tiles());
//        }
        
        private readonly IConfiguration _configuration;
        private TilesMap _map = new TilesMap();
            
        public TilesSet()
        {
            _map = new TilesMap();
        }

        public TilesSet(IConfiguration configuration)
        {
            _configuration = configuration;
            LoadResource();
        }


        public TilesSet(IEnumerable<ITile> tiles)
        {
            SetTiles(tiles);
        }


        public override DateTime? ResourceLastModified
        {
            get { return _configuration != null ? _configuration.ConfigurationLastModified : default(DateTime?); }
        }

        public TilesMap Map
        {
            get { return _map; }
        }

        public IList<ITile> Tiles
        {
            get { return Map.Tiles; }
        }

        public ITile this[string name]
        {
            get { return Map[name]; }
        }

        public bool Contains(string name)
        {
            return Map.Contains(name);
        }

        public void SetTiles(IEnumerable<ITile> tiles)
        {
            Map.Clear();
            foreach (var tile in tiles)
            {
                _map.AddTile(tile);
            }
        }

        public void SetTiles(IConfiguration configuration)
        {
            var newMap = TilesFactory.AssembleFor(configuration).Map;
            var oldMap = Map;
            _map = newMap;
            oldMap.Clear();
        }

        public IConfiguration Configuration
        {
            get { return _configuration; }
        }

        private void RefreshConfiguration()
        {
            if (_configuration != null)
            {
                _configuration.Refresh();
                SetTiles(_configuration);
            }
        }

        protected override void Load()
        {
            RefreshConfiguration();
        }


    }
}
