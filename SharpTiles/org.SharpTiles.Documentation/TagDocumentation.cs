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
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using org.SharpTiles.Common;
using org.SharpTiles.Documentation.DocumentationAttributes;
using org.SharpTiles.Tags;
using DescriptionAttribute = org.SharpTiles.Documentation.DocumentationAttributes.DescriptionAttribute;

namespace org.SharpTiles.Documentation
{
    [DataContract]
    public class TagDocumentation : IDescriptionElement
    {
        private readonly Dictionary<string, TagDocumentation> _tagDictionary;
        private readonly IList<string> _nested;
        private readonly IList<PropertyDocumentation> _list;
        private readonly IList<FunctionDocumentation> _methods;
        private readonly ResourceKeyStack _messagePath;
        private readonly string _name;
        private readonly CategoryAttribute _category;
        private readonly List<NoteValue> _notes = new List<NoteValue>();
        private readonly List<ExampleValue> _examples = new List<ExampleValue>();
        private readonly DescriptionValue _description;

        public TagDocumentation(ResourceKeyStack messagePath, ITag tag,  IList<Func<ITag, TagDocumentation,bool>> specials, Dictionary<string,TagDocumentation> tagDictionary)
        {
            _tagDictionary = tagDictionary;
            _messagePath = messagePath.BranchFor(tag);
            _name = tag.TagName;

            var tagType = tag.GetType();

            _category = CategoryHelper.GetCategory(tagType);

            _list = new List<PropertyDocumentation>();
            _nested = new List<string>();
            _methods = new List<FunctionDocumentation>();
            TagBodyMode = tag.TagBodyMode;

            foreach (var property in tagType.GetCustomAttributes<PropertyAttribute>())
            {
                _list.Add(new PropertyDocumentation(_messagePath, property));
            }

            foreach (var property in tagType.GetProperties(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.SetProperty |
                BindingFlags.FlattenHierarchy))
            {
                if (Equals(property.PropertyType, typeof(ITagAttribute)) &&
                    !IsInternal(property))
                {
                    _list.Add(new PropertyDocumentation(_messagePath, property));
                }
            }
            var extendingTag = tag as ITagExtendTagLib;
            if (extendingTag != null)
            {
                foreach (var nested in extendingTag.TagLibExtension)
                {
                    var hash = nested.GetType().GetHashCode().ToString();
                    if (!_tagDictionary.ContainsKey(hash))
                    {
                        _tagDictionary[hash] = null;
                        var tagDoc = new TagDocumentation(_messagePath, nested, specials, _tagDictionary);
                        _tagDictionary[hash] = tagDoc;
                    }
                    _nested.Add(hash);
                }
            }
            var instanceDocumentation = tag as IInstanceTagDocumentation;
            if (instanceDocumentation!=null)
            {
                _description = instanceDocumentation.Description;
                _examples.AddRange(instanceDocumentation.Examples?? new ExampleValue[] {});
                _notes.AddRange(instanceDocumentation.Notes ?? new NoteValue[] {});
            }
            else
            {
                _description = DescriptionAttribute.Harvest(tagType) ?? _messagePath.Description;

                if (ExampleAttribute.Harvest(tagType))
                {
                    _examples.AddRange(ExampleAttribute.HarvestTags(tagType));
                }
                if (HasExample.Has(tagType))
                {
                    _examples.Add(new ExampleValue(_messagePath.Example));
                }
                if (NoteAttribute.Harvest(tagType))
                {
                    _notes.AddRange(NoteAttribute.HarvestTags(tagType));
                }
                if (HasNote.Has(tagType))
                {
                    _notes.Add(new NoteValue(_messagePath.Note));
                }
            }
        }


        [ScriptIgnore]
        public ResourceKeyStack MessagePath => _messagePath;

        [DataMember]
        public IList<PropertyDocumentation> Properties => _list;

        [ScriptIgnore]
        public IList<TagDocumentation> NestedTags => _nested.Select(t => _tagDictionary[t]).ToList();

        [DataMember]
        public IList<string> NestedTagIds => _nested;

        [DataMember]
        public IList<FunctionDocumentation> Methods => _methods;

        [DataMember]
        public string Category => _category?.Category;

        [DataMember]
        public TagBodyMode TagBodyMode { get; }

        [DataMember]
        public string CategoryDescription
        {
            get
            {
                if (_category != null)
                {
                    return _messagePath.DescriptionForCategory(_category.Category);
                }
                return null;
            }
        }


    #region IDescriptionElement Members

        [DataMember]
        public string Id => _messagePath.Id;

        [DataMember]
        public DescriptionValue Description => _description;
        [ScriptIgnore]
        public string DescriptionKey => _messagePath.DescriptionKey;

        [DataMember]
        public string Name => _name;

        [DataMember]
        public ExampleValue[] Examples => _examples.ToArray();

        [DataMember]
        public NoteValue[] Notes => _notes.ToArray();
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
