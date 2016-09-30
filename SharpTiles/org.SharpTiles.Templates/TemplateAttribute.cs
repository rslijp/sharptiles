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
using System.Text;
using org.SharpTiles.Common;
 using org.SharpTiles.Tags;
 using org.SharpTiles.Tags.CoreTags;
 using org.SharpTiles.Tags.Creators;
 using org.SharpTiles.Templates;

namespace org.SharpTiles.Templates
{
    public class TemplateAttribute : ITagAttribute
    {
        private readonly ParsedTemplate _resultParts;

        public TemplateAttribute(ParsedTemplate resultParts)
        {
            _resultParts = resultParts;
        }

        public string AttributeName { get; set; }

        #region ITagAttribute Members

        public object Evaluate(TagModel model)
        {
            return _resultParts.Count > 1 ? EvaluateMulti(model) : EvaluateSingle(model);
        }

        public ParsedTemplate TemplateParsed => _resultParts;

        public object ConstantValue
        {
            get
            {
                var sb = new StringBuilder();
                foreach (ITemplatePart part in _resultParts)
                {
                    sb.Append(part.ConstantValue);
                }
                return sb.ToString();
            }
        }

        public ParseContext Context
        {
            get { return _resultParts.Context; }
        }

//        public ParseContext Context
//        {
//            get { return OffSet?.Add(_resultParts.Context)?? _resultParts.Context; }
//        }
//
//        public ParseContext OffSet { get; set; }

        public IResourceLocator ResourceLocator
        {
            get { return _resultParts.ResourceLocator; }
        }

       

        #endregion

        public object EvaluateSingle(TagModel model)
        {
            return _resultParts.Count == 1 ? _resultParts[0].Evaluate(model) : null;
        }

        public virtual bool AllowOverWrite => false;

        public object EvaluateMulti(TagModel model)
        {
            var builder = new StringBuilder();
            foreach (ITemplatePart part in _resultParts)
            {
                object result = part.Evaluate(model);
                builder.Append(result != null ? BaseCoreTag.ValueOfWithi18N(model, result) : String.Empty);
            }
            return builder;
        }

        public override string ToString() => ConstantValue?.ToString();
    }
}