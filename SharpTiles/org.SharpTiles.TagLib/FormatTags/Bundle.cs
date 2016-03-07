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
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Tags.FormatTags
{
    [Category("Internationalization"), HasExample]
    public class Bundle : BaseCoreTag, ITag
    {
        public static readonly string NAME = "bundle";

        [Required]
        public ITagAttribute BaseName { get; set; }

        public ITagAttribute Prefix { get; set; }

        public ITagAttribute Extension { get; set; }

        [Internal]
        public ITagAttribute Body { get; set; }

        #region ITag Members

        public string TagName
        {
            get { return NAME; }
        }

        public string Evaluate(TagModel model)
        {
            var baseName = GetAsString(BaseName, model);
            var prefix = GetAsString(Prefix, model);
            var extension = GetAsString(Extension, model);
            IResourceBundle bundle = new ResourceBundle(baseName, prefix, BaseName.ResourceLocator);
            model.PushTagStack();
            if (!string.IsNullOrEmpty(extension))
            {
                var extensionBundle = (IDictionary<string, string>) model.Resolve(extension, true);
                bundle = new ExtendableResourceBundle(bundle, extensionBundle);
            }
            model.Tag[FormatConstants.BUNDLE] = bundle;
            return GetAsString(Body, model) ?? string.Empty;
        }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.Free; }
        }

        #endregion
    }
}

