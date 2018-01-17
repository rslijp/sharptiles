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
 using org.SharpTiles.Common;
using org.SharpTiles.Tags;
 using org.SharpTiles.Tags.Creators;

namespace org.SharpTiles.Templates
{
    public class TextPart : ITemplatePart
    {
        private readonly ParseContext _context;
        private string _text;

        public TextPart(Token token) : this(token.Contents, token.Context)
        {
        }

        public TextPart(string text, ParseContext context)
        {
            _text = text;
            _context = context;
        }

        #region ITemplatePart Members

        public object Evaluate(TagModel model)
        {
            return _text;
        }

        public object ConstantValue
        {
            get { return _text; }
        }

        public ParseContext Context
        {
            get { return _context; }
        }

        #endregion

        public void Append(TextPart part)
        {
            _text += part._text;
        }

        public override string ToString() => _text;
    }
}
