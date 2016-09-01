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
using System.Runtime.Serialization;
using org.SharpTiles.Expressions;
using org.SharpTiles.Expressions.Functions;
using org.SharpTiles.Expressions.Math;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.FormatTags;
using Expression=org.SharpTiles.Expressions.Expression;

namespace org.SharpTiles.Documentation
{
    [DataContract]
    public class DocumentModel
    {
      
        private IList<ExpressionDocumentation> _expressions;
        private IList<FunctionDocumentation> _functions;
        private IList<FunctionDocumentation> _mathFunctions;
        private IList<TagGroupDocumentation> _groups;
        private readonly ResourceKeyStack _resouceKey;
        private readonly TagLib _subject;
        private IDictionary<string, string> _additionalResources;
        private bool _all;
        private IList<Func<ITag, TagDocumentation, bool>> _specials;
        private ExpressionLib _expressionlib;

        public DocumentModel(TagLib subject, bool all, ResourceBundle bundle, IList<Func<ITag, TagDocumentation, bool>> specials=null)
        {
            _expressionlib = new ExpressionLib();

            _subject = subject;
            _all = all;
            _specials = specials??new List<Func<ITag, TagDocumentation, bool>>();
            _additionalResources = new Dictionary<string, string>();
            _resouceKey = new ResourceKeyStack(bundle);
            GatherExpressions();
            GatherFunctions();
            GatherGroups();
        }

        private void GatherGroups()
        {
            _groups = new List<TagGroupDocumentation>();
            var lib = _subject;
           
            foreach (ITagGroup tag in lib)
            {
                _groups.Add(new TagGroupDocumentation(_resouceKey, tag, _specials));
            }
        }

        private void GatherFunctions()
        {
            _functions = new List<FunctionDocumentation>();
            foreach (var functionLib in _expressionlib.FunctionLibs()) {
                foreach (var function in functionLib.Functions)
                {
                     _functions.Add(new FunctionDocumentation(functionLib.GroupName, _resouceKey, function));
                }
            }
            _mathFunctions = new List<FunctionDocumentation>();
            var mathFunctionLib = new MathFunctionLib();
            foreach (var function in mathFunctionLib.Functions)
            {
                _mathFunctions.Add(new FunctionDocumentation(null, _resouceKey, function));
            }
        }

        private void GatherExpressions()
        {
            _expressions = new List<ExpressionDocumentation>();
            foreach (IExpressionParser expr in _expressionlib.GetRegisteredParsers())
            {
                if (!(expr is PropertyParser) && !(expr is FunctionParser) && !(expr is MathFunctionParser))
                {
                    _expressions.Add(new ExpressionDocumentation(_resouceKey, expr));
                }
            }
        }

        [DataMember]
        public IList<ExpressionDocumentation> Expressions => _expressions;
        public IDictionary<string, string> AdditionalResources => _additionalResources;
        public bool All => _all;

        public IEnumerable<ExpressionDocumentation> CategoryOrderedExpressions => Expressions.OrderBy(e=>(e.Category??"other")+"-"+e.Name);


        [DataMember]
        public IList<FunctionDocumentation> Functions
        {
            get { return _functions; }
        }

        [DataMember]
        public IList<FunctionDocumentation> MathFunctions
        {
            get { return _mathFunctions; }
        }

        [DataMember]
        public IList<TagGroupDocumentation> TagGroups
        {
            get { return _groups; }
        }

        public bool ShowTagGroup => _subject.Mode == TagLibMode.Strict;

        [DataMember]
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
                                var parser = new FunctionParser(new BaseFunctionLib());
                                transformed.Add(new ExpressionDocumentation(_resouceKey, parser, typeof(Function)));
                                continue;
                            }
                            transformed.Add(new ExpressionDocumentation(_resouceKey, _expressionlib.GetParser(type)));

                        }
                        list.Add(transformed);
                }
                return list;
            }
        }
    }
}
