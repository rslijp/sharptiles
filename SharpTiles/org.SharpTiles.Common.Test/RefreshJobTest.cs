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
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace org.SharpTiles.Common.Test
{
    [TestFixture]
    public class RefreshJobTest
    {
        #region Setup/Teardown

        [TearDown]
        public void TearDown()
        {
            RefreshJob.RevokeAll();
        }

        #endregion

        public class MockRefreshableResource : RefreshableResource
        {
            private int _loadCount;
            private DateTime? _resourceLastModified;

            public MockRefreshableResource(DateTime? resourceLastModified)
            {
                _resourceLastModified = resourceLastModified;
                LoadResource();
            }

            public int LoadCount
            {
                get { return _loadCount; }
            }

            public override DateTime? ResourceLastModified
            {
                get { return _resourceLastModified; }
            }

            public void SetResourceLastModified(DateTime? value)
            {
                _resourceLastModified = value;
            }

            protected override void Load()
            {
                _loadCount++;
            }
        }


        public class MockAlwaysRefreshableResource : RefreshableResource
        {
            private int _loadCount;

            public MockAlwaysRefreshableResource()
            {
                LoadResource();
            }

            public int LoadCount
            {
                get { return _loadCount; }
            }

            public override DateTime? ResourceLastModified
            {
                get { return DateTime.Now; }
            }

            protected override void Load()
            {
                _loadCount++;
            }
        }

        [Test]
        public void TestRefreshJob()
        {
            RefreshJob.REFRESH_INTERVAL = 250;
            Assert.That(RefreshJob.Count, Is.EqualTo(0));
            var rs1 = new MockRefreshableResource(new DateTime(2005, 5, 5));
            Assert.That(rs1.LoadCount, Is.EqualTo(1));
            Assert.That(RefreshJob.Count, Is.EqualTo(1));
            var rs2 = new MockRefreshableResource(new DateTime(2005, 5, 5));
            Assert.That(rs2.LoadCount, Is.EqualTo(1));
            rs1.SetResourceLastModified(new DateTime(2006, 5, 5));
            Assert.That(RefreshJob.Count, Is.EqualTo(2));
            Thread.Sleep(RefreshJob.REFRESH_INTERVAL*2);
            Assert.That(rs1.LoadCount, Is.GreaterThan(1));
            Assert.That(rs2.LoadCount, Is.EqualTo(1));
        }

        [Test]
        public void TestRefreshJobRevoke()
        {
            RefreshJob.REFRESH_INTERVAL = 250;
            Assert.That(RefreshJob.Count, Is.EqualTo(0));
            var rs1 = new MockAlwaysRefreshableResource();
            var rs2 = new MockAlwaysRefreshableResource();
            Assert.That(rs1.LoadCount, Is.EqualTo(1));
            Assert.That(rs2.LoadCount, Is.EqualTo(1));
            Thread.Sleep(RefreshJob.REFRESH_INTERVAL*3);
            Assert.That(rs1.LoadCount, Is.GreaterThan(1));
            Assert.That(rs2.LoadCount, Is.GreaterThan(1));

            RefreshJob.Revoke(rs2);
            RefreshJob.Revoke(rs1);
            int old1 = rs1.LoadCount;
            int old2 = rs2.LoadCount;
            Thread.Sleep(RefreshJob.REFRESH_INTERVAL*3);
            Assert.That(rs1.LoadCount, Is.EqualTo(old1));
            Assert.That(rs2.LoadCount, Is.EqualTo(old2));
        }
    }
}
