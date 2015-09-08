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
using org.SharpTiles.Common;

namespace org.SharpTiles.Tags.CoreTags
{
    public abstract class BaseUrlTagWithVariable : BaseUrlTag
    {
        [TagDefaultValue(VariableScope.Page)]
        public ITagAttribute Scope { get; set; }

        public ITagAttribute Var { get; set; }

        protected abstract object InternalEvaluate(TagModel model);

        public override string Evaluate(TagModel model)
        {
            object result = InternalEvaluate(model);
            if (Var != null)
            {
                string var = GetAsString(Var, model);
                string scope = GetAutoValueAsString("Scope", model);
                model[scope + "." + var] = result;
                return String.Empty;
            }
            return result != null ? result.ToString() : String.Empty;
        }
    }
}
