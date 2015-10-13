using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Common;
using org.SharpTiles.Expressions;
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Tags.Test.CoreTags
{
    public class AutoValueTest
    {
        public class DecimalAutoValueTestTag : BaseCoreTag, ITag
        {
            public ITagAttribute SomeDecimalValue { get; set; }

            public string TagName { get; }
            public TagBodyMode TagBodyMode { get; }
            public string Evaluate(TagModel model)
            {
                var dec = GetAutoValueAsDecimal(nameof(SomeDecimalValue), model);
                return $"dec={dec}";
            }
        }

        public class DecimalAutoValueTestTagWithDefault : BaseCoreTag, ITag
        {
            [TagDefaultValue(0)]
            public ITagAttribute SomeDecimalValue { get; set; }

            public string TagName { get; }
            public TagBodyMode TagBodyMode { get; }
            public string Evaluate(TagModel model)
            {
                var dec = GetAutoValueAsDecimal(nameof(SomeDecimalValue), model);
                return $"dec={dec}";
            }
        }

        public class DateAutoValueTestTag : BaseCoreTag, ITag
        {
            public ITagAttribute SomeDateValue { get; set; }

            public string TagName { get; }
            public TagBodyMode TagBodyMode { get; }
            public string Evaluate(TagModel model)
            {
                var date = GetAutoValueAsDate(nameof(SomeDateValue), model);
                return $"date={date?.ToString("dd|MM|yyyy")}";
            }
        }

        public class DateAutoValueTestTagWithDefault : BaseCoreTag, ITag
        {
            [TagDefaultValue("2015-10-02")]
            public ITagAttribute SomeDateValue { get; set; }

            public string TagName { get; }
            public TagBodyMode TagBodyMode { get; }
            public string Evaluate(TagModel model)
            {
                var date = GetAutoValueAsDate(nameof(SomeDateValue), model);
                return $"date={date?.ToString("dd|MM|yyyy")}";
            }
        }

        public class DateTimeAutoValueTestTag : BaseCoreTag, ITag
        {
            public ITagAttribute SomeDateValue { get; set; }

            public string TagName { get; }
            public TagBodyMode TagBodyMode { get; }
            public string Evaluate(TagModel model)
            {
                var date = GetAutoValueAsDateTime(nameof(SomeDateValue), model);
                return $"date={date?.ToString("dd|MM|yyyy|HH|mm|ss")}";
            }
        }

        public class DateTimeAutoValueTestTagWithDefault : BaseCoreTag, ITag
        {
            [TagDefaultValue("2015-10-02T14:33:22")]
            public ITagAttribute SomeDateValue { get; set; }

            public string TagName { get; }
            public TagBodyMode TagBodyMode { get; }
            public string Evaluate(TagModel model)
            {
                var date = GetAutoValueAsDateTime(nameof(SomeDateValue), model);
                return $"date={date?.ToString("dd|MM|yyyy|HH|mm|ss")}";
            }
        }

       
        [Test]
        public void TestAutoValueOfDecimal()
        {
            var _model = new TagModel(new Hashtable());
            var tag = new DecimalAutoValueTestTag();
            tag.SomeDecimalValue = new MockAttribute(new Constant("8"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo("dec=8"));
        }

        [Test]
        public void TestAutoValueOfDecimal_Should_Be_Null_Safe()
        {
            var _model = new TagModel(new Hashtable());
            var tag = new DecimalAutoValueTestTag();
            Assert.That(tag.Evaluate(_model), Is.EqualTo("dec="));
        }


        [Test]
        public void TestAutoValueOfDecimal_Should_Be_Null_Safe_Return_0()
        {
            var _model = new TagModel(new Hashtable());
            var tag = new DecimalAutoValueTestTagWithDefault();
            Assert.That(tag.Evaluate(_model), Is.EqualTo("dec=0"));
        }

        [Test]
        public void DateAutoValueTestTag_Should_Be_Null_Safe()
        {
            var _model = new TagModel(new Hashtable());
            var tag = new DateAutoValueTestTag();
            Assert.That(tag.Evaluate(_model), Is.EqualTo("date="));
        }

        [Test]
        public void DateAutoValueTestTag_Should_Parse_Date()
        {
            var _model = new TagModel(new Hashtable() {
                { "SomeDate", "1979-10-02"}
            });
            var tag = new DateAutoValueTestTag();
            tag.SomeDateValue = new MockAttribute(new Property("SomeDate"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo("date=02|10|1979"));
        }

        [Test]
        public void DateAutoValueTestTag_Should_Pass_Date()
        {
            var _model = new TagModel(new Hashtable() {
                { "SomeDate", new DateTime(1979,10,2)}
            });
            var tag = new DateAutoValueTestTag();
            tag.SomeDateValue = new MockAttribute(new Property("SomeDate"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo("date=02|10|1979"));
        }

        [Test]
        public void DateAutoValueTestTag_Should_Be_Null_Safe_And_Return_Default()
        {
            var _model = new TagModel(new Hashtable());
            var tag = new DateAutoValueTestTagWithDefault();
            Assert.That(tag.Evaluate(_model), Is.EqualTo("date=02|10|2015"));
        }

        [Test]
        public void DateTimeAutoValueTestTag_Should_Be_Null_Safe()
        {
            var _model = new TagModel(new Hashtable());
            var tag = new DateTimeAutoValueTestTag();
            Assert.That(tag.Evaluate(_model), Is.EqualTo("date="));
        }

        [Test]
        public void DateTimeAutoValueTestTag_Should_Parse_Date()
        {
            var _model = new TagModel(new Hashtable() {
                { "SomeDate", "1979-10-02T11:55:44"}
            });
            var tag = new DateTimeAutoValueTestTag();
            tag.SomeDateValue = new MockAttribute(new Property("SomeDate"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo("date=02|10|1979|11|55|44"));
        }

        [Test]
        public void DateTimeAutoValueTestTag_Should_Include_Value_And_Pattern_Into_Exception()
        {
            var _model = new TagModel(new Hashtable() {
                { "SomeDate", "pindakaas"}
            });
            try
            {
                var tag = new DateTimeAutoValueTestTag();
                tag.SomeDateValue = new MockAttribute(new Property("SomeDate"));
                tag.Evaluate(_model);
                Assert.Fail("Exception expected");
            } catch (FormatException Fe)
            {
                Console.WriteLine(Fe.Message);
                Assert.That(Fe.Message.Contains("pindakaas"));
                Assert.That(Fe.Message.Contains(PatternStrings.DATETIME_FORMAT));
            }
        }

        [Test]
        public void DateTimeAutoValueTestTag_Should_Pass_Date()
        {
            var _model = new TagModel(new Hashtable() {
                { "SomeDate", new DateTime(1979,10,2,11,55,44)}
            });
            var tag = new DateTimeAutoValueTestTag();
            tag.SomeDateValue = new MockAttribute(new Property("SomeDate"));
            Assert.That(tag.Evaluate(_model), Is.EqualTo("date=02|10|1979|11|55|44"));
        }


        [Test]
        public void DateTimeAutoValueTestTag_Should_Be_Null_Safe_And_Return_Default()
        {
            var _model = new TagModel(new Hashtable());
            var tag = new DateTimeAutoValueTestTagWithDefault();
            Assert.That(tag.Evaluate(_model), Is.EqualTo("date=02|10|2015|14|33|22"));
        }
    }
}
