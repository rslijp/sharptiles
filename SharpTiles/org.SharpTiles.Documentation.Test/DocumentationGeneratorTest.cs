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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Documentation.Test
{
    [TestFixture]
    public class DocumentationGeneratorTest
    {
        [Test]
        public void HappyFlow()
        {
            var dg = new DocumentationGenerator();
            var result = dg.GenerateDocumentation();
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GenerateDocumentationForTagLib()
        {
            var lib = new TagLib();
            lib.Register(new TestGroup());

            //When
            var model = new DocumentationGenerator().For(lib).BuildModel();
            var result = JsonConvert.SerializeObject(model, Formatting.Indented);


            // Then
            File.WriteAllText("c:/Temp/TagLib.json", result);
            Assert.That(result, Is.Not.Null);
        }

    }

    public class TestGroup : BaseTagGroup<TestGroup>
    {
        public override string Name => "Test";

        public TestGroup()
        {
            Register<TestSelfReferencingTag>();
        }
    }

    public class TestSelfReferencingTag : BaseCoreTag, ITagExtendTagLib
    {
        public string TagName => "TestSelfReferencingTag";
        public TagBodyMode TagBodyMode => TagBodyMode.FreeIgnoreUnkown;
        public string Evaluate(TagModel model)
        {
            return string.Empty;
        }

        public ITagGroup TagLibExtension => new TestGroup();
    }
}
