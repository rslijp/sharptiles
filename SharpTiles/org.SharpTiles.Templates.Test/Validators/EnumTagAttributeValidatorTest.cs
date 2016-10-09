using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.CoreTags;
using org.SharpTiles.Templates.Validators;

namespace org.SharpTiles.Templates.Test.Validators
{
    [TestFixture]
    public class EnumTagAttributeValidatorTest
    {
        [Test]
        public void Should_validate_tagAttributes_with_enum()
        {
            Validate(tag => new ConstantAttribute("One", tag), null);
            Validate(tag => new ConstantAttribute("two", tag), null);
            Validate(tag => new ConstantAttribute("Two,Three", tag), null);
            Validate(tag => new ConstantAttribute("*", tag), null);
            Validate(tag => new ConstantAttribute("Four", tag), EnumTagAttributeValidator.InvalidValueException("Four", Enum.GetValues(typeof(OneTwoThree))));
            Validate(tag => new ConstantAttribute("Two,Wrong", tag), EnumTagAttributeValidator.InvalidValueException("Wrong", Enum.GetValues(typeof(OneTwoThree))));
            Validate(tag => null, null);
        }


        private void Validate(Func<TestTag, ITagAttribute> action, TagException expectedException)
        {
            // Given
            var tag = CreateTag(action);
            var validator = new EnumTagAttributeValidator();

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
            Assert.That(result?.Message, Is.EqualTo(expectedException?.Message), tag.EnumValue?.ToString());
        }

        private TestTag CreateTag(Func<TestTag, ITagAttribute> action)
        {
            var tag = new TestTag();
            tag.EnumValue = action(tag);
            return tag;
        }

        class TestTag : BaseCoreTag, ITag
        {
            [EnumProperyType(typeof(OneTwoThree), Multiple = true, Wildcard = "*")]
            public ITagAttribute EnumValue { get; set; }

            public string TagName => "test";
            public TagBodyMode TagBodyMode => TagBodyMode.Free;
            public string Evaluate(TagModel model)
            {
                throw new System.NotImplementedException();
            }
        }

        enum OneTwoThree
        {
            One, Two, Three
        }

    }
}