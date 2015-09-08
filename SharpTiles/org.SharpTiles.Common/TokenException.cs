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
    public class TokenException : ExceptionWithContext
    {
        public TokenException(string msg)
            : base(msg)
        {
        }

        public static PartialExceptionWithContext<TokenException> MoreCharactersExpectedAt(int index)
        {
            string msg = String.Format("Expected a character at {0}", index);
            return MakePartial(new TokenException(msg));
        }


        public static PartialExceptionWithContext<TokenException> UnTerminatedLiteral(int index)
        {
            string msg = String.Format("Literal at {0} is not terminated.", index);
            return MakePartial(new TokenException(msg));
        }


        public static PartialExceptionWithContext<TokenException> IllegalEscapeCharacter(char c)
        {
            string msg = String.Format("Illegal escaped character {0}", c);
            return MakePartial(new TokenException(msg));
        }

        public static PartialExceptionWithContext<TokenException> IllegalStateAtEnd(int index, TokenizerState state)
        {
            string msg =
                String.Format("Tokenizer got invalid state at position {0}. Tokenizer was in state {1}", index, state);
            return MakePartial(new TokenException(msg));
        }


        public static PartialExceptionWithContext<TokenException> IllegalStateAt(char c, int index, TokenizerState state)
        {
            string msg =
                String.Format("Tokenizer got invalid state at position {1}({0}). Tokenizer was in state {2}", c, index,
                              state);
            return MakePartial(new TokenException(msg));
        }

        public static PartialExceptionWithContext<TokenException> Expected(object method, int index)
        {
            string msg = String.Format("Expected {0} at {1}", method, index);
            return MakePartial(new TokenException(msg));
        }
    }
}
