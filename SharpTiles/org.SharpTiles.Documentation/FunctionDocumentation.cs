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
 using System.ComponentModel;
 using System.Linq;
 using System.Runtime.Serialization;
 using System.Web.Script.Serialization;
 using org.SharpTiles.Documentation.DocumentationAttributes;
 using org.SharpTiles.Expressions.Functions;
 using DescriptionAttribute = org.SharpTiles.Documentation.DocumentationAttributes.DescriptionAttribute;

namespace org.SharpTiles.Documentation
{
    [DataContract]
    public class FunctionDocumentation : IDescriptionElement
    {
        private readonly ResourceKeyStack _messagePath;
        private readonly string _name;
        private readonly FunctionArgumentDocumentation[] _arguments;
        private readonly Type _returnTypeValue;
        private readonly DescriptionValue _description;
        private readonly CategoryAttribute _category;
        private readonly string _libName;
           

        public FunctionDocumentation(string libName, ResourceKeyStack messagePath, IFunctionDefinition func)
        {

            _libName = libName;
            _name = func.Name;
            _arguments = func.Arguments.Select(arg=> new FunctionArgumentDocumentation(arg)).ToArray();
            _returnTypeValue = func.ReturnType;
            _messagePath = messagePath.BranchFor(func);
            _description=_messagePath.Description;
            _category = CategoryHelper.GetCategory(func.GetType());
        }

        [DataMember]
        public string Id => "id_Functions_"+(Category??"General")+"_"+_name;

        [DataMember]
        public string Category => _category?.Category;

        [DataMember]
        public string Name
        {
            get { return _name; }
        }

        [DataMember]
        public string LibName
        {
            get { return _libName; }
        }

        [DataMember]
        public DescriptionValue Description => _description;

        [DataMember]
        public FunctionArgumentDocumentation[] Arguments
        {
            get { return _arguments; }
        }

        [ScriptIgnore]
        public Type ReturnTypeValue
        {
            get { return _returnTypeValue; }
        }

        [DataMember]
        public string ReturnType => ReturnTypeValue.AssemblyQualifiedName;
    }
}
