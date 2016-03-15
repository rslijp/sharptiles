using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using org.SharpTiles.Common;

namespace org.SharpTiles.AST
{
    [DataContract]
    public class Context {

        public Context()
        {
            
        }

        public Context(int line, int index)
        {
            LineNumber = line;
            LineIndex = index;
        }

        [DataMember]
        public int LineNumber { get; private set; }

        [DataMember]
        public int LineIndex { get; private set; }

        public static implicit operator Context(ParseContext pc)
        {
            return new Context(pc.LineNumber,pc.LineIndex+1);
        }

    }
}
