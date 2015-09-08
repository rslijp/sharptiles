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
using System.ComponentModel;
using System.Web.Mvc;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.HtmlTags
{
    [Category("Action"), HasExample]
    public class ActionTag : BaseCoreTagWithOptionalVariable, ITag
    {
        public string TagName
        {
            get { return "action"; }
        }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.None; }
        }

        public ITagAttribute Action { get; set; }

        public ITagAttribute Controller { get; set; }

        public override object InternalEvaluate(TagModel model)
        {
            var helper = (UrlHelper) model.Page[Html.PAGE_MODEL_URLHELPER_INSTANCE];
            var actionName = GetAsString(Action, model);
            var controllerName = GetAsString(Controller, model);
            return !String.IsNullOrEmpty(controllerName) ?  helper.Action(actionName, controllerName) : helper.Action(actionName);
        }
    }
}