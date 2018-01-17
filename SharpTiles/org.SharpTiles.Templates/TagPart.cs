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
using org.SharpTiles.Tags;
 using org.SharpTiles.Tags.Creators;

namespace org.SharpTiles.Templates
{
    public class TagPart : ITemplatePart
    {
        private readonly ITag _tag;


        public TagPart(ITag tag)
        {
            _tag = tag;
        }

        public ITag Tag => _tag;

        #region ITemplatePart Members

        public object Evaluate(TagModel model)
        {
            try
            {
                return _tag.Evaluate(model);
            }
            catch (ExceptionWithContext ewc)
            {
                if (ewc.Context == null)
                {
                    throw ExceptionWithContext.MakePartial(ewc).Decorate(_tag.Context);
                }
                throw;
            }
            catch (Exception e)
            {
                throw TagException.EvaluationError(e).Decorate(_tag.Context);
            }
        }

        public object ConstantValue
        {
            get { throw TemplateException.TemplatePartCannotBeUsedAsContant(GetType()); }
        }


        public ParseContext Context
        {
            get { return _tag.Context; }
        }

        #endregion

        public override string ToString() => Context?.Contents;
    }
}
