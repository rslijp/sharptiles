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

using System.Collections;

namespace org.SharpTiles.Common
{
    public class StackModel : IModel
    {
        private readonly IModel _model;
        private readonly IModel _parent;

        public StackModel() : this(null)
        {
        }
        public StackModel(IModel parent) : this(new Reflection(new Hashtable()), parent)
        {
        }

        public StackModel(IModel model, IModel parent)
        {
            _model = model;
            _parent = parent;
        }

        public object this[string property]
        {
            get { return TryGet(property); }
            set { _model[property] = value; }
        }

        public object TryGet(string property)
        {
            var result = Get(property);
            return result?.Result;
        }

        public ReflectionResult Get(string property)
        {
            var result = _model.Get(property);
            if (_parent == null) return result;
            if (result.Partial || result.Full) return result;
            return _parent.Get(property);

        }
    }

}
