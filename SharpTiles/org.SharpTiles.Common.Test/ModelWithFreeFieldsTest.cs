using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace org.SharpTiles.Common.Test
{
    [TestFixture]
    public class ModelWithFreeFieldsTest
    {
        public class TestSubject : ModelWithFreeFields<TestSubject>
        {
            public TestSubject(string getOnly=null)
            {
                GetOnly = getOnly;
            }
            public int Number { get; set; }
            public string Text { get; set; }
            public string GetOnly { get; }
        }

        [Test]
        public void Should_Find_Own_Property()
        {
            var m = new TestSubject() {Number = 42};
            var r = m["Number"];
            Assert.That(r, Is.EqualTo(42));
        }

        [Test]
        public void Should_Find_Own_Property_All_Getters()
        {
            var m = new TestSubject() { Number = 42 };
            Assert.That(m["Number"], Is.EqualTo(42));
            Assert.That(m.Get("Number").Result, Is.EqualTo(42));
            Assert.That(m.TryGet("Number"), Is.EqualTo(42));
        }

        [Test]
        public void Should_Update_Own_Property()
        {
            var m = new TestSubject() { Number = 42 };
            m["Number"] = 37;
            Assert.That(m.Number, Is.EqualTo(37));
        }


        [Test]
        public void Should_Find_Own_Nullable_Property()
        {
            var m = new TestSubject() { Text = "abc" };
            var r = m["Text"];
            Assert.That(r, Is.EqualTo("abc"));
        }

        [Test]
        public void Should_Find_Own_Nullable_Property_All_Getters()
        {
            var m = new TestSubject() { Text = "abc" };
            Assert.That(m["Text"], Is.EqualTo("abc"));
            Assert.That(m.Get("Text").Result, Is.EqualTo("abc"));
            Assert.That(m.TryGet("Text"), Is.EqualTo("abc"));
        }

        [Test]
        public void Should_Update_Own_Nullable_Property()
        {
            var m = new TestSubject() { Text = "abc" };
            m["Text"] = "xyz";
            Assert.That(m.Text, Is.EqualTo("xyz"));
        }

        [Test]
        public void Should_Find_Own_Getter_Only_Property()
        {
            var m = new TestSubject("abc");
            var r = m["GetOnly"];
            Assert.That(r, Is.EqualTo("abc"));
        }

        [Test]
        public void Should_Find_Own_Getter_Only_Property_All_Getters()
        {
            var m = new TestSubject("abc");
            Assert.That(m["GetOnly"], Is.EqualTo("abc"));
            Assert.That(m.Get("GetOnly").Result, Is.EqualTo("abc"));
            Assert.That(m.TryGet("GetOnly"), Is.EqualTo("abc"));
        }

        [Test]
        public void Should_Not_Be_Allowed_To_Update_Get_Only_Property()
        {
            var m = new TestSubject("abc");
            try
            {
                m["GetOnly"] = "xyz";
                Assert.Fail();
            }
            catch (ArgumentException Ae)
            {
                //
            }
        }
        
        [Test]
        public void Should_Update_Set_and_Get_Property()
        {
            var m = new TestSubject();
            m["Free"] = "Willy";
            Assert.That(m["Free"], Is.EqualTo("Willy"));
        }


        [Test]
        public void Should_Update_Set_FreeProperty_With_Reflection()
        {
            var r = new TestSubject();
            var m = new Reflection(r);
            m["Free"] = "Willy";
            Assert.That(r["Free"], Is.EqualTo("Willy"));
        }


        [Test]
        public void Should_Update_Get_FreeProperty_With_Reflection()
        {
            var r = new TestSubject();
            r["Free"] = "Willy";
            var m = new Reflection(r);
            Assert.That(m["Free"], Is.EqualTo("Willy"));
        }


        [Test]
        public void Should_Update_Set_and_Get_Property_With_Nesting()
        {
            var m = new TestSubject();
            m["Free"] = new Dictionary<string,object>();
            m["Free.Whale"] = "Willy";
            Assert.That(m["Free.Whale"], Is.EqualTo("Willy"));
        }

        [Test]
        public void SHould_Be_Able_To_Reload_FreeFields()
        {
            var m = new TestSubject();
            m.ReloadFreeField(new Dictionary<string, object> {{"Free", "Willy"}});
            Assert.That(m["Free"], Is.EqualTo("Willy"));
        }

        [Test]
        public void SHould_Be_Able_To_Serialize_And_Deserialize()
        {
            var m = new TestSubject();
            m.ReloadFreeField(new Dictionary<string, object> { { "Free", "Willy" } });
            var json = JsonConvert.SerializeObject(m);
            Console.WriteLine(json);
            var c = JsonConvert.DeserializeObject<TestSubject>(json);
            Assert.That(c["Free"], Is.EqualTo("Willy"));

        }

        [Test]
        public void SHould_Be_Able_To_Serialize_And_Deserialize_Null_Value()
        {
            var m = new TestSubject();
            m.ReloadFreeField(new Dictionary<string, object> { { "Free", null } });
            var json = JsonConvert.SerializeObject(m);
            Console.WriteLine(json);
            var c = JsonConvert.DeserializeObject<TestSubject>(json);
            Assert.That(c["Free"], Is.EqualTo(null));

        }

    }
}
