using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.SharpTiles.Common;

namespace org.SharpTiles.Tags
{
    public interface ITagLib : IEnumerable<ITagGroup>
    {
        ITagGroup Get(string group, ParseContext context = null);

        ITagLib Register(ITagGroup sharp);
        bool Exists(string group);
    }
}
