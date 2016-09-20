using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.SharpTiles.Documentation.DocumentationAttributes
{
    public interface IInstanceTagDocumentation
    {
        DescriptionAttribute Description { get; }
        ExampleAttribute[] Examples { get; }
        NoteAttribute[] Notes { get; }
    }
}
