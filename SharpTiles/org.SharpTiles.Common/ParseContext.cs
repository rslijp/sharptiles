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
 using System.Runtime.Serialization;
 using System.Text;

namespace org.SharpTiles.Common
{
    public class ParseContext
    {
        private readonly string _contents;
        private int _index;
        private readonly string[] _lines;
        private string _text;
        private IList<ContextLine> _context;
        private int _lineIndex;
        private int _lineNumber;
        private string _lineWithPosition;
      
        public ParseContext(string contents, int index, string text)
        {
            _contents = contents;
            _index = index;
            _text = text;
            _lines = _text.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            CalculateLineNumber();
            CreateContext();
            CreateLineWithPosition();
        }

        public int LineIndex
        {
            get { return _lineIndex; }
        }

        public string Line
        {
            get { return Lines[_lineNumber - 1]; }
        }


        public string Text
        {
            get { return _text; }
        }

        public int LineNumber
        {
            get { return _lineNumber; }
        }

        public string[] Lines
        {
            get { return _lines; }
        }

        public int Index
        {
            get { return _index; }
        }

        private object Contents
        {
            get { return _contents; }
        }

        public string LineWithPosition
        {
            get { return _lineWithPosition; }
        }

        public IList<ContextLine> Context
        {
            get { return _context; }
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
        }

        private void CreateLineWithPosition()
        {
            var posSb = new StringBuilder();
            for (int i = 0; i <= LineIndex - 1; i++)
            {
                posSb.Append(Line[i] == '\t' ? "----" : "-");
            }
            posSb.Append(Line.Length==0 || Line[LineIndex] == '\t' ? "---^" : "^");
            _lineWithPosition = Line.Replace("\t", "    ") + Environment.NewLine + posSb;
        }

        private void CreateContext()
        {
            _context = new List<ContextLine>();
            int start = Math.Max(_lineNumber - 2, 1);
            int end = Math.Min(_lineNumber + 2, Lines.Length - 1);
            for (int i = start - 1; i < end; i++)
            {
                _context.Add(new ContextLine(i + 1, Lines[i]));
            }
        }

        private void CalculateLineNumber()
        {
            int lineNumber = 0;
            int restIndex = Index;
            while (restIndex >= 0 && lineNumber <= _lines.Length)
            {
                _lineIndex = restIndex;
                restIndex -= (_lines[lineNumber].Length + Environment.NewLine.Length);
                lineNumber++;
            }
            if (restIndex >= -Environment.NewLine.Length)
            {
                lineNumber++;
                _lineIndex = 0;
            }
            _lineNumber = lineNumber;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(String.Format("At line {0},{1}: {2}", _lineNumber, _lineIndex + 1, Contents));
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append(LineWithPosition);
            sb.Append(Environment.NewLine);
            if (Context.Count > 1)
            {
                sb.Append(Environment.NewLine);
                foreach (ContextLine line in Context)
                {
                    sb.Append(line);
                    sb.Append(Environment.NewLine);
                }
            }
            return sb.ToString();
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
