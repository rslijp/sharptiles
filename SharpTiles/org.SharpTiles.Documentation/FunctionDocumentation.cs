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
 using System.Runtime.Serialization;
 using org.SharpTiles.Expressions.Functions;

namespace org.SharpTiles.Documentation
{
    [DataContract]
    public class FunctionDocumentation : IDescriptionElement
    {
        private readonly ResourceKeyStack _messagePath;
        private readonly string _name;
        private readonly FunctionArgument[] _arguments;
        private readonly Type _returnType;
        private readonly string _description;

        public FunctionDocumentation(ResourceKeyStack messagePath, IFunctionDefinition func)
        {
            _name = func.Name;
            _arguments = func.Arguments;
            _returnType = func.ReturnType;
            _messagePath = messagePath.BranchFor(func);
            _description = _messagePath.Description;
        }

        [DataMember]
        public string Name
        {
            get { return _name; }
        }

        [DataMember]
        public string Description => _description;

        [DataMember]
        public FunctionArgument[] Arguments
        {
            get { return _arguments; }
        }

        [DataMember]
        public Type ReturnType
        {
            get { return _returnType; }
        }
    }
}
