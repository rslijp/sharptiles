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
 */using System;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using org.SharpTiles.Common;
using org.SharpTiles.Connectors;
using org.SharpTiles.Tags;
using org.SharpTiles.Tiles;
using org.SharpTiles.Tiles.Tile;

namespace org.SharpTiles.NUnit
{
    public class OutputEqualsConstraint : Constraint
    {
        #region fields and properties
        private Constraint _expectedResult;
        private TagModel _model;
        private IViewCache _cache;

        private bool InvertMatch { get; set; }

        private bool ExactMatch { get; set; }

        private bool UseStubTiles { get; set; }

        private string ResultFile { get; set; }
        #endregion

        #region Fluiend

        public OutputEqualsConstraint Not
        {
            get
            {
                InvertMatch = true;
                return this;
            }
        }

        public OutputEqualsConstraint EqualTo
        {
            get
            {
                ExactMatch = true;
                return this;
            }
        }

        public OutputEqualsConstraint Like
        {
            get
            {
                ExactMatch = false;
                return this;
            }
        }


        public OutputEqualsConstraint File(string expected)
        {
            return File(expected, Encoding.UTF8);
        }

        public OutputEqualsConstraint File(string expectedFile, Encoding encoding)
        {
            return Text(encoding.GetString(System.IO.File.ReadAllBytes(expectedFile)));
        }

        public OutputEqualsConstraint Text(string expectedResultStr)
        {
            expectedResultStr = CleanUp(expectedResultStr);
                
            if (InvertMatch)
            {
                _expectedResult = global::NUnit.Framework.SyntaxHelpers.Is.Not.EqualTo(expectedResultStr);
            }
            else
            {
                _expectedResult = global::NUnit.Framework.SyntaxHelpers.Is.EqualTo(expectedResultStr);
            }
                
            return this;
        }


        public OutputEqualsConstraint UsingModel(object model)
        {
             return UsingModel(
                new TagModel(model, 
                new MockSessionState(),
                null,
                new MockResponse(AppPath))
             );
        }

        private string AppPath { get; set; }


        public OutputEqualsConstraint UsingModel(TagModel model)
        {
            _model = model;
            return this;
        }

        public OutputEqualsConstraint StubOutTiles()
        {
            UseStubTiles = true;
            return this;
        }

        public OutputEqualsConstraint StoreResultInFile(string path)
        {
            ResultFile = path;
            return this;
        }


        #endregion

        #region Constraint 

        public override bool Matches(object actual)
        {
            if (actual is string)
            {
                actual = GetTile(actual);
            }
            if (actual is ITile)
            {
                actual = CleanUp(RenderTile(actual, _model));
                Save(actual as String);
            }
            return _expectedResult.Matches(actual);
        }

        private void Save(string content)
        {
            if (!String.IsNullOrEmpty(ResultFile))
            {
                System.IO.File.WriteAllText(ResultFile, content);
            }
        }

        private  string CleanUp(string result)
        {
            return !ExactMatch ?
                                   new string(result.ToList().Where(c => !Char.IsWhiteSpace(c)).ToArray()).Trim() :
                                                                                                                      result;
//                      result.Trim();
        }

        public override void WriteMessageTo(MessageWriter writer)
        {
            _expectedResult.WriteMessageTo(writer);
        }

            
        public override void WriteDescriptionTo(MessageWriter writer)
        {
            _expectedResult.WriteDescriptionTo(writer);
        }

        #endregion

        #region helpers 

        private ITile GetTile(object actual)
        {
            if(_cache==null) throw new NullReferenceException("A tile cache is required. Please state one with the From method.");
            var actualStr = (string)actual;
            var breakUp = actualStr.Split('@');
            var tile = _cache.GetView(breakUp[0]);
            if (breakUp.Length > 1)
            {
                tile = tile.Attributes[breakUp[1]].Value;
            }
            return tile;
        }

        private string RenderTile(object actual, TagModel model)
        {
            var actualTile = (ITile)actual;
            model = model ?? new TagModel(new object());
            var attributes = actualTile.Attributes;
            attributes = StubOut(attributes);
            var result = actualTile.Render(model, attributes);
            return result;
        }

        private AttributeSet StubOut(AttributeSet attributes)
        {
            if(UseStubTiles)
            {
                var tiles = attributes.ToList().ConvertAll(t =>
                                                           new TileAttribute(t.Name,
                                                                             new StringTile("@" + t.Name + "@")
                                                               )
                    );
                attributes = new AttributeSet(null, tiles);
            }
            return attributes;
        }

        #endregion

        public OutputEqualsConstraint From(IViewCache cache)
        {
            _cache = cache;
            return this;
        }

        public OutputEqualsConstraint And()
        {
            return this;
        }

        public OutputEqualsConstraint HavingApplicationPath(String path)
        {
            AppPath = path;
            return this;
        }
    }
}
