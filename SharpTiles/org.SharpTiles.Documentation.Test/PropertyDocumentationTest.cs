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
 using org.SharpTiles.Tags.FormatTags;

namespace org.SharpTiles.Documentation.Test
{
    [TestFixture]
    public class PropertyDocumentationTest
    {
        private ResourceBundle bundle = new ResourceBundle("templates/Documentation", null);

        [Test]
        public void NotRequiredPropertyShouldHaveNotRequiredFlag()
        {
            var doc = new PropertyDocumentation(new ResourceKeyStack(bundle), typeof (Out).GetProperty("EscapeXml"));
            Assert.That(doc.Required, Is.False);
        }

        [Test]
        public void RequiredPropertyShouldHaveRequiredFlag()
        {
            var doc = new PropertyDocumentation(new ResourceKeyStack(bundle), typeof (Set).GetProperty("Var"));
            Assert.That(doc.Required, Is.True);
        }
    }
}
