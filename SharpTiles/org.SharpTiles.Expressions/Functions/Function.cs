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
 using org.SharpTiles.Common;
using TypeConverter=org.SharpTiles.Common.TypeConverter;

namespace org.SharpTiles.Expressions.Functions
{
    [Category("OtherExpression")]
    public class Function : Expression
    {
        
        private readonly IFunctionDefinition _function;
        private Brackets _nested;


        internal Function(IFunctionDefinition function)
        {
            _function = function;
        }

        public Expression Nested
        {
            get { return _nested; }
        }

        public FunctionArgument[] Arguments
        {
            get { return _function.Arguments; }
        }

        public override Type ReturnType
        {
            get { return _function.ReturnType; }
        }




        public override void GuardTypeSafety()
        {
            if (_function.IsParamsFunctions())
            {
                for (int i = 0; i < _nested.Nodes.Count; i++)
                {
                    var argumentType = i<_function.Arguments.Length?_function.Arguments[i].Type:_function.Arguments.Last().Type;
                    Expression node = _nested.Nodes[i];
                    if (node == null || node.ReturnType == null ||
                        typeof(object).Equals(argumentType)) continue;
                    if (_function.Arguments[i].Type.IsInterface)
                    {
                        if (!node.ReturnType.GetInterfaces().Contains(argumentType))
                            throw ConvertException.StaticTypeSafety(argumentType, node.ReturnType,
                                node.ToString());
                    }
                    else
                    {
                        if (node.ReturnType != _function.Arguments[i].Type)
                            throw ConvertException.StaticTypeSafety(argumentType, node.ReturnType,
                                node.ToString());
                    }
                    node.GuardTypeSafety();
                }
            }
            else
            {
                for (int i = 0; i < _function.Arguments.Length; i++)
                {
                    Expression node = _nested.Nodes[i];
                    if (node == null || node.ReturnType == null ||
                        typeof(object).Equals(_function.Arguments[i].Type)) continue;
                    if (_function.Arguments[i].Type.IsInterface)
                    {
                        if (!node.ReturnType.GetInterfaces().Contains(_function.Arguments[i].Type))
                            throw ConvertException.StaticTypeSafety(_function.Arguments[i].Type, node.ReturnType,
                                node.ToString());
                    }
                    else
                    {
                        if (node.ReturnType != _function.Arguments[i].Type)
                            throw ConvertException.StaticTypeSafety(_function.Arguments[i].Type, node.ReturnType,
                                node.ToString());
                    }
                    node.GuardTypeSafety();
                }
            }
        }

        
        public void FillNested(Brackets exp)
        {
            _nested = exp;
        }

        public string PreFix { get; set; }

        public override object Evaluate(IModel model)
        {
            var parameters = new List<object>();
            for (int i = 0; i < _function.Arguments.Length; i++)
            {
                var arg = _function.Arguments[i];
                if (!arg.Params)
                {
                    var node = _nested.Nodes[i];
                    object value = node.Evaluate(model);
                    parameters.Add(TypeConverter.To(value, _function.Arguments[i].Type));
                }
                else
                {
                    for (int pi = i; pi < _nested.Nodes.Count; pi++)
                    {
                        var node = _nested.Nodes[pi];
                        object value = node.Evaluate(model);
                        parameters.Add(TypeConverter.To(value, _function.Arguments[i].Type));
                    }
                }
            }
            try
            {
                return _function.Evaluate(parameters.ToArray());
            }
            catch (ExceptionWithContext EWC)
            {
                if (EWC.Context == null)
                {
                    throw ExceptionWithContext.MakePartial(EWC).Decorate(Token);
                }
                else
                {
                    throw EWC;
                }
            }
            catch (Exception e)
            {
                throw FunctionEvaluationException.EvaluationError(e).Decorate(Token);
            }
        }

        public override string ToString()
        {
            return _function.Name + _nested;
        }
        public override string AsParsable()
        {
            return (PreFix??"")+ _function.Name + _nested;
        }
        /*
        public override void TypeCheck(IModel model)
        {
            _nested.TypeCheck(model);
        }
        */
    }
}