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
 using System.ComponentModel;
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Tags.FormatTags
{
    [Category("Internationalization"), HasExample]
    public class SetBundle : BaseCoreTagWithVariable, ITag
    {
        public static readonly string NAME = "setBundle";

        [Required]
        public ITagAttribute BaseName { get; set; }

        #region ITag Members

        public string TagName
        {
            get { return NAME; }
        }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.None; }
        }

        #endregion

        public override object InternalEvaluate(TagModel model)
        {
            string baseName = GetAsString(BaseName, model);
            return new ResourceBundle(baseName, null);
        }
    }
}
