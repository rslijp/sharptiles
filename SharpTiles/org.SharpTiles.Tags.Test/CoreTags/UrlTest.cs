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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Expressions;
 using org.SharpTiles.NUnit;
 using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Tags.Test.CoreTags
{
    [TestFixture]
    public class UrlTest
    {
        [Test]
        public void CheckParamRequired()
        {
            var tag = new Param();
            try
            {
                RequiredAttribute.Check(tag);
                Assert.Fail("Expected exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message,
                            Is.EqualTo(TagException.MissingRequiredAttribute(typeof (Param), "Name").Message));
            }
            tag.Name = new MockAttribute(new Constant("p1"));
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void CheckUrlRequired()
        {
            var tag = new Url();
            try
            {
                RequiredAttribute.Check(tag);
                Assert.Fail("Expected exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message, Is.EqualTo(TagException.MissingRequiredAttribute(typeof (Url), "Value").Message));
            }
            tag.Value = new MockAttribute(new Constant("www.sharptiles.org"));
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void CheckUrlWithTildeRequired()
        {
            var url = new Url {Value = new MockAttribute(new Constant("~/info"))};
            var model = new TagModel(this, new MockSessionState(), null, new MockResponse { ApplicationPath = "/SharpTiles" });
            Assert.That(url.Evaluate(model), Is.EqualTo("/SharpTiles/info"));

        }

        [Test]
        public void CheckUrlWithTildeRequiredNoAppName()
        {
            var url = new Url { Value = new MockAttribute(new Constant("~/info")) };
            var model = new TagModel(this, new MockSessionState(), null, new MockResponse { ApplicationPath = "/" });
            //            CHANGE DOCUMENTATIE
            Assert.That(url.Evaluate(model), Is.EqualTo("/info"));

        }

        [Test]
        public void TestRenderingOfParamsUrlInVariable()
        {
            var url = new Url();
            url.Value = new MockAttribute(new Constant("www.sharptiles.org"));

            var param1 = new Param();
            param1.Name = new MockAttribute(new Constant("p1"));
            param1.Body = new MockAttribute(new Constant("v1"));

            var param2 = new Param();
            param2.Name = new MockAttribute(new Constant("p2"));
            param2.Body = new MockAttribute(new Constant("v2"));

            url.AddNestedTag(param1);
            url.AddNestedTag(param2);

            url.Var = new MockAttribute(new Constant("target"));

            var model = new TagModel(this);
            Assert.That(url.Evaluate(model), Is.EqualTo(String.Empty));
            Assert.That(model.Page["target"], Is.EqualTo("www.sharptiles.org?p1=v1&p2=v2"));
        }

        [Test]
        public void TestRenderingOfParamsUrlInVariableInDifferentScope()
        {
            var url = new Url();
            url.Value = new MockAttribute(new Constant("www.sharptiles.org"));

            var param1 = new Param();
            param1.Name = new MockAttribute(new Constant("p1"));
            param1.Body = new MockAttribute(new Constant("v1"));

            url.AddNestedTag(param1);

            url.Var = new MockAttribute(new Constant("target"));
            url.Scope = new MockAttribute(new Constant("Session"));
            var model = new TagModel(this, new MockSessionState());

            Assert.That(url.Evaluate(model), Is.EqualTo(String.Empty));
            Assert.That(model.Session["target"], Is.EqualTo("www.sharptiles.org?p1=v1"));
        }

        [Test]
        public void TestRenderingOfParamsUrlWithMultiParam()
        {
            var url = new Url();
            url.Value = new MockAttribute(new Constant("www.sharptiles.org"));

            var param1 = new Param();
            param1.Name = new MockAttribute(new Constant("p1"));
            param1.Body = new MockAttribute(new Constant("v1"));

            var param2 = new Param();
            param2.Name = new MockAttribute(new Constant("p2"));
            param2.Body = new MockAttribute(new Constant("v2"));

            var param3 = new Param();
            param3.Name = new MockAttribute(new Constant("p3"));
            param3.Body = new MockAttribute(new Constant("v3"));

            url.AddNestedTag(param1);
            url.AddNestedTag(param2);
            url.AddNestedTag(param3);
            Assert.That(url.Evaluate(new TagModel(this)), Is.EqualTo("www.sharptiles.org?p1=v1&p2=v2&p3=v3"));
        }

        [Test]
        public void TestRenderingOfParamsUrlWithOneParam()
        {
            var url = new Url();
            url.Value = new MockAttribute(new Constant("www.sharptiles.org"));

            var param = new Param();
            param.Name = new MockAttribute(new Constant("p1"));
            param.Body = new MockAttribute(new Constant("v1"));

            url.AddNestedTag(param);
            Assert.That(url.Evaluate(new TagModel(this)), Is.EqualTo("www.sharptiles.org?p1=v1"));
        }

        [Test]
        public void TestRenderingOfParamsUrlWithSpecialCharacterParams()
        {
            var url = new Url();
            url.Value = new MockAttribute(new Constant("www.sharptiles.org"));

            var param1 = new Param();
            param1.Name = new MockAttribute(new Constant("p1"));
            param1.Body = new MockAttribute(new Constant("v 1"));

            var param2 = new Param();
            param2.Name = new MockAttribute(new Constant("p2"));
            param2.Body = new MockAttribute(new Constant("v 2"));

            url.AddNestedTag(param1);
            url.AddNestedTag(param2);
            Assert.That(url.Evaluate(new TagModel(this)), Is.EqualTo("www.sharptiles.org?p1=v%201&p2=v%202"));
        }

        [Test]
        public void TestRenderingOfParamsUrlWithTwoParams()
        {
            var url = new Url();
            url.Value = new MockAttribute(new Constant("www.sharptiles.org"));

            var param1 = new Param();
            param1.Name = new MockAttribute(new Constant("p1"));
            param1.Body = new MockAttribute(new Constant("v1"));

            var param2 = new Param();
            param2.Name = new MockAttribute(new Constant("p2"));
            param2.Body = new MockAttribute(new Constant("v2"));

            url.AddNestedTag(param1);
            url.AddNestedTag(param2);
            Assert.That(url.Evaluate(new TagModel(this)), Is.EqualTo("www.sharptiles.org?p1=v1&p2=v2"));
        }

        [Test]
        public void TestRenderingOfUrl()
        {
            var url = new Url();
            url.Value = new MockAttribute(new Constant("www.sharptiles.org"));
            Assert.That(url.Evaluate(new TagModel(this)), Is.EqualTo("www.sharptiles.org"));
        }
    }
}
