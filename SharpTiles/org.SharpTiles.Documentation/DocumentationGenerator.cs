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
        private IList<Func<ITag, TagDocumentation, bool>> _specials;

        public DocumentationGenerator()
        {
            _lib = new TagLib();
            _lib.Register(new Sharp());

            _specials = new List<Func<ITag, TagDocumentation, bool>>();
        }
        public string GenerateDocumentation()
        {
            return GenerateDocumentation(_lib,true);
        }

        public void AddSpecial(Func<ITag, TagDocumentation, bool> special)
        {
            _specials.Add(special);
        }

        public string GenerateDocumentation(TagLib tablib, bool all)
        {
            var assembly = Assembly.GetAssembly(typeof (DocumentationGenerator));
            var prefix = assembly.GetName().Name.EndsWith("Documentation") ? "templates" : "Documentation.templates";
            var locator = new AssemblyLocatorFactory(assembly, prefix).CloneForTagLib(_lib);
            var template = locator.Handle("index.htm",true);
            return template.Evaluate(new TagModel(new DocumentModel(tablib, all, _specials)));
        }

    }
}