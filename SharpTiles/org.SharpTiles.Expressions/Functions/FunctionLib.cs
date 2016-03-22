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
 */using System.Collections.Generic;
using System.Linq;
using org.SharpTiles.Expressions.Functions;

namespace org.SharpTiles.Expressions.Functions
{
    public abstract class FunctionLib 
    {
        private readonly IDictionary<string, IFunctionDefinition> functions = new Dictionary<string, IFunctionDefinition>();
        private static readonly List<FunctionLib> LIBS = new List<FunctionLib>();

        public static void Register(FunctionLib lib)
        {
            lock (LIBS)
            {
                if (LIBS.Any(l => l.GetType().Equals(lib.GetType()))) return;
                LIBS.Add(lib);
            }
        }

        protected void RegisterFunction(IFunctionDefinition function)
        {
            lock(function)
            {
                functions.Add(function.Name, function);
            }
        }

        public static IEnumerable<FunctionLib> Libs()
        {
            return LIBS.AsReadOnly();
        }

        public abstract string GroupName { get; }


        public Function Obtain(string functionName)
        {
            IFunctionDefinition definition;
            functions.TryGetValue(functionName, out definition);
            if (definition == null)
            {
                throw FunctionEvaluationException.UnkownFunction(functionName);
            }
            return new Function(definition);
        }

        public IEnumerable<IFunctionDefinition> Functions
        {
            get { return functions.Values; }
        }

        internal static void Clear()
        {
            LIBS.Clear();
        }
    }
}
