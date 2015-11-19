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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.HtmlTags.Form
{
    [Category("Action"), HasExample]
    public class FormTag : IHtmlTag
    {
        public static readonly string ACTION_ATTRIBUTE_NAME = "action";
        public static readonly string CONTROLLER_ATTRIBUTE_NAME = "controller";
        private static readonly ICollection<ParameterExpectation> EXPECTATIONS;
        public static readonly string METHOD_ATTRIBUTE_NAME = "method";
        public static readonly string NAME = "form";

        static FormTag()
        {
            EXPECTATIONS = new List<ParameterExpectation>
                               {
                                   ParameterExpectation.Expect(ACTION_ATTRIBUTE_NAME),
                                   ParameterExpectation.Expect(CONTROLLER_ATTRIBUTE_NAME),
                                   ParameterExpectation.Expect(METHOD_ATTRIBUTE_NAME)
                               };
        }

        [Internal]
        public ITagAttribute Body { get; set; }

        public ITagGroup Group { get; set; }

        public HtmlAttributesParameterValue HtmlAttributes
        {
            get
            {
                return
                    Parameters.ToList().FirstOrDefault(p => p is HtmlAttributesParameterValue) as
                    HtmlAttributesParameterValue;
            }
        }

        public ITagAttribute Action
        {
            get
            {
                var parameter =
                    (ParameterValue) Parameters.ToList().FirstOrDefault(p => p.Name.Equals(ACTION_ATTRIBUTE_NAME));
                return parameter != null ? parameter.Attribute : null;
            }
        }

        public ITagAttribute Controller
        {
            get
            {
                var parameter =
                    (ParameterValue) Parameters.ToList().FirstOrDefault(p => p.Name.Equals(CONTROLLER_ATTRIBUTE_NAME));
                return parameter != null ? parameter.Attribute : null;
            }
        }

       [EnumProperyType(typeof(FormMethod))]
       [TagDefaultValue(FormMethod.Post)]
       public ITagAttribute Method
        {
            get
            {
                var parameter =
                    (ParameterValue) Parameters.ToList().FirstOrDefault(p => p.Name.Equals(METHOD_ATTRIBUTE_NAME));
                return parameter != null ? parameter.Attribute : new ConstantAttribute(FormMethod.Post, this);
            }
        }

        #region IHtmlTag Members

        public string TagName
        {
            get { return NAME; }
        }

        public ICollection<IParameterValue> Parameters { get; set; }

        public ParseContext Context { get; set; }

        public ITagAttribute Id { get; set; }

        public TagState State { get; set; }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.Free; }
        }

        public string Evaluate(TagModel model)
        {
            var builder = new TagBuilder("form");

            EvaluateAction(builder, model);
            EvaluateMethod(builder, model);
            EvaluateOtherHtmlAttributes(builder, model);

            var result = new StringBuilder();
            result.Append(builder.ToString(Body != null ? TagRenderMode.StartTag : TagRenderMode.SelfClosing));
            if (Body != null)
            {
                result.Append(Body.Evaluate(model));
                result.Append(builder.ToString(TagRenderMode.EndTag));
            }
            return result.ToString();
        }

        private void EvaluateOtherHtmlAttributes(TagBuilder builder, TagModel model)
        {
            var htmlAttributes = HtmlAttributes;
            if (htmlAttributes != null) builder.MergeAttributes(htmlAttributes.TypedValue(model));
        }

        private void EvaluateAction(TagBuilder builder, TagModel model)
        {
            if (Action != null)
            {
                builder.MergeAttribute("action", new ActionTag {Action = Action, Controller = Controller}.Evaluate(model));
            }
        }

        private void EvaluateMethod(TagBuilder builder, TagModel model)
        {
            string method = BaseCoreTag.GetAsString(Method, model);
            if(method!=null) builder.MergeAttribute("method", method.ToLower());
        }

        public ITagAttributeSetter AttributeSetter
        {
            get { return new ParameterValueAttributeSetter(this); }
        }

        public void ApplyHtmlAttributes()
        {
            Parameters = HtmlReflectionHelper.ExtractHtmlAttributes(Parameters, EXPECTATIONS);
        }

        #endregion
    }
}