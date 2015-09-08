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
 */using System;
using System.Collections.Generic;
using System.Linq;
using org.SharpTiles.Tags;

namespace org.SharpTiles.HtmlTags
{
    public class HtmlAttributesParameterValue : IParameterValueWithType
    {
        private readonly IDictionary<string, IParameterValue> _attributes;

        public HtmlAttributesParameterValue(IDictionary<String, IParameterValue> attributes)
        {
            _attributes = attributes;
        }

        #region IParameterValueWithType Members

        public string Name
        {
            get { return Html.HTMLATTRIBUTES_PARAM_NAME; }
        }

        public IDictionary<string, object> TypedValue(TagModel model)
        {
            return _attributes.ToDictionary(pair => pair.Key, pair => pair.Value.Value(model));
        }

        public object Value(TagModel model)
        {
            return TypedValue(model);
        }

        public Type Type
        {
            get { return Html.HTMLATTRIBUTES_PARAM_TYPE; }
        }

        public bool ExactMatch
        {
            get { return true; }
        }

        #endregion
    }
}
