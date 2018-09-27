using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Common;

namespace org.SharpTiles.Expressions.Test.Functions
{
    [TestFixture]
    public class IfEmptyFunctionTest
    {
        [Test]
        public void Should_return_source_if_value_not_empty()
        {
            // Given
            var source = 1;
            var replacement = 2;
            var expr = new ExpressionLib().Parse("fn:ifEmpty(src,repl)");

            // When
            var result = expr.Evaluate(new Reflection(new Hashtable { {"src", source}, {"repl", replacement} }));

            // Then
            Assert.That(result, Is.EqualTo(source));
        }

        [Test]
        public void Should_return_replacement_when_empty()
        {
            // Given
            var source = (int?)null;
            var replacement = 2;
            var expr = new ExpressionLib().Parse("fn:ifEmpty(src,repl)");

            // When
            var result = expr.Evaluate(new Reflection(new Hashtable { {"src", source}, {"repl", replacement} }));

            // Then
            Assert.That(result, Is.EqualTo(replacement));
        }

        [Test]
        public void Should_return_source_if_value_not_empty_on_list()
        {
            // Given
            var list = new List<int> {1, 2};
            var replacement = new List<int>();
            var expr = new ExpressionLib().Parse("fn:ifEmpty(list,repl)");

            // When
            var result = (IList) expr.Evaluate(new Reflection(new Hashtable { {"list", list}, {"repl", replacement} }));

            // Then
            Assert.That(result, Is.EqualTo(list));
        }

        [Test]
        public void Should_return_replacement_when_empty_on_list()
        {
            // Given
            var list = new List<int>();
            var replacement = new List<int> {1, 2};
            var expr = new ExpressionLib().Parse("fn:ifEmpty(list,repl)");

            // When
            var result = (IList) expr.Evaluate(new Reflection(new Hashtable { {"list", list}, {"repl", replacement} }));

            // Then
            Assert.That(result, Is.EqualTo(replacement));
        }

    }
}