using System;
using NUnit.Framework;
using WatiN.Core;

namespace SharpTilesSampleTest
{
    [TestFixture]
    public class MVCTest
    {
        private IE _ie = null;
        private DevWebServer _webServer;

        [SetUp]
        public virtual void SetUp()
        {
            IE.Settings.AutoMoveMousePointerToTopLeft = false;

            _webServer = new DevWebServer();
            _webServer.Start();
            _ie = new IE(_webServer.BaseUrl, true);
        }

        [TearDown]
        public virtual void TearDown()
        {
            _webServer.Stop();
            _ie.Close();
        }

        [Test]
        public void TestFollowLink()
        {
            _ie.GoTo(_webServer.BaseUrl + "Home/Index");
            Assert.That(_ie.Url.EndsWith("Home/Index"), "Should open with index");
            _ie.Link(Find.ById("link_to_about")).Click();
            Assert.That(_ie.Url.EndsWith("Home/About"), "Expected change in url");
            Assert.That(_ie.ContainsText("About Page"), "Site should contain 'About page'");
        }

        [Test]
        public void TestRenderReUse()
        {
            BenchMark("Home/Index");
        }

        public void BenchMark(string url)
        {
            BenchMarkHelper helper = new BenchMarkHelper(_ie, _webServer.BaseUrl);
            helper.TestWithWatin(url);
            helper.TestPlain(url);
        }
    }
}