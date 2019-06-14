using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.SharpTiles.Documentation.DocumentationAttributes
{
    public interface IInstanceTagDocumentation
    {
        DescriptionValue Description { get; }
        ExampleValue[] Examples { get; }
        NoteValue[] Notes { get; }
    }
}
