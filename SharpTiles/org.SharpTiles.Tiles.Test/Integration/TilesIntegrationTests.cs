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
 */using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Tags;
using org.SharpTiles.Tiles.Configuration;

namespace org.SharpTiles.Tiles.Test.Integration
{
    [TestFixture]
    public class TileXmlConfiguratorTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            book = new Dictionary<string, object>
                       {
                           {"title", "SharpTiles integration test"},
                           {"author", "R.Z. Slijp"},
                           {"copyrights", "2008 (c) R.Z. Slijp"},
                           {
                               "chapters", new List<object>
                                               {
                                                   new Dictionary<string, object>
                                                       {
                                                           {"url", "#c1"},
                                                           {"title", "Setting up some mock templates"},
                                                           {
                                                               "paragraphs", new List<object>
                                                                                 {"aaa", "bbb", "ccc"}
                                                               }
                                                       },
                                                   new Dictionary<string, object>
                                                       {
                                                           {"url", "#c2"},
                                                           {"title", "Setting up some test data"},
                                                           {
                                                               "paragraphs", new List<object>
                                                                                 {"111", "222"}
                                                               }
                                                       },
                                                   new Dictionary<string, object>
                                                       {
                                                           {"url", "#c3"},
                                                           {"title", "Making the tests"},
                                                           {
                                                               "paragraphs", new List<object>
                                                                                 {"I", "II", "III", "IV", "V"}
                                                               }
                                                       }
                                               }
                               }
                       };
        }

        #endregion

        private IDictionary<string, object> book;

        private static void OutputEquals(string result, string expectedFile)
        {
            result = CleanUp(result);
            string expected = CleanUp(Encoding.UTF8.GetString(File.ReadAllBytes(expectedFile)));
            Assert.That(result, Is.EqualTo(expected));
        }

        private static string CleanUp(string result)
        {
            return result.
                Replace(" ", "").
                Replace("\t", "").
                Replace("\r", "").
                Replace("\n", "").
                Trim();
        }

        [Test]
        public void GenerationFirstDifferentRunsWithOutErrors()
        {
            var set = new TilesSet(new TileXmlConfigurator("tiles.config.xml", "Integration/"));
            string result = set["firstdifferent"].Render(new TagModel(book));
            //            Console.WriteLine(result);
            OutputEquals(result, "Integration\\expected.result.firstdifferent.html");
        }

        [Test]
        public void GenerationFirstPageRunsWithOutErrors()
        {
            var set = new TilesSet(new TileXmlConfigurator("tiles.config.xml", "Integration/"));
            string result = set["firstpage"].Render(new TagModel(book));
            OutputEquals(result, "Integration\\expected.result.firstpage.html");
        }

        [Test]
        public void GenerationOtherPageRunsWithOutErrors()
        {
            var set = new TilesSet(new TileXmlConfigurator("tiles.config.xml", "Integration/"));
            string result = set["otherpage"].Render(new TagModel(book));
            OutputEquals(result, "Integration\\expected.result.otherpage.html");
        }

        [Test]
        public void LoadingOfConfigFile()
        {
            var set = new TilesSet(new TileXmlConfigurator("tiles.config.xml", "Integration/"));
            Assert.That(set.Contains("base.html.book.page"));
            Assert.That(set.Contains("firstpage"));
            Assert.That(set.Contains("otherpage"));
            Assert.That(set.Contains("menu.bar"));
            Assert.That(set.Contains("contents.page"));
            Assert.That(set.Contains("firstdifferent"));
            Assert.That(set.Tiles.Count, Is.EqualTo(6));
        }
    }
}
