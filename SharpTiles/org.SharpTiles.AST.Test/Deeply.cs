using System.Collections;
using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace org.SharpTiles.AST.Test
{
    public static class Deeply
    {
        public static class Is
        {
            public static IsDeeplyEqualConstraint EqualTo(object expected)
            {
                return new IsDeeplyEqualConstraint(expected);
            }
        }

        public static class Has
        {
            public static CollectionDeeplyContainsConstraint Member(object expected)
            {
                return new CollectionDeeplyContainsConstraint(expected);
            }
        }

        public class IsDeeplyEqualConstraint : Constraint
        {
            private ComparisonResult _comparisonResult;

            private readonly object _expectedValue;
            private ComparisonConfig _comparisonConfig = new ComparisonConfig { MaxDifferences = 1000, MembersToIgnore = {"AuditInfo"}};

            public IsDeeplyEqualConstraint(object expectedValue)
            {
                _expectedValue = expectedValue;
            }

            public override bool Matches(object actualValue)
            {
                actual = actualValue;

                _comparisonResult = new CompareLogic(_comparisonConfig).Compare(actualValue, _expectedValue);

                return _comparisonResult.AreEqual;
            }

            public override void WriteDescriptionTo(MessageWriter writer)
            {
                if (_comparisonResult == null)
                    return;

                foreach (var difference in _comparisonResult.Differences)
                    writer.WriteMessageLine($"\n          >>> {difference} <<<");

                writer.WritePredicate("  Equal to:");
                writer.WriteExpectedValue(_expectedValue);
            }

            public IsDeeplyEqualConstraint With(ComparisonConfig comparisonConfig)
            {
                _comparisonConfig = comparisonConfig;
                return this;
            }

            public IsDeeplyEqualConstraint IgnoreObjectTypes(bool ignore = true)
            {
                _comparisonConfig.IgnoreObjectTypes = ignore;
                return this;
            }
        }

        public class CollectionDeeplyContainsConstraint : CollectionConstraint
        {
            private readonly object _expected;

            /// <summary>
            /// Construct a CollectionContainsConstraint
            /// 
            /// </summary>
            /// <param name="expected"/>
            public CollectionDeeplyContainsConstraint(object expected)
            {
                _expected = expected;
            }

            protected override bool doMatch(ICollection actual)
            {
                foreach (object objA in actual)
                {
                    var result = new CompareLogic().Compare(objA, _expected);
                    if (result.AreEqual)
                        return true;
                }
                return false;
            }

            public override void WriteDescriptionTo(MessageWriter writer)
            {
                writer.WritePredicate("collection containing");
                writer.WriteExpectedValue(_expected);
            }

          
        }
    }
}