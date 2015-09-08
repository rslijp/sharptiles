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

namespace org.SharpTiles.Templates
{
    public class TemplateException : Exception, IHaveHttpErrorCode
    {
        public TemplateException(string msg)
            : base(msg)
        {
        }

        public TemplateException(string msg, Exception innerException)
            : base(msg, innerException)
        {
        }

        #region IHaveHttpErrorCode Members

        public int? HttpErrorCode { get; set; }


        public TemplateException WithHttpErrorCode(int code)
        {
            HttpErrorCode = code;
            return this;
        }


        #endregion

        public static TemplateException TemplatePartCannotBeUsedAsContant(Type type)
        {
            var msg = String.Format("ConstantValue is not available for {0}.", type.Name);
            return new TemplateException(msg);
        }


        public static TemplateException TemplateNotFound(string name)
        {
            var msg = String.Format("The template {0} could not be found.", name);
            return new TemplateException(msg);
        }

        public TemplateException HavingHttpErrorCode(int code)
        {
            HttpErrorCode = code;
            return this;
        }


        public static TemplateException TemplateFailedToInitialize(string path, Exception nested)
        {
            string msg = String.Format("A template could not be initialized {0}. {1}", path,
                                       nested != null ? nested.Message : "");
            return new TemplateException(msg, nested);
        }

        public static TemplateException ErrorInTemplate(string path, Exception exception)
        {
            var msg = String.Format("Error in template {0}:{1}", path, exception.Message);
            return new TemplateException(msg, exception);
        }
    }
}