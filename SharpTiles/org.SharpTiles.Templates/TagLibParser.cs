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
 using org.SharpTiles.Common;
using org.SharpTiles.Tags;

namespace org.SharpTiles.Templates
{
    public class TagLibParser
    {
        public static readonly string CLOSE_SLASH = "/";
        public static readonly string CLOSE_TAG = ">";

        public static readonly char QUOTE = '\'';
        public static readonly char DOUBLE_QUOTE = '"';

        public static readonly string FIELD_ASSIGNMENT = "=";
        public static readonly string GROUP_TAG_SEPERATOR = ":";
        public static readonly char[] LITERALS = new[] {QUOTE, DOUBLE_QUOTE};


        public static readonly string START_TAG = "<";
        public static readonly string TAG_FIELD_SEPERATOR = " ";
        
        public static readonly string[] SEPERATORS =
            new[] {START_TAG, CLOSE_SLASH, CLOSE_TAG, GROUP_TAG_SEPERATOR, TAG_FIELD_SEPERATOR, FIELD_ASSIGNMENT};

        
        private readonly ParseHelper _helper;
        private readonly IResourceLocator _locator;

        public TagLibParser(ParseHelper helper, IResourceLocator locator)
        {
            _helper = helper;
            _locator = locator;
        }

        public ITag Parse()
        {
            if (!_helper.At(START_TAG))
            {
                _helper.Read(START_TAG);
            }
            if (_helper.IsAhead(CLOSE_SLASH))
            {
                return ParseCloseTag();
            }
            return ParseOpenTag();
        }

        private ITag ParseCloseTag()
        {
            _helper.Read(CLOSE_SLASH);
            if (!_helper.IsAhead(TokenType.Regular))
            {
                return null;
            }
            Token group = _helper.Read(TokenType.Regular);
            if (!_helper.IsAhead(GROUP_TAG_SEPERATOR))
            {
                return null;
            }
            _helper.Read(GROUP_TAG_SEPERATOR);
            Token name = _helper.Read(TokenType.Regular);
            _helper.DontExpectLiteralsAnyMore();
            _helper.Read(CLOSE_TAG);
            _helper.ExpectLiteralsAgain(); 
            if (_helper.IgnoreUnkownTag() && !TagLib.Exists(group.Contents))
            {
                return null;
            }
            var tag = TagLib.Tags.Get(group).Get(name);
            tag.State = TagState.Closed;
            return tag;
        }

        private ITag ParseOpenTag()
        {
            if (!_helper.IsAhead(TokenType.Regular))
            {
                return null;
            }
            Token group = _helper.Read(TokenType.Regular);
            if (!_helper.IsAhead(GROUP_TAG_SEPERATOR))
            {
                return null;
            }
            _helper.Read(GROUP_TAG_SEPERATOR);
            Token name = _helper.Read(TokenType.Regular);

            if (_helper.IgnoreUnkownTag() && !TagLib.Exists(group.Contents))
            {
                return null;
            }
            ITagGroup tagGroup = TagLib.Tags.Get(group);
            ITag tag = tagGroup.Get(name);

            var tagReflection = tag.AttributeSetter;
            AddAttributes(tagReflection);
            tagReflection.InitComplete();
            
            TagState state = TagState.Opened;
            if (_helper.IsAhead(CLOSE_SLASH))
            {
                _helper.Read(CLOSE_SLASH);
                state = TagState.OpenedPendingClose;
            }
            _helper.DontExpectLiteralsAnyMore();
            _helper.Read(CLOSE_TAG);
            _helper.ExpectLiteralsAgain();
            CheckRequiredAttributes(tag);
            tag.State = state;
            if (state == TagState.Opened)
            {
                HandleBody(tag, tagReflection);
            }
            tag.State = TagState.OpenedAndClosed;
            return tag;
        }

        private void HandleBody(ITag tag, ITagAttributeSetter tagReflection)
        {
            if (tag.TagBodyMode == TagBodyMode.Free)
            {
                HandleFreeNestedBody(tagReflection);
                ITag closingTag = Parse();
                GuardClosingOfTag(closingTag, tag);
            }
            else if (tag.TagBodyMode == TagBodyMode.NestedTags)
            {
                HandleNestedTagBody((ITagWithNestedTags) tag);
            }
            else if (tag.TagBodyMode == TagBodyMode.FreeIgnoreUnkown)
            {
                HandleFreeWithIgnoredUnkownTagsNestedBody(tagReflection);
                ITag closingTag = Parse();
                GuardClosingOfTag(closingTag, tag);
            }
            else
            {
                ITag closingTag = Parse();
                GuardClosingOfTag(closingTag, tag);
            }
        }

        private void HandleNestedTagBody(ITagWithNestedTags tag)
        {
            _helper.PushIgnoreUnkownTag(false);
            do
            {
                ReadWhiteSpace(_helper);
                ITag nested = Parse();
                if (nested.State == TagState.Closed)
                {
                    GuardClosingOfTag(nested, tag);
                    return;
                }
                tag.AddNestedTag(nested);
            } while (_helper.HasMore());
            _helper.PopIgnoreUnkownTag();
        }

        private static void GuardClosingOfTag(ITag close, ITag open)
        {
            if (close.State != TagState.Closed)
            {
                throw TagException.ExpectedCloseTag(open.GetType()).Decorate(close.Context);
            }
            if (!Equals(close.GetType(), open.GetType()))
            {
                throw TagException.UnbalancedCloseingTag(open.GetType(), close.GetType()).Decorate(open.Context);
            }
        }

        private void HandleFreeNestedBody(ITagAttributeSetter tagReflection)
        {
            _helper.PushIgnoreUnkownTag(false);
            var attribute = new TemplateAttribute(Formatter.ParseNested(_helper, _locator));
            tagReflection["Body"] = attribute;
            _helper.PopIgnoreUnkownTag();
        }

        private void HandleFreeWithIgnoredUnkownTagsNestedBody(ITagAttributeSetter tagReflection)
        {
            _helper.PushIgnoreUnkownTag(true);
            var attribute = new TemplateAttribute(Formatter.ParseNested(_helper, _locator));
            tagReflection["Body"] = attribute;
            _helper.PopIgnoreUnkownTag();
        }

        private static void ReadWhiteSpace(ParseHelper helper)
        {
            while (helper.Lookahead != null &&
                   helper.Lookahead.Contents.Trim().Length == 0)
            {
                helper.Next();
            }
        }

        private static void CheckRequiredAttributes(ITag tag)
        {
            RequiredAttribute.Check(tag);
        }

        private void AddAttributes(ITagAttributeSetter tagReflection)
        {
            while (!_helper.IsAhead(CLOSE_TAG, CLOSE_SLASH))
            {
                _helper.Read(TAG_FIELD_SEPERATOR);
                ReadWhiteSpace(_helper);
                if (_helper.IsAhead(CLOSE_SLASH, CLOSE_TAG))
                {
                    return;
                }
                var keyToken = _helper.Read(TokenType.Regular);
                var key = keyToken.Contents;
                _helper.Read(FIELD_ASSIGNMENT);
                var value = _helper.Read(TokenType.Literal).Contents;
                if (tagReflection[key] != null)
                {
                    throw TagException.PropertyAlReadySet(key).Decorate(keyToken.Context);
                }
                tagReflection[key] = new TemplateAttribute(new InternalFormatter(value, false, _locator).Parse());
            }
        }

        public static ITag Parse(string tag)
        {
            var tokenizer = new Tokenizer(tag, true, true, null, SEPERATORS, LITERALS, null);
            var helper = new ParseHelper(tokenizer);
            helper.Init();
            return new TagLibParser(helper, new FileBasedResourceLocator()).Parse();
        }

        public static ITag Parse(ParseHelper helper, IResourceLocator locator)
        {
            helper.PushNewTokenConfiguration(true, true, null, SEPERATORS, null, LITERALS,
                                             ResetIndex.CurrentAndLookAhead);
            try
            {
                return new TagLibParser(helper, locator).Parse();
            }
            finally
            {
                helper.PopTokenConfiguration(ResetIndex.LookAhead);
            }
        }
    }
}
