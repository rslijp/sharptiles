using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.SharpTiles.Tags.Creators;

namespace org.SharpTiles.Templates.Processor
{
    public interface ITemplateProcessor
    {
        void Process(ParsedTemplate template);
    }
}
