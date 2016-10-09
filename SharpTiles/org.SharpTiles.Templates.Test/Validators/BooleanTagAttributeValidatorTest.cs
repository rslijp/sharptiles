using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.CoreTags;
using org.SharpTiles.Templates.Validators;

namespace org.SharpTiles.Templates.Test.Validators
{
    [TestFixture]
    public class BooleanTagAttributeValidatorTest
    {
        [Test]
        public void Should_validate_tagAttributes_with_enum()
        {
            Validate(tag => new ConstantAttribute("true", tag), null);
            Validate(tag => new ConstantAttribute("waar", tag), BooleanTagAttributeValidator.InvalidValue("BooleanValue", "waar"));
            Validate(tag => null, null);
        }


        private void Validate(Func<TestTag, ITagAttribute> action, TagException expectedException)
        {
            // Given
            var tag = CreateTag(action);
            var validator = new BooleanTagAttributeValidator();

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
            Assert.That(result?.Message, Is.EqualTo(expectedException?.Message), tag.BooleanValue?.ToString());
        }

        private TestTag CreateTag(Func<TestTag, ITagAttribute> action)
        {
            var tag = new TestTag();
            tag.BooleanValue = action(tag);
            return tag;
        }

        class TestTag : BaseCoreTag, ITag
        {
            [BooleanPropertyType]
            public ITagAttribute BooleanValue { get; set; }

            public string TagName => "test";
            public TagBodyMode TagBodyMode => TagBodyMode.Free;
            public string Evaluate(TagModel model)
            {
                throw new System.NotImplementedException();
            }
        }


    }
}