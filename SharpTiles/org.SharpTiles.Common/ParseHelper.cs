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
    public class ParseHelper
    {
        private readonly Stack<bool> _ignoreUnkownTags = new Stack<bool>();
        private readonly TokenEnumerator _tokenizer;
        protected Token _current;
        protected Token _lookahead;
        private Token _previous;

        public ParseHelper(Tokenizer tokenizer)
        {
            _tokenizer = tokenizer.GetTokenEnumerator();
        }

        public Token Current
        {
            get { return _current; }
        }

        public Token CurrentTrimmed
        {
            get
            {
                Token token = Current;
                token.Trim();
                return Current;
            }
        }

        public Token Lookahead
        {
            get { return _lookahead; }
        }

        public Token Previous
        {
            get { return _previous; }
        }

        public void Init()
        {
            if (!_tokenizer.MoveNext())
            {
                ParseException.ExpectedToken();
            }
            _lookahead = _tokenizer.Token;
        }

        public bool IsAhead(string expected)
        {
            return Lookahead != null ? Lookahead.Contents.Equals(expected) : false;
        }

       
        public bool IsAhead(ICollection<string> expectedTokens)
        {
            Token lookAhead = Lookahead;
            ICollection<string> set = new SortedSet<string>(expectedTokens);
            return Lookahead != null ? set.Contains(lookAhead.Contents) : false;
        }


        public bool IsAhead(TokenType type)
        {
            return Lookahead != null ? Lookahead.Type.Equals(type) : false;
        }

        public bool At(string expected)
        {
            return Current != null ? Current.Contents.Equals(expected) : false;
        }

        
        public bool At(ICollection<string> expectedTokens)
        {
            Token current = Current;
            return Current != null ? expectedTokens.Contains(current.Contents) : false;
        }

        public Token Expect(string expected)
        {
            Token current = Current;
            if (!current.Contents.Equals(expected))
            {
                throw ParseException.ExpectedToken(expected, current.Contents).Decorate(current);
            }
            return current;
        }

        public Token Expect(TokenType type)
        {
            Token current = Current;
            if (!current.Type.Equals(type))
            {
                throw ParseException.ExpectedToken(type.ToString(), current.Contents).Decorate(current);
            }
            return current;
        }

        
        public Token Expect(string forParser, ICollection<string> expectedTokens)
        {
            Token current = Current;
            if (!expectedTokens.Contains(current.Contents))
            {
                throw ParseException.ExpectedToken(CollectionUtils.ToString(expectedTokens), current.Contents, forParser)
                    .Decorate(current);
            }
            return current;
        }

        public Token Expect(string forParser, string expected)
        {
            Token current = Current;
            if (!expected.Equals(current.Contents))
            {
                throw ParseException.ExpectedToken(expected, current.Contents, forParser)
                    .Decorate(current);
            }
            return current;
        }


        public bool HasMore()
        {
            return Lookahead != null;
        }

        public Token Read(string expected)
        {
            Next();
            return Expect(expected);
        }

        public Token Read(TokenType type)
        {
            Next();
            return Expect(type);
        }

        public Token Next()
        {
            _previous = _current;
            _current = _lookahead;
            _lookahead = null;
            if (_tokenizer.MoveNext())
            {
                _lookahead = _tokenizer.Token;
            }
            if (_current == null)
            {
                throw ParseException.ExpectedToken().Decorate(_previous);
            }
            return _current;
        }

        

        public string ReadUntil(TokenType type, string until)
        {
            var expression = new StringBuilder();
            var read = 0;
            while (HasMore())
            {
                read++;
                Token token = Next();
                if (token.Type == type && Equals(token.Contents, until))
                {
                    break;
                }
                expression.Append(token.Contents);
            }
            if (read == 0)
            {
                throw ParseException.ExpectedToken().Decorate(Current);
            }
            return expression.ToString();
        }
        

        public void PushNewTokenConfiguration(TokenizerConfiguration configuration,
            ResetIndex mode)
        {
            _tokenizer.PushConfiguration(configuration);
            ResetIndex(mode);
        }

        public void DontExpectLiteralsAnyMore()
        {
            _tokenizer.DontExpectLiteralsAnyMore();
        }

        public void ExpectLiteralsAgain()
        {
            _tokenizer.ExpectLiteralsAgain();
        }

        private void ResetIndex(ResetIndex mode)
        {
            switch (mode)
            {
                case Common.ResetIndex.MaintainPosition:
                    _tokenizer.SetIndexTo(Lookahead.Index);
                    ReinitLookAhead();
                    break;
                case Common.ResetIndex.LookAhead:
                    if (Lookahead != null)
                    {
                        _tokenizer.SetIndexTo(Lookahead.Index);
                        InitLookAhead();
                    }
                    break;
                case Common.ResetIndex.CurrentAndLookAhead:
                    _tokenizer.SetIndexTo(Current.Index);
                    ReInit();
                    break;
            }
        }

        public void PopTokenConfiguration(ResetIndex mode)
        {
            _tokenizer.PopConfiguration();
            ResetIndex(mode);
        }

        private void InitLookAhead()
        {
            if (_tokenizer.MoveNext())
            {
                _previous = _current;
                _current = _lookahead;
                _lookahead = _tokenizer.Token;
            }
        }

        private void ReinitLookAhead()
        {
            if (_tokenizer.MoveNext())
            {
                _lookahead = _tokenizer.Token;
            }
        }

        private void ReInit()
        {
            Init();
            InitLookAhead();
        }

        public void Rollback()
        {
            _tokenizer.Rollback();
            ReInit();
        }



        public void Commit()
        {
            _tokenizer.Commit();
        }

        public void PushIgnoreUnkownTag(bool ignore)
        {
            _ignoreUnkownTags.Push(ignore);
        }

        public void PopIgnoreUnkownTag()
        {
            _ignoreUnkownTags.Pop();
        }

        public bool IgnoreUnkownTag()
        {
            return _ignoreUnkownTags.Count > 0 ? _ignoreUnkownTags.Peek() : false;
        }

        public override string ToString() => $"Parser[Current={_current},Previous={_previous},Lookahead={_lookahead}]";

        
    }
}
