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
 using NUnit.Framework;
 using NUnit.Framework.SyntaxHelpers;
 using org.SharpTiles.Connectors;

namespace org.SharpTiles.ConnectorsTest
{
    [TestFixture]
    public class TemplateFieldTest
    {
        [Test]
        public void BuildPrefixAllNull()
        {
            Assert.That(TemplateFieldPrefixHelper.BuildPrefix(null, null), Is.EqualTo(string.Empty));
            Assert.That(TemplateFieldPrefixHelper.BuildPrefix(string.Empty, string.Empty), Is.EqualTo(string.Empty));
        }

        [Test]
        public void BuildPrefixShouldUseHostPathAsPrefix()
        {
            Assert.That(TemplateFieldPrefixHelper.BuildPrefix(@"c:\temp", null), Is.EqualTo(@"c:\temp"));
        }

        [Test]
        public void BuildPrefixShouldAppendUnRoothedPrefixAfterHostPath()
        {
            Assert.That(TemplateFieldPrefixHelper.BuildPrefix(@"c:\temp", "nested"), Is.EqualTo(@"c:\temp\nested"));
        }

        [Test]
        public void BuildPrefixShouldUseRoothedPrefixOverHostPath()
        {
            Assert.That(TemplateFieldPrefixHelper.BuildPrefix(@"c:\temp", @"d:\prefix"), Is.EqualTo(@"d:\prefix"));
        }

        [Test]
        public void BuildPrefixShouldUsePrefixAsPrefixIfNoHostPathIsSupplied()
        {
            Assert.That(TemplateFieldPrefixHelper.BuildPrefix(null, "prefix"), Is.EqualTo("prefix"));
        }
    }
}
