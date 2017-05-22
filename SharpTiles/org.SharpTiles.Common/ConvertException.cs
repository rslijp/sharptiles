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

namespace org.SharpTiles.Common
{
    public class ConvertException : Exception
    {
        public ConvertException(string msg)
            : base(msg)
        {
        }

        private static string PrettyName(Type target)
        {
            if (target == null) return "object";
            if (target.IsGenericType && target.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var realTarget = target.GetGenericArguments()[0];
                return realTarget.Name + "?";
            }
            return target.Name; 
        }

        public static ConvertException CannotConvert(Type expected, object found)
        {
            String msg = String.Format("Cannot convert {0}({1}) to {2}", found ?? "null", PrettyName(found?.GetType()), PrettyName(expected));
            return new ConvertException(msg);
        }

        public static ConvertException CannotConvertValue(Type expected, object found)
        {
            String msg = String.Format("Cannot convert value {0}({1}) to {2}", found ?? "null", PrettyName(found?.GetType()), PrettyName(expected));
            return new ConvertException(msg);
        }

        public static ConvertException StaticTypeSafety(Type expected, Type found, String context)
        {
            String msg = String.Format("Expected {0} but found {1} for {2}", expected.Name, found!=null? found.Name : "null", context);
            return new ConvertException(msg);
        }
    }
}
