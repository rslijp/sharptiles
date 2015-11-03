using System;
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
