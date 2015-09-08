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
using org.SharpTiles.Common;
using org.SharpTiles.Expressions;
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Tags.Test.CoreTags
{
    [TestFixture]
    public class OutTest
    {
        public string Null
        {
            get { return null; }
        }

        public string Value
        {
            get { return "<value>12345</value>"; }
        }

        public string Default
        {
            get { return "<default>12345</default>"; }
        }

        public string Body
        {
            get { return "<body>12345</body>"; }
        }

        public string False
        {
            get { return false.ToString(); }
        }

        public string True
        {
            get { return true.ToString(); }
        }

        [Test]
        public void CheckRequired()
        {
            var tag = new Out();
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void TestAttributesSetByProperty()
        {
            var tag = new Out();
            tag.Value = new MockAttribute(new Property("Value"));
            Assert.That(StringUtils.EscapeXml(Value), Is.EqualTo(tag.Evaluate(new TagModel(this))));
        }


        [Test]
        public void TestBodytOnlySet()
        {
            var tag = new Out();
            tag.Body = new MockAttribute(new Property("Body"));
            Assert.That(StringUtils.EscapeXml(Body), Is.EqualTo(tag.Evaluate(new TagModel(this))));
        }

        [Test]
        public void TestBodytOnlySetExplicitEscapingOfXml()
        {
            var tag = new Out();
            tag.Body = new MockAttribute(new Property("Body"));
            tag.EscapeXml = new MockAttribute(new Constant("True"));
            Assert.That(StringUtils.EscapeXml(Body), Is.EqualTo(tag.Evaluate(new TagModel(this))));
        }

        [Test]
        public void TestBodytOnlySetNoEscapingOfXml()
        {
            var tag = new Out();
            tag.Body = new MockAttribute(new Property("Body"));
            tag.EscapeXml = new MockAttribute(new Constant("False"));
            Assert.That(Body, Is.EqualTo(tag.Evaluate(new TagModel(this))));
        }

        [Test]
        public void TestDefaultOnlySet()
        {
            var tag = new Out();
            tag.Default = new MockAttribute(new Property("Default"));
            Assert.That(StringUtils.EscapeXml(Default), Is.EqualTo(tag.Evaluate(new TagModel(this))));
        }


        [Test]
        public void TestDefaultOnlySetExplicitEscapingOfXml()
        {
            var tag = new Out();
            tag.Default = new MockAttribute(new Property("Default"));
            tag.EscapeXml = new MockAttribute(new Constant("True"));
            Assert.That(StringUtils.EscapeXml(Default), Is.EqualTo(tag.Evaluate(new TagModel(this))));
        }

        [Test]
        public void TestDefaultOnlySetNoEscapingOfXml()
        {
            var tag = new Out();
            tag.Default = new MockAttribute(new Property("Default"));
            tag.EscapeXml = new MockAttribute(new Constant("False"));
            Assert.That(Default, Is.EqualTo(tag.Evaluate(new TagModel(this))));
        }

        [Test]
        public void TestReturnNullIfValueAndDefaultAndBodyAreNull()
        {
            var tag = new Out();
            tag.Value = new MockAttribute(new Property("Null"));
            tag.Default = new MockAttribute(new Property("Null"));
            tag.Body = new MockAttribute(new Property("Null"));
            Assert.That(StringUtils.EscapeXml(String.Empty), Is.EqualTo(tag.Evaluate(new TagModel(this))));
        }

        [Test]
        public void TestUseBodyIfDefaultIsNull()
        {
            var tag = new Out();
            tag.Default = new MockAttribute(new Property("Null"));
            tag.Body = new MockAttribute(new Property("Body"));
            Assert.That(StringUtils.EscapeXml(Body), Is.EqualTo(tag.Evaluate(new TagModel(this))));
        }


        [Test]
        public void TestUseBodyIfValueAndDefaultIsNull()
        {
            var tag = new Out();
            tag.Value = new MockAttribute(new Property("Null"));
            tag.Default = new MockAttribute(new Property("Null"));
            tag.Body = new MockAttribute(new Property("Body"));
            Assert.That(StringUtils.EscapeXml(Body), Is.EqualTo(tag.Evaluate(new TagModel(this))));
        }

        [Test]
        public void TestUseDefaultIfValueIsNull()
        {
            var tag = new Out();
            tag.Value = new MockAttribute(new Property("Null"));
            tag.Default = new MockAttribute(new Property("Default"));
            Assert.That(StringUtils.EscapeXml(Default), Is.EqualTo(tag.Evaluate(new TagModel(this))));
        }

        [Test]
        public void TestValueOnlySetExplicitEscapingOfXml()
        {
            var tag = new Out();
            tag.Value = new MockAttribute(new Property("Value"));
            tag.EscapeXml = new MockAttribute(new Constant("True"));
            Assert.That(StringUtils.EscapeXml(Value), Is.EqualTo(tag.Evaluate(new TagModel(this))));
        }

        [Test]
        public void TestValueOnlySetNoEscapingOfXml()
        {
            var tag = new Out();
            tag.Value = new MockAttribute(new Property("Value"));
            tag.EscapeXml = new MockAttribute(new Constant("False"));
            Assert.That(Value, Is.EqualTo(tag.Evaluate(new TagModel(this))));
        }
    }
}
