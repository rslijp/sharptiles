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
using System.Text;

namespace org.SharpTiles.Tags.CoreTags
{
    public abstract class BaseUrlTag : BaseCoreTag, ITagWithNestedTags, ITagExtendTagLib
    {
        private readonly IList<Param> _nestedTags = new List<Param>();
        private static Type[] EXTENSION = new[] { typeof(Param) };

        #region ITagWithNestedTags Members

        public abstract string TagName { get; }

        public abstract string Evaluate(TagModel model);

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.NestedTags; }
        }

        public void AddNestedTag(ITag tag)
        {
            if (tag is Param)
            {
                _nestedTags.Add((Param) tag);
            }
            else
            {
                throw TagException.OnlyNestedTagsOfTypeAllowed(tag.GetType(), typeof (Param)).Decorate(tag.Context);
            }
        }

        #endregion

        public static string GetAsUrl(ITagAttribute expressions, TagModel model)
        {
            var result = GetAsString(expressions, model);
            if (result!=null && result.StartsWith("~"))
            {
                var relativePart = result.Substring(1);
                var appName = model.ApplicationName;
                result = "";
                if(!"/".Equals(appName))
                {
                    result += model.ApplicationName;
                }
                result += relativePart;
            }
            return result;
        }

        public string ParamsEvaluate(TagModel model)
        {
            bool first = true;
            var builder = new StringBuilder();
            foreach (Param tag in _nestedTags)
            {
                if (first)
                {
                    builder.Append("?");
                }
                else
                {
                    builder.Append("&");
                }
                first = false;
                builder.Append(tag.EvaluateNested(model));
            }
            return builder.ToString();
        }

        public ITagGroup TagLibExtension
        {
            get { return new NestedTagGroup(Group.Name, EXTENSION); }
        }
    }
}
