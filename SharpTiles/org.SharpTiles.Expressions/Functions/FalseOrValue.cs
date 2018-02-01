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
 using System.ComponentModel;
 using org.SharpTiles.Expressions.Functions;
 using TypeConverter = org.SharpTiles.Common.TypeConverter;

namespace org.SharpTiles.Expressions
{
    public class FalseOrValueFunction : IFunctionDefinition
    {
        private static readonly FunctionArgument[] ARGUMENTS = new[]
                             {
                                 new FunctionArgument{ Type = typeof (object), Name = "arg"}
                             };

        #region IFunctionDefinition Members

        public string Name
        {
            get { return "falseOrValue"; }
        }

        public FunctionArgument[] Arguments
        {
            get { return ARGUMENTS; }
        }

        public Type ReturnType
        {
            get { return typeof(bool); }
        }

        public object Evaluate(params object[] parameters)
        {
            object source = parameters != null ? parameters[0] : null;
            if (source == null)
            {
                return false;
            }
            return TypeConverter.To(source, typeof(bool));
        }

       

        #endregion
    }
}
