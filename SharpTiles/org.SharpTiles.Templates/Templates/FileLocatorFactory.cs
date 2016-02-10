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
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.Creators;

namespace org.SharpTiles.Templates.Templates
{
    public class FileLocatorFactory : IResourceLocatorFactory
    {
        protected String _filePrefix;
        private readonly String _filePath;
        private ITagLib _lib;

        public FileLocatorFactory()
        {
            _lib=new TagLib();
        }

        public FileLocatorFactory(string filePrefix) : this()
        {
            _filePrefix = filePrefix;
        }

        public FileLocatorFactory(string filePath, string filePrefix) : this(filePrefix)
        {
            _filePath = filePath;
        }

        public ITagLib Lib
        {
            get { return _lib; }
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

        public IResourceLocatorFactory CloneForTagLib(ITagLib lib)
        {
            var f = new FileLocatorFactory(_filePath,_filePrefix) {_lib=lib??_lib};
            return f;
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