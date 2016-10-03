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
 using System.Reflection;
 using System.Text;
using org.SharpTiles.Common;
using org.SharpTiles.Expressions;
using org.SharpTiles.Tags;
 using org.SharpTiles.Tags.Creators;

namespace org.SharpTiles.Templates
{
    internal class InternalFormatter
    {
        public static readonly string CLOSE_EXPRESSION = "}";
        public static readonly char COMMENT = '\\';
        public static readonly char QUOTE = '\'';
        public static readonly char DOUBLE_QUOTE = '"';
        public static readonly string GROUP_TAG_SEPERATOR = TagLibConstants.GROUP_TAG_SEPERATOR;

        public static readonly char[] LITERALS = new[] { QUOTE, DOUBLE_QUOTE };
        public static readonly string OPEN_EXPRESSION = "{";

        public static readonly string SIGNATURE_EXPRESSION = "$";
        public static readonly string OPEN_TAG = "<";
        public static readonly string CLOSE_TAG = ">";

        public static readonly string OPEN_COMMENT = "%--";
        public static readonly string CLOSE_COMMENT = "--%";
        
        public static readonly string[] SEPERATORS = new[]
                                                         {
                                                             SIGNATURE_EXPRESSION, OPEN_EXPRESSION, CLOSE_EXPRESSION,
                                                             OPEN_TAG, CLOSE_TAG, 
                                                             OPEN_COMMENT, CLOSE_COMMENT,
                                                             GROUP_TAG_SEPERATOR
                                                         };

        private readonly bool _allowTags = true;
        private readonly bool _expectCloseTag;
        private readonly ParseHelper _parser;
        private readonly IResourceLocator _locator;

        private ITag _closeTag;
        private IList<ITemplatePart> _templateParsed;
        private ITagLibParserFactory _tagLibHelper;
        private  ExpressionLib _expressionLib;

        public InternalFormatter(ITagLibParserFactory tagLibHelper, ExpressionLib expressionLib, string template, bool allowTags, IResourceLocator locator, ParseContext offSet = null)
        {
            _tagLibHelper = tagLibHelper;
            _locator = locator;
            _expectCloseTag = false;
            _allowTags = allowTags;
            _parser = new ParseHelper(new Tokenizer(template, true, COMMENT, SEPERATORS, null /*LITERALS*/).AddOffSet(offSet));
            _expressionLib = expressionLib;
        }

        public InternalFormatter(ITagLibParserFactory tagLibHelper, ExpressionLib expressionLib, ParseHelper parser, bool allowTags, bool expectCloseTag, IResourceLocator locator)
        {
            _tagLibHelper = tagLibHelper;
            _locator = locator;
            _allowTags = allowTags;
            _expectCloseTag = expectCloseTag;
            _parser = parser;
            _expressionLib = expressionLib;
        }

        public ParsedTemplate Parse()
        {
            try
            {
                _parser.Init();
                _templateParsed = new List<ITemplatePart>();
                ParseNode();
                GuardCloseTag();
                return ParsedTemplate;
            }
            catch (ExceptionWithContext)
            {
                throw;
            }
//            catch (Exception e)
//            {
//                throw ParseException.UnexpectedError(e).Decorate(_parser.Current?.Context);
//            }
        }

        internal ParsedTemplate ParsedTemplate
        {
            get { return new ParsedTemplate(_locator, _templateParsed); }
        }

        internal ParsedTemplate ParseNested()
        {
            _templateParsed = new List<ITemplatePart>();
            ParseAdditional();
            GuardCloseTag();
            if (_closeTag != null)
            {
                _parser.Rollback();
            }
            return new ParsedTemplate(_locator, _templateParsed);
        }

        private void GuardCloseTag()
        {
            if ((_closeTag == null) == _expectCloseTag)
            {
                if (_closeTag != null)
                {
                    throw ParseException.UnexpectedCloseTag(_closeTag.GetType().Name).Decorate(_parser.Current);
                }
                throw ParseException.ExpectedCloseTag().Decorate(_parser.Previous);
            }
        }

        private void ParseNode()
        {
            _parser.Commit();
            Token current = _parser.Next();
            if (current.Type == TokenType.Seperator)
            {
                ParseSeperator();
            }
            else
            {
                ReducedAdd(new TextPart(current));
            }
            ParseAdditional();
        }

        private void ParseSeperator()
        {
            if (_parser.At(OPEN_TAG) )
            {
                if (_parser.IsAhead(OPEN_COMMENT)) ParseComment(); 
                else ParseTag();
            }
            else if (_parser.At(SIGNATURE_EXPRESSION) && _parser.IsAhead(OPEN_EXPRESSION))
            {
                ParseExpression();
            }
            else
            {
                ReducedAdd(new TextPart(_parser.Current));
            }
        }

        private void ReducedAdd(TextPart part)
        {
            TextPart last = _templateParsed.Count > 0
                                ? _templateParsed[_templateParsed.Count - 1] as TextPart
                                : null;
            if (last != null)
            {
                last.Append(part);
            }
            else
            {
                _templateParsed.Add(part);
            }
        }

        public void ParseAdditional()
        {
            if (_parser.HasMore() && (_closeTag == null))
            {
                ParseNode();
            }
        }

        private void ParseTag()
        {
            var tag = _tagLibHelper.Parse(_parser, _locator);
            if (tag == null)
            {
                _parser.Rollback();
                ReducedAdd(new TextPart(_parser.Current));
            }
            else
            {
                if (tag.State == TagState.Closed)
                {
                    _closeTag = tag;
                }
                else
                {
                    GuardNestedTags();
                    _templateParsed.Add(new TagPart(tag));
                }
            }
        }

        private void GuardNestedTags()
        {
            if (!_allowTags)
            {
                ParseException.TagsNotAllowed();
            }
        }

        private void ParseExpression()
        {
            _parser.Expect(SIGNATURE_EXPRESSION);
            _parser.Read(OPEN_EXPRESSION);
            var offset=_parser.Current.Context;
            var expression = _parser.ReadUntil(TokenType.Seperator, CLOSE_EXPRESSION);
            _parser.Expect(CLOSE_EXPRESSION);
            try
            {
                _templateParsed.Add(new ExpressionPart(_expressionLib.Parse(expression, offset)));
            }
            catch (ExceptionWithContext ewc)
            {
//                ewc.Update(offset.Add(ewc.Context));
                throw ewc;
            }
        }

        private void ParseComment()
        {
            _parser.Expect(OPEN_TAG);
            _parser.Read(OPEN_COMMENT);
            _parser.ReadUntil(TokenType.Seperator, CLOSE_COMMENT);
            _parser.Expect(CLOSE_COMMENT);
            _parser.Read(CLOSE_TAG);
        }
        
    }
}
