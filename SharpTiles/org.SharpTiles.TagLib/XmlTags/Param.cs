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
using org.SharpTiles.Common;
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Tags.XmlTags
{
    [Category("Transform"), HasExample]
    public class Param : BaseCoreTag, ITag
    {
        public static readonly string NAME = "param";


        [Required]
        public ITagAttribute Name { get; set; }

        [Internal]
        public ITagAttribute Body { get; set; }

        [TagDefaultProperty("Body")]
        public ITagAttribute Value { get; set; }

        public ITagAttribute NameSpaceUri { get; set; }

        #region ITag Members

        public string TagName
        {
            get { return NAME; }
        }

        public string Evaluate(TagModel model)
        {
            throw TagException.NotAllowedShouldBePartOf(GetType(), typeof (Transform)).Decorate(Context);
        }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.Free; }
        }

        #endregion

        public XsltParameter EvaluateNested(TagModel model)
        {
            string name = GetAsString(Name, model) ?? String.Empty;
            string nameSpaceUri = GetAsString(NameSpaceUri, model) ?? String.Empty;
            object value = GetAutoValue("Value", model) ?? String.Empty;
            return new XsltParameter(name, nameSpaceUri, value);
        }
    }
}
