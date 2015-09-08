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
using System.Collections;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.CoreTags;
using org.SharpTiles.Templates;
using org.SharpTiles.Templates.Templates;
using org.SharpTiles.Tiles.Tile;

namespace org.SharpTiles.Tiles.Test.Tile
{
    [TestFixture]
    public class FileTemplateTest
    {
        [Test]
        public void ErrorFileShouldSaveParseContext()
        {
            try
            {
                new TilesSet();
                var tile = new TemplateTile(
                    "test",
                    new FileTemplate("errorinfile.htm"),
                    null
                    );
            }
            catch (TemplateExceptionWithContext TEWC)
            {
                Assert.That(TEWC.Context, Is.Not.Null);
                Assert.That(TEWC.Context.LineNumber, Is.EqualTo(2));
                string fullPath = Path.GetFullPath("errorinfile.htm");
                TagException tagException =
                    TagException.UnbalancedCloseingTag(typeof (ForEach), typeof (If)).Decorate(TEWC.Context);
                Assert.That(TEWC.Message,
                            Is.EqualTo(TemplateExceptionWithContext.ErrorInTemplate(fullPath, tagException).Message));
            }
        }


        [Test]
        public void RequiresRefresh()
        {
            string tempTile = Path.GetTempFileName();
            File.Copy("a.htm", tempTile, true);
            try
            {
                var ft = new FileTemplate(tempTile);
                Assert.That(ft.RequiresRefresh(), Is.False);
                File.SetLastWriteTime(tempTile, DateTime.Now.AddDays(-1));
                Assert.That(ft.RequiresRefresh(), Is.True);
                ft.Refresh();
                Assert.That(ft.RequiresRefresh(), Is.False);
            }
            finally
            {
                File.Delete(tempTile);
            }
        }


        [Test]
        public void TestDatesOnFileTile()
        {
            var ft = new FileTemplate("a.htm");
            Assert.That(ft.ResourceLastModified, Is.EqualTo(File.GetLastWriteTime("a.htm")));
            Assert.That(ft.TileLastModified, Is.EqualTo(ft.ResourceLastModified));
        }

        [Test]
        public void TestRefresh()
        {
            string tempTile = Path.GetTempFileName();
            File.Copy("a.htm", tempTile, true);
            try
            {
                var ft = new FileTemplate(tempTile);
                Assert.That(ft.TileLastModified, Is.EqualTo(ft.ResourceLastModified));
                File.SetLastWriteTime(tempTile, DateTime.Now.AddDays(-1));
                Assert.That(ft.TileLastModified, Is.Not.EqualTo(ft.ResourceLastModified));
                ft.Refresh();
                Assert.That(ft.TileLastModified, Is.EqualTo(ft.ResourceLastModified));
            }
            finally
            {
                File.Delete(tempTile);
            }
        }

        [Test]
        public void TestRefreshClearsLastError()
        {
            string tempTile = Path.GetTempFileName();
            File.Copy("a.htm", tempTile, true);
            try
            {
                var ft = new FileTemplate(tempTile);
                File.SetLastWriteTime(tempTile, DateTime.Now.AddDays(-1));
                ft.RefreshException=new Exception("Test");
                ft.Refresh();
                Assert.That(ft.RefreshException, Is.Null);
            }
            finally
            {
                File.Delete(tempTile);
            }
        }



        [Test]
        public void TestLastExceptionIsFilled()
        {
            var tempTile = Path.GetTempFileName();
            File.Copy("a.htm", tempTile, true);
            try
            {
                var ft = new FileTemplate(tempTile);
                Assert.That(ft.TileLastModified, Is.EqualTo(ft.ResourceLastModified));
                File.Delete(tempTile); 
                Assert.That(ft.RefreshException, Is.Null);
                ft.Refresh();
                Assert.That(ft.RefreshException, Is.Not.Null);
                Assert.That(ft.RefreshException.Message, Is.EqualTo(TemplateException.TemplateFailedToInitialize(tempTile, ResourceException.FileNotFound(tempTile)).Message));
            }
            finally
            {
                File.Delete(tempTile);
            }
        }

        [Test]
        public void HttpErrorCode_Is_Set_On_ResourceException()
        {
            var tempTile = Path.GetTempFileName();
            File.Copy("a.htm", tempTile, true);
            try
            {
                var ft = new FileTemplate(tempTile);
                Assert.That(ft.TileLastModified, Is.EqualTo(ft.ResourceLastModified));
                File.Delete(tempTile);
                ft.Refresh();
                Assert.That(ft.RefreshException, Is.Not.Null);
                Assert.That(((IHaveHttpErrorCode) ft.RefreshException).HttpErrorCode, Is.EqualTo(404));
                
            }
            finally
            {
                File.Delete(tempTile);
            }
        }


        [Test]
        public void HttpErrorCode_Is_Set_On_ParseException()
        {
            try
            {
                new FileTemplate("broken.htm");
                Assert.Fail("Expected an exception");
            }
            catch (Exception e)
            {
                Assert.That(((IHaveHttpErrorCode) e).HttpErrorCode, Is.EqualTo(500));
            }
        }

        [Test]
        public void HttpErrorCode_Is_Maintained()
        {
            try
            {
                new FileTemplate("nestedbroken.htm").Template.Evaluate(new TagModel(new Hashtable()).UpdateFactory(new FileLocatorFactory()));
                Assert.Fail("Expected an exception");
            }
            catch (Exception e)
            {
                Assert.That(((IHaveHttpErrorCode)e).HttpErrorCode, Is.EqualTo(500));
            }
        }


        [Test]
        public void TestLastExceptionIsThrownOnParseTemplate()
        {
            var ft = new FileTemplate("a.htm");
            Assert.That(ft.Template, Is.Not.Null);
            ft.RefreshException=new Exception("oops");
            try
            {
                Console.WriteLine(ft.Template);
                Assert.Fail("Should not come here");
            } catch (Exception e)
            {
                Assert.That(e.Message, Is.EqualTo("oops"));
            }
        }
    }
}
