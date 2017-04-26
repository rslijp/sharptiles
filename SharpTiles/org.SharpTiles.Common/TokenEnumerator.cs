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
 using System.Runtime.CompilerServices;
 using System.Text;

namespace org.SharpTiles.Common
{
    public class TokenEnumerator : IEnumerator<Token>
    {
        private int _commitPoint;
        private Stack<TokenizerConfiguration> _configurationStack = new Stack<TokenizerConfiguration>();
        private Token _current;
        private int _index;
        private TokenizerState _state = TokenizerState.NotSet;
        private ParseContext _offSet;
        private bool _overrideNoLiterals;
        private string _template;
        private int _templateLength;

        public TokenEnumerator(string template, TokenizerConfiguration configuration, ParseContext offSet=null)
        {
            _template = template;
            _templateLength = template.Length;
            _offSet = offSet;
            PushConfiguration(configuration);

        }

        #region IEnumerator<Token> Members

        Token IEnumerator<Token>.Current
        {
            get { return _current; }
        }

        public void Dispose()
        {
            _configurationStack.Clear();
            _configurationStack = null;
            _configuration = null;
        }

        #endregion

        #region enumeration

        private bool RequiresRecursion
        {
            get
            {
                return _current != null &&
                       _current.Type == TokenType.Seperator &&
                       !_configuration.ReturnSeperator;
            }
        }

        private TokenizerConfiguration _configuration;

        public bool MoveNext()
        {
            _current = null;
            while (HasNext() && !HasResult)
            {
                InternalMoveNext();
            }
            GuardEndState();
            if (RequiresRecursion)
            {
                return MoveNext();
            }
            return HasResult;
        }

//        public bool MoveNextIgnoreWhiteSpace()
//        {
//            _index+=Configuration.JumpWhiteSpace(_index);
//            return MoveNext();
//        }

        private void InternalMoveNext()
        {
            DetermineState();
            switch (_state)
            {
                case TokenizerState.Seperator:
                    HandeSeperator();
                    break;
                case TokenizerState.Literal:
                    if (_configuration.ReturnLiterals)
                    {
                        HandeLiteral();
                    }
                    else
                    {
                        HandeNormal();
                    }
                    break;
                case TokenizerState.Normal:
                    HandeNormal();
                    break;
                default:
                    throw TokenException.IllegalStateAt(
                        CurrentChar(),
                        _index,
                        _state).Decorate(_current);
            }
        }

        private void HandeLiteral()
        {
            int start = _index;
            var builder = new StringBuilder();
            HandeLiteral(start, builder);
            _current = MakeLiteralToken(start, builder.ToString());
        }

        private void HandeLiteral(int start, StringBuilder builder)
        {
            char open = NextChar(); // Literal
            builder.Append(open);
            while (HasNext() && !(Literal() && open == CurrentChar()))
            {
                if (Escape())
                {
                    NextChar(); // Escape
                }
                builder.Append(NextChar(() => ThrowUnClosedLiteral(start)));
            }
            builder.Append(NextChar(() => ThrowUnClosedLiteral(start)));
        }

        private void ThrowUnClosedLiteral(int start)
        {
            throw TokenException.UnTerminatedLiteral(start+1).Decorate(
                new Token(TokenType.Literal,
                          _template[start].ToString(),
                          start,
                          _template, 
                          _offSet
                    )
                );
        }

        private string SeperatorAt(int offset, ICollection<string> seperators, int maxLength)
        {

            if (!_configuration.ExpectLiterals&&
                _templateLength < offset &&
                _configuration.Literals.Contains(_template[offset]))
            {
                return _template[offset].ToString();
            }

            var remaining = _templateLength - offset;
            var length = remaining < maxLength ? remaining : maxLength;
            for (int i = length; i > 0; i--)
            {
                var sep = _template.Substring(offset, i);
                if (seperators.Contains(sep))
                {
                    return sep;
                }
            }
            return null;
        }

        public string StartsWithWhiteSpaceSurroundedSeperator(int offset)
        {
            string result = null;
            bool whiteSpaceBefore = IndexIsWhiteSpace(offset - 1);
            if (whiteSpaceBefore)
            {
                result = SeperatorAt(offset, _configuration.WhiteSpaceSeperators, _configuration.MaxWhiteSpaceSeperatorsLength);
            }
            bool whiteSpaceAfter = result != null && IndexIsWhiteSpace(offset + result.Length);
            if (!whiteSpaceAfter)
            {
                result = null;
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IndexIsWhiteSpace(int i)
        {
            if (i < 0 || _templateLength <= i)
            {
                return true;
            }
            return char.IsWhiteSpace(_template[i]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string StartsWithSeperator(int offset)
        {
            return SeperatorAt(offset, _configuration.Seperators, _configuration.MaxSeperatorLength) 
                    ?? StartsWithWhiteSpaceSurroundedSeperator(offset);
        }


        private void HandeSeperator()
        {
            int start = _index;
            string seperator = StartsWithSeperator(start);
            Read(seperator);
            if (_configuration.IsWhiteSpaceSeperator(seperator) && HasNext())
            {
                Read(WhiteSpace);
            }
            _current = MakeSeperatorToken(start, seperator);
        }

        private void HandeNormal()
        {
            int start = _index;
            var builder = new StringBuilder();
            while (HasNext() &&
                   !(
                        Seperator() ||
                        (_configuration.ReturnLiterals && Literal()))
                )
            {
                if (Escape())
                {
                    NextChar();
                    builder.Append(NextChar());
                }
                else if (Literal())
                {
                    HandeLiteral(_index, builder);
                }
                else
                {
                    builder.Append(NextChar());
                }
            }
            var tokenStr = builder.ToString();
            if (WhiteSpaceSurroundedSeperator())
            {
                tokenStr = tokenStr.TrimEnd();
            }
            _current = MakeRegularToken(start, tokenStr);
        }

        private void DetermineState()
        {
            if (Literal())
            {
                _state = TokenizerState.Literal;
            }
            else if (Seperator())
            {
                _state = TokenizerState.Seperator;
            }
            else
            {
                _state = TokenizerState.Normal;
            }
        }

        private char Read(Is isMethod)
        {
            if (!isMethod())
            {
                throw TokenException.Expected(isMethod, _index).Decorate(_current);
            }
            return NextChar();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Literal()
        {
            return !_overrideNoLiterals && _configuration.IsLiteral(_template[_index]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Escape()
        {
            return _configuration.IsEscapeCharacter(_template[_index]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool WhiteSpace()
        {
            return Char.IsWhiteSpace(_template[_index]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Seperator()
        {
            return StartsWithSeperator(_index) != null;
        }

        public bool WhiteSpaceSurroundedSeperator()
        {
            return StartsWithWhiteSpaceSurroundedSeperator(_index) != null;
        }

        private void GuardEndState()
        {
            if (!HasNext() && _state != TokenizerState.Normal)
            {
                TokenException.IllegalStateAtEnd(_index, _state);
            }
        }

        private delegate bool Is();

        #endregion

        #region Handle states

        #endregion

        #region Helper methods

        private bool HasResult
        {
            get { return _current != null; }
        }

        public Token Token
        {
            get { return _current; }
        }

        public void Reset()
        {
            _index = 0;
        }

        public object Current
        {
            get { return _current; }
        }

        private Token MakeLiteralToken(int start, string literal)
        {
            return new Token(TokenType.Literal, literal.Substring(1, literal.Length - 2), start, _template, _offSet);
        }

        private Token MakeSeperatorToken(int start, string seperator)
        {
            return new Token(TokenType.Seperator, seperator, start, _template, _offSet);
        }

        private Token MakeRegularToken(int start, string content)
        {
            return new Token(TokenType.Regular, content, start, _template, _offSet);
        }

        private char CurrentChar()
        {
            return _template[_index];
        }

        private void Read(string str)
        {
            foreach (char c in str)
            {
                if (c != NextChar())
                {
                    throw new TokenException("Internal failure expected " + c);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private char NextChar()
        {
            return NextChar(ThrowExpectedMoreCharacters);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private char NextChar(Action onNoMoreCharacters)
        {
            if (!HasNext())
                onNoMoreCharacters();

            return _template[_index++];
        }

        private void ThrowExpectedMoreCharacters()
        {
            throw TokenException.MoreCharactersExpectedAt(_index).Decorate(
                _current ??
                new Token(TokenType.NotSet, 
                          _template[_index-1].ToString(),
                          _index-1,
                          _template, _offSet

                    )
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasNext()
        {
            return _index < _templateLength;
        }

        #endregion

        public void Commit()
        {
            _commitPoint = (_current != null ? _current.Index : _index);
        }

        public void Rollback()
        {
            _index = _commitPoint;
        }

        
        public void PushConfiguration(TokenizerConfiguration configuration)
        {
            _configurationStack.Push(configuration);
            _configuration = configuration;
        }

        public void PopConfiguration()
        {
            _configurationStack.Pop();
            _configuration = _configurationStack.Peek();
        }

        public void SetIndexTo(int index)
        {
            _index = index;
        }

        public void DontExpectLiteralsAnyMore()
        {
            _overrideNoLiterals = true;
        }

        public void ExpectLiteralsAgain()
        {
            _overrideNoLiterals = false;
        }
    }
}
