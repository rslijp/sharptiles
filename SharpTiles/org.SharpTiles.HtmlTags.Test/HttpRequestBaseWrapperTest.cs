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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Connectors;
using org.SharpTiles.Tags;

namespace org.SharpTiles.HtmlTags.Test
{
    [TestFixture]
    public class HttpRequestBaseWrapperTest
    {
        [Test]
        public void Test_IsAuthenticated()
        {
            var request = new MockContextBase.MockRequestBase();
            var requestBase = new HttpRequestBaseWrapper(request);
            Assert.That(requestBase["IsAuthenticated"], Is.False);
            request.ManualAuthenticated = true;
            Assert.That(requestBase["IsAuthenticated"], Is.True);
        }

        [Test]
        public void Test_IsAuthenticated_Through_TagModel()
        {
            var request = new MockContextBase.MockRequestBase();
            var requestBase = new HttpRequestBaseWrapper(request);
            var model = new TagModel(null, null, requestBase, null, null);
            Assert.That(model["Request.IsAuthenticated"], Is.False);
            request.ManualAuthenticated = true;
            Assert.That(model["Request.IsAuthenticated"], Is.True);
        }

        [Test]
        public void Test_Params()
        {
            var request = new MockContextBase.MockRequestBase();
            request.Params["X"] = "1";
            var requestBase = new HttpRequestBaseWrapper(request);
            Assert.That(requestBase["X"], Is.EqualTo("1"));
            request.Params["X"] = "2";
            Assert.That(requestBase["X"], Is.EqualTo("2"));
        }

        [Test]
        public void Test_Params_Through_TagModel()
        {
            var request = new MockContextBase.MockRequestBase();
            var requestBase = new HttpRequestBaseWrapper(request);
            var model = new TagModel(null, null, requestBase, null, null);
            request.Params["X"] = "1";
            Assert.That(model["Request.X"], Is.EqualTo("1"));
            request.Params["X"] = "2";
            Assert.That(model["Request.X"], Is.EqualTo("2"));
        }
    }
}