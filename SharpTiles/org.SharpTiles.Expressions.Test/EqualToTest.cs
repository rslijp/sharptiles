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
using System.Collections.Generic;
using NUnit.Framework;
using org.SharpTiles.Common;

namespace org.SharpTiles.Expressions.Test
{
    [TestFixture]
    public class EqualToTest
    {
        public class NestingObj
        {
            public String Code { get; set; }
        }

        [Test]
        public void TestEqualTo()
        {
            var eq = new EqualTo(new Constant("23"), new Constant("12"));
            Assert.IsFalse((bool) eq.Evaluate(new Reflection(this)));
            eq = new EqualTo(new Constant("12"), new Constant("23"));
            Assert.IsFalse((bool) eq.Evaluate(new Reflection(this)));
            eq = new EqualTo(new Constant("12"), new Constant("12"));
            Assert.IsTrue((bool) eq.Evaluate(new Reflection(this)));
        }

        public String Code { get; set; }

        /*
    [Test]
      public void TestCasting()
      {
          Code = "Dutch National";
          var eq = new EqualTo(new Property("Code"), new Constant("1"));
          Assert.IsFalse((bool) eq.Evaluate(new Reflection(this)));
      }

      [Test]
      public void TestCastingWithParseObj()
      {
          Code = "Dutch National";
          var eq = Expression.Parse("Code == 1");
          Assert.IsFalse((bool)eq.Evaluate(new Reflection(this)));
      }
      */

        [Test]
        public void TestCastingWithParseDictionary()
        {
            var dict = new Dictionary<String, String>
            {
                {"Code", "Dutch National"}
            };
            var eq = new ExpressionLib().Parse("Code == 1");
            Assert.IsFalse((bool)eq.Evaluate(new Reflection(dict)));
        }

        public NestingObj Nesting { get; set; }

        /*
        [Test]
        public void TestCastingWithParseObjWithNesting()
        {
            Nesting = new NestingObj { Code = "Dutch National" };
            var eq = Expression.Parse("Nesting.Code == 1");
            Assert.IsFalse((bool)eq.Evaluate(new Reflection(this)));
        }
        */
    }
}
