using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.SharpTiles.Common;

namespace org.SharpTiles.Tags
{
    public class TagStackModel : IReflectionModel
    {
        private readonly IModel _model;
        private readonly TagStackModel _parent;

        public TagStackModel() : this(null)
        {
        }

        public TagStackModel(TagStackModel parent)
        {
            _model = new Reflection(new Hashtable());
            _parent = parent;
        }

        public object this[string property]
        {
            get { return TryGet(property); }
            set { _model[property] = value; }
        }

        public object TryGet(string property)
        {
           var result = Get(property);
           return result?.Result;
        }

        public ReflectionResult Get(string property)
        {
            var result = _model.Get(property);
            if (_parent == null) return result;
            if (result.Partial || result.Full) return result;
            return _parent.Get(property);

        }
    }
}
