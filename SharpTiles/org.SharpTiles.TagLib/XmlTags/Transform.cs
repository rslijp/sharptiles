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
using System.ComponentModel;
using System.IO;
using System.Xml.XPath;
using System.Xml.Xsl;
using org.SharpTiles.Common;
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Tags.XmlTags
{
    [Category("Transformation"), HasExample]
    public class Transform : BaseCoreTagWithOptionalVariable, ITagWithNestedTags
    {
        public const string NAME = "transform";

//        private static IDictionary<string, XslCompiledTransform> XSLT_CACHE = new Dictionary<string, XslCompiledTransform>();
        /*   
        private ITagAttribute _systemId;
        */
        private readonly IList<Param> _nestedTags = new List<Param>();

        [Required]
        public ITagAttribute Doc { get; set; }

        public ITagAttribute CacheVar { get; set; }

        [TagDefaultValue(VariableScope.Session)]
        public ITagAttribute CacheScope { get; set; }

        [Required]
        public ITagAttribute Xslt { get; set; }

        public IList<Param> NestedTags
        {
            get { return _nestedTags; }
        }

        #region ITagWithNestedTags Members

        public string TagName
        {
            get { return NAME; }
        }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.NestedTags; }
        }

        public void AddNestedTag(ITag tag)
        {
            if (tag is Param)
            {
                _nestedTags.Add((Param) tag);
            }
            else
            {
                throw TagException.OnlyNestedTagsOfTypeAllowed(tag.GetType(), typeof (Param)).Decorate(Context);
            }
        }

        #endregion

        public override object InternalEvaluate(TagModel model)
        {
            object result = null;
            XPathDocument xDoc = XmlHelper.GetAsXmlDocument(Doc, model, null);
            if (xDoc != null)
            {
                result = InternalTransform(model, result, xDoc);
            }
            return result;
        }

        private object InternalTransform(TagModel model, object result, XPathDocument xDoc)
        {
            using (var writer = new StringWriter())
            {
                XslCompiledTransform transform = GetCachedTransformationEngine(model);
                if (transform != null)
                {
                    transform.Transform(xDoc, GetArguments(model), writer);
                    result = writer.ToString();
                }
            }
            return result;
        }

        private XslCompiledTransform GetCachedTransformationEngine(TagModel model)
        {
            string cacheVar = GetAsString(CacheVar, model);
            string cacheScope = GetAutoValueAsString("CacheScope", model);

            XslCompiledTransform transform = null;
            if (!String.IsNullOrEmpty(cacheVar))
            {
                transform = (XslCompiledTransform) model[cacheScope + '.' + cacheVar];
            }
            if (transform == null)
            {
                transform = GetTransformationEngine(model);
            }
            if (!String.IsNullOrEmpty(cacheVar) && transform != null)
            {
                model[cacheScope + '.' + cacheVar] = transform;
            }
            return transform;
        }

        private XslCompiledTransform GetTransformationEngine(TagModel model)
        {
            XslCompiledTransform transform = null;
            XPathDocument xsltDoc = XmlHelper.GetAsXmlDocument(Xslt, model, null);
            if (xsltDoc != null)
            {
                transform = new XslCompiledTransform();
                transform.Load(xsltDoc);
            }
            return transform;
        }

        private XsltArgumentList GetArguments(TagModel model)
        {
            var list = new XsltArgumentList();
            foreach (Param tag in _nestedTags)
            {
                XsltParameter param = tag.EvaluateNested(model);
                list.AddParam(param.Name, param.NameSpaceUri, param.Value);
            }
            return list;
        }
    }
}
