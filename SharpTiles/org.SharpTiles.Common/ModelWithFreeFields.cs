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
    public class ModelWithFreeFields<T> : IReflectionModel where T: ModelWithFreeFields<T>
    {
        private IDictionary<string, object> _freeFieldsRaw;
        private Reflection _freeFields;
      
        public ModelWithFreeFields()
        {
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
                var own = Reflection.AcquirePropertyInfo(property, this);
                if(own.PropertyInfo!=null) return own.PropertyInfo.GetValue(this);
                return _freeFields[property];
            }
            set
            {
                var own = Reflection.AcquirePropertyInfo(property, this);
                if (own.PropertyInfo != null) own.PropertyInfo.SetValue(this, value);
                else _freeFields[property] = value;
            }
        }

        public object TryGet(string property)
        {
            var own = Reflection.AcquirePropertyInfo(property, this);
            return own.PropertyInfo != null ? own.PropertyInfo.GetValue(this) : _freeFields.TryGet(property);
        }

        public ReflectionResult Get(string property)
        {
            var own = Reflection.AcquirePropertyInfo(property, this);
            if(own.PropertyInfo == null) return _freeFields.Get(property);
            try
            {
                var value = own.PropertyInfo.GetValue(this);
                return new ReflectionResult
                {
                    Full = true,
                    Result = value
                };
            }
            catch (Exception e)
            {
                return new ReflectionResult {Full = true, ReflectionException = new ReflectionException(e.Message)};
            }
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
