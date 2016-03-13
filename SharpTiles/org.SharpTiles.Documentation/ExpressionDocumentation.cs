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
 using System.Linq;
 using System.Reflection;
 using System.Runtime.Serialization;
 using System.Text;
 using org.SharpTiles.Expressions;
using org.SharpTiles.Tags;
 using DescriptionAttribute = org.SharpTiles.Documentation.DocumentationAttributes.DescriptionAttribute;

namespace org.SharpTiles.Documentation
{
    [DataContract]
    public class ExpressionDocumentation : IDescriptionElement
    {
        private readonly ResourceKeyStack _messagePath;
        private readonly string _name;
        private List<ExpressionOperatorSign> _tokens;
        private CategoryAttribute _category;
        private DescriptionAttribute _description;


        public ExpressionDocumentation(ResourceKeyStack messagePath, IExpressionParser expr)
            : this(messagePath, expr, expr.ParsedTypes.Single())
        {
        }

        public ExpressionDocumentation(ResourceKeyStack messagePath, IExpressionParser expr, Type type)
        {
            GatherTokens(expr);
            _name = type.Name;
            _messagePath = messagePath.BranchFor(expr);
            _category = CategoryHelper.GetCategory(type);
            _description = _messagePath.Description;
        }
        
        private void GatherTokens(IExpressionParser expr)
        {
            _tokens = new List<ExpressionOperatorSign>();
            if (expr.DistinctToken == null) return;
            _tokens.Add(expr.DistinctToken);
            if(expr.AdditionalTokens!=null)
            {
                _tokens.AddRange(expr.AdditionalTokens);
            }
        }

        [DataMember]
        public string Name
        {
            get { return _name; }
        }

        [DataMember]
        public DescriptionAttribute Description => _description;

        public List<ExpressionOperatorSign> Tokens
        {
            get { return _tokens; }
        }

        public CategoryAttribute Category
        {
            get { return _category; }
        }

        [DataMember]
        public string CategoryDescriptionKey
        {
            get
            {
                if (Category != null)
                {
                    string categoryStr = Category.Category;
                    return "expression_" + categoryStr;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
