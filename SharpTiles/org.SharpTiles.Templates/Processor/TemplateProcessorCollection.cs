using System.Linq;
using org.SharpTiles.Tags.Creators;
using org.SharpTiles.Templates.Processor;

namespace org.SharpTiles.Templates.Validators
{
    public class TemplateProcessorCollection : ITemplateProcessor
    {
        private readonly ITemplateProcessor[] _processors;

        public TemplateProcessorCollection(params ITemplateProcessor[] processors)
        {
            _processors = processors ?? new ITemplateProcessor[0];
        }

        public void Process(ParsedTemplate template)
        {
            foreach (var processor in _processors)
            {
                processor?.Process(template);
            }   
        }

        public override string ToString() => $"TemplateProcessorCollection[{string.Join(",",_processors.Select(v => v?.GetType().Name))}]";
    }
}