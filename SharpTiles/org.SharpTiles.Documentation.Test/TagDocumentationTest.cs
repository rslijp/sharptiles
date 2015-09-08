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
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Documentation.Test
{
    [TestFixture]
    public class TagDocumentationTest
    {
        [Test]
        public void InternalPropertyIsNotIncluded()
        {
            var td = new TagDocumentation(new ResourceKeyStack(), new Out());
            bool hasBody = false;
            foreach (PropertyDocumentation property in td.Properties)
            {
                hasBody |= property.Name.Equals("Body");
            }
            Assert.That(hasBody, Is.False);
        }

        [Test]
        public void NonInternalPropertyIsIncluded()
        {
            var td = new TagDocumentation(new ResourceKeyStack(), new Out());
            bool hasValue = false;
            foreach (PropertyDocumentation property in td.Properties)
            {
                hasValue |= property.Name.Equals("Value");
            }
            Assert.That(hasValue, Is.True);
        }
    }
}
