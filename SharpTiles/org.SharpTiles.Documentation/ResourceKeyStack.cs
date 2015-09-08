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
using System.Linq;
using System.Reflection;
using System.Text;
using org.SharpTiles.Expressions;
using org.SharpTiles.Expressions.Functions;
using org.SharpTiles.Tags;

namespace org.SharpTiles.Documentation
{
    public class ResourceKeyStack : ICloneable
    {
        private ITagGroup _group;
        private ITag _tag;
        private PropertyInfo _property;
        private IExpressionParser _expression;
        private IFunctionDefinition _function;

        public ITagGroup Group
        {
            get { return _group; }
        }

        public ITag Tag
        {
            get { return _tag; }
        }

        public PropertyInfo Property
        {
            get { return _property; }
        }

        public IExpressionParser Expression
        {
            get { return _expression; }
        }

        public IFunctionDefinition Function
        {
            get { return _function; }
        }

        public string Description
        {
            get { 
                StringBuilder result = new StringBuilder("description");
                AppendExpression(result);
                AppendFunction(result);
                AppendGroup(result);
                return result.ToString();
            }
        }

        private void AppendExpression(StringBuilder result)
        {
            if (_expression != null)
            {
                result.Append("_" + _expression.GetType().Name);
            }
        }

        private void AppendFunction(StringBuilder result)
        {
            if (_function != null)
            {
                result.Append("_" + _function.GetType().Name);
            }
        }

        private void AppendGroup(StringBuilder result)
        {
            if(_group!=null)
            {
                if (_tag == null ||
                   _property == null ||
                   Equals(_tag.GetType(), _property.DeclaringType)
                   )
                {
                    result.Append("_" + _group.GetType().Name);
                }
                AppendTag(result);
            }
        }

        private void AppendTag(StringBuilder result)
        {
            if (_tag != null)
            {
                Type descriptionType = _tag.GetType();
                if(_property!=null)
                {
                    descriptionType = _property.DeclaringType;
                }
                result.Append("_" + descriptionType.Name);
                AppendProperty(result);
            }
        }

        private void AppendProperty(StringBuilder result)
        {
            if (_property != null)
            {
                result.Append("_" + _property.Name);
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public ResourceKeyStack BranchFor(ITagGroup group)
        {
            var branch = (ResourceKeyStack) MemberwiseClone();
            branch._group = group;
            return branch;
        }

        public ResourceKeyStack BranchFor(ITag tag)
        {
            var branch = (ResourceKeyStack)MemberwiseClone();
            branch._tag = tag;
            return branch;
        }

        public ResourceKeyStack BranchFor(PropertyInfo property)
        {
            var branch = (ResourceKeyStack)MemberwiseClone();
            branch._property = property;
            return branch;
        }

        public ResourceKeyStack BranchFor(IExpressionParser expression)
        {
            var branch = (ResourceKeyStack)MemberwiseClone();
            branch._expression = expression;
            return branch;
        }

        public ResourceKeyStack BranchFor(IFunctionDefinition function)
        {
            var branch = (ResourceKeyStack)MemberwiseClone();
            branch._function = function;
            return branch;
        }

        public string DescriptionForCategory(string categoryStr)
        {
            var branch = BranchFor(Group);
            branch._tag = null;
            return branch.Description + "_" + categoryStr;
        }


        
    }
}

