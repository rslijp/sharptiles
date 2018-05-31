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
 using org.SharpTiles.Common;
 using org.SharpTiles.Tags;
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

        [Test]
        public void PropertyWithoutDataTypePropertyAttributeShouldSetTextDataType()
        {
            var doc = new PropertyDocumentation(new ResourceKeyStack(bundle), typeof (Set).GetProperty("Var"));
            Assert.That(doc.DataType, Is.EqualTo(TagAttributeDataType.Text));
        }

        [Test]
        public void PropertyWithNumberPropertyAttributeShouldSetNumberDataType()
        {
            var doc = new PropertyDocumentation(new ResourceKeyStack(bundle), typeof (TestTag).GetProperty("Id"));
            Assert.That(doc.DataType, Is.EqualTo(TagAttributeDataType.Number));
        }

        [Test]
        public void PropertyWithBooleanPropertyAttributeShouldSetBooleanDataType()
        {
            var doc = new PropertyDocumentation(new ResourceKeyStack(bundle), typeof (TestTag).GetProperty("Visible"));
            Assert.That(doc.DataType, Is.EqualTo(TagAttributeDataType.Boolean));
        }

        [Test]
        public void PropertyWithEnumPropertyAttributeShouldSetEnumDataType()
        {
            var doc = new PropertyDocumentation(new ResourceKeyStack(bundle), typeof (TestTag).GetProperty("Color"));
            Assert.That(doc.DataType, Is.EqualTo(TagAttributeDataType.Enum));
        }

        [Test]
        public void PropertyWithPropertyAnnotationAttributesShouldSetAnnotations()
        {
            var doc = new PropertyDocumentation(new ResourceKeyStack(bundle), typeof (TestTag).GetProperty("Color"));
            Assert.That(doc.Annotations.Length, Is.EqualTo(2));
            Assert.That(doc.Annotations[0], Is.EqualTo("test1"));
            Assert.That(doc.Annotations[1], Is.EqualTo("test2"));
        }

    }

    internal enum TestEnum
    {
        Black,
        White
    }
    internal class TestTag : ITag
    {
        public ParseContext Context { get; set; }
        public string TagName => "test-tag";
        public ITagGroup Group { get; set; }
        [NumberPropertyType]
        public ITagAttribute Id { get; set; }
        [BooleanPropertyType]
        public ITagAttribute Visible { get; set; }
        [EnumProperyType(typeof(TestEnum))]
        [PropertyAnnotation("test1")]
        [PropertyAnnotation("test2")]
        public ITagAttribute Color { get; set; }
        public TagState State { get; set; }
        public TagBodyMode TagBodyMode { get; }
        public string Evaluate(TagModel model)
        {
            return string.Empty;
        }

        public ITagAttributeSetter AttributeSetter { get; }
    }
}
