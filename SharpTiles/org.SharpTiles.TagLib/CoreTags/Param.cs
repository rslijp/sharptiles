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
using System.Text;
using System.Web;

namespace org.SharpTiles.Tags.CoreTags
{
    [Category("URLRelated")]
    public class Param : BaseCoreTag, ITag
    {
        public static readonly string NAME = "param";
        private ITagAttribute _name;


        [Required]
        public ITagAttribute Name
        {
            get { return _name; }
            set { _name = value; }
        }

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
            throw TagException.NotAllowedShouldBePartOf(GetType(), typeof (Import), typeof (Url)).Decorate(Context);
        }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.Free; }
        }

        #endregion

        public string EvaluateNested(TagModel model)
        {
            var sb = new StringBuilder();
            string paramStr = GetAsString(_name, model) ?? String.Empty;
            if (!String.IsNullOrEmpty(paramStr))
            {
                ITagAttribute valueAttribute = Value ?? Body;
                string valueStr = GetAsString(valueAttribute, model) ??  String.Empty;
                sb.Append(paramStr);
                sb.Append("=");
                sb.Append(HttpUtility.UrlPathEncode(valueStr));
            }
            return sb.ToString();
        }
    }
}
