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
    public class ExceptionWithContext : Exception, IHaveHttpErrorCode
    {
        private ParseContext _context;

        public ExceptionWithContext(string message) : base(message)
        {
        }

        public ExceptionWithContext(string message, Exception inner) : base(message, inner)
        {
        }

        public int? HttpErrorCode
        {
            get;
            set;
        }

        public ParseContext Context
        {
            get { return _context; }
        }

        public ExceptionWithContext Update(ParseContext context)
        {
            _context = context;
            return this;
        }


        public static PartialExceptionWithContext<T> MakePartial<T>(T partial) where T : ExceptionWithContext
        {
            return new PartialExceptionWithContext<T>(partial);
        }

        public string MessageWithOutContext
        {
            get
            {
                return base.Message;
            }
        }

        public override string Message
        {
            get
            {
                return Context != null
                    ?
                        base.Message + Environment.NewLine + Environment.NewLine + Context
                    :
                        base.Message; 
            }
        }

        public override string ToString()
        {
            return Message;
        }

        #region Nested type: PartialExceptionWithContext

        public class PartialExceptionWithContext<T> where T : ExceptionWithContext
        {
            private readonly T _partial;


            public PartialExceptionWithContext(T partial)
            {
                _partial = partial;
            }

            public string Message
            {
                get { return _partial.Message; }
            }

            public T Decorate(Token token)
            {
                return Decorate(token.Context);
            }

            public T Decorate(ParseContext context)
            {
                _partial._context = context;
                return _partial;
            }
        }

        #endregion
    }
}
