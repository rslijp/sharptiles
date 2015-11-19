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
using System.IO;
using System.Reflection;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.Creators;

namespace org.SharpTiles.Templates.Templates
{
    public class AssemblyLocatorFactory : IResourceLocatorFactory
    {
        private const string CONFIG_FILE = "tiles.xml";
        private readonly String _filePrefix;
        private readonly Assembly _inAssembly;
        private ITagLib _lib;

        public AssemblyLocatorFactory(Assembly inAssembly, string filePrefix)
        {
            _lib = new TagLib();
            _inAssembly = inAssembly;
            _filePrefix = filePrefix;
        }

        #region IResourceLocatorFactory Members

        public Stream GetConfiguration()
        {
            string configuration = ScanForTilesConfiguration(_inAssembly);
            if (configuration == null)
            {
                throw ResourceException.ConfigFileNotFoundInAssembly(CONFIG_FILE, _inAssembly);
            }
            return GetNewLocator().GetStream(configuration);
        }


        public IResourceLocator GetNewLocator()
        {
            return new AssemblyBasedResourceLocator(_inAssembly, _filePrefix);
        }

        public ITemplate Handle(String entry, bool throwException)
        {
            return Handle(entry, GetNewLocator(), throwException);
        }

        public ITemplate Handle(String entry, IResourceLocator locator, bool throwException)
        {
            if (throwException || locator.Exists(entry))
            {
                return new ResourceTemplate(_lib,locator, this, entry);
            }
            return null;
        }

        public ITagLib Lib
        {
            get { return _lib; } 
        }


        public IResourceLocatorFactory CloneForTagLib(ITagLib lib)
        {
            var f = new AssemblyLocatorFactory(_inAssembly, _filePrefix){ _lib = lib };
            return f;
        }

        public DateTime? ConfigurationLastModified
        {
            get { return default(DateTime?); }
        }

        #endregion

        private static string ScanForTilesConfiguration(Assembly assembly)
        {
            string configuration = null;
            foreach (string candidate in assembly.GetManifestResourceNames())
            {
                if (candidate.EndsWith(CONFIG_FILE))
                {
                    configuration = candidate;
                }
            }
            return configuration;
        }
    }
}