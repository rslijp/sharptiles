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
using org.SharpTiles.Tags.Templates.SharpTags;
using org.SharpTiles.Templates;
using org.SharpTiles.Templates.Templates;

namespace org.SharpTiles.Documentation
{
    public class DocumentationGenerator
    {
        private TagLib _lib;

        public DocumentationGenerator()
        {
            _lib = new TagLib();
            _lib.Register(new Tiles.Tags.Tiles());
            _lib.Register(new Sharp());

        }
        public string GenerateDocumentation()
        {
            return GenerateDocumentation(_lib);
        }
        public string GenerateDocumentation(TagLib tablib)
        {
            var assembly = Assembly.GetAssembly(typeof (DocumentationGenerator));
            var locator = new AssemblyLocatorFactory(assembly, "templates").CloneForTagLib(_lib);
            var template = locator.Handle("index.htm",true);
            return template.Evaluate(new TagModel(new DocumentModel(tablib)));
        }

    }
}