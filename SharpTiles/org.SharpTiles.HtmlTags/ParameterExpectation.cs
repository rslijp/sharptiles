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
 */using System;
using System.Reflection;
using System.Text;
using org.SharpTiles.Common;

namespace org.SharpTiles.HtmlTags
{
    public class ParameterExpectation
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }

        private bool ExactMatch { get; set; }

        public static ParameterExpectation Expect(IParameterValue parameter)
        {
            if (parameter is IParameterValueWithType) return Expect((IParameterValueWithType) parameter);
            return Expect(parameter.Name);
        }

        public static ParameterExpectation Expect(IParameterValueWithType parameter)
        {
            Type type = parameter.Type;
            if (parameter.ExactMatch)
            {
                return Expect(parameter.Name).Of(type).ItMustBeAnExactMatch();
            }
            return Expect(parameter.Name).Of(type);
        }

        public static ParameterExpectation Expect(String name)
        {
            return new ParameterExpectation {Name = name.ToLowerInvariant()};
        }

        public ParameterExpectation Of(Type type)
        {
            Type = type;
            return this;
        }

        public ParameterExpectation And()
        {
            return this;
        }

        public ParameterExpectation ItMustBeAnExactMatch()
        {
            ExactMatch = true;
            return this;
        }


        public bool Equals(ParameterExpectation obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            bool equals = Equals(obj.Name, Name);
            if (equals && Type != null)
            {
                equals = Equals(obj.Type, Type);
            }
            return equals;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ParameterExpectation)) return false;
            return Equals((ParameterExpectation) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Name);
            if (Type != null)
            {
                sb.Append(" of ");
                if (ExactMatch)
                {
                    sb.Append("exactly ");
                }
                sb.Append(Type);
            }
            return sb.ToString();
        }

        public bool CanBeUsedFor(ParameterInfo actual)
        {
            bool canBeUsed = Name.Equals(actual.Name.ToLowerInvariant());
            if (canBeUsed && Type != null)
            {
                canBeUsed &= ExactMatch
                                 ?
                                     IsItAnExactMatch(actual.ParameterType)
                                 :
                                     IsItAnLooseMatch(actual.ParameterType);
            }

            return canBeUsed;
        }

        private bool IsItAnExactMatch(Type actual)
        {
            return Equals(actual, Type) || (!Equals(actual, typeof (object)) && IsItAnLooseMatch(actual));
        }

        private bool IsItAnLooseMatch(Type actual)
        {
            return TypeConverter.Possible(Type, actual);
        }
    }
}
