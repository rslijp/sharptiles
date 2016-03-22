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
using org.SharpTiles.Expressions.Functions;
using org.SharpTiles.Expressions.Math;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.FormatTags;
using org.SharpTiles.Tags.Templates.SharpTags;
using org.SharpTiles.Templates;
using org.SharpTiles.Templates.Templates;

namespace org.SharpTiles.Documentation
{
    public class DocumentationGenerator
    {
        private TagLib _lib;
        private IList<Func<ITag, TagDocumentation, bool>> _specials;
        private bool _all;
        private bool _fragment;
        private TagLib _subject;

        public DocumentationGenerator()
        {
            _lib = new TagLib();
            _lib.Register(new Sharp());
            _subject = _lib;
            _all = true;
            _fragment = false;
//            FunctionLib.Register(new MathFunctionLib());
            _specials = new List<Func<ITag, TagDocumentation, bool>>();
        }
        public string GenerateDocumentation()
        {
            var assembly = Assembly.GetAssembly(typeof(DocumentationGenerator));
            var prefix = assembly.GetName().Name.EndsWith("Documentation") ? "templates" : "Documentation.templates";
            var locator = new AssemblyLocatorFactory(assembly, prefix).CloneForTagLib(_lib);
            var bundle =  new ResourceBundle("Documentation", null, locator.GetNewLocator());
            var template = locator.Handle(_fragment?"fragment.html":"index.htm", true);
            return template.Evaluate(new TagModel(new DocumentModel(_subject, _all, bundle, _specials)));
        }

        public DocumentModel BuildModel()
        {
            var assembly = Assembly.GetAssembly(typeof(DocumentationGenerator));
            var prefix = assembly.GetName().Name.EndsWith("Documentation") ? "templates" : "Documentation.templates";
            var locator = new AssemblyLocatorFactory(assembly, prefix).CloneForTagLib(_lib);
            var bundle = new ResourceBundle("Documentation", null, locator.GetNewLocator());
            return new DocumentModel(_subject, _all, bundle, _specials);

//            var json = JsonConvert.SerializeObject(dm, new TypeJsonConverter(), new StringEnumConverter());
        }
    
        public DocumentationGenerator AddSpecial(Func<ITag, TagDocumentation, bool> special)
        {
            _specials.Add(special);
            return this;
        }

        public DocumentationGenerator For(TagLib subject)
        {
            _subject = subject;
            return this;
        }

        public DocumentationGenerator WithoutHeader()
        {
            _all = false;
            return this;
        }

        public DocumentationGenerator AsFragment()
        {
            _fragment = true;
            return this;
        }
    }
}