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
using System.Linq;
using System.Reflection;
using org.SharpTiles.Expressions.Functions;

namespace org.SharpTiles.Documentation
{
    public class WrappedFunctionDocumentation : IFunctionDefinition
    {
        private MethodInfo _method;

        public WrappedFunctionDocumentation(MethodInfo method)
        {
            _method = method;
        }

        public string Name
        {
            get { return _method.DeclaringType.Name+"."+_method.Name; }
        }

        public FunctionArgument[] Arguments
        {
            get
            {
               return _method.GetParameters().ToList().Select(
                    p => new FunctionArgument
                             {
                                 Name =  p.Name,
                                 Type = p.ParameterType
                             }
                    ).ToArray();
            }
        }

        public Type ReturnType
        {
            get { return _method.ReturnType; }
        }

        public object Evaluate(params object[] parameters)
        {
            return null;
        }
    }
}
