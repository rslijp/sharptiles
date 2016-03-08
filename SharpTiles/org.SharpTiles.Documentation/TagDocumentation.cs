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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using org.SharpTiles.Documentation.DocumentationAttributes;
using org.SharpTiles.Tags;
using DescriptionAttribute = org.SharpTiles.Documentation.DocumentationAttributes.DescriptionAttribute;

namespace org.SharpTiles.Documentation
{
    public class TagDocumentation : IDescriptionElement
    {
        private readonly IList<TagDocumentation> _nested;
        private readonly IList<PropertyDocumentation> _list;
        private readonly IList<FunctionDocumentation> _methods;
        private readonly ResourceKeyStack _messagePath;
        private readonly string _name;
        private readonly CategoryAttribute _category;
        private readonly bool _hasNote;
        private readonly bool _hasExample;

        public TagDocumentation(ResourceKeyStack messagePath, ITag tag,  IList<Func<ITag, TagDocumentation,bool>> specials)
        {
            _messagePath = messagePath.BranchFor(tag);
            _name = tag.TagName;

            var tagType = tag.GetType();
            DescriptionAttribute.Harvest(_messagePath, tagType);
            _category = CategoryHelper.GetCategory(tagType);
            _hasExample = ExampleAttribute.Harvest(_messagePath, tagType) || HasExample.Has(tagType);
            _hasNote = NoteAttribute.Harvest(_messagePath, tagType) || HasNote.Has(tagType);
            _list = new List<PropertyDocumentation>();
            _nested = new List<TagDocumentation>();
            _methods = new List<FunctionDocumentation>();
            if(specials.Any(s=>s(tag, this))) return;
            foreach (var property in tagType.GetProperties(
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
            var extendingTag = tag as ITagExtendTagLib;
            if (extendingTag != null)
            {
                foreach (var nested in extendingTag.TagLibExtension)
                {
                    _nested.Add(new TagDocumentation(_messagePath, nested, specials));
                }
            }
            Examples = ExampleAttribute.HarvestTags(tagType);
            Notes = NoteAttribute.HarvestTags(tagType);
        }

        public NoteAttribute[] Notes { get; set; }

        public ResourceKeyStack MessagePath => _messagePath;
        
        public IList<PropertyDocumentation> Properties
        {
            get { return _list; }
        }

        public IList<TagDocumentation> NestedTags
        {
            get { return _nested; }
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
                return _hasExample ? _messagePath.ExampleKey : null;
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

        public ExampleAttribute[] Examples { get; }
    }
}
