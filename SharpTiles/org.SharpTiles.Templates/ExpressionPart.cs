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
 using org.SharpTiles.Common;
using org.SharpTiles.Expressions;
using org.SharpTiles.Tags;
 using org.SharpTiles.Tags.Creators;

namespace org.SharpTiles.Templates
{
    public class ExpressionPart : ITemplatePart
    {
        private readonly Expression _expression;

        public ExpressionPart(Expression expression)
        {
            _expression = expression;
        }

        #region ITemplatePart Members

        public object Evaluate(TagModel model)
        {
            try
            {
                return _expression.Evaluate(model);
            }
            catch (ReflectionException Re)
            {
                throw ReflectionException.DecorateWithContext(Re, Context);
            }
        }

        public ParseContext Context
        {
            get { return _expression != null && _expression.Token != null ? _expression.Token.Context : null; }
        }

        public object ConstantValue
        {
            get { throw TemplateException.TemplatePartCannotBeUsedAsContant(GetType()); }
        }

        #endregion
    }
}
