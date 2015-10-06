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
using org.SharpTiles.Common;
using org.SharpTiles.Tags;

namespace org.SharpTiles.Templates
{
    public abstract class AbstractTagLibParser : ITagLibParser
    {
        protected readonly ParseHelper _helper;
        private readonly IResourceLocator _locator;

        public AbstractTagLibParser(ParseHelper helper, IResourceLocator locator)
        {
            _helper = helper;
            _locator = locator;
        }

        public ITag Parse()
        {
            if (!_helper.At(TagLibConstants.START_TAG))
            {
                _helper.Read(TagLibConstants.START_TAG);
            }
            if (_helper.IsAhead(TagLibConstants.CLOSE_SLASH))
            {
                return ParseCloseTag();
            }
            return ParseOpenTag();
        }

        private ITag ParseCloseTag()
        {
            _helper.Read(TagLibConstants.CLOSE_SLASH);
            ITag tag = ParseTagType();
            if (tag == null) return null;
            _helper.DontExpectLiteralsAnyMore();
            _helper.Read(TagLibConstants.CLOSE_TAG);
            _helper.ExpectLiteralsAgain(); 
            
            //var tag = TagLib.Libs.Get(group).Get(name);
            tag.State = TagState.Closed;
            return tag;
        }

        private ITag ParseOpenTag()
        {
            if (!_helper.IsAhead(TokenType.Regular))
            {
                return null;
            }
            ITag tag = ParseTagType();
            if (tag == null) return null;
            var tagReflection = tag.AttributeSetter;
            AddAttributes(tagReflection);
            tagReflection.InitComplete();
            
            TagState state = TagState.Opened;
            if (_helper.IsAhead(TagLibConstants.CLOSE_SLASH))
            {
                _helper.Read(TagLibConstants.CLOSE_SLASH);
                state = TagState.OpenedPendingClose;
            }
            _helper.DontExpectLiteralsAnyMore();
            _helper.Read(TagLibConstants.CLOSE_TAG);
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

        protected abstract ITag ParseTagType();
        protected abstract TagLibMode Mode { get;  }

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
            var attribute = new TemplateAttribute(Formatter.ParseNested(_helper, _locator, Mode));
            tagReflection["Body"] = attribute;
            _helper.PopIgnoreUnkownTag();
        }

        private void HandleFreeWithIgnoredUnkownTagsNestedBody(ITagAttributeSetter tagReflection)
        {
            _helper.PushIgnoreUnkownTag(true);
            var attribute = new TemplateAttribute(Formatter.ParseNested(_helper, _locator, Mode));
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
            while (!_helper.IsAhead(TagLibConstants.CLOSE_TAG, TagLibConstants.CLOSE_SLASH))
            {
                _helper.Read(TokenType.Seperator);
                ReadWhiteSpace(_helper);
                if (_helper.IsAhead(TagLibConstants.CLOSE_SLASH, TagLibConstants.CLOSE_TAG))
                {
                    return;
                }
                var keyToken = _helper.Read(TokenType.Regular);
                var key = LanguageHelper.CamelCaseAttribute(keyToken.Contents);
                _helper.Read(TagLibConstants.FIELD_ASSIGNMENT);
                var value = _helper.Read(TokenType.Literal).Contents;
                if (tagReflection[key] != null)
                {
                    throw TagException.PropertyAlReadySet(key).Decorate(keyToken.Context);
                }
                tagReflection[key] = new TemplateAttribute(new InternalFormatter(value, false, _locator, Mode).Parse());
            }
        }

       
    }
}
