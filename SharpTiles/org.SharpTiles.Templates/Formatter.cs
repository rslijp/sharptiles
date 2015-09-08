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
using System.Text;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.Creators;
using org.SharpTiles.Tags.Templates.SharpTags;
using org.SharpTiles.Templates.Templates;

namespace org.SharpTiles.Templates
{
    public class Formatter
    {
        private readonly IResourceLocatorFactory _locatorFactory;
        private readonly string _template;
        private readonly ParsedTemplate _templateParsed;

        static Formatter()
        {
            TagLib.Register(new Sharp());
        }

        public Formatter(string template)
            : this(template, true)
        {
        }

        //        public Formatter(string template, string currentDir) 
        //            : this(template, true, new FileBasedResourceLocator(currentDir))
        //        {
        //        }

        public Formatter(string template, IResourceLocatorFactory locatorFactory)
            : this(template, true, locatorFactory)
        {
        }

        public Formatter(string template, bool allowTags)
            : this(template, allowTags, new FileLocatorFactory())
        {
        }

        public Formatter(string template, bool allowTags, IResourceLocatorFactory locatorFactory)
        {
            _template = template;
            _templateParsed = new InternalFormatter(template, allowTags, locatorFactory.GetNewLocator()).Parse();
            _locatorFactory = locatorFactory;
        }

        public Formatter(string template, bool allowTags, IResourceLocator locator, IResourceLocatorFactory locatorFactory)
        {
            _template = template;
            _locatorFactory = locatorFactory;
            _templateParsed = new InternalFormatter(template, allowTags, locator).Parse();
        }

        public string Template
        {
            get { return _template; }
        }

        public ParsedTemplate ParsedTemplate
        {
            get { return _templateParsed; }
        }

        #region static constructors

        public static Formatter FileBasedFormatter(string path)
        {
            var locator = new FileBasedResourceLocator();
            return new Formatter(locator.GetDataAsString(path), true,  locator.Update(path), new FileLocatorFactory());
        }

        public static Formatter FileBasedFormatter(string path, Encoding encoding)
        {
            var locator = new FileBasedResourceLocator();
            return new Formatter(locator.GetDataAsString(path, encoding), true, locator.Update(path), new FileLocatorFactory());
        }

        public static Formatter LocatorBasedFormatter(String path, IResourceLocator locator, IResourceLocatorFactory factory)
        {
            var template = locator.GetDataAsString(path);
            return new Formatter(template, true, locator.Update(path), factory);
        }

        #endregion

        public void FormatAndSave(object source, string path)
        {
            FormatAndSave(source, path, Encoding.UTF8);
        }


        public void FormatAndSave(object source, string path, Encoding fallBack)
        {
            var model = new TagModel(source).UpdateFactory(_locatorFactory);
            var formatted = _templateParsed.Evaluate(model);
            var data = (model.Encoding ?? fallBack).GetBytes(formatted);
            File.WriteAllBytes(path, data);
        }

        public void FormatAndSave(TagModel source, string path)
        {
            FormatAndSave(source, path, Encoding.UTF8);
        }


        public void FormatAndSave(TagModel model, string path, Encoding fallBack)
        {
            model.UpdateFactory(_locatorFactory);
            string formatted = _templateParsed.Evaluate(model);
            byte[] data = (model.Encoding ?? fallBack).GetBytes(formatted);
            File.WriteAllBytes(path, data);
        }

        public string Format(object source)
        {
            TagModel model = new TagModel(source).UpdateFactory(_locatorFactory);
            return _templateParsed.Evaluate(model);
        }

        public string Format(TagModel model)
        {
            return _templateParsed.Evaluate(model.UpdateFactory(_locatorFactory));
        }

        public static ParsedTemplate ParseNested(ParseHelper helper, IResourceLocator locator)
        {
            try
            {
                helper.PushNewTokenConfiguration(
                    true,
                    false,
                    InternalFormatter.COMMENT,
                    InternalFormatter.SEPERATORS,
                    null,
                    null, //InternalFormatter.LITERALS, 
                    ResetIndex.LookAhead);
                return new InternalFormatter(helper, true, true, locator).ParseNested();
            }
            finally
            {
                helper.PopTokenConfiguration(ResetIndex.CurrentAndLookAhead);
            }
        }
    }
}