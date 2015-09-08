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
using System.Text;
using System.Web;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;

namespace  org.SharpTiles.Connectors
{
    public class HttpContextBaseResponseWrapper : IResponse
    {
        private readonly HttpContextBase _httpContext;
        private string _appName;


        public HttpContextBaseResponseWrapper(HttpContextBase httpContext, string appName)
        {
            _httpContext = httpContext;
            _appName = appName;
        }

        public void Redirect(string url)
        {
            _httpContext.Response.Redirect(url);
        }

        public string ApplicationPath
        {
            get { return _appName; }
        }

        public Encoding ResponseEncoding
        {
            set { _httpContext.Response.ContentEncoding = value; }
        }

    }
}
