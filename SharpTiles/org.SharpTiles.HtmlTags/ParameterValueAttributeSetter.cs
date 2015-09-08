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
using org.SharpTiles.Common;
using org.SharpTiles.Tags;

namespace org.SharpTiles.HtmlTags
{
    public class ParameterValueAttributeSetter : ITagAttributeSetter 
    {
        private readonly List<ParameterValue> _attributes;
        private readonly IHtmlTag _tag;
        public const String TAG_BODY_NAME = "Body";

        public ParameterValueAttributeSetter(IHtmlTag tag)
        {
            _tag = tag;
            _attributes = new List<ParameterValue>();
        }

        #region ITagAttributeSetter Members

        public ITagAttribute this[string property]
        {
            get
            {
                ParameterValue attribute = _attributes.SingleOrDefault(p => Equals(p.Name, property));
                return attribute != null ? attribute.Attribute : null;
            }
            set
            {
                if (IsBody(property))
                {
                    HandleBody(value);
                }
                else
                {
                    _attributes.Add(new ParameterValue {Name = property, Attribute = value});
                }
            }
        }

        private void HandleBody(ITagAttribute value)
        {
            new Reflection(_tag)[TAG_BODY_NAME] = value;
        }

        private static bool IsBody(string property)
        {
            return property.ToUpper().Equals(TAG_BODY_NAME.ToUpper());
        }



        public void InitComplete()
        {
            _tag.Parameters = _attributes.Cast<IParameterValue>().ToList();
            _tag.ApplyHtmlAttributes();
        }

        #endregion
    }
}
