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
using org.SharpTiles.Common;

namespace org.SharpTiles.Tags.Templates.SharpTags
{
    public class Marker : ITag
    {
        public static readonly string NAME = "marker";
        
        [Internal]
        public ITagAttribute Body { get; set; }

        #region ITag Members

        public ParseContext Context { get; set; }

        public string TagName
        {
            get { return NAME; }
        }

        [Required]
        public ITagAttribute Id { get; set; }


        public string Evaluate(TagModel model)
        {
            object result = Body != null ? Body.Evaluate(model) : String.Empty;
            return result != null ? result.ToString() : null;
        }

        public ITagAttributeSetter AttributeSetter
        {
            get { return new ReflectionAttributeSetter(this); }
        }

        public TagState State { get; set; }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.Free; }
        }

        
        #endregion
    }
}
