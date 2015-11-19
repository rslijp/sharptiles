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
 using System.Linq;
 using org.SharpTiles.Expressions;
 using org.SharpTiles.Expressions.Functions;
 using org.SharpTiles.HtmlTags;
 using org.SharpTiles.Tags;
using org.SharpTiles.Tags.Templates.SharpTags;
using org.SharpTiles.Tiles;
 using Expression=org.SharpTiles.Expressions.Expression;

namespace org.SharpTiles.Documentation
{
    public class DocumentModel
    {
      
        private IList<ExpressionDocumentation> _expressions;
        private IList<FunctionDocumentation> _functions;
        private IList<TagGroupDocumentation> _groups;
        private readonly ResourceKeyStack _resouceKey;

        public DocumentModel()
        {
            
            _resouceKey = new ResourceKeyStack();
            GatherExpressions();
            GatherFunctions();
            GatherGroups();
        }

        private void GatherGroups()
        {
            _groups = new List<TagGroupDocumentation>();
            var lib = new TagLib();
            lib.Register(new Tiles.Tags.Tiles());
            lib.Register(new Sharp());
//            new TilesSet(); //Register Tiles tag
            foreach (ITagGroup tag in lib)
            {
                _groups.Add(new TagGroupDocumentation(_resouceKey, tag));
            }
        }

        private void GatherFunctions()
        {
            _functions = new List<FunctionDocumentation>();
            foreach (var function in FunctionLib.Libs().SelectMany(f=>f.Functions))
            {
                _functions.Add(new FunctionDocumentation(_resouceKey, function));
            }
        }

        private void GatherExpressions()
        {
            _expressions = new List<ExpressionDocumentation>();
            foreach (IExpressionParser expr in org.SharpTiles.Expressions.Expression.GetRegisteredParsers())
            {
                if (!(expr is PropertyParser) && !(expr is FunctionParser))
                {
                    _expressions.Add(new ExpressionDocumentation(_resouceKey, expr));
                }
            }
        }


        public IList<ExpressionDocumentation> Expressions
        {
            get { return _expressions; }
        }

        public IList<FunctionDocumentation> Functions
        {
            get { return _functions; }
        }

        public IList<TagGroupDocumentation> TagGroups
        {
            get { return _groups; }
        }

        public IList<IList<ExpressionDocumentation>> OperatorPrecedence
        {
            get
            {
                var list = new List<IList<ExpressionDocumentation>>();
                foreach (var types in SharpTiles.Expressions.OperatorPrecedence.Precedence)
                {
                        var transformed = new List<ExpressionDocumentation>();
                        foreach (var type in types)
                        {
                            if(type.Equals(typeof(Function)))
                            {
                                var parser = new FunctionParser(FunctionLib.Libs().First());
                                transformed.Add(new ExpressionDocumentation(_resouceKey, parser, typeof(Function)));
                                continue;
                            }
                            transformed.Add(new ExpressionDocumentation(_resouceKey, Expression.GetParser(type)));

                        }
                        list.Add(transformed);
                }
                return list;
            }
        }
    }
}
