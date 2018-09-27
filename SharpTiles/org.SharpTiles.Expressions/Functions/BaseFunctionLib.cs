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
 */using org.SharpTiles.Expressions.Functions;

namespace org.SharpTiles.Expressions.Functions
{
    public class BaseFunctionLib : FunctionLib
    {
        public BaseFunctionLib()
        {
            RegisterFunction(new LengthFunction());
            RegisterFunction(new ReverseFunction());
            RegisterFunction(new ConcatFunction());
            RegisterFunction(new EmptyFunction());
            RegisterFunction(new ContainsFunction());
            RegisterFunction(new ContainsIgnoreCaseFunction());
            RegisterFunction(new IndexOfFunction());
            RegisterFunction(new StartsWithFunction());
            RegisterFunction(new EndsWithFunction());
            RegisterFunction(new ReplaceFunction());
            RegisterFunction(new RegExReplaceFunction());
            RegisterFunction(new JoinFunction());
            RegisterFunction(new PluckFunction());
            RegisterFunction(new SelectFunction());
            RegisterFunction(new SplitFunction());
            RegisterFunction(new EscapeXmlFunction());
            RegisterFunction(new ToLowerCaseFunction());
            RegisterFunction(new ToUpperCaseFunction());
            RegisterFunction(new TrimFunction());
            RegisterFunction(new SubStringFunction());
            RegisterFunction(new SubStringBeforeFunction());
            RegisterFunction(new SubStringAfterFunction());
            RegisterFunction(new NowFunction());
            RegisterFunction(new PathFunction());
            RegisterFunction(new PathCombineFunction());
            RegisterFunction(new FallbackFunction());
            RegisterFunction(new FalseOrValueFunction());
            RegisterFunction(new AddDaysFunction());
            RegisterFunction(new AddMonthsFunction());
            RegisterFunction(new IfEmptyFunction());
        }

        public override string GroupName
        {
            get { return "fn"; }
        }


    }
}
