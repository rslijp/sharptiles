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
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using org.SharpTiles.Common;

namespace org.SharpTiles.Common
{
    public class AssemblyBasedResourceLocator : IResourceLocator
    {
        private readonly Assembly _assembly;
        private readonly string _prefix;

        public AssemblyBasedResourceLocator(Assembly assembly, string prefix)
        {
            _assembly = assembly;
            _prefix = prefix;
        }

        #region IResourceLocator Members

        public IResourceLocator Update(string path)
        {
            var newLocator = new AssemblyBasedResourceLocator(_assembly, _prefix);
            return newLocator;
        }

        public ResourceManager LoadBundle(string filePath)
        {
            if (!Exists(filePath+".resources"))
            {
                throw ResourceException.ResourceBundleNotFound(filePath);
            }
//            Console.WriteLine(_assembly.GetName().Name+"."+PreFixed(filePath));
//            if (!Exists(filePath + ".resources"))
//            {
//                throw ResourceException.ResourceBundleNotFound(filePath);
//            }
            return new ResourceManager(_assembly.GetName().Name+"."+PreFixed(filePath), _assembly); 
        }

        public DateTime? LastModified(string path)
        {
            return default(DateTime?);
        }

        public string[] FindResources(string pattern)
        {
            var regex = new Regex(PreFixed(pattern));
            return _assembly.GetManifestResourceNames().ToList().Where(name => regex.Match(name).Success).ToArray();
        }

        public string PreFixed(string path)
        {

            var prefixed = _prefix;
            if (!String.IsNullOrEmpty(prefixed))
            {
                prefixed = prefixed.EndsWith(PathSeperator)
                               ?
                                   prefixed
                               :
                                   (prefixed + PathSeperator);
            } else
            {
                prefixed = "";
            }
            prefixed += path;
            return prefixed;
        }


        public string PathSeperator
        {
            get { return "."; }
        }

        public string GetDataAsString(string name)
        {
            using (Stream stream = FindResource(PreFixed(name)))
            using (var oSR = new StreamReader(stream))
            {
                return oSR.ReadToEnd();
            }
        }

        public Stream GetStream(string name)
        {
            return FindResource(PreFixed(name));
        }

        public string GetDataAsString(string path, Encoding encoding)
        {
            return encoding.GetString(GetData(path));
        }

        public byte[] GetData(string path)
        {
            using (var stream = FindResource(PreFixed(path)))
            {
                return stream.GetData();
            }
        }

        public bool Exists(string name)
        {
            return TryFindResource(PreFixed(name)) != null;
        }

        #endregion

        public Stream TryFindResource(string path)
        {
            string pathWithassemblyName = _assembly.GetName().Name + "." + path;
            return _assembly.GetManifestResourceStream(path) ?? _assembly.GetManifestResourceStream(pathWithassemblyName);
        }

        public Stream FindResource(string path)
        {
            Stream stream = TryFindResource(path);
            if (stream == null)
            {
                throw ResourceException.ResourceNotFound(path, _assembly);
            }
            return stream;
        }

    }
}