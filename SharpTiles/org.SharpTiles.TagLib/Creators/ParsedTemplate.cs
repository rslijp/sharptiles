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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using org.SharpTiles.Common;
using org.SharpTiles.Tags.CoreTags;
using org.SharpTiles.Tags.FormatTags;

namespace org.SharpTiles.Tags.Creators
{
    public class ParsedTemplate : IEnumerable<ITemplatePart>
    {
        private readonly ParseContext _context;
        private readonly IResourceLocator _locator;
        private readonly IList<ITemplatePart> _templateParsed;

        public ParsedTemplate(IResourceLocator locator, ITemplatePart templateParsed)
        {
            _locator = locator;
            _templateParsed = new List<ITemplatePart>(new[] {templateParsed});
            _context = templateParsed != null ? templateParsed.Context : null;
        }

        public ParsedTemplate(IResourceLocator locator, IList<ITemplatePart> templateParsed)
        {
            _locator = locator;
            _templateParsed = templateParsed;
            _context = templateParsed != null && templateParsed.Count > 0 && templateParsed[0] != null
                           ? templateParsed[0].Context
                           : null;
        }


        public IList<ITemplatePart> TemplateParsed
        {
            get { return _templateParsed; }
        }

        public int Count
        {
            get { return _templateParsed.Count; }
        }

        public ParseContext Context
        {
            get { return _context; }
        }

        public IResourceLocator ResourceLocator
        {
            get { return _locator; }
        }

        public ITemplatePart this[int i]
        {
            get { return _templateParsed[i]; }
        }

        #region IEnumerable<ITemplatePart> Members

        public IEnumerator<ITemplatePart> GetEnumerator()
        {
            return _templateParsed.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _templateParsed.GetEnumerator();
        }

        #endregion

        public string Evaluate(TagModel model)
        {
            var sb = new StringBuilder();
           foreach (var part in _templateParsed)
            {
                var raw = part.Evaluate(model);
                var value = BaseCoreTag.ValueOfWithi18N(model, raw);
                sb.Append(value);
            }
            return sb.ToString();
        }

       
    }
}