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

        public static ConvertException CannotConvert(Type expected, object found)
        {
            String msg = String.Format("Cannot convert {1} to {0}", found ?? "null", expected.Name);
            return new ConvertException(msg);
        }

        public static ConvertException StaticTypeSafety(Type expected, Type found, String context)
        {
            String msg = String.Format("Expected {0} but found {1} for {2}", expected.Name, found!=null? found.Name : "null", context);
            return new ConvertException(msg);
        }
    }
}
