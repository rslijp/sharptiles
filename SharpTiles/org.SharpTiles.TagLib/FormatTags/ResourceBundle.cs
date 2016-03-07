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
using System.Globalization;
using System.Resources;
using System.Threading;
using org.SharpTiles.Common;

namespace org.SharpTiles.Tags.FormatTags
{
    public class ResourceBundle : IResourceBundle
    {
        private static readonly IDictionary<string, ResourceManager> CACHE = new Dictionary<string, ResourceManager>();
        private readonly string _baseName;
        private readonly string _prefix;
        private readonly ResourceManager _resourceManager;

        public ResourceBundle(string baseName, string prefix) : this(baseName, prefix, new FileBasedResourceLocator())
        {
            
        }        
        
        public ResourceBundle(string baseName, string prefix, IResourceLocator locator)
        {
            _baseName = baseName;
            _resourceManager = LoadResourceFileWithCache(baseName, locator);
            _prefix = prefix ?? "";
        }

        public ResourceBundle(string baseName, string prefix, ResourceManager manager)
        {
            _baseName = baseName;
            _resourceManager = manager;
            _prefix = prefix ?? "";
        }

        public string BaseName
        {
            get { return _baseName; }
        }

        public string Prefix
        {
            get { return _prefix; }
        }

        public ResourceManager ResourceManager
        {
            get { return _resourceManager; }
        }

        private static ResourceManager LoadResourceFileWithCache(string filePath, IResourceLocator locator)
        {
            if (!CACHE.ContainsKey(filePath))
            {
                CACHE.Add(filePath, locator.LoadBundle(filePath));
            }
            return CACHE[filePath];
        }

        
        
        public object Get(string key, CultureInfo info, params object[] replacements)
        {
            string msg = ResourceManager.GetString(_prefix + key, info ?? Thread.CurrentThread.CurrentCulture);
            return msg != null ? String.Format(msg, replacements) : "?" + key + "?";
        }

        public object Get(string key, TagModel model, params object[] replacements)
        {
            return Get(key, GetLocale(model), replacements);
        }

        public bool Contains(string key)
        {
            return ResourceManager.GetObject(key)==null;
        }

        public static object GetMessage(string key, TagModel model, params object[] replacements)
        {
            var bundle = (ResourceBundle) model.SearchInTagScope(FormatConstants.BUNDLE);
            return bundle.Get(key, model);
        }

        public static CultureInfo GetLocale(TagModel model)
        {
            return (CultureInfo) model.Resolve(FormatConstants.LOCALE, false);
        }
    }

    public interface IResourceBundle
    {
        object Get(string key, CultureInfo info, params object[] replacements);

        object Get(string key, TagModel model, params object[] replacements);

        bool Contains(string key);

    }
}
