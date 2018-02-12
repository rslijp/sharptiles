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
using org.SharpTiles.Common;

namespace org.SharpTiles.Expressions
{
    public class MacroException : ExceptionWithContext
    {
        public MacroException(string msg) : base(msg)
        {
        }

        public static PartialExceptionWithContext<MacroException> NotFound(string name)
        {
            var msg = $"Macro or function not found, '{name}'";
            return MakePartial(new MacroException(msg));
        }

        public static PartialExceptionWithContext<MacroException> NoMacroOrFunction(string name)
        {
            var msg = $"Resolved value of {name} is neither macro or function";
            return MakePartial(new MacroException(msg));
        }

        public static PartialExceptionWithContext<MacroException> NullNotAllowed(string argumentName)
        {
            var msg = $"Can't find varialbe {argumentName}(or {LanguageHelper.CamelCaseAttribute(argumentName)} which is required, because called function is strict";
            return MakePartial(new MacroException(msg));
        }

        public static PartialExceptionWithContext<MacroException> AgrumentsNotAllowed(string name)
        {
            var msg = $"Macro '{name}' doesn't support agruments";
            return MakePartial(new MacroException(msg));
        }

        public static PartialExceptionWithContext<MacroException> ArgumentIndexOutOfBounds(int index, int bounds)
        {
            var msg = $"Argument index '{index}' exceeds number of arguments of {bounds}";
            return MakePartial(new MacroException(msg));
        }

        public static PartialExceptionWithContext<MacroException> ExcpectedArgumentIndex(int index, int bounds)
        {
            var msg = $"Expected Argument index '{bounds}' instead of of {index}";
            return MakePartial(new MacroException(msg));
        }
    }
}
