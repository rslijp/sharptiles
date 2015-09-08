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
using System.Reflection;
using org.SharpTiles.Common;

namespace org.SharpTiles.Tiles
{
    public class TileException : Exception, IHaveHttpErrorCode
    {
        public TileException(string msg) : base(msg)
        {
        }

        public int? HttpErrorCode { get; set; }

        public TileException(string msg, Exception innerException)
            : base(msg, innerException)
        {
        }

        public static TileException TileRemoved(string path)
        {
            var msg = String.Format("The tile {0} has been removed.", path);
            return new TileException(msg);
        }

        public static TileException DoubleDefinition(string definition)
        {
            var msg = String.Format("There is already a tile with name {0}", definition);
            return new TileException(msg);
        }

        public static TileException AttributeNotFound(string name, string definition)
        {
            var msg = String.Format("There is no tile attribute found on tile {1} with name {0}", name, definition);
            return new TileException(msg);
        }

        public static TileException AttributeNameAlreadyUsed(string name, string tileName)
        {
            var msg = String.Format("There is already a tile attribute defined with name {0} on tile {1}", name, tileName);
            return new TileException(msg);
        }


        public static TileException ErrorInTile(string path, Exception exception)
        {
            var msg = String.Format("Error in tile {0}:{1}", path, exception.Message);
            return new TileException(msg, exception);
        }
    }
}
