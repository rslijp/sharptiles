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
using org.SharpTiles.Tags;

namespace org.SharpTiles.HtmlTags
{
    public class FixedParameterValue : IParameterValueWithType
    {
        public object FixedValue { get; set; }

        #region IParameterValueWithType Members

        public string Name { get; set; }

        public bool ExactMatch { get; set; }

        public object Value(TagModel model)
        {
            return FixedValue;
        }

        public Type Type
        {
            get { return FixedValue.GetType(); }
        }

        #endregion
    }
}
