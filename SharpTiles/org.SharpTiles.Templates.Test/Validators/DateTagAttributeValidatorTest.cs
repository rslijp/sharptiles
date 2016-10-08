using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Expressions;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.CoreTags;
using org.SharpTiles.Templates.Validators;

namespace org.SharpTiles.Templates.Test.Validators
{
    [TestFixture]
    public class DateTagAttributeValidatorTest
    {
        [Test]
        public void Should_validate_tagAttributes_with_enum()
        {
            Validate(tag => new ConstantAttribute("2016-10-08T16:41:00", tag), null);
            Validate(tag => new ConstantAttribute("Four", tag), DateTagAttributeValidator.InvalidValue("DateValue", "Four", PatternStrings.DATETIME_FORMAT));
            Validate(tag => null, null);
        }


        private void Validate(Func<TestTag, ITagAttribute> action, TagException expectedException)
        {
            // Given
            var tag = CreateTag(action);
            var validator = new DateTagAttributeValidator();

            // When
            TagException result = null;
            try
            {
                validator.Validate(tag);
            }
            catch (TagException e)
            {
                result = e;
            }
            Assert.That(result?.Message, Is.EqualTo(expectedException?.Message), tag.DateValue?.ToString());
        }

        private TestTag CreateTag(Func<TestTag, ITagAttribute> action)
        {
            var tag = new TestTag();
            tag.DateValue = action(tag);
            return tag;
        }

        class TestTag : BaseCoreTag, ITag
        {
            [DatePropertyType]
            public ITagAttribute DateValue { get; set; }

            public string TagName => "test";
            public TagBodyMode TagBodyMode => TagBodyMode.Free;
            public string Evaluate(TagModel model)
            {
                throw new System.NotImplementedException();
            }
        }

    }
}