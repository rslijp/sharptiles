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

namespace org.SharpTiles.Tags.CoreTags
{
    [Category("Conditional"), HasExample]
    public class Choose : BaseCoreTag, ITagWithNestedTags
    {
        public static readonly string NAME = "choose";

        private readonly IList<ITag> _nestedTags = new List<ITag>();

        #region ITagWithNestedTags Members

        public string TagName
        {
            get { return NAME; }
        }

        public string Evaluate(TagModel model)
        {
            ChooseItemCoreTag applicable = null;
            foreach (ITag tag in _nestedTags)
            {
                var itemTag = (ChooseItemCoreTag) tag;
                if (itemTag is Otherwise && _nestedTags.IndexOf((ITag) itemTag) != _nestedTags.Count - 1)
                {
                    throw TagException.ShouldBeLast(typeof (Otherwise)).Decorate(itemTag.Context);
                }
                if (itemTag.Applies(model))
                {
                    applicable = itemTag;
                    break;
                }
            }
            return applicable != null ? GetAsString(applicable.Body, model) : String.Empty;
        }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.NestedTags; }
        }

        public void AddNestedTag(ITag tag)
        {
            if (tag is When || tag is Otherwise)
            {
                _nestedTags.Add(tag);
            }
            else
            {
                throw TagException.OnlyNestedTagsOfTypeAllowed(tag.GetType(), typeof (When), typeof (Otherwise)).
                    Decorate(tag.Context);
            }
        }

        #endregion
    }
}
