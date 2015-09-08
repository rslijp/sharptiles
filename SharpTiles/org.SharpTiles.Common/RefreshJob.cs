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
 using System.Collections.Generic;
 using System.Linq;
 using System.Threading;

namespace org.SharpTiles.Common
{
    public class RefreshJob
    {
        public const int SECS = 1000;

        private static readonly IList<RefreshableResource> REGISTERED = new List<RefreshableResource>();
        private static Thread JOB;
        public static int REFRESH_INTERVAL = 300*SECS;

        public static int Count
        {
            get { return REGISTERED.Count; }
        }

        public static void Register(RefreshableResource resource)
        {
            GuardJobActive();
            lock (REGISTERED)
            {
                REGISTERED.Add(resource);
            }
        }

        public static void Revoke(RefreshableResource resource)
        {
            lock (REGISTERED)
            {
                REGISTERED.Remove(resource);
            }
        }

        private static void GuardJobActive()
        {
            if (JOB == null)
            {
                StartRefreshJob();
            }
        }

        private static void StartRefreshJob()
        {
            JOB = new Thread(RefreshTask);
            // Configure the new thread and start it
            JOB.Name = string.Format("Resouce refresh job (interval of #{0})", REFRESH_INTERVAL);
            JOB.IsBackground = true;
            JOB.Priority = ThreadPriority.Normal;
            JOB.Start();
        }

        private static void RefreshTask()
        {
            while (true)
            {
                Thread.Sleep(REFRESH_INTERVAL);
                lock (REGISTERED)
                {
                    var copy = REGISTERED.ToList();
                    foreach (var resource in copy)
                    {
                        resource.Refresh();
                    }
                }
            }
        }


        public static void RevokeAll()
        {
            var clone = new List<RefreshableResource>(REGISTERED);
            foreach (RefreshableResource resource in clone)
            {
                Revoke(resource);
            }
        }
    }
}
