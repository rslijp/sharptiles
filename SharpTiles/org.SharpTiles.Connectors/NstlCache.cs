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
 */using System.Reflection;
using org.SharpTiles.Tags.Creators;
using org.SharpTiles.Templates.Templates;
using org.SharpTiles.Tiles;
using org.SharpTiles.Tiles.Tile;

namespace org.SharpTiles.Connectors
{
    public class NstlCache : BaseCache
    {
        private readonly TilesMap _pages = new TilesMap();
        public static string PostFix = ".htm";

        public override ITile GetView(string view)
        {
            view = PostFixName(view);
            lock (_pages)
            {
                if (!_pages.Contains(view))
                {
                    var template = _factory.Handle(view, true);
                    var tile = new TemplateTile(view, template, null);
                    _pages.AddTile(tile);
                }
            }
            return _pages[view];
        }

        public TilesMap Pages
        {
            get { return _pages; }
        }

        public override string PathSeperator
        {
            get { return _factory.GetNewLocator().PathSeperator; }
        }

        public override IViewCache GuardInit(Assembly assembly)
        {
            InitFactory(assembly);
            return this;
        }

        public override bool HasView(string name)
        {
            name = PostFixName(name);
            if (!_pages.Contains(name))
            {
                return _factory.GetNewLocator().Exists(name);
            }
            return IsHealthy(name);
        }

        private string PostFixName(string name)
        {
            return name + (PostFix ?? "");
        }

        private bool IsHealthy(string name)
        {
            var tile = (TemplateTile) _pages[name];
            if (tile.Template is RefreshableResourceTemplate)
            {
                return ((RefreshableResourceTemplate) tile.Template).RefreshException == null;
            }
            return true;
        }
    }
}
