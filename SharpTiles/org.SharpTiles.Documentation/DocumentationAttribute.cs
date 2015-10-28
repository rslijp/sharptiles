using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.SharpTiles.Documentation
{
    [AttributeUsage(AttributeTargets.All)]
    public class DocumentationAttribute : Attribute
    {
        public DocumentationAttribute(string translation)
        {
            Translation = translation;
        }

        public string Translation { get; }
    }
}
