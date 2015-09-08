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
using System.Globalization;
using org.SharpTiles.Common;
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Tags.FormatTags
{
    [Category("Internationalization"), HasExample]
    public class SetLocale : BaseCoreTag, ITag
    {
        public static readonly string NAME = "setLocale";

        [Required]
        public ITagAttribute Value { get; set; }

        [TagDefaultValue(VariableScope.Page)]
        public ITagAttribute Scope { get; set; }

        #region ITag Members

        public string TagName
        {
            get { return NAME; }
        }

        public string Evaluate(TagModel model)
        {
            string locale = GetAsString(Value, model);
            VariableScope scope = GetAutoValueAs<VariableScope>("Scope", model).Value;
            model[scope + "." + FormatConstants.LOCALE] = new CultureInfo(locale);
            return String.Empty;
        }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.None; }
        }

        #endregion
    }
}
