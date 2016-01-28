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
 */using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace org.SharpTiles.Common.Test
{
    [TestFixture]
    public class NaturalLanguageTest
    {
        public class TestSubject
        {
            public int SimpleInt { get; set; }

            public string SimpleString { get; set; }

            public int? NullableInt { get; set; }


            public TestSubject Nested { get; set; }


            public IList<string> StringList { get; set; }

            public IList<TestSubject> ComplexList { get; set; }

            public string[] StringArray { get; set; }

            public TestSubject[] ComplexArray { get; set; }

            public IEnumerable<string> StringEnumerable { get; set; }

            public IEnumerable<TestSubject> ComplexEnumerable { get; set; }


            public IDictionary<string, string> StringDictionary { get; set; }

            public IDictionary<string, TestSubject> ComplexDictionary { get; set; }

        }

        [Test]
        public void TestGetsByIndexerShouldBeCamelCased()
        {
            var subject = new TestSubject();
            var reflection = new Reflection(subject).AdaptToNaturalLanguage();
            subject.ComplexArray = new TestSubject[3];
            subject.ComplexArray[0] = new TestSubject();
            subject.ComplexArray[1] = new TestSubject();
            subject.ComplexArray[2] = new TestSubject();
            subject.ComplexArray[0].SimpleString = "first";
            subject.ComplexArray[1].SimpleString = "second";
            subject.ComplexArray[2].SimpleString = "third";

            Assert.That(reflection["complex array.0.simple string"], Is.EqualTo("first"));
            Assert.That(reflection["complex array.1.simple string"], Is.EqualTo("second"));
            Assert.That(reflection["complex array.2.simple string"], Is.EqualTo("third"));
        }

        [Test]
        public void TestSetsByIndexerShouldBeCamelCased()
        {
            var subject = new TestSubject();
            var reflection = new Reflection(subject).AdaptToNaturalLanguage();
            subject.ComplexArray = new TestSubject[3];
            subject.ComplexArray[0] = new TestSubject();
            subject.ComplexArray[1] = new TestSubject();
            subject.ComplexArray[2] = new TestSubject();
            subject.ComplexArray[0].SimpleString = "first";
            subject.ComplexArray[1].SimpleString = "second";
            subject.ComplexArray[2].SimpleString = "third";

            reflection["complex array.0.simple string"] = "1 CHANGED";
            reflection["complex array.1.simple string"] = "2 CHANGED";
            reflection["complex array.2.simple string"] = "3 CHANGED";

            Assert.That(subject.ComplexArray[0].SimpleString, Is.EqualTo("1 CHANGED"));
            Assert.That(subject.ComplexArray[1].SimpleString, Is.EqualTo("2 CHANGED"));
            Assert.That(subject.ComplexArray[2].SimpleString, Is.EqualTo("3 CHANGED"));
        }

        [Test]
        public void TestGetsShouldBeCamelCased()
        {
            var subject = new TestSubject();
            var reflection = new Reflection(subject).AdaptToNaturalLanguage();
            subject.ComplexArray = new TestSubject[3];
            subject.ComplexArray[0] = new TestSubject();
            subject.ComplexArray[1] = new TestSubject();
            subject.ComplexArray[2] = new TestSubject();
            subject.ComplexArray[0].SimpleString = "first";
            subject.ComplexArray[1].SimpleString = "second";
            subject.ComplexArray[2].SimpleString = "third";

            Assert.That(reflection.Get("complex array.0.simple string").Result, Is.EqualTo("first"));
            Assert.That(reflection.Get("complex array.1.simple string").Result, Is.EqualTo("second"));
            Assert.That(reflection.Get("complex array.2.simple string").Result, Is.EqualTo("third"));
        }

        [Test]
        public void TestTryGetsShouldBeCamelCased()
        {
            var subject = new TestSubject();
            var reflection = new Reflection(subject).AdaptToNaturalLanguage();
            subject.ComplexArray = new TestSubject[3];
            subject.ComplexArray[0] = new TestSubject();
            subject.ComplexArray[1] = new TestSubject();
            subject.ComplexArray[2] = new TestSubject();
            subject.ComplexArray[0].SimpleString = "first";
            subject.ComplexArray[1].SimpleString = "second";
            subject.ComplexArray[2].SimpleString = "third";

            Assert.That(reflection.TryGet("complex array.0.simple string"), Is.EqualTo("first"));
            Assert.That(reflection.TryGet("complex array.1.simple string"), Is.EqualTo("second"));
            Assert.That(reflection.TryGet("complex array.2.simple string"), Is.EqualTo("third"));
        }
    }
}
