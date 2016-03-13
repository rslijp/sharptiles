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
 using System.Globalization;
 using System.Linq;
using System.Reflection;
using System.Text;
 using org.SharpTiles.Documentation.DocumentationAttributes;
 using org.SharpTiles.Expressions;
using org.SharpTiles.Expressions.Functions;
using org.SharpTiles.Tags;
 using org.SharpTiles.Tags.FormatTags;

namespace org.SharpTiles.Documentation
{
    public class ResourceKeyStack : ICloneable
    {
        private ITagGroup _group;
        private IList<ITag> _tags;
        private PropertyInfo _property;
        private IExpressionParser _expression;
        private IFunctionDefinition _function;
        private ResourceBundle _bundle;
        private static readonly CultureInfo CULTURE= CultureInfo.InvariantCulture;

        public ResourceKeyStack() : this(null)
        {
        }

        public ResourceKeyStack(ResourceBundle bundle)
        {
            _bundle = bundle;
            _tags= new List<ITag>();
        }

        public ITagGroup Group
        {
            get { return _group; }
        }

        
        public IEnumerable<ITag> Tags
        {
            get { return _tags; }
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

        public string Id
        {
            get
            {
                StringBuilder result = new StringBuilder("id");
                AppendExpression(result);
                AppendFunction(result);
                AppendGroup(result);
                return result.ToString();
            }
        }

        public DescriptionAttribute Description => new DescriptionAttribute(Resource(DescriptionKey));

        public string DescriptionKey
        {
            get { 
                StringBuilder result = new StringBuilder("description");
                AppendExpression(result);
                AppendFunction(result);
                AppendGroup(result);
                return result.ToString();
            }
        }

        public string NoteKey => DescriptionKey + "_Note";

        public string ExampleKey => DescriptionKey + "_Example";

        public string TitleKey => DescriptionKey + "_Title";

        public string Title => Resource(TitleKey);

        public string Example => Resource(ExampleKey);
        public string Note => Resource(NoteKey);

        private string Resource(string key)
        {
            return _bundle.Get(key, CULTURE).ToString();
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
                if (!_tags.Any() ||
                   _property == null ||
                   Equals(_tags.Last().GetType(), _property.DeclaringType)
                   )
                {
                    result.Append("_" + _group.GetType().Name);
                }
                AppendTag(result);
            }
        }

        private void AppendTag(StringBuilder result)
        {

            if (_tags.Any())
            {
                if (_tags.Count > 1)
                {
                    foreach (var tag in _tags.Take(_tags.Count - 1))
                    {
                        result.Append("_" + tag.GetType().Name);
                    }
                }
                var descriptionType = _tags.Last().GetType();
                if (_property != null)
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
            branch._tags = new List<ITag>(_tags);
            branch._tags.Add(tag);
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
            branch._tags = new List<ITag>();
            return Resource(branch.DescriptionKey + "_" + categoryStr);
        }


//        public void AddTranslation(string value)
//        {
//            if (_extensions.ContainsKey(Description))
//            {
//                if (_extensions[Description].Equals(value)) return; //make life easy
//                throw new ArgumentException($"There's already a translation with key {Description}");
//            }
//            _extensions.Add(Description, value);
//        }
//
//        public void AddNoteTranslation(string value)
//        {
//            _extensions.Add(NoteKey, value);
//        }
//
//        public void AddExampleTranslation(string value)
//        {
//            _extensions.Add(ExampleKey, value);
//        }
//
//        public void AddTitleTranslation(string value)
//        {
//            _extensions.Add(TitleKey, value);
//        }
    }
}

