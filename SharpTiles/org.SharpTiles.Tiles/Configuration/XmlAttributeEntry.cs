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
using System.Xml.Serialization;
using org.SharpTiles.Common;

namespace org.SharpTiles.Tiles.Configuration
{
    public class XmlAttributeEntry : IAttributeEntry
    {
        #region IXmlAttributeEntry Members

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }

        public TileType? TileType
        {
            get { return !String.IsNullOrEmpty(Type) ? EnumParser<TileType>.Parse(Type) : default(TileType?); }
        }


        [XmlAttribute("type")]
        public string Type { get; set; }

        #endregion
    }
}
