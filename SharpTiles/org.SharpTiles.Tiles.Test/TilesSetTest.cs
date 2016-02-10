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
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.Templates.SharpTags;
using org.SharpTiles.Templates;
using org.SharpTiles.Templates.Templates;
using org.SharpTiles.Tiles.Configuration;
using org.SharpTiles.Tiles.Test.Configuration;
using org.SharpTiles.Tiles.Tile;

namespace org.SharpTiles.Tiles.Test
{
    [TestFixture]
    public class TilesSetTest
    {

        private ITagLib _lib;
        private FileLocatorFactory _factory;

        [SetUp]
        public void SetUp()
        {
            _lib = new TagLib();
            _lib.Register(new Tags.Tiles());
            _lib.Register(new Sharp());
            _factory = new FileLocatorFactory().CloneForTagLib(_lib) as FileLocatorFactory;
        }

        [Test]
        public void TestCache()
        {
            var ts = new TilesSet();
            ITile a = new TemplateTile("a", _factory.Handle("a.htm",true), new List<TileAttribute>());
            ITile b = new TemplateTile("b", _factory.Handle("b.htm",true), new List<TileAttribute>());

            ts.Map.AddTile(a);
            ts.Map.AddTile(b);

            Assert.That(ts["a"], Is.SameAs(a));
            Assert.That(ts["b"], Is.SameAs(b));
        }

        [Test]
        public void TestRefresh()
        {
            string tempTile = Path.GetTempFileName();
            File.Copy("a.htm", tempTile, true);
            try
            {
                var config = new MockConfiguration(tempTile, new DateTime(2007, 1, 1)) {Factory = _factory };
                var ts = new TilesSet(config);

                Assert.That(RequiresRefresh(ts, "a"), Is.False);
                Assert.That(RequiresRefresh(ts, "b"), Is.False);

                File.SetLastWriteTime(tempTile, DateTime.Now.AddDays(-1));
                config.ConfigurationLastModified = config.ConfigurationLastModified.Value.AddDays(1);

                Assert.That(RequiresRefresh(ts, "a"), Is.True);
                Assert.That(RequiresRefresh(ts, "b"), Is.False);

                ts.Refresh();

                Assert.That(RequiresRefresh(ts, "a"), Is.False);
                Assert.That(RequiresRefresh(ts, "b"), Is.False);
            }
            finally
            {
                File.Delete(tempTile);
            }
        }

        [Test]
        public void TestRefreshConfig()
        {
            var config = new MockConfiguration(new DateTime(2007, 1, 1)) { Factory = _factory };
            var ts = new TilesSet(config);

            Assert.That(ts.Contains("a"));
            Assert.That(ts.Contains("b"));
            Assert.That(!ts.Contains("c"));

            config.RefreshAndChange(new DateTime(2008, 1, 1));
            ts.Refresh();

            Assert.That(ts.Contains("a"));
            Assert.That(ts.Contains("b"));
            Assert.That(ts.Contains("c"));
        }


        [Test]
        public void TestRefreshConfigByFile()
        {
            string tempTile = Path.GetTempFileName();
            File.Copy("Configuration\\tiles.smallconfig.xml", tempTile, true);
            File.SetLastWriteTime(tempTile, DateTime.Now.AddDays(-1));
            try
            {

                var config = new TileXmlConfigurator(_lib, tempTile);
                var ts = new TilesSet(config);

                Assert.That(ts.Contains("a"));
                Assert.That(ts.Contains("b"));
                Assert.That(!ts.Contains("c"));

                File.Copy("Configuration\\tiles.config.xml", tempTile, true);
                ts.Refresh();

                Assert.That(ts.Contains("a"));
                Assert.That(ts.Contains("b"));
                Assert.That(ts.Contains("c"));
            }
            finally
            {
                File.Delete(tempTile);
            }
        }

        private static bool RequiresRefresh(TilesSet ts, String name)
        {
            var templateTile = (TemplateTile)ts[name];
            var fileTemlpate = (FileTemplate)templateTile.Template;
            return fileTemlpate.RequiresRefresh();
        }

        [Test]
        public void TestSetTiles()
        {
            var set = new TilesSet();
            IList<ITile> list = new List<ITile>();
            ITile a = new TemplateTile("a", _factory.Handle("a.htm",true), new List<TileAttribute>());
            ITile b = new TemplateTile("b", _factory.Handle("b.htm",true), new List<TileAttribute>());
            ITile c = new TemplateTile("c", _factory.Handle("c.htm",true), new List<TileAttribute>());
            list.Add(b);
            list.Add(c);

            set.Map.AddTile(a);
            Assert.IsTrue(set.Contains("a"));
            Assert.IsFalse(set.Contains("b"));
            Assert.IsFalse(set.Contains("c"));
            set.SetTiles(list);
            Assert.IsFalse(set.Contains("a"));
            Assert.IsTrue(set.Contains("b"));
            Assert.IsTrue(set.Contains("c"));
        }

        [Test]
        public void TestUnkownTile()
        {
            var ts = new TilesSet();
            try
            {
                var x = ts["?"];
                Assert.Fail("Expected exception on " + x);
            }
            catch (TemplateException Te)
            {
                Assert.That(Te.Message, Is.EqualTo(TemplateException.TemplateNotFound("?").Message));
            }
        }
    }
}
