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
 */using System.Collections.Specialized;
using System.Web;

namespace org.SharpTiles.HtmlTags.Test
{
    public class MockContextBase : HttpContextBase
    {
        private readonly MockRequestBase _request = new MockRequestBase();
        private readonly MockResponseBase _response = new MockResponseBase();


        public override HttpResponseBase Response
        {
            get { return _response; }
        }

        public override HttpRequestBase Request
        {
            get { return _request; }
        }

        public string Result
        {
            get { return _response.Result; }
        }

        #region Nested type: MockRequestBase

        public class MockRequestBase : HttpRequestBase
        {
            private readonly NameValueCollection _collection = new NameValueCollection();

            public override bool IsAuthenticated
            {
                get { return ManualAuthenticated;  }
            }

            public bool ManualAuthenticated { get; set; }

            public override NameValueCollection Params
            {
                get { return _collection; }
            }

            public override string ApplicationPath
            {
                get { return "/SharpTiles"; }
            }

            public override NameValueCollection ServerVariables
            {
                get
                {
                    return _collection;
                }
            }
        }

        #endregion

        #region Nested type: MockResponseBase

        public class MockResponseBase : HttpResponseBase
        {
            private string _result;

            public string Result
            {
                get { return _result; }
            }

            public override void Write(string s)
            {
                _result = s;
            }

            public override void AddHeader(string name, string value)
            {
                //
            }

            public override string ApplyAppPathModifier(string virtualPath)
            {
                return virtualPath;
            }
        }

        #endregion
    }
}
