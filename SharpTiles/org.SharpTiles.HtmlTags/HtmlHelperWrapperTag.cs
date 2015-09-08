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
using System.Reflection;
using System.Web.Mvc;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;

namespace org.SharpTiles.HtmlTags
{
    public abstract class HtmlHelperWrapperTag : IHtmlTag
    {
        protected HtmlReflectionHelper _helper;

        protected HtmlHelperWrapperTag()
        {
            InitHelper();
        }

        public abstract string MethodName { get; }

        public abstract Type WrappedType { get; }

        public MethodInfo Method
        {
            get { return _helper.CandidateMethod(); }
        }

        #region IHtmlTag Members

        public void ApplyHtmlAttributes()
        {
            _helper.ApplyHtmlAttributes();
        }

        public ICollection<IParameterValue> Parameters
        {
            get { return _helper.Parameters; }
            set { _helper.Parameters = value; }
        }

        public ParseContext Context
        {
            get { return _helper.Context; }
            set { _helper.Context = value; }
        }

        public abstract string TagName { get; }

        public ITagAttribute Id { get; set; }

        public TagState State { get; set; }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.None; } //for now
        }

        public string Evaluate(TagModel model)
        {
            //Convert to string or cast?
            return
                (String) _helper.InternalEvaluate((HtmlHelper) model.Page[Html.PAGE_MODEL_HTMLHELPER_INSTANCE], model);
        }

        public ITagAttributeSetter AttributeSetter
        {
            get { return new ParameterValueAttributeSetter(this); }
        }

        #endregion

        private void InitHelper()
        {
            _helper = new HtmlReflectionHelper(WrappedType, MethodName);
        }
    }
}