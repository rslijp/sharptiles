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
using org.SharpTiles.Common;
using org.SharpTiles.Templates;

namespace org.SharpTiles.Tiles
{
    public class TilesMap
    {
        private readonly IDictionary<string, ITile> cache = new Dictionary<string, ITile>();

        public IList<ITile> Tiles
        {
            get { return new List<ITile>(cache.Values); }
        }

        
        public bool Contains(string name)
        {
            return cache.ContainsKey(name);
        }

        public ITile SafeGet(string name)
        {
            return CollectionUtils.SafeGet(cache, name);
        }

        public ITile this[string name]
        {
            get { return Get(name); }
        }


        public ITile Get(string name)
        {
            try
            {
                return cache[name];
            }
            catch (KeyNotFoundException)
            {
                throw TemplateException.TemplateNotFound(name).HavingHttpErrorCode(404);
            }
        }

        public void AddTile(ITile tile)
        {
            if (cache.ContainsKey(tile.Name))
            {
                throw TileException.DoubleDefinition(tile.Name);
            }
            cache.Add(tile.Name, tile);
        }

        public void Clear()
        {
            foreach (ITile tile in cache.Values)
            {
                var disposable = tile as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
            cache.Clear();
        }
    }


}
