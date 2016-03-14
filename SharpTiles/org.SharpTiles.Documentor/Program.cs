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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using org.SharpTiles.Common;
using org.SharpTiles.Documentation;
using org.SharpTiles.Documentation.DocumentationAttributes;
using org.SharpTiles.HtmlTags;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.Templates.SharpTags;

namespace org.SharpTiles.Documentor
{
    internal class Program
    {
        private static readonly string ASSEMLBY_NAME =
            Assembly.GetAssembly(typeof(Program)).GetName().FullName;


        public static TagLib Scope
        {
            get
            {
                var lib = new TagLib();
                lib.Register(new Tiles.Tags.Tiles());
                lib.Register(new Html());
                lib.Register(new Sharp());
                return lib;
            }
        }

        public static bool HandleHtmlTags(ITag tag, TagDocumentation documentation)
        {
            if (tag is HtmlHelperWrapperTag)
            {
                var htmlTag = (HtmlHelperWrapperTag)tag;
                var htmlHelper = new HtmlReflectionHelper(htmlTag.WrappedType, htmlTag.MethodName);
                foreach (var method in htmlHelper.AllMethods)
                {
                    documentation.Methods.Add(new FunctionDocumentation(documentation.MessagePath, new WrappedFunctionDocumentation(method)));
                }
                return true;
            }
            return false;
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Sharptiles documentation generation");
            if (args.Length != 1 && args.Length != 2)
            {
                throw new ArgumentException("Usage " + ASSEMLBY_NAME +
                                            " <documentation target path> [--json]");
            }
            var json = false;
            var targetPath = args[0];
            if (args.Length == 2 && args[1].Equals("--json"))
            {
                json = true;
            }
            Console.WriteLine($"Generating to {targetPath}");

            var generator = new DocumentationGenerator()
                .For(Scope)
                .AddSpecial(HandleHtmlTags);
//            generator.CopyFiles(targetPath, templatePath);
            try
            {
                if (json)
                {
                    var documentation = generator.BuildModel();
                    var result = JsonConvert.SerializeObject(documentation, Formatting.Indented, new TypeJsonConverter(), new StringEnumConverter());
                    File.WriteAllText(targetPath, result);
                }
                else
                {
                    var documentation = generator.GenerateDocumentation();
                    File.WriteAllText(targetPath, documentation);
                }
            }
            catch (ExceptionWithContext EWC)
            {
                Console.WriteLine(EWC.Message);
                Console.WriteLine(EWC.Context);
            }
            Console.WriteLine("Finished");
        }

        public class TypeJsonConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue(((Type)value).Name);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
            }

            public override bool CanRead
            {
                get { return false; }
            }

            public override bool CanConvert(Type objectType)
            {
                return typeof(Type).IsAssignableFrom(objectType);
            }
        }


    }
}
