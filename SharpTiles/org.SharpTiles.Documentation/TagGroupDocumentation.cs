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
using org.SharpTiles.Documentation.DocumentationAttributes;
using org.SharpTiles.Tags;

namespace org.SharpTiles.Documentation
{
    public class TagGroupDocumentation : IDescriptionElement
    {
        private readonly ResourceKeyStack _messagePath;
        private readonly string _name;
        private readonly IList<TagDocumentation> _tags;
        private bool _hasExample;
        private bool _hasNote;
        private IList<Func<ITag, TagDocumentation, bool>> _specials;

        public TagGroupDocumentation(ResourceKeyStack messagePath, ITagGroup tagGroup, IList<Func<ITag, TagDocumentation, bool>> specials)
        {
            _messagePath = messagePath.BranchFor(tagGroup);
            _name = tagGroup.Name;
            _specials = specials;
            _tags = new List<TagDocumentation>();
            var tagGroupType=tagGroup.GetType();
            DescriptionAttribute.Harvest(_messagePath, tagGroupType);
            TitleAttribute.HarvestTagLibrary(_messagePath, tagGroupType);
            _hasExample = ExampleAttribute.Harvest(_messagePath, tagGroupType) ||HasExample.Has(tagGroupType);
            _hasNote = NoteAttribute.Harvest(_messagePath, tagGroupType)||HasNote.Has(tagGroupType);
            foreach (ITag _tag in tagGroup)
            {
                _tags.Add(new TagDocumentation(_messagePath, _tag, _specials));
            }
            Examples = ExampleAttribute.HarvestTags(tagGroupType);
            Notes = NoteAttribute.HarvestTags(tagGroupType);

        }

        public NoteAttribute[] Notes { get; set; }

        public ExampleAttribute[] Examples { get; set; }


        public IList<TagDocumentation> Tags => _tags;

        #region IDescriptionElement Members

        public string Name => _name;

        public string Id => _messagePath.Id;

        public string DescriptionKey => _messagePath.Description;

        public string ExampleKey => _hasExample ? _messagePath.ExampleKey : null;

        public string NoteKey => _hasNote ? _messagePath.NoteKey : null;

        #endregion
    }
}
