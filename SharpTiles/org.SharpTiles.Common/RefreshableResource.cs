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

namespace org.SharpTiles.Common
{
    public abstract class RefreshableResource : IDisposable
    {
        protected RefreshableResource()
        {
            RefreshJob.Register(this);
        }

        public abstract DateTime? ResourceLastModified { get; }

        protected DateTime? LastModified { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        protected abstract void Load();

        public bool RequiresRefresh()
        {
            return LastModified.HasValue && !Equals(LastModified, ResourceLastModified);
        }

        public Exception RefreshException { get; set; }


        public bool Refresh()
        {
            if (RequiresRefresh())
            {
                RefreshResource();
                return true;
            }
            return false;
        }

        public void RefreshResource()
        {
            try
            {
                RefreshException = null;
                LoadResource();
            } catch (Exception e){
                RefreshException = e;
            }
        }


        public void LoadResource()
        {
            Load();
            LastModified = ResourceLastModified;
        }
    }
}
