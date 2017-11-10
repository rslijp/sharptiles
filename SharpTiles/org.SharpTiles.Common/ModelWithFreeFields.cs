using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace org.SharpTiles.Common
{
    [DataContract]
    public class ModelWithFreeFields<T> : IModel where T: ModelWithFreeFields<T>
    {
        private IDictionary<string, object> _freeFieldsRaw;
        private Reflection _freeFields;
        private readonly Reflection _self;

        public ModelWithFreeFields()
        {
            _self = new Reflection(this);
            ReloadFreeField();
        }

        public T ReloadFreeField(IDictionary<string, object> freeFields=null)
        {
            _freeFieldsRaw = freeFields ?? new Dictionary<string, object>();
            _freeFields = new Reflection(_freeFieldsRaw);
            return this as T;
        }

        public T ReloadFreeField<S>(ModelWithFreeFields<S> other) where S : ModelWithFreeFields<S>
        {
            return ReloadFreeField(new Dictionary<string, object>(other._freeFieldsRaw));
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

        [DataMember]
        public JObject FreeFields
        {
            get
            {
                var j = new JObject();
                foreach (var freeField in _freeFieldsRaw)
                {
                    j[freeField.Key] = JToken.FromObject(freeField.Value);
                }
                return j;
            }
            set
            {
                var dictionary = new Dictionary<string, object>();
                if (value != null)
                {
                    dictionary = value.ToObject<Dictionary<string, object>>();
                }
                ReloadFreeField(dictionary);
            }
        }
    }
  
}
