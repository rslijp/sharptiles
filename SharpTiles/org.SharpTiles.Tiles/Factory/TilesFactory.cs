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
using org.SharpTiles.Tiles.Configuration;

namespace org.SharpTiles.Tiles.Factory
{
    public class TilesFactory
    {
        private static readonly List<IAttributeCreator> ATTRIBUTE_CREATORS = new List<IAttributeCreator>
                                                                                 {
                                                                                     new StringTileAttributeCreator(),
                                                                                     new TemplateTileAttributeCreator(),
                                                                                     new DefinitionTileAttributeCreator(),
                                                                                     new AutoTileAttributeCreator()
                                                                                 };

        private static readonly List<ITileCreator> TILE_CREATORS = new List<ITileCreator>
                                                                       {
                                                                           new DefinitionTileCreator(),
                                                                           new TemplateTileCreator(),
                                                                           new TemplateOverridingDefinitionTileCreator()
                                                                       };


        private readonly IConfiguration _config;
        private readonly IResourceLocatorFactory _factory;
        private readonly TilesMap _map;

        public TilesFactory(IResourceLocatorFactory factory)
        {
            _map = new TilesMap();
            _factory = factory;
        }

        public TilesFactory(IConfiguration config) : this(config.GetFactory())
        {
            _config = config;
        }

        public TilesMap Map
        {
            get { return _map; }
        }


        public void Decorate()
        {
            foreach (ITileEntry tileEntry in _config.Entries)
            {
                ITile tile = ConstructTile(tileEntry);
                _map.AddTile(tile);
            }
        }

        public ITile ConstructTile(ITileEntry tileEntry)
        {
            ITileCreator creator = TILE_CREATORS.Find(c => c.Applies(tileEntry));
            return creator.Create(tileEntry, this);
        }

        public static TilesFactory AssembleFor(IConfiguration config)
        {
            var factory = new TilesFactory(config);
            factory.Decorate();
            return factory;
        }

        public IEnumerable<TileAttribute> CreateAttributes(ITileEntry tileEntry)
        {
            return CreateAttributes(tileEntry.TileAttributes);
        }

        public IEnumerable<TileAttribute> CreateAttributes(IEnumerable<IAttributeEntry> attributeEntries)
        {
            IList<TileAttribute> attributes = new List<TileAttribute>();
            if (attributeEntries == null)
            {
                return attributes;
            }
            foreach (IAttributeEntry attributeEntry in attributeEntries)
            {
                IAttributeCreator creator = ATTRIBUTE_CREATORS.Find(c => c.Applies(attributeEntry));
                TileAttribute tile = creator.Create(attributeEntry, this);
                attributes.Add(tile);
            }
            return attributes;
        }


        public ITemplate GetTemplate(IEntry entry, bool throwException)
        {
            return _factory.Handle(entry.Value, throwException);
        }
    }
}