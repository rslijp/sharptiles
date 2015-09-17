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
 using System.Web;
using System.Web.SessionState;
using org.SharpTiles.Tags;

namespace org.SharpTiles.Connectors
{
    public class SimpleHttpSessionState : ISimpleSessionState
    {
        private static HttpSessionState Session
        {
            get { return HttpContext.Current.Session; }
        }

        #region ISimpleSessionState Members

        public object this[string key]
        {
            get { return Session[key]; }
            set
            {
                if (value != null)
                {
                    Session.Remove(key);
                    Session.Add(key, value);
                }
                else
                {
                    Session.Remove(key);
                }
            }
        }

        public object TryGet(string property)
        {
            return Session[property];
        }

        public void Clear()
        {
            Session.Clear();
        }

        #endregion
    }
}
