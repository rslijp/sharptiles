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
        private readonly bool _returnLiterals;
        private readonly bool _returnSeperator;
        private readonly ICollection<string> _seperators;
        private readonly ICollection<string> _whiteSpaceSeperators;
        private readonly bool _expectLiterals;

        public TokenizerConfiguration(
            char? escapeCharacter,
            string[] seperators,
            string[] whiteSpaceSeperators,
            char[] literals,
            bool returnSeperator,
            bool returnLiterals
            ) : this(
                escapeCharacter, 
                new SortedSet<string>(seperators), 
                whiteSpaceSeperators!=null?new SortedSet<string>(whiteSpaceSeperators):null,
                new SortedSet<char>(literals != null ? literals : new char[] { }), 
                returnSeperator,
                returnLiterals)
        {
           
        }

        public TokenizerConfiguration(
           char? escapeCharacter,
           ICollection<string> seperators,
           ICollection<string> whiteSpaceSeperators,
           ICollection<char> literals,
           bool returnSeperator,
           bool returnLiterals
           ) : this(
               escapeCharacter, 
               seperators, CalculaterMaxTokenLength(seperators), 
               whiteSpaceSeperators, CalculaterMaxTokenLength(whiteSpaceSeperators), 
               literals,
               returnSeperator,
               returnLiterals
               )
        {
           
        }

        public TokenizerConfiguration(
//           string template,
           char? escapeCharacter,
           ICollection<string> seperators,
           int maxSeperatorLength,
           ICollection<string> whiteSpaceSeperators,
           int maxWhiteSpaceSeperatorLength,
           ICollection<char> literals,
           bool returnSeperator,
           bool returnLiterals
           )
        {
            _escapeCharacter = escapeCharacter;
            _seperators = seperators;
            _whiteSpaceSeperators = whiteSpaceSeperators;
            if (whiteSpaceSeperators == null)
            {
                _whiteSpaceSeperators = new HashSet<string>();
            }
            _literals = literals;
            _expectLiterals = literals != null && literals.Count > 0;
            _returnSeperator = returnSeperator;
            _returnLiterals = returnLiterals;

            _maxSeperatorLength = maxSeperatorLength;
            _maxWhiteSpaceSeperatorLength = maxWhiteSpaceSeperatorLength;
        }


        public char? EscapeCharacter
        {
            get { return _escapeCharacter; }
        }

        public ICollection<string> Seperators
        {
            get { return _seperators; }
        }

        public ICollection<string> WhiteSpaceSeperators
        {
            get { return _whiteSpaceSeperators; }
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

        public int MaxSeperatorLength => _maxSeperatorLength;
        public int MaxWhiteSpaceSeperatorsLength => _maxWhiteSpaceSeperatorLength;
        public bool ExpectLiterals => _expectLiterals;


        public static int CalculaterMaxTokenLength(ICollection<string> seps)
        {
            if (seps == null) return 0;
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



        

        public bool IsWhiteSpaceSeperator(string seperator)
        {
            return _whiteSpaceSeperators.Contains(seperator);
        }

//        public void DontExpectLiteralsAnyMore()
//        {
//            _expectLiterals = false;
//        }
//
//        public void ExpectLiteralsAgain()
//        {
//            _expectLiterals = true;
//        }

    }
}
