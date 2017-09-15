using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Expressions.Functions;

namespace org.SharpTiles.Expressions.Test.Functions
{
    [TestFixture]
    public class AddDaysFunctionTest
    {
        [Test]
        public void Should_add_days_to_date()
        {
            // Given
            var function = new AddDaysFunction();
            var source = new DateTime(2017,9,15,9,10,0);

            // When
            var result = function.Evaluate(source, "3");

            // Then
            Assert.That(result, Is.EqualTo(source.AddDays(3)));
        }

        [Test]
        public void Should_add_partial_days_to_date()
        {

            // Given
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var function = new AddDaysFunction();
            var source = new DateTime(2017,9,15,9,10,0);

            // When
            var result = function.Evaluate(source, "3.5");

            // Then
            Assert.That(result, Is.EqualTo(source.AddDays(3.5)));
        }

        [Test]
        public void Should_return_null_when_source_is_null()
        {
            // Given
            var function = new AddDaysFunction();
            var source = (DateTime?)null;

            // When
            var result = function.Evaluate(source, "3");

            // Then
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Should_return_source_when_days_is_null()
        {
            // Given
            var function = new AddDaysFunction();
            var source = new DateTime(2017,9,15,9,10,0);

            // When
            var result = function.Evaluate(source, null);

            // Then
            Assert.That(result, Is.EqualTo(source));
        }

    }
}