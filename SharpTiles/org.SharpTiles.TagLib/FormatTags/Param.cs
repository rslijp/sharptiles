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
using System.ComponentModel;
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Tags.FormatTags
{
    [Category("Internationalization")]
    public class Param : BaseCoreTag, ITag
    {
        public static readonly string NAME = "param";

        [Internal]
        public ITagAttribute Body { get; set; }

        public ITagAttribute Value { get; set; }

        #region ITag Members

        public string TagName
        {
            get { return NAME; }
        }

        public string Evaluate(TagModel model)
        {
            throw TagException.NotAllowedShouldBePartOf(GetType(), typeof (Message)).Decorate(Context);
        }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.Free; }
        }

        #endregion

        public object EvaluateNested(TagModel model)
        {
            ITagAttribute valueAttribute = Value ?? Body;
            return Get(valueAttribute, model) ?? String.Empty;
        }
    }
}
