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
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using org.SharpTiles.Common;

namespace org.SharpTiles.Common
{
    public class FileBasedResourceLocator : IResourceLocator
    {

        public const string COMPILED_EXTENSION = ".resources";
        public const string STUDIO_EXTENSION = ".resx";

        private bool _absolute = false;
        
        private readonly string _currentDir;

        public FileBasedResourceLocator()
        {
        }

        public FileBasedResourceLocator AbsolutePaths()
        {
            _absolute = true;
            return this;
        }

        public FileBasedResourceLocator(string prefix)
        {
            _currentDir = prefix;
        }

        public IResourceLocator Update(string path)
        {
            var newLocator = new FileBasedResourceLocator(
                _absolute ? 
                _currentDir : 
                Path.GetDirectoryName(PreFixed(path))
                );
            return _absolute ? newLocator.AbsolutePaths() : newLocator;
        }

        #region IResourceLocator Members

        
        public string GetDataAsString(string path)
        {
            path = PreFixed(path);
            Guard(path);
            return File.ReadAllText(path);
        }

        public string GetDataAsString(string path, Encoding encoding)
        {
            return encoding.GetString(GetData(path));
        }

        public Stream GetStream(string path)
        {
            path = PreFixed(path);
            return File.OpenRead(path);
        }

        public byte[] GetData(string path)
        {
            path = PreFixed(path);
            Guard(path);
            return File.ReadAllBytes(PreFixed(path));
        }

        private static void Guard(string path)
        {
            if (!File.Exists(path))
            {
                throw ResourceException.FileNotFound(path);
            }
        }

        public bool Exists(string path)
        {
            try
            {
                return File.Exists(PreFixed(path));
            } catch (ArgumentException)
            {
                return false;
            }
        }

        public string PreFixed(string path)
        {
            var prefixed = path;
            if (!String.IsNullOrEmpty(_currentDir) && !Path.IsPathRooted(path))
            {
                var currentDir = _currentDir.EndsWith(@"\") ? _currentDir.Substring(0, _currentDir.Length-1) : _currentDir;
                
                while (path.StartsWith(@"..\") || path.StartsWith("../"))
                {
                    path = path.Substring(3);
                    currentDir = Path.GetDirectoryName(currentDir);
                }
                //var currentDir = _currentDir.EndsWith(@"\") ? _currentDir : _currentDir + @"\";
                prefixed = Path.Combine(currentDir, path);
            }
            return prefixed;
        }

        public static string HandleParent(string dir)
        {
            while(dir.StartsWith(@"..\") || dir.StartsWith("../"))
            {
                dir = Path.GetPathRoot(dir);
            }
            return dir;
        }

        private string[] FindResources(string pattern)
        {
            var dir = Path.GetDirectoryName(pattern);
            var fixDir = String.IsNullOrEmpty(dir);
            dir = PreFixed(fixDir ? "." : dir);
            var files = Directory.GetFiles(
                dir,
                Path.GetFileName(pattern)
                );
            if(fixDir)
            {
                files = files.ToList().ConvertAll(file => file.Substring(dir.Length+1)).ToArray();
            }
            return files;
        }

        public string PathSeperator
        {
            get { return @"\"; }
        }

        public string CurrentDir
        {
            get { return _currentDir; }
        }

        public ResourceManager LoadBundle(string originalfilePath)
        {
            GuardExistence(originalfilePath);
            return RequiresCompilation(originalfilePath) ? CompileAndLoadBundle(originalfilePath) : LoadCompiledBundle(originalfilePath);
        }

        public DateTime? LastModified(string path)
        {
            path = PreFixed(path);
            return File.GetLastWriteTime(path);
        }


        #endregion

        #region internals for loading resource bundles

        private ResourceManager LoadCompiledBundle(string filePath)
        {
            filePath = PreFixed(filePath);
            string file = Path.GetFileName(filePath);
            string dir = Path.GetDirectoryName(filePath);
            return ResourceManager.CreateFileBasedResourceManager(file, dir, null);
        }

        private void GuardExistence(string originalfilePath)
        {
            if (!(Exists(originalfilePath + STUDIO_EXTENSION) || Exists(originalfilePath + COMPILED_EXTENSION)))
            {
                throw ResourceException.ResourceBundleNotFound(PreFixed(originalfilePath));
            }
        }

        private bool RequiresCompilation(string originalfilePath)
        {
            return Exists(originalfilePath + STUDIO_EXTENSION);
        }

        public ResourceManager CompileAndLoadBundle(string orignalFilePath)
        {
            string tempPath = Path.GetTempPath();
            var locator  = CompileBundles(orignalFilePath, tempPath);
            return locator.LoadCompiledBundle(Shorten(RemovePathChars(orignalFilePath)));
        }

        private FileBasedResourceLocator CompileBundles(string uncompiledPath, string toPath)
        {
            string[] resourceNames = FindResources(uncompiledPath + "*" + STUDIO_EXTENSION);
            foreach (string fileName in resourceNames)
            {
                string extension = Path.GetExtension(fileName);
                string fileToCompile = fileName.Substring(0, fileName.Length - extension.Length);
                CompileBundle(fileToCompile, toPath);
            }
            return new FileBasedResourceLocator(toPath);
        }

        private void CompileBundle(string uncompiledPath, string toPath)
        {
            string compiledPath = toPath + Shorten(RemovePathChars(uncompiledPath));
            using (IResourceReader reader = new ResXResourceReader(PreFixed(uncompiledPath + STUDIO_EXTENSION)))
            using (var writer = new ResourceWriter(compiledPath + COMPILED_EXTENSION))
            {
                foreach (DictionaryEntry entry in reader)
                {
                    writer.AddResource((string)entry.Key, entry.Value);
                }
                writer.Generate();
            }
        }

        private static string Shorten(string path)
        {
            if (path.Length > 40)
            {
                int hash = path.GetHashCode();
                path = hash + path.Substring(path.Length - 40);
            }
            return path;
        }

        private static string RemovePathChars(string path)
        {

            return path.Replace('\\', '_').Replace('/', '_');//.Replace('.', '_');
        }




        #endregion
    }
}