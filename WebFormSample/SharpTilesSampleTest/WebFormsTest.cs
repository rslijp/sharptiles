using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core;

namespace SharpTilesSampleTest
{
    [TestFixture]
    public class WebFormsTest
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
        public void TestTileRendered()
        {
            _ie.GoTo(_webServer.BaseUrl + "default.aspx");
            Assert.That(_ie.Url.EndsWith("default.aspx"), "Should open with default.aspx");
            Assert.That(_ie.Element(Find.ById("paragraph1")).InnerHtml, Is.EqualTo("This is a paragraph"));
            Assert.That(_ie.Element(Find.ById("paragraph2")).InnerHtml, Is.EqualTo("Some text"));
        }


        [Test]
        public void TestRenderReUse()
        {
            BenchMark("default.aspx");
        }

        public void BenchMark(string url)
        {
            BenchMarkHelper helper = new BenchMarkHelper(_ie, _webServer.BaseUrl);
            helper.TestWithWatin(url);
            helper.TestPlain(url);
        }
    }
}