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
using System.Linq;
using org.SharpTiles.Common;
using org.SharpTiles.Expressions;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.Creators;
using org.SharpTiles.Templates.Validators;

namespace org.SharpTiles.Templates
{
    public abstract class AbstractTagLibParser : ITagLibParser
    {
        private static ICollection<string> CLOSING_TOKENS = new HashSet<string>(new string[] { TagLibConstants.CLOSE_SLASH, TagLibConstants.CLOSE_TAG}); 
        private static readonly  TokenizerConfiguration Configuration = new TokenizerConfiguration(InternalFormatter.COMMENT, InternalFormatter.SEPERATORS, null, null, true, false);

        protected readonly ParseHelper _helper;
        private IResourceLocator _locator;
        protected TagLibForParsing _lib;
        protected IResourceLocatorFactory _factory;
        private readonly ITagValidator _tagValidator;
        private ExpressionLib _expressionLib;

        public AbstractTagLibParser(TagLibForParsing lib, ExpressionLib expressionLib, ParseHelper helper, IResourceLocator locator, IResourceLocatorFactory factory, ITagValidator tagValidator)
        {
            _lib = lib;
            _expressionLib = expressionLib;
            _helper = helper;
            _locator = locator;
            _factory = factory;
            _tagValidator = tagValidator;
        }


       
        public ITag Parse(bool expectTag=false)
        {
            if (!_helper.At(TagLibConstants.START_TAG))
            {
                _helper.Read(TagLibConstants.START_TAG);
            }
            if (_helper.IsAhead(TagLibConstants.CLOSE_SLASH))
            {
                return ParseCloseTag();
            }
            var context = _helper.Current.Context;
            if(expectTag && !_helper.IsAhead(TokenType.Regular))
            {
                throw TagException.ExpectedTagOrGroupName(_helper.Lookahead.Contents).Decorate(_helper.Lookahead.Context);
            }
            var tag = ParseOpenTag();
            if(tag!=null) tag.Context = context;
            ValidateTag(tag);
            return tag;
        }

        private void ValidateTag(ITag tag)
        {
            var tagWithValidator = tag as IHaveTagValidator;
            var validator = new TagValidatorCollection(
                                    new BooleanTagAttributeValidator(), 
                                    new EnumTagAttributeValidator(), 
                                    new DateTagAttributeValidator(), 
                                    new NumberTagAttributeValidator(), 
                                    _tagValidator, 
                                    tagWithValidator?.TagValidator);
            validator.Validate(tag);
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
            PushTagLibExtension(tag);
            DecorateFactory(tag);
            if (tag == null) return null;
            var tagReflection = tag.AttributeSetter;
            AddAttributes(tagReflection, tag);
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
            PopTagLibExtension(tag);
            return tag;
        }

        private void DecorateFactory(ITag tag)
        {
            var requireTagLib = tag as ITagRequiringTagLib;
            if (requireTagLib!=null) requireTagLib.TagLib = _lib;
            var requireFactory = tag as ITagWithResourceFactory;
            if (requireFactory != null) requireFactory.Factory = _factory;
        }

        private void PushTagLibExtension(ITag tag)
        {
            var t = tag as ITagExtendTagLib;
            if (t == null) return;
            _lib.Push(t.TagLibExtension);
        }

        private void PopTagLibExtension(ITag tag)
        {
            var t = tag as ITagExtendTagLib;
            if (t == null) return;
            _lib.Pop();
        }

        protected string[] TagNames => _lib?.SelectMany(lib => lib.TagNames).ToArray();

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
                var nested = Parse(true);
                if (nested?.State == TagState.Closed)
                {
                    GuardClosingOfTag(nested, tag);
                    return;
                }
                if (nested != null)
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
                if (Equals(close.Group.Name, open.Group.Name) &&
                       Equals(close.TagName, open.TagName)) return; //edge case. Close of nested tag with same name as parent tag
                throw TagException.UnbalancedCloseingTag(open, close).Decorate(open.Context);
            }
        }

        private void HandleFreeNestedBody(ITagAttributeSetter tagReflection)
        {
            _helper.PushIgnoreUnkownTag(false);
            var attribute = new TemplateAttribute(ParseNested(_helper, _locator));
            tagReflection["Body"] = attribute;
            _helper.PopIgnoreUnkownTag();
        }

        private void HandleFreeWithIgnoredUnkownTagsNestedBody(ITagAttributeSetter tagReflection)
        {
            _helper.PushIgnoreUnkownTag(true);
            var attribute = new TemplateAttribute(ParseNested(_helper, _locator));
            tagReflection["Body"] = attribute;
            _helper.PopIgnoreUnkownTag();
        }

        public ParsedTemplate ParseNested(ParseHelper helper, IResourceLocator locator)
        {
            try
            {
                helper.PushNewTokenConfiguration(
                    Configuration,
                    ResetIndex.LookAhead);
                return new InternalFormatter(new TagLibParserFactoryAdapter(this), _expressionLib, helper, true, true, locator).ParseNested();
            }
            finally
            {
                helper.PopTokenConfiguration(ResetIndex.CurrentAndLookAhead);
            }
        }

        private static void ReadWhiteSpace(ParseHelper helper)
        {
            while (helper.Lookahead != null &&
                   helper.Lookahead.IsWhiteSpace)
            {
                helper.Next();
            }
//        helper.NextIgnoreWhiteSpace();  
//            Console.WriteLine(helper.Lookahead);
        }

        private static void CheckRequiredAttributes(ITag tag)
        {
            RequiredAttribute.Check(tag);
        }

        private void AddAttributes(ITagAttributeSetter tagReflection, ITag tag)
        {
            while (!_helper.IsAhead(CLOSING_TOKENS))
            {
//                _helper.Read(TokenType.Seperator);
                _helper.Next();
                ReadWhiteSpace(_helper);
                if (_helper.IsAhead(CLOSING_TOKENS))
                {
                    return;
                }
                var keyToken = _helper.Read(TokenType.Regular);
                var key = tagReflection.SupportNaturalLanguage?LanguageHelper.CamelCaseAttribute(keyToken.Contents): keyToken.Contents;
                _helper.Read(TagLibConstants.FIELD_ASSIGNMENT);
                var value = _helper.Read(TokenType.Literal).Contents;
                var existing = tagReflection[key];
                if (!existing?.AllowOverWrite ?? false)
                {
                    throw TagException.PropertyAlReadySet(key).Decorate(keyToken.Context);
                }
                if (string.IsNullOrEmpty(value))
                {
                    tagReflection[key] =new ConstantAttribute("", tag) {AttributeName = key,Context = keyToken.Context };
                    continue;
                }
                if (!value.Contains("${"))
                {
                    tagReflection[key] = new ConstantAttribute(value, tag) { AttributeName = key, Context = keyToken.Context, ResourceLocator = _locator};
                    continue;
                }
                var offSet = _helper.Current.Context;
                var attr=new TemplateAttribute(new InternalFormatter(new TagLibParserFactoryAdapter(this), _expressionLib, value, false, _locator, offSet).Parse()) { AttributeName = key };
                tagReflection[key] = attr;
            }
        }

        public class TagLibParserFactoryAdapter : TagLibParserFactory
        {
           
            public TagLibParserFactoryAdapter(AbstractTagLibParser parser) : base(parser._lib, parser._expressionLib, parser._factory, parser._tagValidator) 
            {
            }

         
        }
    }
}
