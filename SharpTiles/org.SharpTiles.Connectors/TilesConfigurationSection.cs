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
using System.Configuration;

namespace org.SharpTiles.Connectors
{
    public class TilesConfigurationSection : ConfigurationSection
    {
        public const string SECTION_NAME = "tilesConfiguration";

        private static readonly ConfigurationProperty _configFilePath =
            new ConfigurationProperty("ConfigFilePath",
                                      typeof (string), "tiles.xml",
                                      ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty _filePrefix =
            new ConfigurationProperty("FilePrefix",
                                      typeof (string), null,
                                      ConfigurationPropertyOptions.None);
      
        private static readonly ConfigurationProperty _resourceFactory =
          new ConfigurationProperty("ResourceFactory",
                                    typeof(string), null,
                                    ConfigurationPropertyOptions.None);

      
        private static readonly ConfigurationProperty _refreshIntervalSeconds =
            new ConfigurationProperty("RefreshIntervalSeconds",
                                      typeof (int), 15,
                                      ConfigurationPropertyOptions.None);

        private static ConfigurationPropertyCollection _properties;

        public TilesConfigurationSection()
        {
            // Property initialization
            _properties =
                new ConfigurationPropertyCollection
                    {
                        _configFilePath, 
                        _filePrefix, 
                        _refreshIntervalSeconds,
                        _resourceFactory
                    };
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get { return _properties; }
        }

        public string ConfigFilePath
        {
            get { return (string) this[_configFilePath.Name]; }
        }

        public string FilePrefix
        {
            get { return (string) this[_filePrefix.Name]; }
        }

        public String ResourceFactory
        {
            get { return (String)this[_resourceFactory.Name]; }
        }

        public Type ResourceFactoryType
        {
            get { return ResourceFactory != null ? Type.GetType(ResourceFactory) : null; }
        }

        public int RefreshIntervalSeconds
        {
            get { return (int) this[_refreshIntervalSeconds.Name]; }
        }


        public static TilesConfigurationSection Get()
        {
            var config =
                ConfigurationManager.GetSection(SECTION_NAME) as TilesConfigurationSection;
            if (config == null)
            {
                throw new ConfigurationErrorsException(
                    "Missing TilesConfiguration configuration section, Section name must be " + SECTION_NAME);
            }
            return config;
        }
    }
}
