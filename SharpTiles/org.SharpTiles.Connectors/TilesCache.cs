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
using System.Reflection;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.FormatTags;
using org.SharpTiles.Tiles;
using org.SharpTiles.Tiles.Configuration;

namespace org.SharpTiles.Connectors
{
    public class TilesCache : BaseCache
    {
        private TilesSet _cache;
        
        public override ITile GetView(string view)
        {
            return _cache[view];
        }

        public override bool HasView(string view)
        {
            return _cache.Contains(view);
        }


        public IConfiguration Configuration
        {
            get { return _cache.Configuration;  }
        }

        public TilesSet Cache
        {
            get { return _cache; }
        }

        public override IViewCache GuardInit(Assembly assembly)
        {
            InitFactory(assembly);
            InitTilesCache();
            return this;
        }

        public override string PathSeperator
        {
            get { return "."; }
        }

        private void InitTilesCache()
        {
            if (_cache == null)
            {
                _cache = new TilesSet(new TileXmlConfigurator(_lib, _factory));
                
            }
        }

    }
}
