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
        public TagGroupDocumentation(ResourceKeyStack messagePath, ITagGroup tagGroup)
        {
            _messagePath = messagePath.BranchFor(tagGroup);
            _name = tagGroup.Name;
            _tags = new List<TagDocumentation>();
            _hasExample = HasExample.Has(tagGroup.GetType());
            _hasNote = HasNote.Has(tagGroup.GetType());
            foreach (ITag _tag in tagGroup)
            {
                _tags.Add(new TagDocumentation(_messagePath, _tag));
            }
        }

        public IList<TagDocumentation> Tags
        {
            get { return _tags; }
        }

        #region IDescriptionElement Members

        public string Name
        {
            get { return _name; }
        }

        public string DescriptionKey
        {
            get { return _messagePath.Description; }
        }

        public string ExampleKey
        {
            get
            {
                return _hasExample ? DescriptionKey + "_Example" : null;
            }
        }

        public string NoteKey
        {
            get
            {
                return _hasNote ? DescriptionKey + "_Note" : null;
            }
        }


        #endregion
    }
}