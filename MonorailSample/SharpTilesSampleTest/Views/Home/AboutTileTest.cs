using System.Collections;
using System.Reflection;
using NUnit.Framework;
using org.SharpTiles.Connectors;
using org.SharpTiles.NUnit;
using SharpTilesSample.Controllers;

namespace SharpTilesSampleTest.Views.Home
{
    [TestFixture]
    public class AboutTileTest
    {
        private IViewCache _cache;

        [SetUp]
        public void SetUp()
        {
            _cache = new TilesCache().GuardInit(Assembly.GetAssembly(typeof(HomeController)));
        }

        [Test]
        public void TestEqualToOnAttribute()
        {
            Assert.That("home.About@body",
                        Output.
                            Is.EqualTo.
                            File("Views/Home/about.expected.body.html").
                            From(_cache).
                            UsingModel(new Hashtable {{"Title", "TEST TITLE"}}));
        }

        [Test]
        public void TestEqualToOnFullTile()
        {
            Assert.That("home.About",
                        Output.
                            Is.EqualTo.
                            File("Views/Home/about.expected.full.html").
                            From(_cache).
                            And().HavingApplicationPath("/SharpTilesSample").
                            UsingModel(new Hashtable {{"Title", "TEST TITLE"}}));
        }

        [Test]
        public void TestEqualToOnStructureTile()
        {
            Assert.That("home.About",
                        Output.
                            Is.EqualTo.
                            File("Views/Home/about.expected.tile.html").
                            From(_cache).
                            And().
                            HavingApplicationPath("/SharpTilesSample").
                            UsingModel(new Hashtable {{"Title", "TEST TITLE"}}).
                            StubOutTiles());
        }

        [Test]
        public void TestLike()
        {
            Assert.That("home.About@body",
                        Output.
                            Is.Not.EqualTo.
                            File("Views/Home/about.expected.different.spacing.body.html").
                            From(_cache).
                            UsingModel(new Hashtable {{"Title", "TEST TITLE"}}));

            Assert.That("home.About@body",
                        Output.
                            Is.Like.
                            File("Views/Home/about.expected.different.spacing.body.html").
                            From(_cache).
                            UsingModel(new Hashtable {{"Title", "TEST TITLE"}}));
        }
    }
}