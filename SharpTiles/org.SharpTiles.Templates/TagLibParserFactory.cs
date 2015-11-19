using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;

namespace org.SharpTiles.Templates
{
    public class TagLibParserFactory
    {
        private readonly TagLibForParsing _lib;
        private readonly TagLibMode _mode;
        private static readonly string[] WHITESPACES = new []{
                " ","\t", "\r", "\n"
        };

        public TagLibParserFactory(TagLibForParsing lib) : this(lib,TagLibMode.Strict)
        {
        }

        public TagLibParserFactory(TagLibForParsing lib, TagLibMode mode)
        {
            _lib = lib;
            _mode = mode;
        }

        public ITagLibParser Construct(ParseHelper helper, IResourceLocator locator)
        {
            if(_mode==TagLibMode.Strict) return new StrictTagLibParser(_lib,helper, locator);
            if (_mode == TagLibMode.StrictResolve) return new StrictResolveTagLibParser(_lib,helper, locator);
            if (_mode == TagLibMode.RelaxedResolve) return new RelaxedResolveTagLibParser(_lib,helper, locator);
            return null;
        }

        public ITag Parse(string tag)
        {
            var tokenizer = new Tokenizer(tag, true, true, null, TagLibConstants.SEPERATORS, TagLibConstants.LITERALS, null);
            var helper = new ParseHelper(tokenizer);
            helper.Init();
            return Construct(helper, new FileBasedResourceLocator()).Parse();
        }

        public ITag Parse(ParseHelper helper, IResourceLocator locator)
        {
            helper.PushNewTokenConfiguration(true, true, null, TagLibConstants.SEPERATORS, null, TagLibConstants.LITERALS,
                                             ResetIndex.CurrentAndLookAhead);
            try
            {
                return Construct(helper, locator).Parse();
            }
            finally
            {
                helper.PopTokenConfiguration(ResetIndex.LookAhead);
            }
        }

        public static TagLibParserFactory Base()
        {
            return new TagLibParserFactory(new TagLibForParsing(new TagLib()));
        }
    }
}
