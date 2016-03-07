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
 using System.Resources;
 using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Tags.FormatTags
{
    [Category("Internationalization"), HasExample]
    public class Message : BaseCoreTagWithOptionalVariable, ITagWithNestedTags
    {
        public static readonly string NAME = "message";

        private readonly IList<Param> _nestedTags = new List<Param>();

        [Required]
        public ITagAttribute Key { get; set; }

        public ITagAttribute Bundle { get; set; }

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
            IResourceBundle bundle = null;
            string key = GetAsString(Key, model);
            bundle = GetBundle(model);
            object[] i18nparams = GetParams(model);
            try
            {
                return bundle.Get(key, model, i18nparams);
            } catch (Exception e)
            {
                throw TagException.EvaluationMessageError(key, e).Decorate(Context);
            }
        }

        private object[] GetParams(TagModel model)
        {
            var i18nparams = new object[_nestedTags.Count];
            for (int i = 0; i < _nestedTags.Count; i++)
            {
                i18nparams[i] = _nestedTags[i].EvaluateNested(model);
            }
            return i18nparams;
        }

        private IResourceBundle GetBundle(TagModel model)
        {
            IResourceBundle bundle;
            if (Bundle != null)
            {
                var bundleName = GetAsString(Bundle, model);
                bundle = (IResourceBundle) model[bundleName];
                if (bundle == null)
                {
                    throw TagException.NoResourceBundleFoundUnder(bundleName).Decorate(Bundle.Context);
                }
            }
            else
            {
                bundle = (IResourceBundle) model.SearchInTagScope(FormatConstants.BUNDLE);
                bundle = bundle ?? (IResourceBundle) model.Global[FormatConstants.BUNDLE];
                if (bundle == null)
                {
                    throw TagException.NoResourceBundleFoundInTagScope().Decorate(Context);
                }
            }
            return bundle;
        }
    }
}
