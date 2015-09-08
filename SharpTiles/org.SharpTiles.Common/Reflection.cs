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

namespace org.SharpTiles.Common
{
    public class Reflection : IReflectionModel
    {
        private static IDictionary<CacheKey, PropertyHandler> HANDLER_CACHE = new Dictionary<CacheKey, PropertyHandler>();
        private static IDictionary<CacheKey, PropertyInfo> PROPERTY_CACHE = new Dictionary<CacheKey, PropertyInfo>();

        #region Delegates

        public delegate object ResolveObject(object source);

        #endregion

        public const string SEPERATOR = ".";


        private readonly object _subject;
        private ResolveObject _objectResolver;

        public Reflection(object subject)
        {
            _subject = subject;
        }


        public ResolveObject ObjectResolver
        {
            set { _objectResolver = value; }
        }

        #region IReflectionModel Members

        public object this[string property]
        {
            get {
                try
                {
                    return GetProperty(_subject, property, 0);
                }
                catch (ReflectionException Re)
                {
                    if (Re.IgnoreOnGet) return null;
                    throw Re;
                }
            }
            set { SetProperty(_subject, property, value, 0); }
        }

        #endregion

        private void SetProperty(object subject, string property, object value, int level)
        {
            GuardPropertyIsSet(property);
            int split = property.IndexOf(SEPERATOR);
            if (split >= 0)
            {
                string head = property.Substring(0, split);
                string tail = property.Substring(split + 1);
                SetProperty(GetCurrentProperty(subject, head, level), tail, value, level + 1);
            }
            else
            {
                SetCurrentProperty(subject, property, value, level);
            }
        }

        private object GetProperty(object subject, string property, int level)
        {
            GuardPropertyIsSet(property);
            int split = property.IndexOf(SEPERATOR);
            if (split >= 0)
            {
                string head = property.Substring(0, split);
                string tail = property.Substring(split + 1);
                return GetProperty(GetCurrentProperty(subject, head, level), tail, level+1);
            }
            return GetCurrentProperty(subject, property, level);
        }

        private void SetCurrentProperty(object source, string property, object value, int level)
        {
            DeterminePropertyHandlerCached(source, property).Set(property, source, value, level);
        }


        private object GetCurrentProperty(object source, string property, int level)
        {
            object result = DeterminePropertyHandlerCached(source, property).Get(property, source, level);
            result = Resolve(result);
            return result;
        }

        private static void GuardPropertyIsSet(string property)
        {
            if (property == null)
            {
                throw ReflectionException.NoPropertyAvailable();
            }
        }

        
        private class CacheKey 
        {
            private readonly  Type _type;
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
                CacheKey cacheKey = obj as CacheKey;
                if (cacheKey == null) return false;
                return Equals(_type, cacheKey._type) && Equals(_property, cacheKey._property);
            }

            public override int GetHashCode()
            {
                return _hashKey;
            }
        }

        private class PropertyHandler
        {
            public delegate object GetResult(string property, object source);
            public delegate void SetResult(string property, object source, object value);

            private GetResult _get;
            private SetResult _set;

            public PropertyHandler(GetResult get, SetResult set)
            {
                _get = get;
                _set = set;
            }

            public object Get(string property, object source, int level)
            {
                try
                {
                    return _get(property, source);
                } catch(ReflectionException Re)
                {
                    Re.Nesting = level;
                    throw Re;
                }
            }

            public void Set(string property, object source, object value, int level)
            {
                try
                {
                    _set(property, source, value);
                }
                catch (ReflectionException Re)
                {
                    Re.Nesting = level;
                    throw Re;
                }
            }
        }

        private static PropertyHandler DeterminePropertyHandlerCached(object source, string property)
        {
            GuardSource(property, source);
            var key = new CacheKey(property, source);
            PropertyHandler cacheResult;
            HANDLER_CACHE.TryGetValue(key, out cacheResult);
            if (cacheResult!=null)
            {
                return cacheResult;
            } 
            cacheResult = DeterminePropertyHandler(source);
            HANDLER_CACHE[key] = cacheResult;
            return cacheResult;
        }

        private static PropertyHandler DeterminePropertyHandler(object source)
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
            return result;
        }

        private static void SetEnumerable(string property, object source, object value)
        {
            throw ReflectionException.SetNotAvailable("Enumerable");
        }

        private static void SetNameValueCollection(string property, object source, object value)
        {
            throw ReflectionException.SetNotAvailable("NameValueCollection");
        }

        private object Resolve(object result)
        {
            if (_objectResolver != null)
            {
                result = _objectResolver(result);
            }
            return result;
        }

        private static void GuardSource(string property, object source)
        {
            if (source == null)
            {
                throw ReflectionException.NoSourceAvailable(property);
            }
        }

        private static void SetDictionary(string property, object source, object value)
        {
            var sourceAsList = (IDictionary) source;
            if (sourceAsList.Contains(property))
            {
                sourceAsList[property] = value;
            }
            else
            {
                sourceAsList.Add(property, value);
            }
        }

        private static void SetLookUpDictionary(string property, object source, object value)
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
        }

        private static object GetDictionary(string property, object source)
        {
            var sourceAsList = (IDictionary) source;
            return sourceAsList[property];
        }

        private static object GetNameValueCollection(string property, object source)
        {
            var sourceAsList = (NameValueCollection) source;
            return sourceAsList[property];
        }


        private static object GetLookUpDictionary(string property, object source)
        {
            var sourceAsList = (IDictionary<string, object>)source;
            return sourceAsList[property];
        }

        private static void SetList(string property, object source, object value)
        {
            var sourceAsList = (IList) source;
            if (property.Equals("add"))
            {
                sourceAsList.Add(value);
            }
            else
            {
                int index = int.Parse(property);
                try
                {
                    sourceAsList[index] = value;
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw ReflectionException.IndexOutOfBounds(source.GetType(), index);
                }
            }
        }


        private static object GetList(string property, object source)
        {
            var sourceAsList = (IList) source;
            int index = int.Parse(property);
            try
            {
                return sourceAsList[index];
            }
            catch (ArgumentOutOfRangeException)
            {
                throw ReflectionException.IndexOutOfBounds(source.GetType(), index);
            }
        }

        private static void SetArray(string property, object source, object value)
        {
            var sourceAsArray = (Array) source;
            int index = int.Parse(property);
            try
            {
                sourceAsArray.SetValue(value, index);
            }
            catch (IndexOutOfRangeException)
            {
                throw ReflectionException.IndexOutOfBounds(source.GetType(), index);
            }
        }

        private static object GetArray(string property, object source)
        {
            var sourceAsArray = (Array) source;
            int index = int.Parse(property);
            try
            {
                return sourceAsArray.GetValue(index);
            }
            catch (IndexOutOfRangeException)
            {
                throw ReflectionException.IndexOutOfBounds(source.GetType(), index);
            }
        }

        private static object GetEnumerable(string property, object source)
        {
            var sourceAsEnumerable = (IEnumerable) source;
            int index = int.Parse(property);
            IEnumerator enumerator = sourceAsEnumerable.GetEnumerator();
            int i = 0;
            do
            {
                if (!enumerator.MoveNext())
                {
                    throw ReflectionException.IndexOutOfBounds(source.GetType(), index);
                }
                i++;
            } while (i <= index);
            return enumerator.Current;
        }

        private static void SetSimple(string property, object source, object value)
        {
            PropertyInfo info = AcquirePropertyInfo(property, source);
            value = TypeConverter.To(value, info.PropertyType);
            try
            {
                info.SetValue(source, value, null);
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

        private static object GetSimple(string property, object source)
        {
            try
            {
                PropertyInfo info = AcquirePropertyInfo(property, source);
                return info.GetValue(source, null);
            } catch (TargetInvocationException TIe)
            {
                if(TIe.InnerException!=null)
                {
                    throw TIe.InnerException;
                }
                throw;
            }
        }

        private static object GetReflection(string property, object source)
        {
            return ((IReflectionModel) source)[property];
        }

        private static void SetReflection(string property, object source, object value)
        {
            ((IReflectionModel) source)[property] = value;
        }

        private static PropertyInfo AcquirePropertyInfo(string property, object source)
        {
            Type type = source.GetType();
            var key = new CacheKey(property, type);
            PropertyInfo info;
            PROPERTY_CACHE.TryGetValue(key, out info);
            if(info!=null)
            {
                return info;
            }
            info = InternalAcquirePropertyInfo(property, type);
            PROPERTY_CACHE.Add(key, info);
            return info;
        }

        private static PropertyInfo InternalAcquirePropertyInfo(string property, Type type)
        {
            PropertyInfo info = type.GetProperty(property);
            if (info == null)
            {
                throw ReflectionException.PropertyNotFound(property, type);
            }
            return info;
        }
    }
}
