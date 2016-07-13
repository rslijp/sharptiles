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
 using System.Text;

namespace org.SharpTiles.Common
{
    public class ParseContext
    {
        private readonly string _contents;
        private int _index;
        private string _text;

        private CalculatedParseContext _calculatedParseContext;
      
        public ParseContext(string contents, int index, string text)
        {
            _contents = contents;
            _index = index;
            _text = text;
        }

        private void CalculateParseContext()
        {
            if (_calculatedParseContext == null)
                _calculatedParseContext = new CalculatedParseContext(_contents, _index, _text);
        }

        public int LineIndex
        {
            get
            {
                CalculateParseContext();
                return _calculatedParseContext.LineIndex;
            }
        }

        public string Line
        {
            get
            {
                CalculateParseContext();
                return _calculatedParseContext.Line;
            }
        }


        public string Text => _text;

        public int Index => _index;

        private object Contents => _contents;

        public int LineNumber
        {
            get
            {
                CalculateParseContext();
                return _calculatedParseContext.LineNumber;
            }
        }

        public string[] Lines
        {
            get
            {
                CalculateParseContext();
                return _calculatedParseContext.Lines;
            }
        }

        public string LineWithPosition
        {
            get
            {
                CalculateParseContext();
                return _calculatedParseContext.LineWithPosition;
            }
        }

        public IList<ContextLine> Context
        {
            get
            {
                CalculateParseContext();
                return _calculatedParseContext.Context;
            }
        }

        public ParseContext Add(ParseContext offset)
        {
            return new ParseContext(offset._contents, _index + offset._index+1, _text);
        }

        public void SetGlobalContext(ParseContext context)
        {
            if (context == null) return;
            _index = _index + context._index;
            _text = context._text;
            _calculatedParseContext = null;
        }

        public override string ToString()
        {
            CalculateParseContext();
            return _calculatedParseContext.ToString();
        }

        #region Nested type: ContextLine

        public class ContextLine
        {
            private readonly string _line;
            private readonly int _lineNumber;


            public ContextLine(int lineNumber, string line)
            {
                _lineNumber = lineNumber;
                _line = line;
            }

            public int LineNumber
            {
                get { return _lineNumber; }
            }

            public string Line
            {
                get { return _line; }
            }

            public override string ToString()
            {
                return String.Format("{0:00000}: {1}", _lineNumber, _line);
            }
        }

        #endregion

        
    }
}
