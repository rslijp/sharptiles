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

namespace org.SharpTiles.Common
{
    public class Token
    {
        private readonly int _index;
        private readonly string _text;
        private readonly TokenType _type;
        private string _contents;

        public Token(TokenType type, string contents, int index, string text)
        {
            _type = type;
            _contents = contents;
            _index = index;
            _text = text;
        }

        public string Contents
        {
            get { return _contents; }
        }


        public TokenType Type
        {
            get { return _type; }
        }

        public bool HasValue
        {
            get { return !String.IsNullOrEmpty(_contents); }
        }

        public bool HasValueAfterTrim
        {
            get { return !String.IsNullOrEmpty((_contents??"").Trim()); }
        }

        public int Index
        {
            get { return _index; }
        }

        public ParseContext Context
        {
            get { return new ParseContext(Contents, Index, Text); }
        }

        public string Text
        {
            get { return _text; }
        }

        public void Trim()
        {
            _contents = _contents.Trim();
        }

        public override string ToString()
        {
            return _type + "[" + Index + "]:" + _contents;
        }
    }
}
