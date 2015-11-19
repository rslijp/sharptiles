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
using org.SharpTiles.Templates;

namespace org.SharpTiles.Templates.Templates
{
    public class ResourceTemplate : ITemplate
    {
        private readonly string _path;
        private ParsedTemplate _template;
        private readonly IResourceLocator _resource;
        private IResourceLocatorFactory _factory;
        private ITagLib _lib;

        public ResourceTemplate(ITagLib lib, IResourceLocator resource, IResourceLocatorFactory factory, string path)
        {
            _lib = lib;
            _path = path;
            _resource = resource;
            _factory = factory;
            Load();
        }

        public ParsedTemplate Template
        {
            get { return _template; }
        }

        public string Description
        {
            get { return _path; }
        }

        public string Path
        {
            get { return _path; }
        }


        protected void Load()
        {
            try
            {
                _template = Formatter.LocatorBasedFormatter(_lib, _path, _resource, _factory).ParsedTemplate;
            }
            catch (ResourceException FNFe)
            {
                throw TemplateException.TemplateFailedToInitialize(_path, FNFe).WithHttpErrorCode(404);
            }
            catch (ExceptionWithContext EWC)
            {
                throw TemplateExceptionWithContext.ErrorInTemplate(_path, EWC).AddErrorCodeIfNull(500);
            }
            catch (Exception e)
            {
                throw TemplateException.ErrorInTemplate(_path, e).WithHttpErrorCode(500);
            }
        }
    }
}