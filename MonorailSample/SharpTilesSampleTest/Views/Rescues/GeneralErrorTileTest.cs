using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using org.SharpTiles.Connectors;
using org.SharpTiles.NUnit;
using SharpTilesSample.Controllers;

namespace SharpTilesSampleTest.Views.Rescues
{
    [TestFixture]
    public class GeneralErrorTileTest
    {
        private IViewCache _cache;

        [SetUp]
        public void SetUp()
        {
            _cache = new TilesCache().GuardInit(Assembly.GetAssembly(typeof(HomeController)));
        }

        [Test]
        public void TestEqualToOnFullTile()
        {
            Assert.That("rescues.generalerror",
                        Output.
                            Is.EqualTo.
                            File("Views/Rescues/generalerror.expected.tile.html").
                            From(_cache).
                            And().HavingApplicationPath("/SharpTilesSample").
                            UsingModel(new Hashtable {{"Exception", new Exception("Test exception message")}}));
        }
    }
}