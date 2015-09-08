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
using org.SharpTiles.Expressions;
 using org.SharpTiles.NUnit;
 using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Tags.Test.CoreTags
{
    [TestFixture]
    public class RedirectTest
    {
        [Test]
        public void CheckUrlRequired()
        {
            var tag = new Redirect();
            try
            {
                RequiredAttribute.Check(tag);
                Assert.Fail("Expected exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message,
                            Is.EqualTo(TagException.MissingRequiredAttribute(typeof (Redirect), "Url").Message));
            }
            tag.Url = new MockAttribute(new Constant("www.sharptiles.org"));
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void TestRedirectNoRepsonseSet()
        {
            var url = new Redirect();
            url.Url = new MockAttribute(new Constant("www.sharptiles.org"));
            try
            {
                url.Evaluate(new TagModel(this));
            }
            catch (TagException Te)
            {
                Assert.AreEqual(Te.Message, TagException.HttpResponseNotAvailable().Message);
            }
        }

        [Test]
        public void TestRedirectOnMock()
        {
            var url = new Redirect();
            var response = new MockResponse();
            var model = new TagModel(this, new MockSessionState(), null, response, null);
            url.Url = new MockAttribute(new Constant("www.sharptiles.org"));
            Assert.IsNull(response.LastRedirectUrl);
            url.Evaluate(model);
            Assert.That(response.LastRedirectUrl, Is.EqualTo("www.sharptiles.org"));
        }
    }
}
