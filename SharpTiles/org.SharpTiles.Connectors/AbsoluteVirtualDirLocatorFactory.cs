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
using org.SharpTiles.Common;
using org.SharpTiles.Templates.Templates;

namespace org.SharpTiles.Connectors
{
    public class AbsoluteVirtualDirLocatorFactory : VirtualDirLocatorFactory
    {
        public AbsoluteVirtualDirLocatorFactory() : this("tiles.xml"){}

        public AbsoluteVirtualDirLocatorFactory(string filePath)
            : base(filePath)
        {
        }

        public override IResourceLocator GetNewLocator()
        {
            return new FileBasedResourceLocator(_filePrefix).AbsolutePaths();
        }
    }
}