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
 using org.SharpTiles.Templates;

namespace org.SharpTiles.HtmlTags.Test
{
    public class MockAttribute : ITagAttribute
    {
        private readonly Expression _expression;

        public MockAttribute(Expression expression)
        {
            _expression = expression;
        }

        #region ITagAttribute Members

        public object Evaluate(TagModel model)
        {
            return _expression.Evaluate(model);
        }

        public object ConstantValue
        {
            get { throw TemplateException.TemplatePartCannotBeUsedAsContant(GetType()); }
        }

        public ParseContext Context { get; set; }

        public IResourceLocator ResourceLocator
        {
            get { return new FileBasedResourceLocator(); }
        }

        #endregion
    }
}