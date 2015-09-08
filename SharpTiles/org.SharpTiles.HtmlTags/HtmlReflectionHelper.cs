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
 */using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;

namespace org.SharpTiles.HtmlTags
{
    public class HtmlReflectionHelper
    {
        private static IDictionary<String, IList<MethodInfo>> CACHE = new Dictionary<String, IList<MethodInfo>>();

        private readonly String _methodName;
        private readonly Type _wrappedType;
        private Dictionary<string, ITagAttribute> _attributes;
        private MethodInfo _candidate;
        private readonly IEnumerable<MethodInfo> _allMethods;

        public HtmlReflectionHelper(Type wrappedType, String methodName)
        {
            _wrappedType = wrappedType;
            _methodName = methodName;
            _allMethods = GetMethods(wrappedType, methodName);
                          
            
        }

        private static IEnumerable<MethodInfo> GetMethods(Type wrappedType, String methodName)
        {
            String key = wrappedType.Name + ":" + methodName;
            IList<MethodInfo> methods;
            CACHE.TryGetValue(key, out methods);
            if(methods==null)
            {
                methods = wrappedType.GetMethods().ToList().Where(method => method.Name.Equals(methodName) && FilterOutHtmlAttributes(method)).ToList();
                CACHE.Add(key, methods);
            }
            return methods;
        }

        private static bool FilterOutHtmlAttributes(MethodInfo method)
        {
            var suspect = method.GetParameters().SingleOrDefault(p => p.Name.Equals(Html.HTMLATTRIBUTES_PARAM_NAME));
            if (suspect == null) return true;
            return suspect.ParameterType.Equals(Html.HTMLATTRIBUTES_PARAM_TYPE);
        }

        public ICollection<IParameterValue> Parameters { get; set; }
        public ParseContext Context { get; set; }

        public Type WrappedType
        {
            get { return _wrappedType; }
        }

        public String MethodName
        {
            get { return _methodName; }
        }


        private static ICollection<ParameterExpectation> AsExpectations(IEnumerable<IParameterValue> paramaters)
        {
            var offered = new HashSet<ParameterExpectation>();
            paramaters.ToList().ForEach(p => offered.Add(ParameterExpectation.Expect(p)));
            return offered;
        }

        public object InternalEvaluate(HtmlHelper helper, TagModel model)
        {
            MethodInfo method = CandidateMethod();
            return method.Invoke(null,
                                          AssembleParameters(method, helper, model)
                );
        }

        public object[] AssembleParameters(MethodInfo method, HtmlHelper helper, TagModel model)
        {
            Dictionary<string, object> parameterTable = Parameters.ToDictionary(p => p.Name.ToLowerInvariant(),
                                                                                p => p.Value(model));
            parameterTable.Add(Html.HTMLHELPER_PARAM_NAME.ToLowerInvariant(), helper);
            var parameterValues = new List<object>();
            string parameterName = null;
            try
            {
                foreach (ParameterInfo parameter in method.GetParameters())
                {
                    parameterName = parameter.Name.ToLowerInvariant();
                    parameterValues.Add(TypeConverter.To(parameterTable[parameterName], parameter.ParameterType));
                }
            } catch (KeyNotFoundException)
            {
                throw HtmlHelperTagException.RequiredArgumentMissing(parameterName, method).Decorate(Context);
                
            }
            return parameterValues.ToArray();
        }

        internal void ApplyHtmlAttributes()
        {
            Parameters = ExtractHtmlAttributes(Parameters, CollectAvailableParameters());
        }

        #region determine method

        public IModel AttributeModel
        {
            get
            {
                if (_attributes == null)
                {
                    _attributes = new Dictionary<string, ITagAttribute>();
                }
                return new Reflection(_attributes);
            }
        }

        public IEnumerable<MethodInfo> AllMethods
        {
            get { return _allMethods; }
        }

        public MethodInfo CandidateMethod()
        {
            if (_candidate == null)
            {
                IEnumerable<MethodInfo> candidates = CandidateMethods();
                candidates = candidates.OrderBy(m => m.GetParameters().Length);
                _candidate = candidates.FirstOrDefault();
                if (_candidate == null)
                {
                    throw HtmlHelperTagException.NoSuitableMethodFound(MethodName, AsExpectations(Parameters)).Decorate(
                        Context);
                }
            }
            return _candidate;
        }

        public IEnumerable<MethodInfo> CandidateMethods()
        {
            ICollection<ParameterExpectation> offered = AsExpectations(Parameters);
            offered.Add(
                ParameterExpectation.Expect(Html.HTMLHELPER_PARAM_NAME).Of(Html.HTMLHELPER_PARAM_TYPE).And().
                    ItMustBeAnExactMatch());
            
            return FindMethods(offered);
        }

        private IEnumerable<MethodInfo> FindMethods(IEnumerable<ParameterExpectation> expected)
        {
            var candidates = new List<MethodInfo>();
            foreach (MethodInfo info in AllMethods)
            {
                var available = new HashSet<ParameterExpectation>(expected);
                info.GetParameters().ToList().ForEach(p => RemoveParameter(p, available));
                if (available.Count == 0)
                {
                    candidates.Add(info);
                }
            }
            return candidates;
        }

        private static void RemoveParameter(ParameterInfo requested, ICollection<ParameterExpectation> available)
        {
            available.Remove(available.SingleOrDefault(m => m.CanBeUsedFor(requested)));
        }

        #endregion

        #region static constructor

        private ICollection<ParameterExpectation> CollectAvailableParameters()
        {
            var options = new HashSet<ParameterExpectation>();
            foreach (MethodInfo info in AllMethods)
            {
                info.GetParameters().ToList().ForEach(p => options.Add(ParameterExpectation.Expect(p.Name)));
            }
            return options;
        }

        public static ICollection<IParameterValue> ExtractHtmlAttributes(ICollection<IParameterValue> offered,
                                                                          ICollection<ParameterExpectation> with)
        {
            var attributes = new Dictionary<String, IParameterValue>();
            foreach (IParameterValue param in offered.ToList())
            {
                if (with.Contains(ParameterExpectation.Expect(param.Name))) continue;
                offered.Remove(param);
                attributes.Add(param.Name, param);
            }
            if (attributes.Count > 0)
            {
                offered.Add(new HtmlAttributesParameterValue(attributes));
            }
            return offered;
        }

        #endregion
    }
}
