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
using System.Web.UI;
 using org.SharpTiles.Common;
 using org.SharpTiles.Tags;

namespace org.SharpTiles.Connectors
{
    internal class PageWrapper : IResponse
    {
        private readonly Page _page;

        public PageWrapper(Page page)
        {
            _page = page;
        }

        #region IResponse Members

        public void Redirect(string url)
        {
            _page.Response.Redirect(url, true);
        }

        public string ApplicationPath
        {
            get { return _page.NamingContainer.Site.Name; }

        }

        public Encoding ResponseEncoding
        {
            get { return Encoding.GetEncoding(_page.ResponseEncoding); }
            set { _page.ResponseEncoding = value.EncodingName; }
        }

        #endregion

    }   
}
