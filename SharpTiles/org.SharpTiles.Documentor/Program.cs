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
using System.Linq;
using System.Reflection;
using System.Text;
using org.SharpTiles.Common;
using org.SharpTiles.Documentation;
using org.SharpTiles.Tags;

namespace org.SharpTiles.Documentor
{
    internal class Program
    {
        private static readonly string ASSEMLBY_NAME =
            Assembly.GetAssembly(typeof(Program)).GetName().FullName;

        public static void Main(string[] args)
        {
            Console.WriteLine("Sharptiles documentation generation");
            if (args.Length != 1)
            {
                throw new ArgumentException("Usage " + ASSEMLBY_NAME +
                                            " <documentation target path>");
            }
            var targetPath = args[0];
            Console.WriteLine("Generating to {0}", targetPath);

            var generator = new DocumentationGenerator();
//            generator.CopyFiles(targetPath, templatePath);
            try
            {
                var documentation = generator.GenerateDocumentation();
                File.WriteAllText(targetPath+@"\documentation.html", documentation);
            }
            catch (ExceptionWithContext EWC)
            {
                Console.WriteLine(EWC.Message);
                Console.WriteLine(EWC.Context);
            }
            Console.WriteLine("Finished");
        }
    }
}
