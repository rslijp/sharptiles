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
using System.Text;
using org.SharpTiles.Common;

namespace org.SharpTiles.Expressions
{
    public class ComparisionException : ExceptionWithContext
    {
        public ComparisionException(string message) : base(message)
        {
        }

        public ComparisionException(string message, Exception inner) : base(message, inner)
        {
        }

        public static PartialExceptionWithContext<ComparisionException> UnComparable(Type lhs, Type rhs)
        {
            String msg = String.Format("Can't compare {0} and {1}", lhs.Name, rhs.Name);
            return MakePartial(new ComparisionException(msg));
        }
    }
}
