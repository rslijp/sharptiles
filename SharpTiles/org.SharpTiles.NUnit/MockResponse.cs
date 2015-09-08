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
using System.Collections;
using System.Text;
 using org.SharpTiles.Common;
 using org.SharpTiles.Tags;

namespace org.SharpTiles.NUnit
{
    public class MockResponse : IResponse
    {
        private Hashtable _table;

        public MockResponse()
        {
            _table = new Hashtable();
        }

        public MockResponse(string applicationPath)
        {
            ApplicationPath = applicationPath;
        }
        
        public string LastRedirectUrl { get; set; }

        #region IResponse Members

        public void Redirect(string url)
        {
            LastRedirectUrl = url;
        }

        public string ApplicationPath
        { get; set; }
        
        public Encoding ResponseEncoding { get; set; }

        public object this[string property]
        {
            get { return new Reflection(_table)[property]; }
            set { new Reflection(_table)[property] = value; }
        }

        #endregion
    }
}