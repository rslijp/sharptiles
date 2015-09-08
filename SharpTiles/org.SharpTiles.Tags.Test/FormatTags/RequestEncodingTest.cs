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
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Expressions;
 using org.SharpTiles.NUnit;
 using org.SharpTiles.Tags.FormatTags;

namespace org.SharpTiles.Tags.Test.FormatTags
{
    [TestFixture]
    public class RequestEncodingTest
    {
        [Test]
        public void CheckUrlRequired()
        {
            var tag = new RequestEncoding();
            try
            {
                RequiredAttribute.Check(tag);
                Assert.Fail("Expected exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message,
                            Is.EqualTo(TagException.MissingRequiredAttribute(typeof (RequestEncoding), "Value").Message));
            }
            tag.Value = new MockAttribute(new Constant(Encoding.ASCII.ToString()));
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void TestRequestEncodingNoRepsonseSet()
        {
            var tag = new RequestEncoding();
            tag.Value = new MockAttribute(new Constant("UTF-8"));
            var model = new TagModel(this);
            Assert.That(tag.Evaluate(model), Is.EqualTo(String.Empty));
            Assert.That(model.Encoding, Is.EqualTo(Encoding.UTF8));
        }

        [Test]
        public void TestResponseOnMock()
        {
            var tag = new RequestEncoding();
            var response = new MockResponse();
            var model = new TagModel(this, new MockSessionState(), null, response, null);
            tag.Value = new MockAttribute(new Constant("UTF-7"));
            Assert.IsNull(response.ResponseEncoding);
            tag.Evaluate(model);
            Assert.That(response.ResponseEncoding, Is.EqualTo(Encoding.UTF7));
        }
    }
}
