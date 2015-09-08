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

namespace org.SharpTiles.Expressions.Functions
{
    public class SplitFunction : IFunctionDefinition
    {
        private static readonly FunctionArgument[] ARGUMENTS = new[]
                             {
                                 new FunctionArgument{ Type = typeof (string), Name = "source"},
                                 new FunctionArgument{ Type = typeof (string), Name = "seperator"}
                             };


        #region IFunctionDefinition Members

        public string Name
        {
            get { return "split"; }
        }

        public FunctionArgument[] Arguments
        {
            get { return ARGUMENTS; }
        }

        public Type ReturnType
        {
            get { return typeof (string[]); }
        }

        public object Evaluate(params object[] parameters)
        {
            string input = (string) parameters[0] ?? "";
            string seperator = (string) parameters[1] ?? "";
            return input.Split(new[] {seperator}, StringSplitOptions.None);
        }

        #endregion
    }
}
