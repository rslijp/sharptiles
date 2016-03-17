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
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Common;
using org.SharpTiles.Expressions;
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Tags.Test.CoreTags
{
    [TestFixture]
    public class CatchTest
    {
        [Test]
        public void CheckRequired()
        {
            var tag = new Catch();
            RequiredAttribute.Check(tag);
        }

        public class Broken
        {
            public object error { get; set; }
            public string Point
            {

                get
                {
                    new NullReferenceException("Smoke");
                    return null;
                }
            } 
        }

        [Test]
        public void TestCatchOfException()
        {
            var modelData = new Hashtable();
            modelData.Add("Existing", "Hi");
            modelData.Add("Model", new Hashtable());
            var tag = new Catch();
            tag.Var = new MockAttribute(new Constant("error"));
            tag.Body = new MockAttribute(new Property("Broken.Point"));
            var model = new TagModel(new Broken());
            Assert.That(tag.Evaluate(model), Is.EqualTo(String.Empty));
            Assert.That(((ReflectionException) model["Model.error"]).Message,
                        Text.StartsWith(ReflectionException.PropertyNotFound("Broken", typeof(Broken)).Message));
        }

        [Test]
        public void TestCatchOfExceptionDifferentPageScope()
        {
            var modelData = new Hashtable();
            modelData.Add("PageScope", VariableScope.Page.ToString());
            modelData.Add("Broken", new Broken());
            modelData.Add("Page", new Hashtable());
            var tag = new Catch();
            tag.Var = new MockAttribute(new Constant("error"));
            tag.Body = new MockAttribute(new Property("Broken.Banana"));
            tag.Scope = new MockAttribute(new Property("PageScope"));
            var model = new TagModel(modelData);
            Assert.That(tag.Evaluate(model), Is.EqualTo(String.Empty));
            Assert.That(model["Model.error"], Is.Null);
            Assert.That(((ReflectionException) model["Page.error"]).Message,
                         Text.StartsWith(ReflectionException.PropertyNotFound("Banana", typeof(Broken)).Message));
        }

        [Test]
        public void TestNoCatchOfException()
        {
            var modelData = new Hashtable();
            modelData.Add("Existing", "Hi");
            modelData.Add("Model", new Hashtable());
            var tag = new Catch();
            tag.Var = new MockAttribute(new Constant("error"));
            tag.Body = new MockAttribute(new Property("Existing"));
            var model = new TagModel(modelData);
            Assert.That(tag.Evaluate(model), Is.EqualTo("Hi"));
            Assert.That(model["Model.error"], Is.Null);
        }
    }
}
