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
using System.Collections.Generic;
using System.Globalization;

namespace org.SharpTiles.Tags.FormatTags
{
    public class ExtendableResourceBundle : IResourceBundle
    {
        private readonly IResourceBundle _baseBundle;
        private readonly IDictionary<string, string> _extensionBundle;
         
        public ExtendableResourceBundle(IResourceBundle baseBundle, IDictionary<string, string> extensionBundle)
        {
            _baseBundle = baseBundle;
            _extensionBundle = extensionBundle;
        }

        public object Get(string key, CultureInfo info, params object[] replacements)
        {
            if (_extensionBundle.ContainsKey(key))
            {
                var value = _extensionBundle[key];
                return string.Format(value, replacements);
            }
            return _baseBundle.Get(key, info, replacements);
        }

        public object Get(string key, TagModel model, params object[] replacements)
        {
            if (_extensionBundle.ContainsKey(key))
            {
                var value = _extensionBundle[key];
                return string.Format(value, replacements);
            }
            return _baseBundle.Get(key, model, replacements);
        }

        public bool Contains(string key)
        {
            if (_extensionBundle.ContainsKey(key)) return true;
            return _baseBundle.Contains(key);
        }
    }
}
