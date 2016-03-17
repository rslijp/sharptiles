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
    public class ReflectionException : Exception
    {
        public ReflectionException(string msg) : base(msg)
        {
            Nesting = -1;
            IgnoreOnGet = false;
        }

        public int Nesting { get; set; }
        public bool IgnoreOnGet { get; private set; }

        public static ReflectionException PropertyNotFound(string property, Type type, object source = null)
        {
            string msg = $"There's no (public) property '{property}' found on '{type.FullName}'.";
            if (source != null)
                msg += $" ({source})";
            return new ReflectionException(msg);
        }

        public static ReflectionException NoSourceAvailable(string property)
        {
            string msg = string.Format("There's no source(null) to evaluate property '{0}' on.", property);
            return new ReflectionException(msg) { IgnoreOnGet = true };
        }

        public static ReflectionException NoPropertyAvailable()
        {
            string msg = String.Format("There's no property to evaluate");
            return new ReflectionException(msg);
        }

        public static ReflectionException IndexOutOfBounds(Type type, int index)
        {
            string msg = String.Format("The index '{0}' is out of range on property on '{1}'.", index, type.FullName);
            return new ReflectionException(msg);
        }

        public static ReflectionException SetNotAvailable(string typeStr)
        {
            string msg = String.Format("There's no handler for setting values on a {0}", typeStr);
            return new ReflectionException(msg);
        }

        public static ReflectionExceptionWithContext DecorateWithContext(ReflectionException re, ParseContext context)
        {
            return ReflectionExceptionWithContext.MakePartial(new ReflectionExceptionWithContext(re.Message)).Decorate(context);
        }

        public class ReflectionExceptionWithContext : ExceptionWithContext
        {
            public ReflectionExceptionWithContext(string message) : base(message)
            {
            }

            public ReflectionExceptionWithContext(string message, Exception inner) : base(message, inner)
            {
            }
        }
    }
}
