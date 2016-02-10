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
 using System.IO;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
 using org.SharpTiles.Tags.Creators;

namespace org.SharpTiles.Tiles.Tile
{
    public class TemplateTile : ITile
    {
        private readonly string _name;

        public AttributeSet Attributes
        {
            get { return _attributes; }
        }

        private readonly AttributeSet _attributes;

        private readonly ITemplate _template;

        public TemplateTile(string name, ITemplate template, IEnumerable<TileAttribute> attributes)
        {
            _name = name;
            _template = template;
            _attributes = new AttributeSet(name, attributes);
        }

        public ITemplate Template
        {
            get { return _template; }
        }

//        public ParsedTemplate ParsedTemplate
//        {
//            get { return _template.Template; }
//        }

        #region ITile Members

        public string Name
        {
            get { return _name; }
        }

        public string Render(TagModel model)
        {
            return Render(model, Attributes);
        }

        public string Render(TagModel model, AttributeSet attributes)
        {
            try
            {
                using (model.Decorate().With(attributes))
                {
                    return _template.Evaluate(model);
                }
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
                throw TileExceptionWithContext.ErrorInTile(_template.Description, EWC);
            }
            catch (Exception e)
            {
                throw TileException.ErrorInTile(_template.Description, e);
            }
        }

        #endregion
    }
}
