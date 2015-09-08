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
using System.IO;
using System.Web;
using System.Web.Hosting;
using org.SharpTiles.Templates.Templates;

namespace org.SharpTiles.Connectors
{
    public class VirtualDirLocatorFactory : FileLocatorFactory
    {
        public VirtualDirLocatorFactory() : this("tiles.xml"){}

        public VirtualDirLocatorFactory(string filePath) : base(filePath, FilePrefix())
        {
        }

        private static string FilePrefix()
        {
            var path = HostingEnvironment.ApplicationPhysicalPath;
            //var path = HttpContext.Current.Server.MapPath("/");
            String filePrefix = null;
            try
            {
                filePrefix = TilesConfigurationSection.Get().FilePrefix;
            } catch
            {
                Debug.WriteLine("No prefix found");
            }
            if(!String.IsNullOrEmpty(filePrefix)) path = Path.Combine(path, filePrefix);
            return path;
        }
    }
}