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
using System.Reflection;
using org.SharpTiles.Common;

namespace org.SharpTiles.Tags
{
    public class TagException : ExceptionWithContext
    {
        public TagException(string msg) : base(msg)
        {
        }

        public TagException(Exception e)
            : base(e.Message, e)
        {
        }

        public TagException(string msg, Exception e)
            : base(msg, e)
        {
        }

        public static PartialExceptionWithContext<TagException> ShouldBeLast(Type type)
        {
            String msg = String.Format("Tag {0} should be last", type.Name);
            return MakePartial(new TagException(msg));
        }

        public static PartialExceptionWithContext<TagException> NotAllowedShouldBePartOf(Type notAllowed,
                                                                                         params Type[] partOf)
        {
            PropertyInfo name = typeof (Type).GetProperty("TagName");
            String msg =
                String.Format("Expression {0} is only allowed inside {1}.", notAllowed.Name,
                              CollectionUtils.ToString(partOf, name));
            return MakePartial(new TagException(msg));
        }

        public static PartialExceptionWithContext<TagException> OnlyTagsAllowedInside()
        {
            return MakePartial(new TagException("Only tags are allowed inside the body of tag"));
        }

        public static PartialExceptionWithContext<TagException> MissingRequiredAttribute(Type tag,
                                                                                         params string[] attributes)
        {
            String msg =
                String.Format("Tag {0} is missing required attribute {1}.", tag.Name,
                              CollectionUtils.ToString(attributes));
            return MakePartial(new TagException(msg));
        }

        public static PartialExceptionWithContext<TagException> OnlyNestedTagsOfTypeAllowed(Type tag,
                                                                                            params Type[] types)
        {
            PropertyInfo name = typeof (Type).GetProperty("Name");
            String msg =
                String.Format("Found nested Tag {0} but only nested tags of type {1} allowed.", tag.Name,
                              CollectionUtils.ToString(types, name));
            return MakePartial(new TagException(msg));
        }

        public static PartialExceptionWithContext<TagException> UnbalancedCloseingTag(Type expected, Type found)
        {
            String msg =
                String.Format("Found a different closing tag, {0} than expected {1}.", expected.Name, found.Name);
            return MakePartial(new TagException(msg));
        }

        public static PartialExceptionWithContext<TagException> UnbalancedCloseingTag(ITag expected, ITag found)
        {
            String msg =
                String.Format("Found a different closing tag, {0}:{1} than expected {2}:{3}.", expected.Group.Name, expected.TagName, found.Group.Name, found.TagName);
            return MakePartial(new TagException(msg));
        }

        public static PartialExceptionWithContext<TagException> ExpectedCloseTag(Type type)
        {
            return MakePartial(new TagException("Expected a closing tag but none was found."));
        }

        public static PartialExceptionWithContext<TagException> PropertyAlReadySet(string property)
        {
            string msg = String.Format("The property '{0}' is already set.", property);
            return MakePartial(new TagException(msg));
            ;
        }

        public static TagException HttpResponseNotAvailable()
        {
            return
                new TagException(
                    "Redirect and request encoding not available without setting a web page in the tag model.");
        }

        public static PartialExceptionWithContext<TagException> NoResourceBundleFoundInTagScope()
        {
            return MakePartial(new TagException("There was no resource bundle found in the tag scope"));
        }

        public static PartialExceptionWithContext<TagException> NoResourceBundleFoundUnder(string name)
        {
            string msg = String.Format("There was no resource bundle found under '{0}'.", name);
            return MakePartial(new TagException(msg));
        }

        public static PartialExceptionWithContext<TagException> XmlShouldIncludeVarName(string xPathWithDocVar)
        {
            string msg =
                String.Format("XPath should include varname which precedes the '\\\\'. '{0}' doesn't have '\\\\'.",
                              xPathWithDocVar);
            return MakePartial(new TagException(msg));
        }

        public static PartialExceptionWithContext<TagException> SingleNodeExpected(string xpath)
        {
            string msg = String.Format("Expected a single node as result of XPath expression , '{0}'", xpath);
            return MakePartial(new TagException(msg));
        }

        public static PartialExceptionWithContext<TagException> UnsupportedInput(Type found, params Type[] expected)
        {
            PropertyInfo name = typeof (Type).GetProperty("Name");
            string msg = String.Format("Expected input of type {1} but found {0}", found.Name,
                                       CollectionUtils.ToString(expected, name));
            return MakePartial(new TagException(msg));
        }

        public static PartialExceptionWithContext<TagException> UnkownTagGroup(string group)
        {
            string msg = String.Format("Unkown tag group '{0}'", group);
            return MakePartial(new TagException(msg));
        }

        public static PartialExceptionWithContext<TagException> UnkownTag(string tag)
        {
            string msg = String.Format("Unkown tag '{0}'", tag);
            return MakePartial(new TagException(msg));
        }

        public static PartialExceptionWithContext<TagException> EvaluationError(Exception e)
        {
            return MakePartial(new TagException(e));
        }

        public static PartialExceptionWithContext<TagException> EvaluationMessageError(string key, Exception e)
        {
            string msg = String.Format("Error evaluation key '{0}'. Error : {1}", key, e.Message);
            return MakePartial(new TagException(msg, e));
        }

        public static PartialExceptionWithContext<TagException> IllegalXPath(Exception fe)
        {
            string msg = String.Format("Error during evaluating of an xpath expression. Error : {0}", fe.Message);
            return MakePartial(new TagException(msg, fe));
        }

        public static PartialExceptionWithContext<TagException> ParseException(string input, string type)
        {
            string msg = String.Format("Could not parse '{0}' to a valid {1}", input, type);
            return MakePartial(new TagException(msg));
        }
    }
}
