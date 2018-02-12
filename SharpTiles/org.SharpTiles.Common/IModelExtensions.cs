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
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace org.SharpTiles.Common
{
    public static class IModelExtensions
    {
        public static IModel AdaptToNaturalLanguage(this IModel model)
        {
            return new NaturalLanguageModelWrapper(model);
        }

        public static IModel AsReadonlyModel(this IModel model)
        {
            if(model==null) return null;
            return new ReadonlyWrapper(model);
        }

        public class ReadonlyWrapper : IModel
        {
            private IModel _backing;

            public ReadonlyWrapper(IModel backing)
            {
                _backing = backing;
            }
            public object this[string property]
            {
                get { return _backing[property]; }
                set { throw ReflectionException.SetNotAvailableInForkedModel(property); }
            }

            public object TryGet(string property)
            {
                return _backing.TryGet(property);
            }

            public ReflectionResult Get(string property)
            {
                return _backing.Get(property);
            }
        }

        public class NaturalLanguageModelWrapper : IModel
        {
            private IModel inner;

            public NaturalLanguageModelWrapper(IModel reflection)
            {
                this.inner = reflection;
            }

            public object TryGet(string property)
            {
                return inner.TryGet(LanguageHelper.CamelCaseProperty(property));
            }

            public ReflectionResult Get(string property)
            {
                return inner.Get(LanguageHelper.CamelCaseProperty(property));
            }

            public object this[string property]
            {
                get { return inner[LanguageHelper.CamelCaseProperty(property)]; }
                set { inner[LanguageHelper.CamelCaseProperty(property)] = value; }
            }
        }
    }
}
