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
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using org.SharpTiles.HtmlTags;
 using org.SharpTiles.HtmlTags.Input;
 using org.SharpTiles.Tags;

namespace org.SharpTiles.Documentation
{
    public class TagDocumentation : IDescriptionElement
    {
        private readonly IList<PropertyDocumentation> _list;
        private readonly IList<FunctionDocumentation> _methods;
        private readonly ResourceKeyStack _messagePath;
        private readonly string _name;
        private readonly CategoryAttribute _category;
        private readonly bool _hasNote;
        private readonly bool _hasExample;

        public TagDocumentation(ResourceKeyStack messagePath, ITag tag)
        {
            _messagePath = messagePath.BranchFor(tag);
            _name = tag.TagName;
            _category = CategoryHelper.GetCategory(tag.GetType());
            _hasExample = HasExample.Has(tag.GetType());
            _hasNote = HasNote.Has(tag.GetType());
            _list = new List<PropertyDocumentation>();
            _methods = new List<FunctionDocumentation>();
            if (tag is HtmlHelperWrapperTag)
            {
                var htmlTag = (HtmlHelperWrapperTag)tag;
                var htmlHelper = new HtmlReflectionHelper(htmlTag.WrappedType, htmlTag.MethodName);
                foreach (var method in htmlHelper.AllMethods)
                {
                    _methods.Add(new FunctionDocumentation(_messagePath, new WrappedFunctionDocumentation(method)));
                }
            } else 
           {
               foreach (var property in tag.GetType().GetProperties(
                   BindingFlags.Instance |
                   BindingFlags.Public |
                   BindingFlags.SetProperty |
                   BindingFlags.FlattenHierarchy))
               {
                   if (Equals(property.PropertyType, typeof(ITagAttribute)) && !IsInternal(property))
                   {
                       _list.Add(new PropertyDocumentation(_messagePath, property));
                   }
               }
           }
        }

        public IList<PropertyDocumentation> Properties
        {
            get { return _list; }
        }

        public IList<FunctionDocumentation> Methods
        {
            get { return _methods; }
        }

        public CategoryAttribute Category
        {
            get { return _category; }
        }

        #region IDescriptionElement Members

        public string DescriptionKey
        {
            get { return _messagePath.Description; }
        }


        public string NoteKey
        {
            get
            {
                return _hasNote ? DescriptionKey +"_Note" : null;
            }
        }

        public string ExampleKey
        {
            get
            {
                return _hasExample ? DescriptionKey + "_Example" : null;
            }
        }

        public string CategoryDescriptionKey
        {
            get
            {
                if (Category != null)
                {
                    string categoryStr = Category.Category;
                    return _messagePath.DescriptionForCategory(categoryStr);
                } else
                {
                    return null;
                }
            }
        }
        
        public string Name
        {
            get { return _name; }
        }

        #endregion

        private bool IsInternal(PropertyInfo property)
        {
            bool isInternal = false;
            foreach (object attribute in property.GetCustomAttributes(false))
            {
                isInternal |= attribute is InternalAttribute;
            }
            return isInternal;
        }
    }
}