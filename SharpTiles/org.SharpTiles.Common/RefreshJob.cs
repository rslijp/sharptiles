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
using System.Collections.Generic;
 using System.Linq;
 using System.Threading;

namespace org.SharpTiles.Common
{
    public class RefreshJob
    {
        public const int SECS = 1000;

        private static readonly IList<WeakReference<RefreshableResource>> REGISTERED = new List<WeakReference<RefreshableResource>>();
        private static Thread JOB;
        public static int REFRESH_INTERVAL = 300*SECS;

        public static int Count => REGISTERED.Count;

        public static void Register(RefreshableResource resource)
        {
            lock (REGISTERED)
            {
                GuardJobActive();
                REGISTERED.Add(new WeakReference<RefreshableResource>(resource));
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
            JOB.Name = $"Resouce refresh job (interval of #{REFRESH_INTERVAL})";
            JOB.IsBackground = true;
            JOB.Priority = ThreadPriority.Normal;
            JOB.Start();
        }

        private static void RefreshTask()
        {
            while (true)
            {
                Thread.Sleep(REFRESH_INTERVAL);
                ClearUnreferencedItems();
                lock (REGISTERED)
                {
                    var copy = REGISTERED.ToList();
                    foreach (var reference in copy)
                    {
                        RefreshableResource resource;
                        reference.TryGetTarget(out resource);
                        resource?.Refresh();
                    }
                }
            }
        }

        public static void ClearUnreferencedItems()
        {
            lock (REGISTERED)
            {
                RefreshableResource resource;
                var deletable = REGISTERED.Where(r => !r.TryGetTarget(out resource)).ToList();
                foreach (var reference in deletable)
                    REGISTERED.Remove(reference);
            }
        }


        public static void RevokeAll()
        {
            lock (REGISTERED)
            {
                REGISTERED.Clear();
            }
        }
    }
}
