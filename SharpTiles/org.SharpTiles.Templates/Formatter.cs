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
using System.Linq;
using System.Text;
using org.SharpTiles.Common;
using org.SharpTiles.Expressions;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.Creators;
using org.SharpTiles.Tags.Templates.SharpTags;
using org.SharpTiles.Templates.Templates;

namespace org.SharpTiles.Templates
{
    public class Formatter
    {
        private IResourceLocatorFactory _locatorFactory = new FileLocatorFactory();
        private IResourceLocator _initialLocator = null;
        private bool _allowTags = true;
        private readonly string _template;
        private ParsedTemplate _templateParsed;
        private ExpressionLib _expressionLib;

//        private TagLibMode _mode = TagLibMode.Strict;
        private ITagLib _lib = null;

        public Formatter()
        {
            _lib = new TagLib();
            _expressionLib = new ExpressionLib();
            _lib.Register(new Sharp());
        }

        public Formatter(string template) : this()

        {
            _template = template;
        }

        public Formatter OverrideLib(ITagLib lib)
        {
            if (lib == null) return this;
            _lib = lib;
            return this;
        }

        public Formatter OverrideExpressionLib(ExpressionLib lib)
        {
            if (lib == null) return this;
            _expressionLib = lib;
            return this;
        }

        public Formatter SetLocatorFactory(IResourceLocatorFactory locatorFactory)
        {
            _locatorFactory = locatorFactory;
            return this;
        }

        public Formatter SetInitialLocator(IResourceLocator locator)
        {
            _initialLocator = locator;
            return this;
        }

        public Formatter SwitchToMode(TagLibMode mode)
        {
            if (_templateParsed != null)
                throw new InvalidOperationException("SwitchToMode can only be called before calling Parse!");

            var lib=new TagLib(mode,_lib.ToArray());
            _lib = lib;
            return this;
        }

        public Formatter AllowTags(bool allowTags)
        {
            _allowTags = allowTags;
            return this;
        }

        public Formatter Parse()
        {
            var f = _locatorFactory.CloneForTagLib(_lib);
            if (_initialLocator == null)
            {
                _initialLocator = f.GetNewLocator();
            }
            var formatter  = new InternalFormatter(new TagLibParserFactory(new TagLibForParsing(_lib), _expressionLib, f), _expressionLib, _template, _allowTags, _initialLocator);
            try
            {
                formatter.Parse();
            }
            finally
            {
                _templateParsed = formatter.ParsedTemplate;
            }
            return this;
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

        public static Formatter FileBasedFormatter(string path, ITagLib lib=null)
        {
            return FileBasedFormatter(path, TagLibMode.Strict, lib);
        }
        public static Formatter FileBasedFormatter(string path, TagLibMode mode, ITagLib lib=null)
        {
            var locator = new FileBasedResourceLocator();
            return new Formatter(locator.GetDataAsString(path)).
                        OverrideLib(lib).
                        AllowTags(true).
                        SetLocatorFactory(new FileLocatorFactory()).
                        SetInitialLocator(locator.Update(path)).
                        SwitchToMode(mode).
                        Parse();
        }

        public static Formatter FileBasedFormatter(string path, Encoding encoding,ITagLib lib=null)
        {
            var locator = new FileBasedResourceLocator();
            return new Formatter(locator.GetDataAsString(path)).
                        OverrideLib(lib).
                        AllowTags(true).
                        SetLocatorFactory(new FileLocatorFactory()).
                        SetInitialLocator(locator.Update(path)).
                        Parse();
        }

        public static Formatter LocatorBasedFormatter(ITagLib lib, string path, IResourceLocator locator, IResourceLocatorFactory factory)
        {
            var template = locator.GetDataAsString(path);
            return new Formatter(template).
                        OverrideLib(lib).
                        AllowTags(true).
                        SetLocatorFactory(factory).
                        SetInitialLocator(locator.Update(path)).
                        Parse();
        }

        #endregion

        public void FormatAndSave(object source, string path)
        {
            FormatAndSave(source, path, Encoding.UTF8);
        }


        public void FormatAndSave(object source, string path, Encoding fallBack)
        {
            var model = new TagModel(source);
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
            string formatted = _templateParsed.Evaluate(model);
            byte[] data = (model.Encoding ?? fallBack).GetBytes(formatted);
            File.WriteAllBytes(path, data);
        }

        public string Format(object source)
        {
            TagModel model = new TagModel(source);
            return _templateParsed.Evaluate(model);
        }

        public string Format(TagModel model)
        {
            return _templateParsed.Evaluate(model);
        }

        
    }
}