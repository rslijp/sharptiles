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
using System.Diagnostics;
using System.Reflection;
using org.SharpTiles.Common;
using org.SharpTiles.Tags.Creators;
using org.SharpTiles.Templates.Templates;
using org.SharpTiles.Tiles;
using org.SharpTiles.Tiles.Configuration;

namespace org.SharpTiles.Connectors
{
    public abstract class BaseCache : IViewCache
    {
        protected Assembly _assembly;
        protected IResourceLocatorFactory _factory;

        public Assembly Of
        {
            get { return _assembly; }
        }

        public abstract ITile GetView(string view);
        public abstract bool HasView(string name);
        public abstract IViewCache GuardInit(Assembly assembly);

        public IResourceLocatorFactory Factory
        {
            get { return _factory; }
            set { _factory = value; }
        }

        protected void InitFactory(Assembly assembly)
        {
            if (_factory == null)
            {
                _assembly = assembly;
                Type factoryType = null;
                String prefix = null;
                try
                {
                    var config = TilesConfigurationSection.Get();
                    factoryType = config.ResourceFactoryType;
                    prefix = config.FilePrefix;
                    RefreshJob.REFRESH_INTERVAL = config.RefreshIntervalSeconds;
                }
                catch
                {
                    Debug.WriteLine("No config section found for tiles, using assembly configuration");
                }
                _factory = factoryType == null
                               ?
                                   new AssemblyLocatorFactory(_assembly, prefix)
                               :
                                   TileXmlConfigurator.GetCustomFactory(factoryType);
            }
        }

        public abstract String PathSeperator { get; }
    }
}