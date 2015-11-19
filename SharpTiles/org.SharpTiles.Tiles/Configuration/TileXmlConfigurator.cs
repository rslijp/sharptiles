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
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.Creators;
using org.SharpTiles.Templates.Templates;

namespace org.SharpTiles.Tiles.Configuration
{
    public class TileXmlConfigurator : IConfiguration
    {
        private XmlConfigurationDefinitions _definitions;
        private DateTime? _lastModified;
        private IResourceLocatorFactory _factory;

        public TileXmlConfigurator(ITagLib lib,Assembly assembly)
            : this(lib, new AssemblyLocatorFactory(assembly, null))
        {
            
        }

        public TileXmlConfigurator(ITagLib lib, Assembly assembly, string prefix)
            : this(lib, new AssemblyLocatorFactory(assembly, prefix))
        {

        }

        public TileXmlConfigurator(ITagLib lib, string filePath)
            : this(lib, new FileLocatorFactory(filePath, null))
        {
        }

        public TileXmlConfigurator(ITagLib lib, string filePath, string filePrefix)
            : this(lib,new FileLocatorFactory(filePath, filePrefix))
        {
        }

        public TileXmlConfigurator(ITagLib lib, Type factoryType)
            : this(lib, GetCustomFactory(lib,factoryType))
        {
        }

        public TileXmlConfigurator(ITagLib lib, IResourceLocatorFactory factory)
        {
            _factory = factory.CloneForTagLib(lib);
            Load();
        }

        #region IConfiguration Members

        public void Refresh()
        {
            if (_lastModified != null)
            {
                Load();
            }
        }

        public IResourceLocatorFactory GetFactory()
        {
            return _factory;
                       
        }

        public static IResourceLocatorFactory GetCustomFactory(ITagLib lib, Type factoryType)
        {
            if(factoryType==null) return null;
            var factory = Activator.CreateInstance(factoryType);
            if(factory is IResourceLocatorFactory) 
            {
                var resourceFactory=(IResourceLocatorFactory) factory;
                return resourceFactory.CloneForTagLib(lib);
            }
            throw new XmlException(string.Format("Configured factory {0} doesn't implement IResourceLocatorFactory.", factoryType.FullName));
        }

        public DateTime? ConfigurationLastModified
        {
            get { return _factory.ConfigurationLastModified; }
        }


        public IList<ITileEntry> Entries
        {
            get { return _definitions.Entries.ConvertAll(e => (ITileEntry)e); }
        }

        #endregion


      
        private void Load()
        {
            var serializer = GetSerializer();
            using (var stream = _factory.GetConfiguration())
            {
                _definitions = (XmlConfigurationDefinitions)serializer.Deserialize(stream);
                _lastModified = _factory.ConfigurationLastModified;
            }
        }

        

        private static XmlSerializer GetSerializer()
        {
            var serializer = new XmlSerializer(typeof(XmlConfigurationDefinitions));
            serializer.UnknownAttribute += XmlParseErrorOnAttribute;
            serializer.UnknownElement += XmlParseErrorOnElement;
            serializer.UnknownNode += XmlParseErrorOnNode;
            serializer.UnreferencedObject += XmlParseErrorOnUnreferencedObject;
            return serializer;
        }

        private static void XmlParseErrorOnUnreferencedObject(object sender, UnreferencedObjectEventArgs e)
        {
            throw new XmlException("Unrefferenced id " + e.UnreferencedId);
        }

        private static void XmlParseErrorOnNode(object sender, XmlNodeEventArgs e)
        {
            throw new XmlException("Error on node " + e.NodeType + "(" + e.Text + ") at " + e.LineNumber + "," +
                                   e.LinePosition);
        }

        private static void XmlParseErrorOnElement(object sender, XmlElementEventArgs e)
        {
            throw new XmlException("Error on element attribute " + e.Element + " at " + e.LineNumber + "," +
                                   e.LinePosition);
        }

        private static void XmlParseErrorOnAttribute(object sender, XmlAttributeEventArgs e)
        {
            throw new XmlException("Unkown attribute " + e.Attr + " at " + e.LineNumber + "," + e.LinePosition);
        }
    }
}
