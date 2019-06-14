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
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using org.SharpTiles.Documentation.DocumentationAttributes;
using org.SharpTiles.Tags;

namespace org.SharpTiles.Documentation
{
    [DataContract]
    public class TagGroupDocumentation : IDescriptionElement
    {
        private readonly ResourceKeyStack _messagePath;
        private readonly string _name;
        private readonly IList<string> _tags;
        private IList<Func<ITag, TagDocumentation, bool>> _specials;
        private readonly Dictionary<string, TagDocumentation> _tagDictionary;
        private readonly List<NoteValue> _notes = new List<NoteValue>();
        private readonly List<ExampleValue> _examples = new List<ExampleValue>();
        private readonly DescriptionValue _description;
        private readonly string _title;

        public TagGroupDocumentation(ResourceKeyStack messagePath, ITagGroup tagGroup, IList<Func<ITag, TagDocumentation, bool>> specials, Dictionary<string,TagDocumentation> tagDictionary)
        {
            _messagePath = messagePath.BranchFor(tagGroup);
            _name = tagGroup.Name;
            _specials = specials;
            _tagDictionary = tagDictionary;
            _tags = new List<string>();
            var tagGroupType = tagGroup.GetType();
            _description = DescriptionAttribute.Harvest(tagGroupType)?? _messagePath.Description;

            _title = TitleAttribute.HarvestTagLibrary(tagGroupType);
            foreach (ITag _tag in tagGroup)
            {
                var hash = _tag.GetType().GetHashCode().ToString();
                if (!_tagDictionary.ContainsKey(hash))
                {
                    _tagDictionary[hash] = null;
                    var tagDoc = new TagDocumentation(_messagePath, _tag, _specials, _tagDictionary);
                    _tagDictionary[hash] = tagDoc;
                }
                _tags.Add(hash);
            }
            if (ExampleAttribute.Harvest(tagGroupType))
            {
                _examples.AddRange(ExampleAttribute.HarvestTags(tagGroupType));
            }
            if (HasExample.Has(tagGroupType))
            {
                _examples.Add(new ExampleValue(_messagePath.Example));
            }
            if (NoteAttribute.Harvest(tagGroupType))
            {
                _notes.AddRange(NoteAttribute.HarvestTags(tagGroupType));
            }
            if (HasNote.Has(tagGroupType))
            {
                _notes.Add(new NoteValue(_messagePath.Note));
            }
        }


        [DataMember]
        public ExampleValue[] Examples => _examples.ToArray();

        [DataMember]
        public NoteValue[] Notes => _notes.ToArray();

        [ScriptIgnore]
        public IList<TagDocumentation> Tags => _tags.Select(t => _tagDictionary[t]).ToList();


        [DataMember]
        public IList<string> TagIds => _tags;

        #region IDescriptionElement Members

        [DataMember]
        public string Name => _name;

        [DataMember]
        public string Id => _messagePath.Id;

        [DataMember]
        public DescriptionValue Description => _description;

        [ScriptIgnore]
        public string DescriptionKey => _messagePath.DescriptionKey;

        [DataMember]
        public string Title => _title;

        #endregion
    }
}
