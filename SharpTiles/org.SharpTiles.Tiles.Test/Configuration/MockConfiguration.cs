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
using System.Collections.Generic;
using System.Reflection;
using org.SharpTiles.Tags.Creators;
using org.SharpTiles.Templates.Templates;
using org.SharpTiles.Tiles.Configuration;

namespace org.SharpTiles.Tiles.Test.Configuration
{
    public class MockConfiguration : IConfiguration
    {
        private readonly string _tempTile;
        private IList<ITileEntry> _configuration = new List<ITileEntry>();
        private bool _addC = false;


        public MockConfiguration(string tempTile, DateTime lastModified)
        {
            _tempTile = tempTile;
            ConfigurationLastModified = lastModified;
        }

        public MockConfiguration()
        {
            
        }

        public MockConfiguration(DateTime time) : this("a.htm", time)
        {
            
        }

        #region IConfiguration Members

        public string FilePrefix
        { 
            get; set;
        }

        public Assembly InAssembly
        { get; set; }
      
        public DateTime? ConfigurationLastModified { get; set; }

        public IList<ITileEntry> Entries
        {
            get { return _configuration; }
        }

        public void Refresh()
        {
            ITileEntry a = new MockTileEntry
                               {
                                   Name = "a",
                                   Path = _tempTile
                               };
            ITileEntry b = new MockTileEntry
                               {
                                   Name = "b",
                                   Path = "b.htm"
                               };
            _configuration = new List<ITileEntry>();
            _configuration.Add(a);
            _configuration.Add(b);
            if(_addC)
            {
                _configuration.Add(new MockTileEntry
                {
                    Name = "c",
                    Path = "c.htm"
                });
            }
        }


        public void RefreshAndChange(DateTime date)
        {
            ConfigurationLastModified = date;
            _addC = true;
        }

        public IResourceLocatorFactory GetFactory()
        {
            return InAssembly != null
                       ? new AssemblyLocatorFactory(InAssembly, FilePrefix)
                       : (IResourceLocatorFactory)new FileLocatorFactory(FilePrefix);
        }

        #endregion
    }
}
