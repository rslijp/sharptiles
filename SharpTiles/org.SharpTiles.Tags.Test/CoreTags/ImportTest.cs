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
using System.IO;
using System.Net;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Expressions;
 using org.SharpTiles.NUnit;
 using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Tags.Test.CoreTags
{
    [TestFixture]
    public class ImportTest
    {
        private static string GetUrl(string relativeFileUrl)
        {
            string fileUrl = Path.GetFullPath(relativeFileUrl);
            fileUrl = fileUrl.Replace("\\", "/");
            fileUrl = "file://" + fileUrl;
            return fileUrl;
        }

        [Test]
        public void CheckUrlRequired()
        {
            var tag = new Import();
            try
            {
                RequiredAttribute.Check(tag);
                Assert.Fail("Expected exception");
            }
            catch (TagException Te)
            {
                Assert.That(Te.Message,
                            Is.EqualTo(TagException.MissingRequiredAttribute(typeof (Import), "Url").Message));
            }
            tag.Url = new MockAttribute(new Constant("www.sharptiles.org"));
            RequiredAttribute.Check(tag);
        }

        [Test]
        public void TestAcquiringOfImport()
        {
            var url = new Import();
            string fileUrl = GetUrl("CoreTags/import.txt");
            url.Url = new MockAttribute(new Constant(fileUrl));
            Assert.That(url.Evaluate(new TagModel(this)), Is.EqualTo("some text"));
        }

        [Test]
        public void TestAcquiringOfImportNotFound()
        {
            var url = new Import();
            string fileUrl = GetUrl("CoreTags/import_nonexisting.txt");
            url.Url = new MockAttribute(new Constant(fileUrl));
            try
            {
                url.Evaluate(new TagModel(this));
                Assert.Fail("Expected exception");
            }
            catch (WebException)
            {
            }
        }


        [Test]
        public void TestAcquiringOfParamsImportInVariable()
        {
            var url = new Import();
            string fileUrl = GetUrl("CoreTags/import.txt");
            url.Url = new MockAttribute(new Constant(fileUrl));
            url.Var = new MockAttribute(new Constant("target"));

            var model = new TagModel(this);
            Assert.That(url.Evaluate(model), Is.EqualTo(String.Empty));
            Assert.That(model.Page["target"], Is.EqualTo("some text"));
        }

        [Test]
        public void TestAcquiringOfParamsImportInVariableInDifferentScope()
        {
            var url = new Import();
            string fileUrl = GetUrl("CoreTags/import.txt");
            url.Url = new MockAttribute(new Constant(fileUrl));
            url.Var = new MockAttribute(new Constant("target"));
            url.Scope = new MockAttribute(new Constant("Session"));
            var model = new TagModel(this, new MockSessionState());

            Assert.That(url.Evaluate(model), Is.EqualTo(String.Empty));
            Assert.That(model.Session["target"], Is.EqualTo("some text"));
        }
    }
}
