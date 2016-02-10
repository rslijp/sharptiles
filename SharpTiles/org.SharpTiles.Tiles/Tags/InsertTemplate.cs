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
using System.Linq;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.CoreTags;
using org.SharpTiles.Tiles.Configuration;
using org.SharpTiles.Tiles.Factory;
using org.SharpTiles.Tiles.Tile;

namespace org.SharpTiles.Tiles.Tags
{
    [HasExample]
    public class InsertTemplate : BaseCoreTag, ITagWithResourceFactory, ITagWithNestedTags
    {
        public static readonly string NAME = "insertTemplate";

        private readonly IList<PutAttribute> _nestedTags = new List<PutAttribute>();
        private ITile _tile;

        public ITile Tile
        {
            get { return _tile; }
        }

        [Required]
        public ITagAttribute Template { get; set; }

        private IResourceLocator ResourceLocator
        {
            get { return Template.ResourceLocator; }
        }


        #region ITagWithNestedTags Members

        public string TagName
        {
            get { return NAME; }
        }

        public string Evaluate(TagModel model)
        {
            var tileName = GetAutoValueAsString("Template", model);

            try
            {
                LoadTile(model, tileName);
                return _tile.Render(model) ?? String.Empty;
            }
            catch (TileExceptionWithContext)
            {
                throw;
            }
            catch (TileException)
            {
                throw;
            }
            catch (ExceptionWithContext EWC)
            {
                throw TileExceptionWithContext.ErrorInTile(tileName, EWC);
            }
            catch (Exception e)
            {
                throw TileException.ErrorInTile(tileName, e);
            }
        }

        private void LoadTile(TagModel model, string tileName)
        {
            lock (this)
            {
                if (_tile == null || !Equals(tileName, _tile.Name))
                {
                    var template = Factory.Handle(tileName, ResourceLocator, true);
                    _tile = new TemplateTile(
                        tileName, 
                        template,  
                        new TilesFactory(Factory).CreateAttributes(AsAttributeEntries(model)));
                }
            } 
        }

        public void AddNestedTag(ITag tag)
        {
            if (tag is PutAttribute)
            {
                _nestedTags.Add((PutAttribute) tag);
            }
            else
            {
                throw TagException.OnlyNestedTagsOfTypeAllowed(tag.GetType(), typeof (PutAttribute)).Decorate(
                    tag.Context);
            }
        }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.NestedTags; }
        }

        #endregion

        private IEnumerable<IAttributeEntry> AsAttributeEntries(TagModel model)
        {
            return _nestedTags.Select(n => n.Yield(model));
        }
    }
}