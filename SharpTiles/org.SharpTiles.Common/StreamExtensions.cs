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
using System.IO;

namespace org.SharpTiles.Common
{
    public static class StreamExtensions
    {
        public static byte[] GetData(this Stream stream)
        {
//            int totalSize = 0;
            using (var ms = new MemoryStream())
            {
                while (true)
                {
                    var block = new byte[2048];
                    int size = stream.Read(block, 0, block.Length);
                    if (size <= 0)
                    {
                        break;
                    }
//                    totalSize += size;
                    ms.Write(block, 0, size);
                }
//                var data
//                Array.Copy(block, 0, data, totalSize, size);
//                return data;
                return ms.ToArray();
            }
        }
    }
}