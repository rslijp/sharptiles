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
using org.SharpTiles.Expressions.Math;

namespace org.SharpTiles.Expressions.Test.Math
{
    public class AbsFunctionTest
    {
        
        [Test]
        public void Evaluate_Should_Return_6_on_input_6()
        {
            Assert.That(new AbsFunction().Evaluate(6), Is.EqualTo(6m));
        }

        [Test]
        public void Evaluate_Should_Return_6_on_input_minus_6()
        {
            Assert.That(new AbsFunction().Evaluate(-6), Is.EqualTo(6m));
        }

        [Test]
        public void Evaluate_Should_Return_6_on_string_input_minus_6()
        {
            Assert.That(new AbsFunction().Evaluate((-6).ToString()), Is.EqualTo(6m));
        }
    }
}
