using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Common;
using org.SharpTiles.Expressions;

namespace org.SharpTiles.Expressions.Test
{
    [TestFixture]
    public class ConcatTest
    {
        public string Left => "lhs";
        public string Right => "rhs";
        public string Empty => string.Empty;
        public string Null => null;

        [Test]
        public void TestSimpleConcat()
        {
            var concat = new Concat(new Property("Left"), new Property("Right"));
            Assert.That(concat.Evaluate(new Reflection(this)), Is.EqualTo("lhsrhs"));
        }

        [Test]
        public void TestConcatBothEmpty()
        {
            var concat = new Concat(new Property("Empty"), new Property("Empty"));
            Assert.That(concat.Evaluate(new Reflection(this)), Is.EqualTo(string.Empty));
        }

        [Test]
        public void TestConcatBothNull()
        {
            var concat = new Concat(new Property("Null"), new Property("Null"));
            Assert.That(concat.Evaluate(new Reflection(this)), Is.EqualTo(string.Empty));
        }

        [Test]
        public void TestConcatLhsNotEmptyRhsNull()
        {
            var concat = new Concat(new Property("Left"), new Property("Null"));
            Assert.That(concat.Evaluate(new Reflection(this)), Is.EqualTo("lhs"));
        }

        [Test]
        public void TestConcatRhsNotEmptyLhsNull()
        {
            var concat = new Concat(new Property("Null"), new Property("Right"));
            Assert.That(concat.Evaluate(new Reflection(this)), Is.EqualTo("rhs"));
        }

    }
}