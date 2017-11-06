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
 using System.Linq;
 using System.Text;
using System.Threading;
using org.SharpTiles.Common;
 using org.SharpTiles.Tags.Creators;
 using org.SharpTiles.Tags.FormatTags;

namespace org.SharpTiles.Tags
{
    public class TagModel : IModel
    {
        public static readonly string PAGE_MODEL_PRINCIPAL_INSTANCE = "User";
        private static readonly IModel GLOBAL = new Reflection(new Hashtable());
        public static readonly IDictionary<string, VariableScope> SCOPE_MAPPING;
        public static readonly IList<VariableScope> SCOPE_ORDER;
        private readonly Reflection _internal;
        private readonly IModel _model;
        private readonly IModel _page;
        private readonly IResponse _response;
        private readonly IModel _request;
        private readonly IModel _session;
        
        private Encoding _encoding;
        private bool _hasTags;
        private Stack<TagStackModel> _tagVariables = new Stack<TagStackModel>();
        
        static TagModel()
        {
            GLOBAL[FormatConstants.LOCALE] = Thread.CurrentThread.CurrentCulture;
            var scopeOrder = new List<VariableScope>();
            foreach (VariableScope scope in Enum.GetValues(typeof (VariableScope)))
            {
                scopeOrder.Add(scope);
            }
            scopeOrder.Sort();
            SCOPE_ORDER = scopeOrder;

            SCOPE_MAPPING = new Dictionary<string, VariableScope>();
            foreach (VariableScope scope in Enum.GetValues(typeof (VariableScope)))
            {
                SCOPE_MAPPING.Add(scope.ToString(), scope);
            }
        }

        public TagModel(object model)
            : this(new Reflection(model), null, null, null)
        {
        }

        public TagModel BecomeStrict()
        {
            _internal.BecomeStrict();
            return this;
        }

        public TagModel(IModel model)
            : this(model, null, null, null)
        {
        }

        public TagModel(object model, IModel session)
            : this(new Reflection(model), session, null, null)
        {
        }

        public TagModel(object model, IModel session, IModel request, IResponse response)
            : this(new Reflection(model), session, request, response)
        {
        }

        public TagModel(IModel model, IModel session, IModel request, IResponse response)
        {
            _model = model;
            _page = new Reflection(new Hashtable());
            _request = request;
            _session = session != null ? new Reflection(session) : null;
            _internal = new Reflection(this);
            _response = response;
        }

        public bool ThrowExceptionOnGet { get; set; } = true;

        public Stack<TagStackModel> TagVariables
        {
            get { return _tagVariables; }
            set { _tagVariables = value; }
        }

        public IModel Tag
        {
            get { return _tagVariables.Any()?_tagVariables.Peek():null; }
        }

        public IModel Model
        {
            get { return _model; }
        }

        public IModel Page
        {
            get { return _page; }
        }

        public IModel Request
        {
            get { return _request; }
        }

        public IModel Session
        {
            get { return _session; }
        }

        public static IModel GlobalModel
        {
            get { return GLOBAL; }
        }

        public IModel Global
        {
            get { return GLOBAL; }
        }

//        public IResourceLocatorFactory Factory
//        {
//            get { return _factory; }
//        }

        public Encoding Encoding
        {
            get { return _encoding; }
            set
            {
                _encoding = value;
                if (_response != null)
                {
                    _response.ResponseEncoding = value;
                }
            }
        }

        public bool ContainsKey(string property)
        {
            return HasScopePrefix(property)
                ? _internal.Get(property)?.ReflectionException == null
                : Get(property)?.ReflectionException == null;
        }

        #region IModel Members

        public object this[string property]
        {
            get
            {
                return HasScopePrefix(property) ? _internal[property] : Resolve(property, ThrowExceptionOnGet);
            }
            set
            {
                if (HasScopePrefix(property))
                {
                    _internal[property] = value;
                }
                else
                {
                    _internal[VariableScope.Model + Reflection.SEPERATOR + property] = value;
                }
            }
        }

        public object TryGet(string property)
        {
            return HasScopePrefix(property) ? _internal.TryGet(property) : Resolve(property, false);
        }

        #endregion

        public void PushTagStack(bool peekInParent = false)
        {
            peekInParent &= _tagVariables.Count > 0;
            _tagVariables.Push(new TagStackModel(peekInParent?_tagVariables.Peek():null));
            _hasTags = true;
        }

        public void PopTagStack()
        {
            _tagVariables.Pop();
            _hasTags = _tagVariables.Count > 0;
        }

        private static bool HasScopePrefix(string property)
        {
            var split = property.IndexOf(Reflection.SEPERATOR, StringComparison.Ordinal);
            if (split == -1)
                return false;

            var head = property.Substring(0, split);
            return SCOPE_MAPPING.ContainsKey(head);
        }

        public ReflectionResult Get(string property)
        {
            ReflectionException lastException = null;
            foreach (VariableScope scope in SCOPE_ORDER)
            {

                IModel model = GetModel(scope);
                if (model != null)
                {
                    ReflectionResult reflectionResult = model.Get(property);
                    if (reflectionResult.Partial|| reflectionResult.Full)
                    {
                        return reflectionResult;
                    }
                    if (lastException == null ||
                        (   reflectionResult.ReflectionException != null &&
                            lastException.Nesting < reflectionResult.ReflectionException.Nesting)
                        )
                    {
                        lastException = reflectionResult.ReflectionException;
                    }
                }
            }
            if (lastException != null)
            {
                return new ReflectionResult {ReflectionException = lastException};
            }
            return new ReflectionResult();
        }

        public object Resolve(string property, bool throwException)
        {
            var result = Get(property);
            if (throwException && result.ReflectionException != null)
            {
                throw result.ReflectionException;
            }
            return result.Result;
        }

        private IModel GetModel(VariableScope scope)
        {
            switch (scope)
            {
                case VariableScope.Global:
                    return GLOBAL;
                case VariableScope.Model:
                    return _model;
                case VariableScope.Page:
                    return _page;
                case VariableScope.Request:
                    return _session;
                case VariableScope.Session:
                    return _session;
                case VariableScope.Tag:
                    return _hasTags ? _tagVariables.Peek() : null;
                default:
                    throw new NotImplementedException("GetModel not implemented for " + scope);
            }
        }

        public object SearchInTagScope(string property)
        {
            object result = null;
            foreach (IModel model in _tagVariables)
            {
                result = model[property];
                if (result != null)
                {
                    break;
                }
            }
            return result;
        }

        public string ApplicationName
        {
            get { return _response != null ? _response.ApplicationPath : String.Empty; }
        }

        public void Redirect(string url)
        {
            GuardResponse();
            _response.Redirect(url);
        }

        public void GuardResponse()
        {
            if (_response == null)
            {
                throw TagException.HttpResponseNotAvailable();
            }
        }
        
//        public TagModel UpdateFactory(IResourceLocatorFactory factory)
//        {
//            _factory = factory;
//            return this;
//        }

    }
}
