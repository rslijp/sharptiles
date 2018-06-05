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
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Documentation.DocumentationAttributes;
using org.SharpTiles.Tags;
 using org.SharpTiles.Tags.CoreTags;
using org.SharpTiles.Tags.FormatTags;
using DescriptionAttribute = org.SharpTiles.Documentation.DocumentationAttributes.DescriptionAttribute;

namespace org.SharpTiles.Documentation.Test
{
    [TestFixture]
    public class TagDocumentationTest
    {
        private ResourceBundle bundle = new ResourceBundle("templates/Documentation", null);

        [Test]
        public void InternalPropertyIsNotIncluded()
        {
            var tagDict = new Dictionary<int,TagDocumentation>();
            var td = new TagDocumentation(new ResourceKeyStack(bundle), new Out(), new List<Func<ITag, TagDocumentation, bool>>(), tagDict);
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
            var tagDict = new Dictionary<int,TagDocumentation>();
            var td = new TagDocumentation(new ResourceKeyStack(bundle), new Out(), new List<Func<ITag, TagDocumentation, bool>>(),tagDict);
            bool hasValue = false;
            foreach (PropertyDocumentation property in td.Properties)
            {
                hasValue |= property.Name.Equals("Value");
            }
            Assert.That(hasValue, Is.True);
        }

        [Test]
        public void IInstanceTagDocumentationPropertyIsUsed()
        {
            var tagDict = new Dictionary<int,TagDocumentation>();
            var td = new TagDocumentation(new ResourceKeyStack(bundle), new InstanceTagDocumentationOut(), new List<Func<ITag, TagDocumentation, bool>>(), tagDict);
            bool hasBody = false;
            Assert.That(td.Description.Value, Is.EqualTo("Hi"));
         }

        public class InstanceTagDocumentationOut : Out, IInstanceTagDocumentation
        {
            public DescriptionAttribute Description => new DescriptionAttribute("Hi");
            public ExampleAttribute[] Examples => null;
            public NoteAttribute[] Notes => null;

        }

//        [Test]
//        public void TestParagraphStrip()
//        {
//            var x = "<p>The communication tag lib provides <em>functionality</em> to:</p>";
//            Console.WriteLine(DocumentationAttributes.DescriptionAttribute.INNER_CONTENT.IsMatch(x));
//            Console.WriteLine(DocumentationAttributes.DescriptionAttribute.INNER_CONTENT.Match(x).Groups[1].Value);
//        }

    }
}
