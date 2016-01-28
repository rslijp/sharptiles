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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.SharpTiles.Common
{   
    public static class LanguageHelper
    {
        public static string CamelCaseProperty(string input)
        {
            var r = "";
            var capitalize = true;
            for (var i = 0; i < input.Length; i++)
            {
                if (input[i] == ' ')
                {
                    capitalize = true;
                    continue;
                }
                if (capitalize)
                {
                    r += (input[i]).ToString().ToUpperInvariant();
                    capitalize = false;
                }
                else
                {
                    if (input[i] == '.')
                    {
                        capitalize = true;
                    }
                    r += input[i];
                }

            }
            return r;
        }

        public static string CamelCaseAttribute(string input)
        {
            var r = "";
            var capitalize = true;
            for (var i = 0; i < input.Length; i++)
            {
                if (input[i] == '-')
                {
                    capitalize = true;
                    continue;
                }
                if (capitalize)
                {
                    r += (input[i]).ToString().ToUpperInvariant();
                    capitalize = false;
                }
                else
                {
                    r += input[i];
                }

            }
            return r;
        }

        public static string DashProperty(string input)
        {
            var r = "";
            foreach (char t in input)
            {
                if (char.IsUpper(t) && r.Length>0)
                {
                    r += "-";
                }
                r += (t).ToString().ToLowerInvariant();            
            }
            return r;
        }
    }
}
