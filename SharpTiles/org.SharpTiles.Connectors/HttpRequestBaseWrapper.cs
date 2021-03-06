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
using System.Web;
using org.SharpTiles.Common;

namespace org.SharpTiles.Connectors
{
    public class HttpRequestBaseWrapper : IReflectionModel
    {
        private HttpRequestBase _request;

        public HttpRequestBaseWrapper(HttpRequestBase request)
        {
            _request = request;
        }

        public object this[string property]
        {
            get { 
                var result = _request.Params.Get(property);
                if (result != null) return result;
                return new Reflection(_request)[property];
            }
            set { throw new NotSupportedException("Request is read only");}
        }

        public ReflectionResult Get(string property)
        {
            var result = _request.Params.Get(property);
            if (result != null) return new ReflectionResult {Result = result};
            return new Reflection(_request).Get(property);
            
        }

        public object TryGet(string property)
        {
            var result = _request.Params.Get(property);
            if (result != null) return result;
            return new Reflection(_request).TryGet(property);
        }
    }
}
