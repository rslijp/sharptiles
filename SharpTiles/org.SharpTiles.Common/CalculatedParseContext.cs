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
    public class CalculatedParseContext
    {
        private readonly string _contents;
        private string _text;
        private IList<ParseContext.ContextLine> _context;
        private int _lineIndex;
        private int _lineNumber;
        private string _lineWithPosition;

        public CalculatedParseContext(string contents, int index, string text)
        {
            _contents = contents;
            Index = index;
            _text = text;
            Lines = _text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            CalculateLineNumber();
            CreateContext();
            CreateLineWithPosition();
        }

        public int LineIndex => _lineIndex;

        public string Line => Lines[_lineNumber - 1];


        public int LineNumber => _lineNumber;

        public string[] Lines { get; }

        public int Index { get; }

        private object Contents => _contents;

        public string LineWithPosition => _lineWithPosition;

        public IList<ParseContext.ContextLine> Context => _context;

        private void CreateLineWithPosition()
        {
            var posSb = new StringBuilder();
            for (int i = 0; i <= LineIndex - 1; i++)
            {
                posSb.Append(Line[i] == '\t' ? "----" : "-");
            }
            posSb.Append(Line.Length == 0 || Line[LineIndex] == '\t' ? "---^" : "^");
            _lineWithPosition = Line.Replace("\t", "    ") + Environment.NewLine + posSb;
        }

        private void CreateContext()
        {
            _context = new List<ParseContext.ContextLine>();
            int start = Math.Max(_lineNumber - 2, 1);
            int end = Math.Min(_lineNumber + 2, Lines.Length - 1);
            for (int i = start - 1; i < end; i++)
            {
                _context.Add(new ParseContext.ContextLine(i + 1, Lines[i]));
            }
        }

        private void CalculateLineNumber()
        {
            int lineNumber = 0;
            int restIndex = Index;
            while (restIndex >= 0 && lineNumber <= Lines.Length)
            {
                _lineIndex = restIndex;
                restIndex -= (Lines[lineNumber].Length + Environment.NewLine.Length);
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
            sb.Append($"At line {_lineNumber},{_lineIndex + 1}: {Contents}");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append(LineWithPosition);
            sb.Append(Environment.NewLine);
            if (Context.Count > 1)
            {
                sb.Append(Environment.NewLine);
                foreach (ParseContext.ContextLine line in Context)
                {
                    sb.Append(line);
                    sb.Append(Environment.NewLine);
                }
            }
            return sb.ToString();
        }
    }
}
