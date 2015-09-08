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
using System.Reflection;
using System.Text;
using org.SharpTiles.Common;

namespace org.SharpTiles.HtmlTags
{
    public class HtmlHelperTagException : ExceptionWithContext
    {
        public HtmlHelperTagException(string msg) : base(msg)
        {
        }

        public static PartialExceptionWithContext<HtmlHelperTagException> NoSuitableMethodFound(String methodName,
                                                                                                ICollection
                                                                                                    <
                                                                                                    ParameterExpectation
                                                                                                    > expections)
        {
            string msg = String.Format("No HtmlHelper method found with name '{0}' and parameters {1}", methodName,
                                       CollectionUtils.ToString(expections));
            return MakePartial(new HtmlHelperTagException(msg));
        }

        public static PartialExceptionWithContext<HtmlHelperTagException> RequiredArgumentMissing(string parameterName,
                                                                                                  MethodInfo method)
        {
            string msg = String.Format("Missing argument '{0}' for call {1}", parameterName, Format(method));
            return MakePartial(new HtmlHelperTagException(msg));
        }

        public static string Format(MethodInfo method)
        {
            var builder = new StringBuilder();
            builder.Append(method.Name);
            builder.Append("(");
            bool first = true;
            foreach (ParameterInfo info in method.GetParameters())
            {
                if (!first) builder.Append(", ");
                builder.Append(info.ParameterType);
                builder.Append(" ");
                builder.Append(info.Name);
                first = false;
            }
            builder.Append(")");
            return builder.ToString();
        }
    }
}