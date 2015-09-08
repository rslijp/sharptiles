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
 */using System.Collections;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using org.SharpTiles.Connectors;
using org.SharpTiles.NUnit;

namespace org.SharpTiles.NUnit.Test
{
    [TestFixture]
    public class OutputConstraintTest
    {
        private IViewCache _cache;

        [SetUp]
        public void SetUp()
        {
            _cache = new TilesCache().GuardInit(Assembly.GetAssembly(typeof(OutputConstraintTest)));
        }

        [Test]
        public void TestEqualToOnAttribute()
        {
            Assert.That("a@body",
                        Output.
                            Is.EqualTo.
                            File("a.expected.body.html").
                            UsingModel(new Hashtable {{"Data", "TEST DATA"}}).From(_cache));
        }

        [Test]
        public void TestEqualToOnFullTile()
        {
            Assert.That("a",
                        Output.
                            Is.EqualTo.
                            File("a.expected.full.html").
                            UsingModel(new Hashtable { { "Data", "TEST DATA" } }).From(_cache));
        }

        [Test]
        public void TestEqualToOnStructureTile()
        {
            Assert.That("a",
                        Output.
                            Is.EqualTo.
                            File("a.expected.tile.html").
                            StubOutTiles().From(_cache));
        }

        [Test]
        public void TestLike()
        {
            Assert.That("a@body",
                        Output.
                            Is.Not.EqualTo.
                            File("a.expected.different.spacing.body.html").
                            UsingModel(new Hashtable { { "Data", "TEST DATA" } }).From(_cache)
                );
            Assert.That("a@body",
                        Output.
                            Is.Like.
                            File("a.expected.different.spacing.body.html").
                            UsingModel(new Hashtable { { "Data", "TEST DATA" } }).From(_cache));
        }

        [Test]
        public void TestSaveToFile()
        {
            var path = Path.GetRandomFileName();
            try
            {
                Assert.That(!File.Exists(path));
                Assert.That("a",
                            Output.
                                Is.EqualTo.
                                File("a.expected.full.html").
                                UsingModel(new Hashtable {{"Data", "TEST DATA"}}).
                                StoreResultInFile(path).From(_cache)
                    );
                Assert.That(File.Exists(path));
                Assert.That("a",
                            Output.
                                Is.EqualTo.
                                File(path).
                                UsingModel(new Hashtable { { "Data", "TEST DATA" } }).From(_cache));
            } finally
            {
                if(File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}
