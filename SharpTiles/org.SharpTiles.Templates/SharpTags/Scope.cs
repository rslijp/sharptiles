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
 using org.SharpTiles.Tags;
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Templates.SharpTags
{
    public class Scope : BaseCoreTag, ITag
    {
        public static readonly string NAME = "scope";
        
        [Internal]
        public ITagAttribute Body { get; set; }

        public string Evaluate(TagModel model)
        {
            model.PushTagStack();
            object result = Body != null ? Body.Evaluate(model) : String.Empty;
            model.PopTagStack();
            return result != null ? result.ToString() : null;
        }

        public string TagName
        {
            get { return NAME; }
        }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.Free; }
        }
    }
}

