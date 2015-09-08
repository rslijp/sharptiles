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
using System.Web.Mvc;
using org.SharpTiles.HtmlTags.Form;
using org.SharpTiles.HtmlTags.Input;
using org.SharpTiles.HtmlTags.Select;
using org.SharpTiles.HtmlTags.TextArea;
using org.SharpTiles.HtmlTags.Validation;
using org.SharpTiles.Tags;

namespace org.SharpTiles.HtmlTags
{
    [HasNote]
    public class Html : BaseTagGroup<Html>
    {
        public static readonly string HTML_GROUP_NAME = "html";
        public static readonly string HTMLATTRIBUTES_PARAM_NAME = "htmlAttributes";
        public static readonly Type HTMLATTRIBUTES_PARAM_TYPE = typeof (IDictionary<string, object>);
        public static readonly string HTMLHELPER_PARAM_NAME = "htmlHelper";
        public static readonly Type HTMLHELPER_PARAM_TYPE = typeof (HtmlHelper);
        public static readonly string PAGE_MODEL_HTMLHELPER_INSTANCE = "htmlHelperInstance";
        public static readonly string PAGE_MODEL_URLHELPER_INSTANCE = "urlHelperInstance";
        
        static Html()
        {
            Register<ActionTag>();
            Register<FormTag>();
            Register<CheckBoxTag>();
            Register<HiddenTag>();
            Register<PasswordTag>();
            Register<RadioButtonTag>();
            Register<TextBoxTag>(); 
            Register<DropDownListTag>();
            Register<ListBoxTag>();
            Register<TextAreaTag>(); 
            Register<ValidationMessageTag>();
            Register<ValidationSummaryTag>();
        }

        public override string Name
        {
            get
            {
                return HTML_GROUP_NAME;
            }
        }

        public static T New<T>(List<IParameterValue> paramaters) where T : IHtmlTag, new()
        {
            var tag = new T { Parameters = paramaters };
            tag.ApplyHtmlAttributes();
            return tag;
        }

    }

}