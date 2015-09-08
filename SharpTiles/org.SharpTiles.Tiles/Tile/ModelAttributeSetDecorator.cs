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
using System.Collections.Generic;
using org.SharpTiles.Tags;

namespace org.SharpTiles.Tiles.Tile
{
    public class TagModelAttributeStack : IDisposable
    {
        public static readonly string VARIABLE_NAME = "tilesAttributes";
        
        private TagModel _model;
        private bool _pop;

        public TagModelAttributeStack(TagModel model)
        {
            _model = model;
            GuardTagAttributeStack();
        }

        public Stack<AttributeSet> Stack
        {
            get { return (Stack<AttributeSet>) _model.Page[VARIABLE_NAME]; }
            set { _model.Page[VARIABLE_NAME] = value;  }
        }

        public AttributeSet Current
        {
            get { return Stack.Peek();  }
        }

        private void GuardTagAttributeStack()
        {
            if (Stack == null)
            {
                Stack = new Stack<AttributeSet>();
            }
        }

        public TagModelAttributeStack With(AttributeSet attributes)
        {
            Stack.Push(attributes);
            _pop = true;
            return this;
        }


        public void Dispose()
        {
            if(_pop)
            {
                Stack.Pop();
            } 
        }

    }
}
