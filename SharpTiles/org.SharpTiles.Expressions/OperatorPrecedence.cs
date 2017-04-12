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
 using System;
using System.Collections.Generic;
 using org.SharpTiles.Expressions.Functions;

namespace org.SharpTiles.Expressions
{
    public class OperatorPrecedence
    {
        private static readonly IDictionary<Type, int> _lookup = new Dictionary<Type, int>();
        private static readonly List<IList<Type>> PRECENDENCE = new List<IList<Type>>();

        static OperatorPrecedence()
        {
            PRECENDENCE.Add(new List<Type>(new[] { typeof(Assignment) }));
            PRECENDENCE.Add(new List<Type>(new[] {typeof (Function)}));
            PRECENDENCE.Add(new List<Type>(new[] {typeof (Brackets)}));
            PRECENDENCE.Add(new List<Type>(new[] { typeof(BooleanTernaryExpression) }));
            PRECENDENCE.Add(new List<Type>(new[] {typeof (And)}));
            PRECENDENCE.Add(new List<Type>(new[] {typeof (Or)}));
            PRECENDENCE.Add(
                new List<Type>(
                    new[]
                        {typeof (GreaterThan), typeof (GreaterThanOrEqual), typeof (LessThan), typeof (LessThanOrEqual)}));
            PRECENDENCE.Add(new List<Type>(new[] {typeof (EqualTo), typeof (NotEqualTo)}));
            PRECENDENCE.Add(new List<Type>(new[] {typeof (Add), typeof (Minus), typeof (Not), typeof(Concat), typeof(ConcatWithSpace)}));
            PRECENDENCE.Add(new List<Type>(new[] {typeof (Multiply), typeof (Divide), typeof (Modulo)}));
            PRECENDENCE.Add(new List<Type>(new[] {typeof (Power)}));
            Flatten();
        }

        public static IList<IList<Type>> Precedence
        {
            get { return PRECENDENCE.AsReadOnly(); }
        }

        private static void Flatten()
        {
            for (int i = 0; i < PRECENDENCE.Count; i++)
            {
                int order = i + 1;
                foreach (Type op in PRECENDENCE[i])
                {
                    _lookup.Add(op, order);
                }
            }
        }

        public static int Of(Type op)
        {
            return _lookup[op];
        }

        public static int Of(Expression op)
        {
            return Of(op.GetType());
        }

        public static bool IsHigherThan(Type you, int otherPriority)
        {
            int myPrecendence = Of(you);
            return myPrecendence >= otherPriority;
        }

        public static bool Applicable(Type expression)
        {
            return _lookup.ContainsKey(expression);
        }
    }
}
