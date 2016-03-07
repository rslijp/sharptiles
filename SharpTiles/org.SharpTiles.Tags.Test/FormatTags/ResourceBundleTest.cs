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
using System.Globalization;
using System.IO;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Common;
using org.SharpTiles.Tags.FormatTags;

namespace org.SharpTiles.Tags.Test.FormatTags
{
    [TestFixture]
    public class ResourceBundleTest : RequiresEnglishCulture
    {
        public const double BENCHMARK_FACTOR = 0.8;

        private static void BenchMark(string baseName, string key, int run)
        {
            var bundle = new ResourceBundle(baseName, "");
            bundle.Get(key, new CultureInfo("en-US"));

            DateTime start = DateTime.Now;
            for (int i = 0; i < run; i++)
            {
                bundle.Get(key, new CultureInfo("en-US"));
            }
            DateTime end = DateTime.Now;
            TimeSpan time = end.Subtract(start);
            double avg = (run/(time.TotalMilliseconds/1000.0));
            Assert.That(avg, Is.GreaterThan(run*BENCHMARK_FACTOR));
            Console.WriteLine(baseName + ": " + avg + " average formats per second no model");
        }

        private static void BenchMarkWithModel(string baseName, string key, int run)
        {
            var model = new TagModel(new object());
            model.PushTagStack();
            model.Tag[FormatConstants.BUNDLE] = new ResourceBundle(baseName, "");
            model.PushTagStack();
            ResourceBundle.GetMessage(key, model);

            DateTime start = DateTime.Now;
            for (int i = 0; i < run; i++)
            {
                ResourceBundle.GetMessage(key, model);
            }
            DateTime end = DateTime.Now;
            TimeSpan time = end.Subtract(start);
            double avg = (run/(time.TotalMilliseconds/1000.0));
            Assert.That(avg, Is.GreaterThan((double) run));
            Console.WriteLine(baseName + ": " + avg + " average formats per second no model");
        }

        [Test]
        public void BenchMarkTranslationsCompiledNoModel()
        {
            BenchMark("FormatTags/compiled", "b", 100000);
        }

        [Test]
        public void BenchMarkTranslationsCompiledWithModel()
        {
            BenchMarkWithModel("FormatTags/compiled", "b", 17000);
        }

        [Test]
        public void BenchMarkTranslationsWithCompileNoModel()
        {
            BenchMark("FormatTags/test", "b", 100000);
        }

        [Test]
        public void BenchMarkTranslationsWithCompileWithModel()
        {
            BenchMarkWithModel("FormatTags/test", "b", 17000);
        }

        [Test]
        public void TestCache()
        {
            string sourcePath = "FormatTags/compiled";
            string tempPath = Path.GetTempPath();
            tempPath += Path.GetFileName(sourcePath);
            File.Copy(sourcePath + FileBasedResourceLocator.COMPILED_EXTENSION,
                      tempPath + FileBasedResourceLocator.COMPILED_EXTENSION, true);
            var bundle1 = new ResourceBundle(tempPath, null);
            var bundle2 = new ResourceBundle(tempPath, null);
            Assert.That(bundle1, Is.Not.SameAs(bundle2));
            Assert.That(bundle1.ResourceManager, Is.SameAs(bundle2.ResourceManager));
        }

        [Test]
        public void TestFileNotFoundCompiled()
        {
            try
            {
                new ResourceBundle("wrong", null);
                Assert.Fail("Expected exception");
            }
            catch (ResourceException Te)
            {
                Assert.That(Te.Message,
                            Is.EqualTo(ResourceException.ResourceBundleNotFound("wrong").Message));
            }
        }

        [Test]
        public void TestGetTranslationsCompiled()
        {
            var bundle = new ResourceBundle("FormatTags/compiled", null);
            Assert.That(bundle.Get("a", new CultureInfo("en-US")), Is.EqualTo("defaultA"));
            Assert.That(bundle.Get("a", new CultureInfo("nl-NL")), Is.EqualTo("nederlandseA"));
        }

        [Test]
        public void TestGetTranslationsCompiledWithFilePrefix()
        {
            var bundle = new ResourceBundle("compiled", null, new FileBasedResourceLocator("FormatTags"));
            Assert.That(bundle.Get("a", new CultureInfo("en-US")), Is.EqualTo("defaultA"));
            Assert.That(bundle.Get("a", new CultureInfo("nl-NL")), Is.EqualTo("nederlandseA"));
        }


        [Test]
        public void TestGetTranslationsOfNonExistingKey()
        {
            var bundle = new ResourceBundle("FormatTags/complex", null);
            Assert.That(bundle.Get("wrong", new CultureInfo("en-US")), Is.EqualTo("?wrong?"));
        }

        [Test]
        public void TestGetTranslationsWithCompile()
        {
            var bundle = new ResourceBundle("FormatTags/test", null);
            Assert.That(bundle.Get("a", new CultureInfo("en-US")), Is.EqualTo("defaultA"));
            Assert.That(bundle.Get("a", new CultureInfo("nl-NL")), Is.EqualTo("nederlandseA"));
        }

        [Test]
        public void TestGetTranslationsWithCompileFromAssembly()
        {
            var bundle = new ResourceBundle("FormatTags.embedded_test", null,
                                            new AssemblyBasedResourceLocator(GetType().Assembly, null));
            Assert.That(bundle.Get("a", new CultureInfo("en-US")), Is.EqualTo("defaultA"));
            Assert.That(bundle.Get("a", new CultureInfo("nl-NL")), Is.EqualTo("nederlandseA"));
        }

        [Test]
        public void TestGetTranslationsWithCompileWithFilePrefix()
        {
            var bundle = new ResourceBundle("test", null, new FileBasedResourceLocator("FormatTags"));
            Assert.That(bundle.Get("a", new CultureInfo("en-US")), Is.EqualTo("defaultA"));
            Assert.That(bundle.Get("a", new CultureInfo("nl-NL")), Is.EqualTo("nederlandseA"));
        }

        [Test]
        public void TestGetTranslationsWithCompileWithFilePrefixFromAssembly()
        {
            var bundle = new ResourceBundle("embedded_test", null,
                                            new AssemblyBasedResourceLocator(GetType().Assembly,
                                                                             "FormatTags"));
            Assert.That(bundle.Get("a", new CultureInfo("en-US")), Is.EqualTo("defaultA"));
            Assert.That(bundle.Get("a", new CultureInfo("nl-NL")), Is.EqualTo("nederlandseA"));
        }


        [Test]
        public void TestGetTranslationsWithPrefix()
        {
            var bundle = new ResourceBundle("FormatTags/compiled", "pre_");
            Assert.That(bundle.Get("a", new CultureInfo("en-US")), Is.EqualTo("prefixedA"));
            Assert.That(bundle.Get("a", new CultureInfo("nl-NL")), Is.EqualTo("aMetVoorvoegsel"));
        }

        [Test]
        public void TestGetTranslationsWithReplacements()
        {
            var bundle = new ResourceBundle("FormatTags/complex", null);
            Assert.That(bundle.Get("noreplacment", new CultureInfo("en-US")), Is.EqualTo("fixed"));
            Assert.That(bundle.Get("onevar", new CultureInfo("en-US"), "one"), Is.EqualTo("the one var"));
            Assert.That(bundle.Get("onevar", new CultureInfo("en-US"), 1), Is.EqualTo("the 1 var"));
            Assert.That(bundle.Get("twovars", new CultureInfo("en-US"), 1, 2), Is.EqualTo("two 1, 2 vars"));
            Assert.That(bundle.Get("twovars", new CultureInfo("en-US"), "a", "b"), Is.EqualTo("two a, b vars"));
            Assert.That(bundle.Get("twovars", new CultureInfo("en-US"), 1, "b"), Is.EqualTo("two 1, b vars"));
            Assert.That(bundle.Get("twovarswithgap", new CultureInfo("en-US"), "a", "b", "c"),
                        Is.EqualTo("two a, gap, c vars"));
        }

        [Test]
        public void TestGetTranslationsWithReplacementsMessageButNoParametersProvided()
        {
            var bundle = new ResourceBundle("FormatTags/complex", null);
            try
            {
                bundle.Get("onevar", new CultureInfo("en-US"));
                Assert.Fail("Expected exception");
            }
            catch (Exception e)
            {
                Assert.That(e is FormatException, Is.True);
            }
        }

        [Test]
        public void TestGetTranslationsWithReplacementsNonUsedParametersCanBeNull()
        {
            var bundle = new ResourceBundle("FormatTags/complex", null);
            Assert.That(bundle.Get("twovarswithgap", new CultureInfo("en-US"), "a", null, "c"),
                        Is.EqualTo("two a, gap, c vars"));
        }

        [Test]
        public void TestGetTranslationsWithReplacementssedParametersCanNotBeNull()
        {
            var bundle = new ResourceBundle("FormatTags/complex", null);
            try
            {
                bundle.Get("onevar", new CultureInfo("en-US"), null);
                Assert.Fail("Expected exception");
            }
            catch (Exception e)
            {
                Assert.That(e is ArgumentNullException, Is.True);
            }
        }

        [Test]
        public void TestGetTranslationsWithReplacementsTooManyParametersShouldBeIgnored()
        {
            var bundle = new ResourceBundle("FormatTags/complex", null);
            Assert.That(bundle.Get("onevar", new CultureInfo("en-US"), "one", "two", "three"), Is.EqualTo("the one var"));
        }


        [Test]
        public void TestGetTranslationWithResourceBundleAndLocaleInModel()
        {
            var model = new TagModel(new object());
            model.PushTagStack();
            model.Tag[FormatConstants.BUNDLE] = new ResourceBundle("FormatTags/compiled", "");
            model.PushTagStack();
            model.Page[FormatConstants.LOCALE] = new CultureInfo("en-US");
            Assert.That(ResourceBundle.GetMessage("b", model), Is.EqualTo("defaultB"));
            model.Page[FormatConstants.LOCALE] = new CultureInfo("nl-NL");
            Assert.That(ResourceBundle.GetMessage("b", model), Is.EqualTo("nederlandseB"));
        }

        [Test]
        public void TestGetTranslationWithResourceBundleInModel()
        {
            var model = new TagModel(new object());
            model.PushTagStack();
            model.Tag[FormatConstants.BUNDLE] = new ResourceBundle("FormatTags/compiled", "");
            model.PushTagStack();
            CultureInfo oldCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            try
            {
                Assert.That(ResourceBundle.GetMessage("b", model), Is.EqualTo("defaultB"));
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = oldCulture;
            }
        }

        [Test]
        public void TestLoad()
        {
            var bundle = new ResourceBundle("FormatTags/compiled", null);
            Assert.That(bundle.Prefix, Is.EqualTo(""));
            Assert.That(bundle.BaseName, Is.EqualTo("FormatTags/compiled"));
        }

        [Test]
        public void TestResouceNotFoundCompiled()
        {
            try
            {
                new ResourceBundle("wrong", null, new AssemblyBasedResourceLocator(GetType().Assembly, null));
                Assert.Fail("Expected exception");
            }
            catch (ResourceException Te)
            {
                Assert.That(Te.Message,
                            Is.EqualTo(ResourceException.ResourceBundleNotFound("wrong").Message));
            }
        }
    }
}