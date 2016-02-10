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
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Templates;
using org.SharpTiles.Templates.Templates;

namespace org.SharpTiles.Documentation
{
    internal class DocumentationGenerator
    {
        private static readonly string ASSEMLBY_NAME =
            Assembly.GetAssembly(typeof (DocumentationGenerator)).GetName().FullName;

        public static void Main(string[] args)
        {
            Console.WriteLine("Sharptiles documentation generation");
            if (args.Length != 2)
            {
                throw new ArgumentException("Usage " + ASSEMLBY_NAME +
                                            " <path to templates> <documentation target path>");
            }
            var templatePath = args[0];
            var targetPath = args[1];
            Console.WriteLine("Generating to {0}", targetPath);

            CopyFiles(targetPath, templatePath);
            GenerateDocumentation(targetPath, templatePath);
            Console.WriteLine("Finished");
        }

        private static void GenerateDocumentation(string targetPath, string templatePath)
        {
            try
            {
                Console.WriteLine("Generting documentation");
                var formatter = Formatter.FileBasedFormatter(templatePath + "/index.htm");
                formatter.FormatAndSave(new TagModel(new DocumentModel()), targetPath + "/reference.html");
            }
            catch (ExceptionWithContext EWC)
            {
                Console.WriteLine(EWC.Message);
                Console.WriteLine(EWC.Context);
            }
        }

        private static void CopyFiles(string targetPath, string templatePath)
        {
            var files = new List<string>(Directory.GetFiles(templatePath, "*.css"));
            files.AddRange(Directory.GetFiles(templatePath, "*.gif"));
            foreach (string file in files)
            {
                string fileStripped = Path.GetFileName(file);
                Console.WriteLine("Copying " + fileStripped);
                File.Copy(templatePath + "/" + fileStripped, targetPath + "/" + fileStripped, true);
            }
        }
    }
}