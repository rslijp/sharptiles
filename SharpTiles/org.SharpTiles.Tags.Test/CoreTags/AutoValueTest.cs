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
                var date = GetAutoValueAsDateTime(nameof(SomeDateValue), model);
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
                var date = GetAutoValueAsDateTime(nameof(SomeDateValue), model);
                return $"date={date?.ToString("dd|MM|yyyy")}";
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
        public void DateAutoValueTestTag_Should_Be_Null_Safe_And_Return_Default()
        {
            var _model = new TagModel(new Hashtable());
            var tag = new DateAutoValueTestTagWithDefault();
            Assert.That(tag.Evaluate(_model), Is.EqualTo("date=02|10|2015"));
        }

    }
}
