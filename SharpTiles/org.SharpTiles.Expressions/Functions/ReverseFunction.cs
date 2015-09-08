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
using System.Collections;
using org.SharpTiles.Common;

namespace org.SharpTiles.Expressions.Functions
{
    public class ReverseFunction : IFunctionDefinition
    {
        private static readonly FunctionArgument[] ARGUMENTS = new[]
                             {
                                 new FunctionArgument{ Type = typeof (IEnumerable), Name = "object"}
                             };


        #region IFunctionDefinition Members

        public string Name
        {
            get { return "reverse"; }
        }

        public FunctionArgument[] Arguments
        {
            get { return ARGUMENTS; }
        }

        public Type ReturnType
        {
            get { return typeof (int); }
        }

        public object Evaluate(params object[] parameters)
        {
            object source = parameters != null ? parameters[0] : null;
            if (source == null)
            {
                return null;
            }
            else if (source is string)
            {
                return StringUtils.Reverse((string) source);
            }
            else if (source is Array)
            {
                return CollectionUtils.Reverse((Array)source);
            }
            else if (source is IEnumerable)
            {
                Stack reversed = new Stack();
                foreach (var o in source as IEnumerable)
                {
                    reversed.Push(o);
                }
                return reversed;
            }
            else
            {
                throw FunctionEvaluationException.WrongParameter(this, 0, source);
            }
        }

        #endregion

        private static int CountEnumerable(IEnumerable enumerable)
        {
            int count = 0;
            foreach (object o in enumerable)
            {
                count++;
            }
            return count;
        }
    }
}
