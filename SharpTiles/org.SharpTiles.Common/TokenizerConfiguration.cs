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

namespace org.SharpTiles.Common
{
    public class TokenizerConfiguration
    {
        private readonly char? _escapeCharacter;
        private readonly ICollection<char> _literals;
        private readonly int _maxSeperatorLength;
        private readonly int _maxWhiteSpaceSeperatorLength;
        private bool _returnLiterals;
        private readonly bool _returnSeperator;
        private readonly ICollection<string> _seperators;
        private readonly string _template;
        private readonly ICollection<string> _whiteSpaceSeperators;
        private bool _expectLiterals = true;

        public TokenizerConfiguration(
            string template,
            char? escapeCharacter,
            string[] seperators,
            string[] whiteSpaceSeperators,
            char[] literals,
            bool returnSeperator,
            bool returnLiterals
            )
        {
            _template = template;
            _escapeCharacter = escapeCharacter;
            _seperators = new HashSet<string>(seperators);
            if (whiteSpaceSeperators != null)
            {
                _whiteSpaceSeperators = new HashSet<string>(whiteSpaceSeperators);
            }
            else
            {
                _whiteSpaceSeperators = new HashSet<string>();
            }
            _literals = new HashSet<char>(literals != null ? literals : new char[] {});
            _returnSeperator = returnSeperator;
            _returnLiterals = returnLiterals;

            _maxSeperatorLength = CalculaterMaxTokenLength(_seperators);
            _maxWhiteSpaceSeperatorLength = CalculaterMaxTokenLength(_whiteSpaceSeperators);
        }

        public string Template
        {
            get { return _template; }
        }

        public char? EscapeCharacter
        {
            get { return _escapeCharacter; }
        }

        public ICollection<string> Seperators
        {
            get { return _seperators; }
        }

        public ICollection<char> Literals
        {
            get { return _literals; }
        }

        public bool ReturnSeperator
        {
            get { return _returnSeperator; }
        }

        public bool ReturnLiterals
        {
            get { return _returnLiterals; }
//            internal set { _returnLiterals = value; }
        }

        private int CalculaterMaxTokenLength(ICollection<string> seps)
        {
            int result = 0;
            foreach (string sep in seps)
            {
                result = Math.Max(result, sep.Length);
            }
            return result;
        }

        public bool IsEscapeCharacter(char c)
        {
            return EscapeCharacter.HasValue ? Equals(EscapeCharacter, c) : false;
        }

        public bool IsLiteral(char c)
        {
            return _expectLiterals ? Literals.Contains(c) : false;
        }

        public string StartsWithSeperator(int offset)
        {
            string result = SeperatorAt(offset, _seperators, _maxSeperatorLength);
            if (result == null)
            {
                result = StartsWithWhiteSpaceSurroundedSeperator(offset);
            }
            return result;
        }

        private string SeperatorAt(int offset, ICollection<string> seperators, int maxLength)
        {
            
            if (!_expectLiterals && 
                _template.Length<offset &&
                Literals.Contains(_template[offset]))
            {
                return _template[offset].ToString();
            }
        
            string result = null;
            string sub = _template.Substring(offset);
            for (int i = 1; i <= maxLength && i <= sub.Length; i++)
            {
                string sep = sub.Substring(0, i);
                if (seperators.Contains(sep))
                {
                    result = sep;
                }
            }
            return result;
        }

        public string StartsWithWhiteSpaceSurroundedSeperator(int offset)
        {
            string result = null;
            bool whiteSpaceBefore = IndexIsWhiteSpace(offset - 1);
            if (whiteSpaceBefore)
            {
                result = SeperatorAt(offset, _whiteSpaceSeperators, _maxWhiteSpaceSeperatorLength);
            }
            bool whiteSpaceAfter = result != null && IndexIsWhiteSpace(offset + result.Length);
            if (!whiteSpaceAfter)
            {
                result = null;
            }
            return result;
        }

        private bool IndexIsWhiteSpace(int i)
        {
            if (i < 0 || _template.Length <= i)
            {
                return true;
            }
            return Char.IsWhiteSpace(_template[i]);
        }

        public bool IsWhiteSpaceSeperator(string seperator)
        {
            return _whiteSpaceSeperators.Contains(seperator);
        }

        public void DontExpectLiteralsAnyMore()
        {
            _expectLiterals = false;
        }

        public void ExpectLiteralsAgain()
        {
            _expectLiterals = true;
        }
    }
}
