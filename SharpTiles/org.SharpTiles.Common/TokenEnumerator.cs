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
    public class TokenEnumerator : IEnumerator<Token>
    {
        private int _commitPoint;
        private Stack<TokenizerConfiguration> _configurationStack = new Stack<TokenizerConfiguration>();
        private Token _current;
        private int _index;
        private TokenizerState _state = TokenizerState.NotSet;

        public TokenEnumerator(TokenizerConfiguration configuration)
        {
            _configurationStack.Push(configuration);
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
        }

        #endregion

        #region enumeration

        private bool RequiresRecursion
        {
            get
            {
                return _current != null &&
                       _current.Type == TokenType.Seperator &&
                       !Configuration.ReturnSeperator;
            }
        }

        private TokenizerConfiguration Configuration
        {
            get { return _configurationStack.Peek(); }
        }

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

        private void InternalMoveNext()
        {
            DetermineState();
            switch (_state)
            {
                case TokenizerState.Seperator:
                    HandeSeperator();
                    break;
                case TokenizerState.Literal:
                    if (Configuration.ReturnLiterals)
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
            char open = Read(Literal);
            builder.Append(open);
            while (HasNext() && !(At(Literal) && open == CurrentChar()))
            {
                if (At(Escape))
                {
                    Read(Escape);
                }
                builder.Append(NextChar(() => ThrowUnClosedLiteral(start)));
            }
            builder.Append(NextChar(() => ThrowUnClosedLiteral(start)));
        }

        private void ThrowUnClosedLiteral(int start)
        {
            throw TokenException.UnTerminatedLiteral(start+1).Decorate(
                new Token(TokenType.Literal,
                          Configuration.Template[start].ToString(),
                          start,
                          Configuration.Template
                    )
                );
        }

        private void HandeSeperator()
        {
            int start = _index;
            string seperator = Configuration.StartsWithSeperator(start);
            Read(seperator);
            if (Configuration.IsWhiteSpaceSeperator(seperator) && HasNext())
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
                        At(Seperator) ||
                        (Configuration.ReturnLiterals && At(Literal)))
                )
            {
                if (At(Escape))
                {
                    Read(Escape);
                    builder.Append(NextChar());
                }
                else if (At(Literal))
                {
                    HandeLiteral(_index, builder);
                }
                else
                {
                    builder.Append(NextChar());
                }
            }
            string tokenStr = builder.ToString();
            if (At(WhiteSpaceSurroundedSeperator))
            {
                tokenStr = tokenStr.TrimEnd();
            }
            _current = MakeRegularToken(start, tokenStr);
        }

        private void DetermineState()
        {
            if (At(Literal))
            {
                _state = TokenizerState.Literal;
            }
            else if (At(Seperator))
            {
                _state = TokenizerState.Seperator;
            }
            else
            {
                _state = TokenizerState.Normal;
            }
        }

        private bool At(Is isMethod)
        {
            return isMethod(_index);
        }

        private char Read(Is isMethod)
        {
            if (!At(isMethod))
            {
                throw TokenException.Expected(isMethod, _index).Decorate(_current);
            }
            return NextChar();
        }

        private bool Literal(int index)
        {
            return Configuration.IsLiteral(Configuration.Template[index]);
        }

        private bool Escape(int index)
        {
            return Configuration.IsEscapeCharacter(Configuration.Template[index]);
        }

        private bool WhiteSpace(int index)
        {
            return Char.IsWhiteSpace(Configuration.Template[index]);
        }

        public bool Seperator(int index)
        {
            return Configuration.StartsWithSeperator(_index) != null;
        }

        public bool WhiteSpaceSurroundedSeperator(int index)
        {
            return Configuration.StartsWithWhiteSpaceSurroundedSeperator(_index) != null;
        }

        private void GuardEndState()
        {
            if (!HasNext() && _state != TokenizerState.Normal)
            {
                TokenException.IllegalStateAtEnd(_index, _state);
            }
        }

        private delegate bool Is(int index);

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
            return new Token(TokenType.Literal, literal.Substring(1, literal.Length - 2), start, Configuration.Template);
        }

        private Token MakeSeperatorToken(int start, string seperator)
        {
            return new Token(TokenType.Seperator, seperator, start, Configuration.Template);
        }

        private Token MakeRegularToken(int start, string content)
        {
            return new Token(TokenType.Regular, content, start, Configuration.Template);
        }

        private char CurrentChar()
        {
            return Configuration.Template[_index];
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

        private char NextChar()
        {
            return NextChar(ThrowExpectedMoreCharacters);
        }

        private char NextChar(Action onNoMoreCharacters)
        {
            if (!HasNext())
            {
                onNoMoreCharacters();
            }
            char c = Configuration.Template[_index];
            _index++;
            return c;
        }

        private void ThrowExpectedMoreCharacters()
        {
            throw TokenException.MoreCharactersExpectedAt(_index).Decorate(
                _current ??
                new Token(TokenType.NotSet, 
                          Configuration.Template[_index-1].ToString(),
                          _index-1,
                          Configuration.Template
                              
                    )
                );
        }

        private bool HasNext()
        {
            return (_index + 1) <= Configuration.Template.Length;
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

        

        public void PushConfiguration(bool returnTokens, bool returnLiterals, char? escapeChar, string[] seperators,
                                      string[] whiteSpaceSeperators, char[] literals)
        {
            var newConfiguration = new TokenizerConfiguration(
                Configuration.Template,
                escapeChar,
                seperators,
                whiteSpaceSeperators,
                literals,
                returnTokens,
                returnLiterals);
            _configurationStack.Push(newConfiguration);
        }

        public void PopConfiguration()
        {
            _configurationStack.Pop();
        }

        public void SetIndexTo(int index)
        {
            _index = index;
        }

        public void DontExpectLiteralsAnyMore()
        {
            Configuration.DontExpectLiteralsAnyMore();
        }

        public void ExpectLiteralsAgain()
        {
            Configuration.ExpectLiteralsAgain();
        }
    }
}
