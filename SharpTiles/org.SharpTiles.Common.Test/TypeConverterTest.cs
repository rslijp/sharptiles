using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace org.SharpTiles.Common.Test
{
    [TestFixture]
    public class TypeConverterTest
    {
        [Test]
        public void Should_Parse_Date_Only()
        {
            Assert.That(TypeConverter.To<DateTime>("2018-02-15"), Is.EqualTo(new DateTime(2018,2,15).ToUniversalTime()));   
        }

        [Test]
        public void Should_Parse_Date_Time()
        {
            Assert.That(TypeConverter.To<DateTime>("2018-02-15 17:21:30"), Is.EqualTo(new DateTime(2018, 2, 15,17,21,30).ToUniversalTime()));
        }

        [Test]
        public void Should_Parse_Date_Time_2()
        {
            Assert.That(TypeConverter.To<DateTime>("2018-02-15T17:21:30"), Is.EqualTo(new DateTime(2018, 2, 15, 17, 21, 30).ToUniversalTime()));
        }
    }
}
