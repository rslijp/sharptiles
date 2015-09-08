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
 namespace org.SharpTiles.Tags.XmlTags
{
    public class XsltParameter
    {
        private readonly string _name;
        private readonly string _nameSpaceUri;
        private readonly object _value;


        public XsltParameter(string name, string nameSpaceUri, object value)
        {
            _name = name;
            _nameSpaceUri = nameSpaceUri;
            _value = value;
        }


        public string Name
        {
            get { return _name; }
        }

        public string NameSpaceUri
        {
            get { return _nameSpaceUri; }
        }

        public object Value
        {
            get { return _value; }
        }
    }
}
