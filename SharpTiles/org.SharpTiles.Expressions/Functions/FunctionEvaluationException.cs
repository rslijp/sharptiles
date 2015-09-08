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

namespace org.SharpTiles.Expressions.Functions
{
    public class FunctionEvaluationException : ExceptionWithContext
    {
        public FunctionEvaluationException(string msg) : base(msg)
        {
        }

        public FunctionEvaluationException(Exception inner)
            : base(inner.Message, inner)
        {
        }

        public static FunctionEvaluationException WrongParameter(IFunctionDefinition function, int paramterIndex,
                                                                 object parameter)
        {
            string msg = String.Format("Can't evaluate function {0} expected {1} but found {2} on argument {3}({4})",
                                       function.Name,
                                       function.Arguments[paramterIndex].Type.Name,
                                       parameter.GetType().Name,
                                       function.Arguments[paramterIndex].Name,
                                       paramterIndex
                );
            return new FunctionEvaluationException(msg);
        }

        public static FunctionEvaluationException UnkownFunction(string name)
        {
            string msg = String.Format("Function {0} is not defined.", name);
            return new FunctionEvaluationException(msg);
        }


        public static PartialExceptionWithContext<FunctionEvaluationException> EvaluationError(Exception e)
        {
            return MakePartial(new FunctionEvaluationException(e));
        }
    }
}
