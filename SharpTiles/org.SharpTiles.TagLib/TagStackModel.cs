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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.SharpTiles.Common;

namespace org.SharpTiles.Tags
{
    public class TagStackModel : IReflectionModel
    {
        private readonly IModel _model;
        private readonly TagStackModel _parent;

        public TagStackModel() : this(null)
        {
        }

        public TagStackModel(TagStackModel parent)
        {
            _model = new Reflection(new Hashtable());
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
