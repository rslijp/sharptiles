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
 */using System.Collections;
using System.Collections.Generic;

namespace org.SharpTiles.Tiles.Tile
{
    public class AttributeSet : IEnumerable<TileAttribute>
    {
        private readonly string _definitionName = null;
        private readonly IDictionary<string, TileAttribute> _attributes;
        private ITile _mergeLazyWith = null;


        public AttributeSet(string definitionName, params TileAttribute[] attributes)
            : this(definitionName, attributes != null ? new List<TileAttribute>(attributes) : null)
        {
        }

        public AttributeSet(string definitionName, IEnumerable<TileAttribute> attributes)
        {
            attributes = attributes ?? new List<TileAttribute>();
            _attributes = new Dictionary<string, TileAttribute>();
            _definitionName = definitionName;
            AddAttributes(attributes, true);
        }

        private void AddAttributes(IEnumerable<TileAttribute> attributes, bool throwExceptionOnDoubles)
        {
            foreach (TileAttribute attribute in attributes)
            {
                string name = attribute.Name;
                if (_attributes.ContainsKey(name))
                {
                    if (throwExceptionOnDoubles)
                    {
                        throw TileException.AttributeNameAlreadyUsed(name, _definitionName);
                    }
                    continue;
                }
                _attributes.Add(name, attribute);
            }
        }

        public TileAttribute this[string name]
        {
            get
            {
                try
                {
                    return Attributes[name];
                }
                catch (KeyNotFoundException)
                {
                    throw TileException.AttributeNotFound(name, _definitionName);
                }
            }
        }



        private IDictionary<string, TileAttribute> Attributes
        {
            get
            {
                GuardLazyMerge();
                return _attributes;
            }
        }

        private void GuardLazyMerge()
        {
            if(_mergeLazyWith!=null)
            {
                Merge(_mergeLazyWith.Attributes);
                _mergeLazyWith = null;
            }
        }

        public int Count
        {
            get { return Attributes.Count; }
        }

        public void MergeTileLazy(ITile other)
        {
            if (other == null)
            {
                return;
            }
            _mergeLazyWith = other;
        }

        public void Merge(AttributeSet attributes)
        {
            AddAttributes(attributes, false);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TileAttribute>) this).GetEnumerator();
        }

        public IEnumerator<TileAttribute> GetEnumerator()
        {
            return _attributes.Values.GetEnumerator();
        }

        public bool HasDefinitionFor(string name)
        {
            return Attributes.ContainsKey(name);
        }
    }
}
