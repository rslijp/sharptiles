using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.SharpTiles.Tags.Creators;

namespace org.SharpTiles.Tags
{
    public interface ITagWithResourceFactory
    {
         IResourceLocatorFactory Factory { get; set; }
    }
}
