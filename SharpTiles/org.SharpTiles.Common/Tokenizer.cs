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
 using System.Collections;
using System.Collections.Generic;

namespace org.SharpTiles.Common
{
    public class Tokenizer : IEnumerable<Token>
    {
        private readonly TokenizerConfiguration _configuration;

        public Tokenizer(string template, char? escapeCharacter, string[] seperators, char[] literals)
            : this(template, false, escapeCharacter, seperators, literals, null)
        {
        }

        public Tokenizer(string template, char? escapeCharacter, string[] seperators, char[] literals,
                         string[] whiteSpaceSeperators)
            : this(template, false, escapeCharacter, seperators, literals, whiteSpaceSeperators)
        {
        }

        public Tokenizer(string template, bool returnSeperator, char? escapeCharacter, string[] seperators,
                         char[] literals)
            : this(template, returnSeperator, escapeCharacter, seperators, literals, null)
        {
        }

        public Tokenizer(string template, bool returnSeperator, char? escapeCharacter, string[] seperators,
                         char[] literals, string[] whiteSpaceSeperators)
            : this(template, returnSeperator, false, escapeCharacter, seperators, literals, whiteSpaceSeperators)

        {
        }

        public Tokenizer(string template, bool returnSeperator, bool returnLiterals, char? escapeCharacter,
                         string[] seperators, char[] literals, string[] whiteSpaceSeperators)
        {
            _configuration =
                new TokenizerConfiguration(template, escapeCharacter, seperators, whiteSpaceSeperators, literals,
                                           returnSeperator, returnLiterals);
        }

        #region IEnumerable<Token> Members

        public IEnumerator<Token> GetEnumerator()
        {
            return GetTokenEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetTokenEnumerator();
        }

        #endregion

        public TokenEnumerator GetTokenEnumerator()
        {
            return new TokenEnumerator(_configuration);
        }
    }
}
