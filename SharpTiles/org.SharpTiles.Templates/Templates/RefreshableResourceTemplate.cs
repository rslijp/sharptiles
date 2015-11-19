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
using org.SharpTiles.Common;
using org.SharpTiles.Tags.Creators;

namespace org.SharpTiles.Templates.Templates
{
    public class RefreshableResourceTemplate : RefreshableResource, ITemplate
    {
        private readonly IResourceLocator _locator;
        private readonly IResourceLocatorFactory _factory;
        private readonly string _name;
        private ParsedTemplate _template;

        public RefreshableResourceTemplate(IResourceLocator locator, IResourceLocatorFactory factory, string name)
        {
            _name = name;
            _locator = locator;
            _factory = factory;
            LoadResource();
        }

        public override DateTime? ResourceLastModified
        {
            get { return _locator.LastModified(_name); }
        }

        public DateTime TileLastModified
        {
            get { return LastModified.Value; }
        }

        #region ITemplate Members

        public ParsedTemplate Template
        {
            get
            {
                if (RefreshException != null) throw RefreshException;
                return _template;
            }
        }


        public string Description
        {
            get { return _name; }
        }

        public string Path
        {
            get { return _name; }
        }

        #endregion

        protected override void Load()
        {
            try
            {
                _template = Formatter.LocatorBasedFormatter(_factory.Lib, _name, _locator, _factory).ParsedTemplate;
            }
            catch (ResourceException FNFe)
            {
                throw TemplateException.TemplateFailedToInitialize(_name, FNFe).WithHttpErrorCode(404);
            }
            catch (ExceptionWithContext EWC)
            {
                throw TemplateExceptionWithContext.ErrorInTemplate(_name, EWC).AddErrorCodeIfNull(500);
            }
            catch (Exception e)
            {
                throw TemplateException.ErrorInTemplate(_name, e).WithHttpErrorCode(500);
            }
        }
    }
}