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

namespace org.SharpTiles.Common
{
    public class ParseException : ExceptionWithContext
    {
        public ParseException(string msg) : base(msg)
        {
        }

        public ParseException(string msg, Exception e) : base(msg, e)
        {
        }

        public static PartialExceptionWithContext<ParseException> ExpectedToken()
        {
            String msg = String.Format("Expected more tokens");
            return MakePartial(new ParseException(msg));
        }


        public static PartialExceptionWithContext<ParseException> ExpectedToken(string what)
        {
            String msg = String.Format("Expected {0}", what);
            return MakePartial(new ParseException(msg));
        }

        public static PartialExceptionWithContext<ParseException> ExpectedToken(char expected)
        {
            String msg = String.Format("Expected token {0}", expected);
            return MakePartial(new ParseException(msg));
        }

        public static PartialExceptionWithContext<ParseException> TagsNotAllowed()
        {
            return MakePartial(new ParseException("Tags are not allowed at this position"));
        }

        public static PartialExceptionWithContext<ParseException> CantResolveTag(string tagName, string libs)
        {
            string msg = $"A tag with name '{tagName}' is found in multiple tag libs({libs}), a hint is required prefix with the '<tag group name>:'";
            return MakePartial(new ParseException(msg));
        }

        public static PartialExceptionWithContext<ParseException> UnexpectedCloseTag(string tag)
        {
            String msg = String.Format("Did not expect close tag {0}", tag);
            return MakePartial(new ParseException(msg));
        }

        public static PartialExceptionWithContext<ParseException> ExpectedCloseTag()
        {
            return MakePartial(new ParseException("Expected a closing tag but none was found."));
        }

        public static PartialExceptionWithContext<ParseException> UnexpectedError(Exception e)
        {
            return MakePartial(new ParseException("Unexpected error: "+e.Message, e));
        }
    }
}
