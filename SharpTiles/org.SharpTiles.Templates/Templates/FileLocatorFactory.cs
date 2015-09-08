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
using System.IO;
using org.SharpTiles.Common;
using org.SharpTiles.Tags.Creators;

namespace org.SharpTiles.Templates.Templates
{
    public class FileLocatorFactory : IResourceLocatorFactory
    {
        protected String _filePrefix;
        private readonly String _filePath;

        public FileLocatorFactory()
        {
        }

        public FileLocatorFactory(string filePrefix)
        {
            _filePrefix = filePrefix;
        }

        public FileLocatorFactory(string filePath, string filePrefix)
        {
            _filePath = filePath;
            _filePrefix = filePrefix;
        }

        public ITemplate Handle(String entry, bool throwException)
        {
            return Handle(entry, GetNewLocator(), throwException);
        }

        public ITemplate Handle(String entry, IResourceLocator locator, bool throwException)
        {    if (throwException || locator.Exists(entry))
        {
            return new FileTemplate(locator, this, locator.PreFixed(entry));
        }
            return null;
        }

        public Stream GetConfiguration()
        {
            return GetNewLocator().GetStream(_filePath);
        }

        public DateTime? ConfigurationLastModified
        {
            get { return _filePath != null ? GetNewLocator().LastModified(_filePath) : default(DateTime?); }
        }

        public virtual IResourceLocator GetNewLocator()
        {
            return new FileBasedResourceLocator(_filePrefix);
        }
    }
}