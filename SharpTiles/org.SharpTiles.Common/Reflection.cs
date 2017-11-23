/*
 * SharpTiles, R.Z. Slijp(2008), www.sharptiles.org
 *
 * This file is part of SharpTiles.
 * 
 * SharpTiles is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * SharpTiles is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with SharpTiles.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace org.SharpTiles.Common
{
    public enum ReflectionMode
    {
        Normal, Strict, Loose
    }
    public class Reflection : IReflectionModel
    {
        private static IDictionary<CacheKey, ReflectionPropertyResult> HANDLER_CACHE = new Dictionary<CacheKey, ReflectionPropertyResult>();
        private static readonly object HANDLER_CACHE_LOCK = new object();

        private static IDictionary<CacheKey, PropertyInfoResult> PROPERTY_CACHE = new Dictionary<CacheKey, PropertyInfoResult>();
        private static readonly object PROPERTY_CACHE_LOCK = new object();

        #region Delegates

        public delegate object ResolveObject(object source);

        #endregion

        public const string SEPERATOR = ".";
        public ReflectionMode Mode { get; private set; } = ReflectionMode.Normal;

        private readonly object _subject;
        private ResolveObject _objectResolver;

        private class ReflectionPropertyResult
        {
            public ReflectionException ReflectionException { get; set; }
            public PropertyHandler PropertyHandler { get; set; }
        }

        public class PropertyInfoResult
        {
            public ReflectionException ReflectionException { get; set; }
            public PropertyInfo PropertyInfo { get; set; }
        }

        public Reflection(object subject)
        {
            _subject = subject;
        }


        public ResolveObject ObjectResolver
        {
            set { _objectResolver = value; }
        }

        public Reflection BecomeStrict()
        {
            Mode = ReflectionMode.Strict;
            return this;
        }

        public Reflection BecomeLoose()
        {
            Mode = ReflectionMode.Loose;
            return this;
        }

        #region IReflectionModel Members

        public object this[string property]
        {
            get
            {
                var result = GetProperty(_subject, property, 0);
                if (result.ReflectionException != null && (Mode == ReflectionMode.Strict || !result.ReflectionException.IgnoreOnGet)) throw result.ReflectionException;
                return result.Result;
            }
            set
            {
                var result = SetProperty(_subject, property, value, 0);
                if (result.ReflectionException != null) throw result.ReflectionException;
            }
        }

        public virtual ReflectionResult Get(string property)
        {
            return GetProperty(_subject, property, 0);
        }

        public bool Exist(string property)
        {
            return GetProperty(_subject, property, 0).ReflectionException == null;
        }

        public object TryGet(string property)
        {
            try
            {
                var result = GetProperty(_subject, property, 0);
                return result.Result;
            }
            catch (ReflectionException)
            {
                return null;
            }
        }

        #endregion

        private ReflectionResult SetProperty(object subject, string property, object value, int level)
        {
            var exception = GuardPropertyIsSet(property);
            if (exception != null) return new ReflectionResult { ReflectionException = exception };
            int split = property.IndexOf(SEPERATOR, StringComparison.Ordinal);
            if (split >= 0)
            {
                string head = property.Substring(0, split);
                string tail = property.Substring(split + 1);
                var temp = GetCurrentProperty(subject, head, level);
                if (temp.ReflectionException != null)
                {
                    return temp;
                }
                return SetProperty(temp.Result, tail, value, level + 1);
            }
            return SetCurrentProperty(subject, property, value, level);
        }

        private ReflectionResult GetProperty(object subject, string property, int level)
        {
            var exception = GuardPropertyIsSet(property);
            if (exception != null) return new ReflectionResult { ReflectionException = exception };
            int split = property.IndexOf(SEPERATOR, StringComparison.Ordinal);
            if (split >= 0)
            {
                string head = property.Substring(0, split);
                string tail = property.Substring(split + 1);
                var temp = GetCurrentProperty(subject, head, level);
                if (temp.ReflectionException != null)
                {
                    return temp;
                }
                var result = GetProperty(temp.Result, tail, level + 1);
                if (temp.Result != null) result.Partial = true;
                return result;
            }
            return GetCurrentProperty(subject, property, level);
        }

        private ReflectionResult SetCurrentProperty(object source, string property, object value, int level)
        {
            var propertyHandler = DeterminePropertyHandlerCached(source, property);
            if (propertyHandler.ReflectionException != null)
            {
                return new ReflectionResult { ReflectionException = propertyHandler.ReflectionException };
            }
            return propertyHandler.PropertyHandler.Set(property, source, value, level);
        }

        public object GetDirectProperty(string property)
        {
            var exception = GuardPropertyIsSet(property);
            if (exception != null) throw exception;
            var result = GetCurrentProperty(_subject, property, 0);
            if(result.ReflectionException !=null) throw result.ReflectionException;
            return result.Result;
        }

        private ReflectionResult GetCurrentProperty(object source, string property, int level)
        {
            if (source == null && Mode == ReflectionMode.Loose)
                return new ReflectionResult {Result = null};
            var propertyHandler = DeterminePropertyHandlerCached(source, property);
            if (propertyHandler.ReflectionException != null)
            {
                return new ReflectionResult { ReflectionException = propertyHandler.ReflectionException };
            }
            ReflectionResult result = propertyHandler.PropertyHandler.Get(property, source, level);
            if (result.Result != null)
            {
                result.Result = Resolve(result.Result);
            }
            if (result.Result != null)
            {
                result.Full = true;
            }
            return result;
        }

        private static ReflectionException GuardPropertyIsSet(string property)
        {
            if (property == null)
            {
                return ReflectionException.NoPropertyAvailable();
            }
            return null;
        }


        private class CacheKey
        {
            private readonly Type _type;
            private readonly string _property;
            private readonly int _hashKey;

            public CacheKey(string property, Type type)
            {
                _property = property;
                _type = type;
                _hashKey = _type.GetHashCode() + 29 * _property.GetHashCode();
            }

            public CacheKey(string property, object obj)
            {
                _property = property;
                _type = obj.GetType();
                _hashKey = _type.GetHashCode() + 29 * _property.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                var cacheKey = obj as CacheKey;
                if (cacheKey == null) return false;
                return _type == cacheKey._type && Equals(_property, cacheKey._property);
            }

            public override int GetHashCode()
            {
                return _hashKey;
            }
        }

        private class PropertyHandler
        {
            public delegate ReflectionResult GetResult(string property, object source);
            public delegate ReflectionResult SetResult(string property, object source, object value);

            private GetResult _get;
            private SetResult _set;

            public PropertyHandler(GetResult get, SetResult set)
            {
                _get = get;
                _set = set;
            }

            public ReflectionResult Get(string property, object source, int level)
            {
                var result = _get(property, source);
                if (result.ReflectionException != null)
                {
                    result.ReflectionException.Nesting = level;
                }
                return result;
            }

            public ReflectionResult Set(string property, object source, object value, int level)
            {
                var result = _set(property, source, value);
                if (result.ReflectionException != null)
                {
                    result.ReflectionException.Nesting = level;
                }
                return result;
            }
        }

        private static ReflectionPropertyResult DeterminePropertyHandlerCached(object source, string property)
        {
            var result = GuardSource(property, source);
            if (result != null) return result;
            var key = new CacheKey(property, source);
            ReflectionPropertyResult cacheResult;
            HANDLER_CACHE.TryGetValue(key, out cacheResult);
            if (cacheResult != null)
                return cacheResult;

            lock (HANDLER_CACHE_LOCK)
            {
                HANDLER_CACHE.TryGetValue(key, out cacheResult);
                if (cacheResult != null)
                    return cacheResult;

                cacheResult = DeterminePropertyHandler(source);
                HANDLER_CACHE[key] = cacheResult;
                return cacheResult;
            }
        }

        private static ReflectionPropertyResult DeterminePropertyHandler(object source)
        {
            PropertyHandler result;
            if (source.GetType().IsMarshalByRef)
            {
                result = new PropertyHandler(GetSimple, SetSimple);
            }
            else if (source is IReflectionModel)
            {
                result = new PropertyHandler(GetReflection, SetReflection);
            }
            else if (source is IHaveIndexer)
            {
                result = new PropertyHandler(GetIndexer, SetIndexer);
            }
            else if (source is IDictionary)
            {
                result = new PropertyHandler(GetDictionary, SetDictionary);
            }
            else if (source is IDictionary<string, object>)
            {
                result = new PropertyHandler(GetLookUpDictionary, SetLookUpDictionary);
            }
            else if (source is NameValueCollection)
            {
                result = new PropertyHandler(GetNameValueCollection, SetNameValueCollection);
            }
            else if (source is JObject)
            {
                result = new PropertyHandler(GetJObject, SetNameValueCollection);
            }
            else if (source is Array)
            {
                result = new PropertyHandler(GetArray, SetArray);
            }
            else if (source is IList)
            {
                result = new PropertyHandler(GetList, SetList);
            }
            else if (source is IEnumerable)
            {
                result = new PropertyHandler(GetEnumerable, SetEnumerable);
            }
            else
            {
                result = new PropertyHandler(GetSimple, SetSimple);
            }
            return new ReflectionPropertyResult { PropertyHandler = result };
        }

        private static ReflectionResult SetEnumerable(string property, object source, object value)
        {
            return new ReflectionResult { ReflectionException = ReflectionException.SetNotAvailable("Enumerable") };
        }

        private static ReflectionResult SetNameValueCollection(string property, object source, object value)
        {
            return new ReflectionResult { ReflectionException = ReflectionException.SetNotAvailable("NameValueCollection") };
        }

        private object Resolve(object result)
        {
            if (_objectResolver != null)
            {
                result = _objectResolver(result);
            }
            return result;
        }

        private static ReflectionPropertyResult GuardSource(string property, object source)
        {
            if (source == null)
            {
                return new ReflectionPropertyResult { ReflectionException = ReflectionException.NoSourceAvailable(property) };
            }
            return null;
        }

        private static ReflectionResult SetDictionary(string property, object source, object value)
        {
            var sourceAsList = (IDictionary)source;
            if (sourceAsList.Contains(property))
            {
                sourceAsList[property] = value;
            }
            else
            {
                sourceAsList.Add(property, value);
            }
            return new ReflectionResult();
        }

        private static ReflectionResult SetLookUpDictionary(string property, object source, object value)
        {
            var sourceAsList = (IDictionary<string, object>)source;
            if (sourceAsList.ContainsKey(property))
            {
                sourceAsList[property] = value;
            }
            else
            {
                sourceAsList.Add(property, value);
            }
            return new ReflectionResult();
        }

        private static ReflectionResult SetIndexer(string property, object source, object value)
        {
            var result = SetSimple(property, source, value);
            if (result.ReflectionException != null)
                return result;

            var sourceAsIndexer = (IHaveIndexer)source;
            sourceAsIndexer[property] = value;
            return new ReflectionResult();
        }

        private static ReflectionResult GetDictionary(string property, object source)
        {
            try
            {
                var sourceAsList = (IDictionary) source;
                var result = new ReflectionResult {Result = sourceAsList[property]};
                if (result.Result == null)
                    result.Full = sourceAsList.Contains(property);
                return result;
            }
            catch (KeyNotFoundException e)
            {
                return new ReflectionResult
                {
                    ReflectionException = ReflectionException.KeyNotFound(property)
                };
            }
        }


        private static ReflectionResult GetNameValueCollection(string property, object source)
        {
            try
            {
                var sourceAsList = (NameValueCollection)source;
                return new ReflectionResult { Result = sourceAsList[property] };
            }
            catch (KeyNotFoundException e)
            {
                return new ReflectionResult
                {
                    ReflectionException = ReflectionException.KeyNotFound(property)
                };
            }
        }

        private static ReflectionResult GetJObject(string property, object source)
        {
            try
            {
                var json = (JObject)source;
                return new ReflectionResult { Result = json[property] };
            }
            catch (KeyNotFoundException e)
            {
                return new ReflectionResult
                {
                    ReflectionException = ReflectionException.KeyNotFound(property)
                };
            }
        }


        private static ReflectionResult GetLookUpDictionary(string property, object source)
        {
            try
            {
                var sourceAsList = (IDictionary<string, object>)source;
                return new ReflectionResult { Result = sourceAsList[property] };
            }
            catch (KeyNotFoundException e)
            {

                return new ReflectionResult
                {
                    ReflectionException = ReflectionException.KeyNotFound(property)
                };
            }
        }

        private static ReflectionResult GetIndexer(string property, object source)
        {
            var result = GetSimple(property, source);
            if (result.ReflectionException == null)
                return result;
            try
            {
                var sourceAsList = (IHaveIndexer)source;
                return new ReflectionResult { Result = sourceAsList[property] };
            }
            catch (KeyNotFoundException e)
            {
                return new ReflectionResult
                {
                    ReflectionException = ReflectionException.KeyNotFound(property)
                };
            }
        }

        private static ReflectionResult SetList(string property, object source, object value)
        {
            var sourceAsList = (IList)source;
            if (property.Equals("add"))
            {
                sourceAsList.Add(value);
                return new ReflectionResult();
            }
            else
            {
                int index = int.Parse(property);
                try
                {
                    sourceAsList[index] = value;
                    return new ReflectionResult();
                }
                catch (ArgumentOutOfRangeException)
                {
                    return new ReflectionResult
                    {
                        ReflectionException = ReflectionException.IndexOutOfBounds(source.GetType(), index)
                    };
                }
            }
        }


        private static ReflectionResult GetList(string property, object source)
        {
            var sourceAsList = (IList)source;
            int index = int.Parse(property);
            try
            {
                return new ReflectionResult { Result = sourceAsList[index] };
            }
            catch (ArgumentOutOfRangeException)
            {
                return new ReflectionResult
                {
                    ReflectionException = ReflectionException.IndexOutOfBounds(source.GetType(), index)
                };
            }
        }

        private static ReflectionResult SetArray(string property, object source, object value)
        {
            var sourceAsArray = (Array)source;
            int index = int.Parse(property);
            try
            {
                sourceAsArray.SetValue(value, index);
                return new ReflectionResult();
            }
            catch (IndexOutOfRangeException)
            {
                return new ReflectionResult { ReflectionException = ReflectionException.IndexOutOfBounds(source.GetType(), index) };
            }
        }

        private static ReflectionResult GetArray(string property, object source)
        {
            var sourceAsArray = (Array)source;
            int index = int.Parse(property);
            try
            {
                return new ReflectionResult { Result = sourceAsArray.GetValue(index) };
            }
            catch (IndexOutOfRangeException)
            {
                return new ReflectionResult { ReflectionException = ReflectionException.IndexOutOfBounds(source.GetType(), index) };
            }
        }

        private static ReflectionResult GetEnumerable(string property, object source)
        {
            var sourceAsEnumerable = (IEnumerable)source;
            int index = -1;
            if (!int.TryParse(property, out index))
            {
                return new ReflectionResult
                {
                    ReflectionException = ReflectionException.NumberIndexerExepected(source.GetType(), property)
                };
            }
            IEnumerator enumerator = sourceAsEnumerable.GetEnumerator();
            int i = 0;
            do
            {
                if (!enumerator.MoveNext())
                {
                    return new ReflectionResult
                    {
                        ReflectionException = ReflectionException.IndexOutOfBounds(source.GetType(), index)
                    };
                }
                i++;
            } while (i <= index);
            return new ReflectionResult { Result = enumerator.Current };
        }

        private static ReflectionResult SetSimple(string property, object source, object value)
        {
            PropertyInfoResult info = AcquirePropertyInfo(property, source);
            if (info.ReflectionException != null)
            {
                return new ReflectionResult { ReflectionException = info.ReflectionException };
            }
            value = TypeConverter.To(value, info.PropertyInfo.PropertyType);
            try
            {
                info.PropertyInfo.SetValue(source, value, null);
                return new ReflectionResult();
            }
            catch (TargetInvocationException Te)
            {
                if (Te.InnerException != null)
                {
                    throw Te.InnerException;
                }
                throw;
            }
        }

        private static ReflectionResult GetSimple(string property, object source)
        {
            try
            {
                PropertyInfoResult info = AcquirePropertyInfo(property, source);
                if (info.ReflectionException != null)
                {
                    return new ReflectionResult { ReflectionException = info.ReflectionException };
                }
                return new ReflectionResult { Result = info.PropertyInfo.GetValue(source, null) };
            }
            catch (TargetInvocationException TIe)
            {
                if (TIe.InnerException != null)
                {
                    throw TIe.InnerException;
                }
                throw;
            }
        }

        private static ReflectionResult GetReflection(string property, object source)
        {
            return new ReflectionResult { Result = ((IReflectionModel)source)[property] };
        }

        private static ReflectionResult SetReflection(string property, object source, object value)
        {
            ((IReflectionModel)source)[property] = value;
            return new ReflectionResult();
        }

        public bool HasProperty(string property)
        {
            return AcquirePropertyInfo(property, _subject)?.PropertyInfo !=  null;
        }

        public bool HasPropertyOfType(string property, Type expectedType)
        {
            var propertyInfo =  AcquirePropertyInfo(property, _subject)?.PropertyInfo;
            if (propertyInfo == null) return false;
            return expectedType.IsAssignableFrom(propertyInfo.PropertyType);
        }

        public bool HasPropertyGetter(string property)
        {
            return AcquirePropertyInfo(property, _subject)?.PropertyInfo?.GetMethod?.IsPublic ?? false;
        }

        public bool HasPropertySetter(string property)
        {
            return AcquirePropertyInfo(property, _subject)?.PropertyInfo?.SetMethod?.IsPublic ?? false;
        }

        public static PropertyInfoResult AcquirePropertyInfo(string property, object source)
        {
            Type type = source.GetType();
            var key = new CacheKey(property, type);
            PropertyInfoResult info;
            PROPERTY_CACHE.TryGetValue(key, out info);
            if (info != null)
                return info;

            lock (PROPERTY_CACHE_LOCK)
            {
                PROPERTY_CACHE.TryGetValue(key, out info);
                if (info != null)
                    return info;
                info = InternalAcquirePropertyInfo(property, type, source);
                PROPERTY_CACHE.Add(key, info);
                return info;
            }
        }

        private static PropertyInfoResult InternalAcquirePropertyInfo(string property, Type type, object source = null)
        {
            PropertyInfo info = type.GetProperty(property);
            if (info == null)
            {
                return new PropertyInfoResult
                {
                    ReflectionException = ReflectionException.PropertyNotFound(property, type, source)
                };
            }
            return new PropertyInfoResult { PropertyInfo = info };
        }
    }
}
