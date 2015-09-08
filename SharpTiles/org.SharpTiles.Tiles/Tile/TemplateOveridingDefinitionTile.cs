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
using System.Collections.Generic;
using org.SharpTiles.Tags.Creators;

namespace org.SharpTiles.Tiles.Tile
{
    public class TemplateOveridingDefinitionTile : TemplateTile
    {
        private readonly ITile _extends;

        public TemplateOveridingDefinitionTile(string name, ITemplate template, ITile extends,
                                           IEnumerable<TileAttribute> attributes)
            : base(name, template, attributes)
        {
            _extends = extends;
            Attributes.MergeTileLazy(_extends);
        }

        public ITile Extends
        {
            get { return _extends; }
        }
    }
}
