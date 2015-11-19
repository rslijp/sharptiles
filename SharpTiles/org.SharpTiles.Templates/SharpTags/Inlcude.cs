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
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.CoreTags;
using org.SharpTiles.Tags.Creators;

namespace org.SharpTiles.Templates.SharpTags
{
    public class Include : BaseCoreTag, ITag, ITagRequiringTagLib
    {
        public static readonly string NAME = "include";

        private ITemplate _body;
        private ITagAttribute _file;

        [Required]
        public ITagAttribute File
        {
            set
            {
                _file = value;
            }
            get { return _file; }
        }

        #region ITag Members

        public string TagName
        {
            get { return NAME; }
        }


        public string Evaluate(TagModel model)
        {
            var fileName = _file.Evaluate(model);
                
            try
            {
                if (fileName == null) return String.Empty;
                LoadTile(model, fileName.ToString());
                return _body.Template.Evaluate(model);
            }
            catch (Exception e)
            {
                throw ErrorInInclude(e, fileName.ToString());
            }
        }

        public ITagLib TagLib { get; set; }

        private void LoadTile(TagModel model, String fileName)
        {

            lock (this)
            {
                if (_body == null || !Equals(fileName, _body.Path))
                {
                    _body = model.Factory.CloneForTagLib(TagLib).Handle(fileName, _file.ResourceLocator, true);
                }
            }
        }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.None; }
        }

        #endregion


        public IncludeException ErrorInInclude(Exception exception, String fileName)
        {
            return IncludeException.Make(fileName, exception).Decorate(Context);
        }

        #region Nested type: IncludeException

        public class IncludeException : ExceptionWithContext
        {
            private IncludeException(string msg, Exception exception) : base(msg, exception)
            {
            }

            public static PartialExceptionWithContext<IncludeException> Make(string fileName, Exception exception)
            {
                var msg = String.Format("Error in file {0}:{1}", fileName, exception.Message);
                return MakePartial(new IncludeException(msg, exception).EnsureHttpErrorCode());
            }

            public IncludeException EnsureHttpErrorCode()
            {
                HttpErrorCode = 500;
                if(InnerException == null) return this;
                if(!(InnerException is IHaveHttpErrorCode)) return this;
                HttpErrorCode = ((IHaveHttpErrorCode) InnerException).HttpErrorCode;
                return this;
            }
        }

        #endregion
    }
}