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
 */using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.SharpTiles.Common;
using org.SharpTiles.Expressions;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.Creators;
using org.SharpTiles.Templates.Validators;

namespace org.SharpTiles.Templates
{
    public class TagLibParserFactory : ITagLibParserFactory
    {
        private readonly TagLibForParsing _lib;
        private static readonly string[] WHITESPACES = new []{
                " ","\t", "\r", "\n"
        };

        private readonly IResourceLocatorFactory _factory;
        private readonly ITagValidator _tagValidator;
        private ExpressionLib _expressionLib;
        private static TokenizerConfiguration Configuration = new TokenizerConfiguration(null, TagLibConstants.SEPERATORS, null, TagLibConstants.LITERALS, true, true);


        public TagLibParserFactory(TagLibForParsing lib, ExpressionLib expressionLib, IResourceLocatorFactory factory, ITagValidator tagValidator)
        {
            _lib = lib;
            _expressionLib = expressionLib;
            _factory = factory;
            _tagValidator = tagValidator;
        }

        public ITagLibParser Construct(ParseHelper helper, IResourceLocator locator)
        {
            var mode = _lib.Mode;
            if(mode==TagLibMode.Strict) return new StrictTagLibParser(_lib, _expressionLib,helper, locator, _factory, _tagValidator);
            if (mode == TagLibMode.StrictResolve) return new StrictResolveTagLibParser(_lib, _expressionLib, helper, locator, _factory, _tagValidator);
            if (mode == TagLibMode.RelaxedResolve) return new RelaxedResolveTagLibParser(_lib, _expressionLib, helper, locator, _factory, _tagValidator);
            if (mode == TagLibMode.IgnoreResolve) return new IgnoreResolveTagLibParser(_lib, _expressionLib, helper, locator, _factory, _tagValidator);
            return null;
        }

        public ITag Parse(string tag)
        {
            var tokenizer = new Tokenizer(tag, Configuration);
            var helper = new ParseHelper(tokenizer);
            helper.Init();
            return Construct(helper,_factory.GetNewLocator()).Parse();
        }

        public ITag Parse(ParseHelper helper, IResourceLocator locator)
        {
            helper.PushNewTokenConfiguration(Configuration,ResetIndex.CurrentAndLookAhead);
            try
            {
                return Construct(helper, locator).Parse();
            }
            finally
            {
                helper.PopTokenConfiguration(ResetIndex.LookAhead);
            }
        }

    }
}
