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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace org.SharpTiles.Common.Test
{
    [TestFixture]
    public class ReflectionTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _subject = new TestSubject();
            _reflection = new Reflection(_subject);
        }

        #endregion

        private TestSubject _subject;
        private Reflection _reflection;
        private readonly Hashtable _model = new Hashtable();

        public Hashtable Model
        {
            get { return _model; }
        }


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

            public Proxy Proxy { get; set; }
        }

        public class EnumerableWrapper<T> : IEnumerable<T>
        {
            private readonly IList<T> _list;

            public EnumerableWrapper(IList<T> list)
            {
                _list = list;
            }

            #region IEnumerable<T> Members

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return _list.GetEnumerator();
            }

            public IEnumerator GetEnumerator()
            {
                return _list.GetEnumerator();
            }

            #endregion
        }

        public class Proxy
        {
            private readonly TestSubject _source;


            public Proxy(TestSubject source)
            {
                _source = source;
            }


            public TestSubject Source
            {
                get { return _source; }
            }
        }

        [Test]
        public void HandlingOfNullValueAtEndWithHashtable()
        {
            var model = new Hashtable();
            var reflection = new Reflection(model);
            model.Add("Model", new Hashtable());
            Assert.IsNull(reflection["Model.text"]);
        }


        [Test]
        public void HandlingOfNullValueAtEndWithProperty()
        {
            var reflection = new Reflection(this);
            Assert.IsNull(reflection["Model.newtext"]);
        }


        [Test]
        public void HandlingOfSetOfNullValueAtEndWithHashtable()
        {
            var model = new Hashtable();
            var reflection = new Reflection(model);
            model.Add("Model", new Hashtable());
            reflection["Model.text"] = "abc";
            Assert.That(((Hashtable) model["Model"])["text"], Is.EqualTo("abc"));
        }

        [Test]
        public void HandlingOfSetOfNullValueAtEndWithProperty()
        {
            var reflection = new Reflection(this);
            reflection["Model.newtext"] = "bla";
            Assert.That(Model["newtext"], Is.EqualTo("bla"));
        }

        [Test]
        public void TestArrayComplexProperty()
        {
            _subject.ComplexArray = new TestSubject[3];
            _subject.ComplexArray[0] = new TestSubject();
            _subject.ComplexArray[1] = new TestSubject();
            _subject.ComplexArray[2] = new TestSubject();
            _subject.ComplexArray[0].SimpleString = "first";
            _subject.ComplexArray[1].SimpleString = "second";
            _subject.ComplexArray[2].SimpleString = "third";

            Assert.That(_reflection["ComplexArray.0.SimpleString"], Is.EqualTo("first"));
            Assert.That(_reflection["ComplexArray.1.SimpleString"], Is.EqualTo("second"));
            Assert.That(_reflection["ComplexArray.2.SimpleString"], Is.EqualTo("third"));
        }

        [Test]
        public void TestArrayProperty()
        {
            _subject.StringArray = new string[3];
            _subject.StringArray[0] = "first";
            _subject.StringArray[1] = "second";
            _subject.StringArray[2] = "third";

            Assert.That(_reflection["StringArray.0"], Is.EqualTo("first"));
            Assert.That(_reflection["StringArray.1"], Is.EqualTo("second"));
            Assert.That(_reflection["StringArray.2"], Is.EqualTo("third"));
        }

        [Test]
        public void TestArrayPropertyOutOfRangeToLarge()
        {
            _subject.StringArray = new string[3];
            _subject.StringArray[0] = "first";
            _subject.StringArray[1] = "second";
            _subject.StringArray[2] = "third";

            try
            {
                object value = _reflection["StringArray.4"];
            }
            catch (ReflectionException Re)
            {
                Assert.That(Re.Message, Is.EqualTo(ReflectionException.IndexOutOfBounds(typeof (string[]), 4).Message));
            }
            try
            {
                object value = _reflection["StringArray.-1"];
            }
            catch (ReflectionException Re)
            {
                Assert.That(Re.Message, Is.EqualTo(ReflectionException.IndexOutOfBounds(typeof (string[]), -1).Message));
            }
        }

        [Test]
        public void TestDictionaryComplexProperty()
        {
            _subject.ComplexDictionary = new Dictionary<string, TestSubject>();
            _subject.ComplexDictionary.Add("a", new TestSubject());
            _subject.ComplexDictionary.Add("b", new TestSubject());
            _subject.ComplexDictionary.Add("c", new TestSubject());
            _subject.ComplexDictionary["a"].SimpleString = "first";
            _subject.ComplexDictionary["b"].SimpleString = "second";
            _subject.ComplexDictionary["c"].SimpleString = "third";

            Assert.That(_reflection["ComplexDictionary.a.SimpleString"], Is.EqualTo("first"));
            Assert.That(_reflection["ComplexDictionary.b.SimpleString"], Is.EqualTo("second"));
            Assert.That(_reflection["ComplexDictionary.c.SimpleString"], Is.EqualTo("third"));
        }

        [Test]
        public void TestDictionaryProperty()
        {
            _subject.StringDictionary = new Dictionary<string, string>();
            _subject.StringDictionary.Add("a", "first");
            _subject.StringDictionary.Add("b", "second");
            _subject.StringDictionary.Add("c", "third");

            Assert.That(_reflection["StringDictionary.a"], Is.EqualTo("first"));
            Assert.That(_reflection["StringDictionary.b"], Is.EqualTo("second"));
            Assert.That(_reflection["StringDictionary.c"], Is.EqualTo("third"));
        }

        [Test]
        public void TestEnumerableComplexProperty()
        {
            var list = new List<TestSubject>();
            list.Add(new TestSubject());
            list.Add(new TestSubject());
            list.Add(new TestSubject());
            list[0].SimpleString = "first";
            list[1].SimpleString = "second";
            list[2].SimpleString = "third";
            _subject.ComplexEnumerable = new EnumerableWrapper<TestSubject>(list);

            Assert.That(_reflection["ComplexEnumerable.0.SimpleString"], Is.EqualTo("first"));
            Assert.That(_reflection["ComplexEnumerable.1.SimpleString"], Is.EqualTo("second"));
            Assert.That(_reflection["ComplexEnumerable.2.SimpleString"], Is.EqualTo("third"));
        }

        [Test]
        public void TestEnumerableProperty()
        {
            var list = new List<string>();
            list.Add("first");
            list.Add("second");
            list.Add("third");
            _subject.StringEnumerable = new EnumerableWrapper<string>(list);

            Assert.That(_reflection["StringEnumerable.0"], Is.EqualTo("first"));
            Assert.That(_reflection["StringEnumerable.1"], Is.EqualTo("second"));
            Assert.That(_reflection["StringEnumerable.2"], Is.EqualTo("third"));
        }

        [Test]
        public void TestEnumerablePropertyOutOfRangeToLarge()
        {
            var list = new List<string>();
            list.Add("first");
            list.Add("second");
            list.Add("third");
            _subject.StringEnumerable = new EnumerableWrapper<string>(list);

            try
            {
                object value = _reflection["StringEnumerable.4"];
            }
            catch (ReflectionException Re)
            {
                Assert.That(Re.Message,
                            Is.EqualTo(
                                ReflectionException.IndexOutOfBounds(typeof (EnumerableWrapper<string>), 4).Message));
            }
            try
            {
                object value = _reflection["StringEnumerable.-1"];
            }
            catch (ReflectionException Re)
            {
                Assert.That(Re.Message,
                            Is.EqualTo(ReflectionException.IndexOutOfBounds(typeof (List<string>), -1).Message));
            }
        }

        [Test]
        public void TestListComplexProperty()
        {
            _subject.ComplexList = new List<TestSubject>();
            _subject.ComplexList.Add(new TestSubject());
            _subject.ComplexList.Add(new TestSubject());
            _subject.ComplexList.Add(new TestSubject());
            _subject.ComplexList[0].SimpleString = "first";
            _subject.ComplexList[1].SimpleString = "second";
            _subject.ComplexList[2].SimpleString = "third";

            Assert.That(_reflection["ComplexList.0.SimpleString"], Is.EqualTo("first"));
            Assert.That(_reflection["ComplexList.1.SimpleString"], Is.EqualTo("second"));
            Assert.That(_reflection["ComplexList.2.SimpleString"], Is.EqualTo("third"));
        }

        [Test]
        public void TestListProperty()
        {
            _subject.Nested = new TestSubject();
            _subject.StringList = new List<string>();
            _subject.StringList.Add("first");
            _subject.StringList.Add("second");
            _subject.StringList.Add("third");
            Assert.That(_reflection["StringList.0"], Is.EqualTo("first"));
            Assert.That(_reflection["StringList.1"], Is.EqualTo("second"));
            Assert.That(_reflection["StringList.2"], Is.EqualTo("third"));
        }

        [Test]
        public void DefaultNesting_For_Get_Should_Be_0()
        {
            _subject.Nested = new TestSubject();
            try
            {
                Console.WriteLine(_reflection["Wrong"]);
                Assert.Fail("Expected exception");
            } catch (ReflectionException Re)
            {
                Assert.That(Re.Nesting, Is.EqualTo(0));
            }
        }

        [Test]
        public void DefaultNesting_For_Set_Should_Be_0()
        {
            _subject.Nested = new TestSubject();
            try
            {
                _reflection["Wrong"]=null;
                Assert.Fail("Expected exception");
            }
            catch (ReflectionException Re)
            {
                Assert.That(Re.Nesting, Is.EqualTo(0));
            }
        }

        [Test]
        public void DefaultNesting_For_Get_Should_Increase_With_Nesting()
        {
            _subject.Nested = new TestSubject();
            try
            {
                Console.WriteLine(_reflection["Nested.Wrong"]);
                Assert.Fail("Expected exception");
            }
            catch (ReflectionException Re)
            {
                Assert.That(Re.Nesting, Is.EqualTo(1));
            }
        }

        [Test]
        public void DefaultNesting_For_Set_Should_Increase_With_Nesting()
        {
            _subject.Nested = new TestSubject();
            try
            {
                _reflection["Nested.Wrong"] = null;
                Assert.Fail("Expected exception");
            }
            catch (ReflectionException Re)
            {
                Assert.That(Re.Nesting, Is.EqualTo(1));
            }
        }

        [Test]
        public void DefaultNesting_For_Get_Should_Increase_With_Nesting_In_Lists()
        {
            _subject.ComplexList = new List<TestSubject> {new TestSubject()};
            try
            {
                Console.WriteLine(_reflection["ComplexList.0.Wrong"]);
                Assert.Fail("Expected exception");
            }
            catch (ReflectionException Re)
            {
                Assert.That(Re.Nesting, Is.EqualTo(2));
            }
        }

        [Test]
        public void DefaultNesting_For_Set_Should_Increase_With_Nesting_In_Lists()
        {
            _subject.ComplexList = new List<TestSubject> { new TestSubject() };
            try
            {
                _reflection["ComplexList.0.Wrong"] = null;
                Assert.Fail("Expected exception");
            }
            catch (ReflectionException Re)
            {
                Assert.That(Re.Nesting, Is.EqualTo(2));
            }

        }

        [Test]
        public void DefaultNesting_For_Get_Should_Increase_With_Multi_Nesting()
        {
            _subject.ComplexList = new List<TestSubject> { new TestSubject { Nested = new TestSubject() } };
            try
            {
                Console.WriteLine(_reflection["ComplexList.0.Nested.Wrong"]);
                Assert.Fail("Expected exception");
            }
            catch (ReflectionException Re)
            {
                Console.WriteLine(Re.Message);
                Assert.That(Re.Nesting, Is.EqualTo(3));
            }
        }

        [Test]
        public void DefaultNesting_For_Set_Should_Increase_With_Multi_Nesting()
        {
            _subject.ComplexList = new List<TestSubject> { new TestSubject { Nested = new TestSubject() } };
            try
            {
                _reflection["ComplexList.0.Nested.Wrong"] = null;
                Assert.Fail("Expected exception");
            }
            catch (ReflectionException Re)
            {
                Assert.That(Re.Nesting, Is.EqualTo(3));
            }

        }

        [Test]
        public void TestListPropertyOutOfRangeToLarge()
        {
            _subject.Nested = new TestSubject();
            _subject.StringList = new List<string>();
            _subject.StringList.Add("first");
            _subject.StringList.Add("second");
            _subject.StringList.Add("third");
            try
            {
                object value = _reflection["StringList.4"];
            }
            catch (ReflectionException Re)
            {
                Assert.That(Re.Message,
                            Is.EqualTo(ReflectionException.IndexOutOfBounds(typeof (List<string>), 4).Message));
            }
            try
            {
                object value = _reflection["StringList.-1"];
            }
            catch (ReflectionException Re)
            {
                Assert.That(Re.Message,
                            Is.EqualTo(ReflectionException.IndexOutOfBounds(typeof (List<string>), -1).Message));
            }
        }

        [Test]
        public void TestNestedIntExistingProperty()
        {
            _subject.Nested = new TestSubject();
            _subject.Nested.SimpleInt = 42;
            Assert.That(_reflection["Nested.SimpleInt"], Is.EqualTo(42));
        }

        [Test]
        public void TestNestedStringExistingProperty()
        {
            _subject.Nested = new TestSubject();
            _subject.Nested.SimpleString = "abc";
            Assert.That(_reflection["Nested.SimpleString"], Is.EqualTo("abc"));
        }

        [Test]
        public void TestNonExistingNestedProperty()
        {
            try
            {
                _subject.Nested = new TestSubject();
                object result = _reflection["Nested.NonExistingProperty"];
                Assert.Fail("Expected an exception");
            }
            catch (ReflectionException Re)
            {
                ReflectionException test =
                    ReflectionException.PropertyNotFound("NonExistingProperty", typeof (TestSubject));
                Assert.That(Re.Message, Is.EqualTo(test.Message));
            }
        }


        [Test]
        public void TestNonExistingProperty()
        {
            try
            {
                object result = _reflection["NonExistingProperty"];
                Assert.Fail("Expected an exception");
            }
            catch (ReflectionException Re)
            {
                ReflectionException test =
                    ReflectionException.PropertyNotFound("NonExistingProperty", typeof (TestSubject));
                Assert.That(Re.Message, Is.EqualTo(test.Message));
            }
        }

        [Test]
        public void TestNoProperty()
        {
            try
            {
                object result = _reflection[null];
                Assert.Fail("Expected an exception");
            }
            catch (ReflectionException Re)
            {
                ReflectionException test = ReflectionException.NoPropertyAvailable();
                Assert.That(Re.Message, Is.EqualTo(test.Message));
            }
        }

        [Test]
        public void SET_TestNoSourceProvided()
        {
            try
            {
                _reflection = new Reflection(null);
                _reflection["SimpleString"]="a";
                Assert.Fail("Expected an exception");
            }
            catch (ReflectionException Re)
            {
                ReflectionException test = ReflectionException.NoSourceAvailable("SimpleString");
                Assert.That(Re.Message, Is.EqualTo(test.Message));
            }
        }
        [Test]
        public void GET_TestNoSourceProvided()
        {
            _reflection = new Reflection(null);
            Assert.That(_reflection["SimpleString"], Is.Null);
        }

        [Test]
        public void SET_TestNoSourceProvidedNested()
        {
            try
            {
                _reflection = new Reflection(_subject);
               _reflection["Nested.SimpleString"]="a";
                Assert.Fail("Expected an exception");
            }
            catch (ReflectionException Re)
            {
                ReflectionException test = ReflectionException.NoSourceAvailable("SimpleString");
                Assert.That(Re.Message, Is.EqualTo(test.Message));
            }
        }

        [Test]
        public void GET_TestNoSourceProvidedNested()
        {
            _reflection = new Reflection(_subject);
            Assert.That(_reflection["Nested.SimpleString"], Is.Null);
        }


        [Test]
        public void TestNullableIntExistingNestedProperty()
        {
            _subject.Nested = new TestSubject();
            _subject.Nested.NullableInt = 42;
            Assert.That(_reflection["Nested.NullableInt"], Is.EqualTo(42));
        }

        [Test]
        public void TestNullableIntExistingNestedPropertyButValueIsNull()
        {
            _subject.Nested = new TestSubject();
            _subject.Nested.NullableInt = null;
            Assert.That(_reflection["Nested.NullableInt"], Is.Null);
        }

        [Test]
        public void ReflectionShouldBeNullSafeWithNestedNullValues()
        {
            _subject.Nested = null;
            Assert.That(_reflection["Nested.NullableInt"], Is.Null);
        }

        [Test]
        public void TestNullableIntExistingProperty()
        {
            _subject.NullableInt = 42;
            Assert.That(_reflection["NullableInt"], Is.EqualTo(42));
        }

        [Test]
        public void TestNullableIntExistingPropertyButValueIsNull()
        {
            _subject.NullableInt = null;
            Assert.That(_reflection["NullableInt"], Is.Null);
        }

        [Test]
        public void TestNullReferenceArrayProperty()
        {
            _subject.StringArray = null;
            try
            {
                _reflection["StringArray.1"] = "first";
            }
            catch (ReflectionException Re)
            {
                ReflectionException test = ReflectionException.NoSourceAvailable("1");
                Assert.That(Re.Message, Is.EqualTo(test.Message));
            }
        }

        [Test]
        public void GET_TestNullReferenceArrayProperty()
        {
            _subject.StringArray = null;
            Assert.That(_reflection["StringArray.1"], Is.Null);
        }

        [Test]
        public void SET_TestNullReferenceDictionaryProperty()
        {
            _subject.StringDictionary = null;
            try
            {
                _reflection["StringDictionary.a"]="first";
            }
            catch (ReflectionException Re)
            {
                Console.WriteLine(Re.Message);
                ReflectionException test = ReflectionException.NoSourceAvailable("a");
                Assert.That(Re.Message, Is.EqualTo(test.Message));
            }
        }

        [Test]
        public void GET_TestNullReferenceDictionaryProperty()
        {
            _subject.StringDictionary = null;
            Assert.That(_reflection["StringDictionary.a"], Is.Null);
        }

        [Test]
        public void SET_TestNullReferenceEnumerableProperty()
        {
            _subject.StringEnumerable = null;
            try
            {
                _reflection["StringEnumerable.1"]="first";
            }
            catch (ReflectionException Re)
            {
                ReflectionException test = ReflectionException.NoSourceAvailable("1");
                Assert.That(Re.Message, Is.EqualTo(test.Message));
            }
        }

        [Test]
        public void GET_TestNullReferenceEnumerableProperty()
        {
            _subject.StringEnumerable = null;
            Assert.That(_reflection["StringEnumerable.1"], Is.Null);
        }


        [Test]
        public void GET_TestNullReferenceListProperty()
        {
            _subject.StringEnumerable = null;
            Assert.That(_reflection["StringList.1"], Is.Null);
        }

        [Test]
        public void SET_TestNullReferenceListProperty()
        {
            _subject.StringEnumerable = null;
            try
            {
                _reflection["StringList.1"]="first";
            }
            catch (ReflectionException Re)
            {
                ReflectionException test = ReflectionException.NoSourceAvailable("1");
                Assert.That(Re.Message, Is.EqualTo(test.Message));
            }
        }

        [Test]
        public void TestSetArrayItem()
        {
            _subject.ComplexArray = new TestSubject[3];
            _subject.ComplexArray[0] = new TestSubject();
            _subject.ComplexArray[1] = new TestSubject();
            _subject.ComplexArray[2] = new TestSubject();
            _subject.ComplexArray[0].SimpleString = "first";
            _subject.ComplexArray[1].SimpleString = "second";
            _subject.ComplexArray[2].SimpleString = "third";

            _reflection["ComplexArray.2.SimpleString"] = "fourth";
            Assert.That(_subject.ComplexArray[2].SimpleString, Is.EqualTo("fourth"));
        }

        [Test]
        public void TestSetDictionaryListItem()
        {
            _subject.ComplexList = new List<TestSubject>();
            _subject.ComplexList.Add(new TestSubject());
            _subject.ComplexList.Add(new TestSubject());
            _subject.ComplexList.Add(new TestSubject());
            _subject.ComplexList[0].SimpleString = "first";
            _subject.ComplexList[1].SimpleString = "second";
            _subject.ComplexList[2].SimpleString = "third";

            _reflection["ComplexList.2.SimpleString"] = "fourth";
            Assert.That(_subject.ComplexList[2].SimpleString, Is.EqualTo("fourth"));
        }

        [Test]
        public void TestSetDictionaryProperty()
        {
            _subject.ComplexDictionary = new Dictionary<string, TestSubject>();
            _subject.ComplexDictionary.Add("a", new TestSubject());
            _subject.ComplexDictionary.Add("b", new TestSubject());
            _subject.ComplexDictionary.Add("c", new TestSubject());
            _subject.ComplexDictionary["a"].SimpleString = "first";
            _subject.ComplexDictionary["b"].SimpleString = "second";
            _subject.ComplexDictionary["c"].SimpleString = "third";

            _reflection["ComplexDictionary.c.SimpleString"] = "fourth";
            Assert.That(_subject.ComplexDictionary["c"].SimpleString, Is.EqualTo("fourth"));
        }

        [Test]
        public void TestSetNestedIntExistingProperty()
        {
            _subject.Nested = new TestSubject();
            _subject.Nested.SimpleString = "42";
            _reflection["Nested.SimpleString"] = "666";
            Assert.That(_subject.Nested.SimpleString, Is.EqualTo("666"));
        }

        [Test]
        public void TestSetNestedIntToNull()
        {
            _subject.Nested = new TestSubject();
            _subject.Nested.SimpleString = "42";
            _reflection["Nested.SimpleString"] = null;
            Assert.That(_subject.Nested.SimpleString, Is.Null);
        }

        [Test]
        public void TestSetNestedNonExistingArrayItem()
        {
            _subject.ComplexArray = new TestSubject[3];
            _subject.ComplexArray[0] = new TestSubject();
            _subject.ComplexArray[1] = new TestSubject();
            _subject.ComplexArray[2] = new TestSubject();
            _subject.ComplexArray[0].SimpleString = "first";
            _subject.ComplexArray[1].SimpleString = "second";
            _subject.ComplexArray[2].SimpleString = "third";

            try
            {
                _reflection["ComplexArray.3.SimpleString"] = null;
                Assert.Fail("Expected exception");
            }
            catch (ReflectionException Re)
            {
                Console.WriteLine(Re.Message);
                Assert.AreEqual(Re.Message, ReflectionException.IndexOutOfBounds(typeof (TestSubject[]), 3).Message);
            }
        }

        [Test]
        public void TestSetNestedNonExistingDictionaryKey()
        {
            _subject.ComplexDictionary = new Dictionary<string, TestSubject>();
            _subject.ComplexDictionary.Add("a", new TestSubject());
            _subject.ComplexDictionary.Add("b", new TestSubject());
            _subject.ComplexDictionary.Add("c", new TestSubject());
            _subject.ComplexDictionary["a"].SimpleString = "first";
            _subject.ComplexDictionary["b"].SimpleString = "second";
            _subject.ComplexDictionary["c"].SimpleString = "third";

            try
            {
                _reflection["ComplexDictionary.e.SimpleString"] = null;
                Assert.Fail("Expected exception");
            }
            catch (ReflectionException Re)
            {
                Assert.AreEqual(Re.Message, ReflectionException.NoSourceAvailable("SimpleString").Message);
            }
        }

        [Test]
        public void TestSetNestedNonExistingListItem()
        {
            _subject.ComplexList = new List<TestSubject>();
            _subject.ComplexList.Add(new TestSubject());
            _subject.ComplexList.Add(new TestSubject());
            _subject.ComplexList.Add(new TestSubject());
            _subject.ComplexList[0].SimpleString = "first";
            _subject.ComplexList[1].SimpleString = "second";
            _subject.ComplexList[2].SimpleString = "third";

            try
            {
                _reflection["ComplexList.3.SimpleString"] = null;
                Assert.Fail("Expected exception");
            }
            catch (ReflectionException Re)
            {
                Console.WriteLine(Re.Message);
                Assert.AreEqual(Re.Message, ReflectionException.IndexOutOfBounds(typeof (List<TestSubject>), 3).Message);
            }
        }

        [Test]
        public void TestSetNestedNonExistingProperty()
        {
            _subject.Nested = new TestSubject();
            _subject.Nested.SimpleString = "42";
            try
            {
                _reflection["NonExisting.SimpleString"] = null;
                Assert.Fail("Expected exception");
            }
            catch (ReflectionException Re)
            {
                Assert.AreEqual(Re.Message,
                                ReflectionException.PropertyNotFound("NonExisting", typeof (TestSubject)).Message);
            }
        }

        [Test]
        public void TestSetNewArrayItem()
        {
            _subject.ComplexArray = new TestSubject[4];
            _subject.ComplexArray[0] = new TestSubject();
            _subject.ComplexArray[1] = new TestSubject();
            _subject.ComplexArray[2] = new TestSubject();
            _subject.ComplexArray[0].SimpleString = "first";
            _subject.ComplexArray[1].SimpleString = "second";
            _subject.ComplexArray[2].SimpleString = "third";

            var sub = new TestSubject();
            sub.SimpleString = "fourth";
            _reflection["ComplexArray.3"] = sub;

            Assert.That(_subject.ComplexArray[3].SimpleString, Is.EqualTo("fourth"));
        }

        [Test]
        public void TestSetNewDictionaryProperty()
        {
            _subject.ComplexDictionary = new Dictionary<string, TestSubject>();
            _subject.ComplexDictionary.Add("a", new TestSubject());
            _subject.ComplexDictionary.Add("b", new TestSubject());
            _subject.ComplexDictionary.Add("c", new TestSubject());
            _subject.ComplexDictionary["a"].SimpleString = "first";
            _subject.ComplexDictionary["b"].SimpleString = "second";
            _subject.ComplexDictionary["c"].SimpleString = "third";

            var sub = new TestSubject();
            sub.SimpleString = "fourth";
            _reflection["ComplexDictionary.d"] = sub;

            Assert.That(_subject.ComplexDictionary["d"].SimpleString, Is.EqualTo("fourth"));
        }

        [Test]
        public void TestSetNewListItem()
        {
            _subject.ComplexList = new List<TestSubject>();
            _subject.ComplexList.Add(new TestSubject());
            _subject.ComplexList.Add(new TestSubject());
            _subject.ComplexList.Add(new TestSubject());
            _subject.ComplexList[0].SimpleString = "first";
            _subject.ComplexList[1].SimpleString = "second";
            _subject.ComplexList[2].SimpleString = "third";

            var sub = new TestSubject();
            sub.SimpleString = "fourth";
            _reflection["ComplexList.add"] = sub;

            Assert.That(_subject.ComplexList[3].SimpleString, Is.EqualTo("fourth"));
        }

        [Test]
        public void TestSetNonExisting()
        {
            try
            {
                _reflection["NonExisting"] = null;
                Assert.Fail("Expected exception");
            }
            catch (ReflectionException Re)
            {
                Assert.AreEqual(Re.Message,
                                ReflectionException.PropertyNotFound("NonExisting", typeof (TestSubject)).Message);
            }
        }

        [Test]
        public void TestSetSimpleIntExistingProperty()
        {
            _reflection["NullableInt"] = 666;
            Assert.That(_subject.NullableInt, Is.EqualTo(666));
        }

        [Test]
        public void TestSetSimpleIntToNull()
        {
            _reflection["NullableInt"] = null;
            Assert.That(_subject.NullableInt, Is.Null);
        }

        [Test]
        public void TestSimpleIntExistingProperty()
        {
            _subject.SimpleInt = 42;
            Assert.That(_reflection["SimpleInt"], Is.EqualTo(42));
        }

        [Test]
        public void TestSimpleStringExistingProperty()
        {
            _subject.SimpleString = "abc";
            Assert.That(_reflection["SimpleString"], Is.EqualTo("abc"));
        }

        [Test]
        public void TestTypeResovlerDoesntInteferForNonProxies()
        {
            var nested = new TestSubject();
            nested.SimpleString = "intefer";
            _subject.Proxy = new Proxy(nested);
            _subject.SimpleString = "don't intefer";
            _reflection.ObjectResolver = delegate(object source)
                                             {
                                                 if (source is Proxy)
                                                 {
                                                     return ((Proxy) source).Source;
                                                 }
                                                 else
                                                 {
                                                     return source;
                                                 }
                                             };
            Assert.That(_reflection["Proxy.SimpleString"], Is.EqualTo("intefer"));
            Assert.That(_reflection["SimpleString"], Is.EqualTo("don't intefer"));
        }

        [Test]
        public void TestTypeResovlerForProxies()
        {
            var nested = new TestSubject();
            nested.SimpleString = "String in proxy";
            _subject.Proxy = new Proxy(nested);
            try
            {
                object value = _reflection["Proxy.SimpleString"];
                Assert.Fail("Expected exception");
            }
            catch (ReflectionException Re)
            {
                Assert.That(Re.Message,
                            Is.EqualTo(ReflectionException.PropertyNotFound("SimpleString", typeof (Proxy)).Message));
            }
            _reflection.ObjectResolver = delegate(object source)
                                             {
                                                 if (source is Proxy)
                                                 {
                                                     return ((Proxy) source).Source;
                                                 }
                                                 else
                                                 {
                                                     return source;
                                                 }
                                             };
            Assert.That(_reflection["Proxy.SimpleString"], Is.EqualTo("String in proxy"));
        }
    }
}
