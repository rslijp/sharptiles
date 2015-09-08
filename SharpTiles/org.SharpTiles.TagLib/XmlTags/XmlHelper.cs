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
using System.Xml;
using System.Xml.XPath;
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Tags.XmlTags
{
    public class XmlHelper : BaseCoreTag
    {
        public static XPathDocument GetAsXmlDocument(ITagAttribute attribute, TagModel model, string fallBack)
        {
            XPathDocument xDoc = null;
            object raw = Get(attribute, model) ?? fallBack;
            if (raw == null)
            {
                return xDoc;
            }

            if (raw is XPathDocument)
            {
                xDoc = (XPathDocument) raw;
            }
            else if (raw is string)
            {
                if (!String.IsNullOrEmpty((string) raw))
                {
                    var rawAsString = (string) raw;
                    using (var textReader = new StringReader(rawAsString.Trim()))
                    {
                        xDoc = new XPathDocument(textReader);
                    }
                }
                else
                {
                    xDoc = null;
                }
            }
            else if (raw is Stream)
            {
                xDoc = new XPathDocument((Stream) raw);
            }
            else if (raw is TextReader)
            {
                xDoc = new XPathDocument((TextReader) raw);
            }
            else if (raw is XmlReader)
            {
                xDoc = new XPathDocument((XmlReader) raw);
            }
            else
            {
                throw TagException.UnsupportedInput(raw.GetType(), typeof (XPathDocument), typeof (string),
                                                    typeof (Stream), typeof (TextReader),
                                                    typeof (XmlReader)).Decorate(attribute.Context);
            }

            return xDoc;
        }

        public static XPathNodeIterator GetAndEvaluate(ITagAttribute source, ITagAttribute xPath, TagModel model)
        {
            string xPathAsStr = GetAsString(xPath, model);
            object var = model[GetAsString(source, model) ?? String.Empty];
            XPathNodeIterator result = null;
            if (var != null && xPathAsStr != null)
            {
//                XmlNode node;
                XPathNavigator navigator;
                if (var is XPathNavigator)
                {
                    navigator = (XPathNavigator) var;
                }
                else
                {
                    navigator = ((IXPathNavigable) var).CreateNavigator();
                }

                var nsmgr = new XmlNamespaceManager(navigator.NameTable);
                nsmgr.AddNamespace("xsl", "http://www.w3.org/1999/XSL/Transform");
                result = navigator.Select(xPathAsStr, nsmgr);
            }
            return result;
        }

        public static object Reduce(XPathNodeIterator list)
        {
            if (list.Count == 1)
            {
                list.MoveNext();
                XPathNavigator node = list.Current;
                return (object) TryParseBoolean(node) ?? (object) TryParseNumber(node) ?? (object) node;
            }
            else
            {
                return list;
            }
        }

        public static bool? TryParseBoolean(XPathNavigator node)
        {
            try
            {
                return Boolean.Parse(node.Value);
            }
            catch
            {
                return null;
            }
        }

        public static decimal? TryParseNumber(XPathNavigator node)
        {
            try
            {
                return decimal.Parse(node.Value);
            }
            catch
            {
                return null;
            }
        }

        public static bool GetAndEvaluateAsBool(ITagAttribute source, ITagAttribute xPath, TagModel model, bool fallBack)
        {
            bool result = fallBack;
            try
            {
                XPathNodeIterator nodes = GetAndEvaluate(source, xPath, model);
                if (nodes != null && nodes.Count > 0)
                {
                    XPathNavigator node = GuardSingleNode(model, nodes, xPath);
                    result = ParseBoolean(nodes.Current) ?? fallBack;
                }
            } catch (FormatException Fe)
            {
                throw TagException.IllegalXPath(Fe).Decorate(xPath.Context);
            }
            return result;
        }

        private static bool? ParseBoolean(XPathNavigator node)
        {
            string text = node != null ? node.Value : null;
            bool? result = default(bool?);
            if (!String.IsNullOrEmpty(text))
            {
                result = Boolean.Parse(text);
            }
            return result;
        }

        private static XPathNavigator GuardSingleNode(TagModel model, XPathNodeIterator nodes, ITagAttribute select)
        {
            if (nodes.Count > 1)
            {
                throw TagException.SingleNodeExpected(GetAsString(select, model)).Decorate(select.Context);
            }
            nodes.MoveNext();
            return nodes.Current;
        }

        public static XmlReader GetAsXmlReader(ITagAttribute attribute, TagModel model, string fallBack)
        {
            XmlReader reader = null;
            object raw = Get(attribute, model) ?? fallBack;
            if (raw == null)
            {
                return reader;
            }
            if (raw is string && !String.IsNullOrEmpty((string) raw))
            {
                var textReader = new StringReader((string) raw);
                reader = XmlReader.Create(textReader);
            }
            else if (raw is Stream)
            {
                XmlReader.Create((Stream) raw);
            }
            else if (raw is TextReader)
            {
                reader = XmlReader.Create((TextReader) raw);
            }
            else if (raw is XmlReader)
            {
                reader = (XmlReader) raw;
            }
            else
            {
                throw TagException.UnsupportedInput(raw.GetType(), typeof (string), typeof (Stream), typeof (TextReader),
                                                    typeof (XmlReader)).Decorate(attribute.Context);
            }

            return reader;
        }
    }
}
