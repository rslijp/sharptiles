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

namespace org.SharpTiles.Expressions.Test
{
    [TestFixture]
    public class PropertyTest
    {
        public String Name
        {
            get { return "VALUE"; }
        }

        [Test]
        public void PropertyShouldEvaluateProperty()
        {
            var property = new Property("Name");
            Assert.That(property.Evaluate(new Reflection(this)), Is.EqualTo(Name));
        }

        [Test]
        public void PropertyShouldThrowExceptionOnNonExistingProperty()
        {
            var property = new Property("NonExisting");
            try
            {
                property.Evaluate(new Reflection(this));
                Assert.Fail("Expected failure");
            }
            catch (ReflectionException)
            {
                //oke
            }
        }
    }
}
