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
    public static class VariableHelper
    {
        public static string Evaluate(ITagWithVariable tag, TagModel model, bool naturalLanguage=false)
        {
            var result = tag.InternalEvaluate(model);
            var var = tag.GetAutoValueAsString("Var", model);
            if (naturalLanguage)
            {
                var=LanguageHelper.CamelCaseAttribute(var);
            }
            var scope = tag.GetAutoValueAs<VariableScope>("Scope", model);
            if (scope != VariableScope.Page || !model.TryUpdateTag(var, result))
            {
                model[scope + "." + var] = result;
            }
            var postTag = tag as ITagWithVariableAndPostEvaluate;
            postTag?.PostEvaluate(model, result);
            return string.Empty;
        }

        public static string EvaluateOptional(ITagWithVariable tag, TagModel model)
        {
            var result = tag.InternalEvaluate(model);
            if (tag.Var != null)
            {
                var var = tag.GetAutoValueAsString("Var", model);
                var scope = tag.GetAutoValueAsString("Scope", model);
                model[scope + "." + var] = result;
            }
            var postTag = tag as ITagWithVariableAndPostEvaluate;
            postTag?.PostEvaluate(model, result);
            return tag.Var == null ? (result?.ToString() ?? string.Empty) : string.Empty;
        }
    }
}
