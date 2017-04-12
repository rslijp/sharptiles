using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Common;

namespace org.SharpTiles.Expressions.Test
{
    [TestFixture]
    public class ConcatWithSpaceTest
    {
        public string Left => "lhs";
        public string Right => "rhs";
        public string Empty => string.Empty;
        public string Null => null;

        [Test]
        public void TestSimpleConcatWithSpace()
        {
            var concat = new ConcatWithSpace(new Property("Left"), new Property("Right"));
            Assert.That(concat.Evaluate(new Reflection(this)), Is.EqualTo("lhs rhs"));
        }

        [Test]
        public void TestConcatWithSpaceBothEmpty()
        {
            var concat = new ConcatWithSpace(new Property("Empty"), new Property("Empty"));
            Assert.That(concat.Evaluate(new Reflection(this)), Is.EqualTo(string.Empty));
        }

        [Test]
        public void TestConcatWithSpaceBothNull()
        {
            var concat = new ConcatWithSpace(new Property("Null"), new Property("Null"));
            Assert.That(concat.Evaluate(new Reflection(this)), Is.EqualTo(string.Empty));
        }

        [Test]
        public void TestConcatWithSpaceLhsNotEmptyRhsNull()
        {
            var concat = new ConcatWithSpace(new Property("Left"), new Property("Null"));
            Assert.That(concat.Evaluate(new Reflection(this)), Is.EqualTo("lhs"));
        }

        [Test]
        public void TestConcatWithSpaceRhsNotEmptyLhsNull()
        {
            var concat = new ConcatWithSpace(new Property("Null"), new Property("Right"));
            Assert.That(concat.Evaluate(new Reflection(this)), Is.EqualTo("rhs"));
        }

        [Test]
        public void TestParseConcatWithSpaceExpression()
        {
            var parsed = new ExpressionLib().Parse("'Lhs' ~ 'Rhs' ");

            Assert.That(parsed.GetType(), Is.EqualTo(typeof(ConcatWithSpace)));
            var concat = (ConcatWithSpace)parsed;
            Assert.That(((Constant)concat.Lhs).Value, Is.EqualTo("Lhs"));
            Assert.That(((Constant)concat.Rhs).Value, Is.EqualTo("Rhs"));

        }
    }
}