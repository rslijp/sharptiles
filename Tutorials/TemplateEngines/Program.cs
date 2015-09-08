using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Templates;
using org.SharpTiles.Templates.Templates;
using org.SharpTiles.Tiles;
using org.SharpTiles.Tiles.Configuration;

namespace TemplateEngine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Template();
            Tile();
        }

        private static void Tile()
        {
            try
            {
                Console.WriteLine("Rendering tile");
                var set = new TilesSet( new TileXmlConfigurator("tiles.xml"));
                var model = new TagModel(new Hashtable
                                             {
                                                 {
                                                     "Key",
                                                     new List<Person>
                                                         {
                                                             new Person {Name = "Ruth Kingsley", Age = 42},
                                                             new Person {Name = "Jacob van Dijk", Age = 23},
                                                             new Person {Name = "Stuward Langley", Age = 66}
                                                         }
                                                     }
                                             }).UpdateFactory(new FileLocatorFactory());
                Console.WriteLine(set["template"].Render(model));
            }
            catch (ExceptionWithContext EWC)
            {
                Console.WriteLine(EWC.Message);
                Console.WriteLine(EWC.Context);
            }
            Console.WriteLine("Hit enter");
            Console.ReadLine();
        }

        private static void Template()
        {
            try
            {
                Console.WriteLine("Rendering template");
                var formatter = Formatter.FileBasedFormatter("templates/template.txt");
                Console.WriteLine( formatter.Format(new Hashtable{{"Key",
                                                                      new List<Person>{
                                                                                          new Person{Name="Ruth Kingsley", Age=42},
                                                                                          new Person{Name="Jacob van Dijk", Age=23},
                                                                                          new Person{Name="Stuward Langley", Age=66}
                                                                                      }}
                                                                 }));
            }
            catch (ExceptionWithContext EWC)
            {
                Console.WriteLine(EWC.Message);
                Console.WriteLine(EWC.Context);
            }
            Console.WriteLine("Hit enter");
            Console.ReadLine();
        }

        public class Person
        {
            public String Name { get; set; }
            public int Age { get; set; }
        }
    }
}
