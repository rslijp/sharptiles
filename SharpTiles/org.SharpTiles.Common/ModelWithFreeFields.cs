using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.SharpTiles.Common
{
    public class ModelWithFreeFields<T> : IModel where T: ModelWithFreeFields<T>
    {
        private Reflection _freeFields;
        private readonly Reflection _self;

        public ModelWithFreeFields()
        {
            _self = new Reflection(this);
            ReloadFreeField();
        }

        public T ReloadFreeField(IDictionary<string, object> freeFields=null)
        {
            _freeFields = new Reflection(freeFields?? new Dictionary<string, object>());
            return this as T;
        }

        public object this[string property]
        {
            get
            {
                if (_self.Exist(property)) return _self[property];
                return _freeFields[property];
            }
            set
            {
                if (_self.Exist(property)) _self[property] = value;
                else _freeFields[property] = value;
            }
        }

        public object TryGet(string property)
        {
            return _self.Exist(property) ? _self.TryGet(property) : _freeFields.TryGet(property);
        }

        public ReflectionResult Get(string property)
        {
            return _self.Exist(property) ? _self.Get(property) : _freeFields.Get(property);
        }
    }
}
