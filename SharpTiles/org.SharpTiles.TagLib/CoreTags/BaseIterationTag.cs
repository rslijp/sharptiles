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
using System.Collections;
using System.Text;
using org.SharpTiles.Common;

namespace org.SharpTiles.Tags.CoreTags
{
    public abstract class BaseIterationTag : BaseCoreTag, ITag
    {
        [TagDefaultValue(0)]
        public ITagAttribute Begin { get; set; }

        public ITagAttribute End { get; set; }

        [TagDefaultValue(1)]
        public ITagAttribute Step { get; set; }

        [TagDefaultValue("Item")]
        public ITagAttribute Var { get; set; }

        [TagDefaultValue("Status")]
        public ITagAttribute VarStatus { get; set; }

        [Internal]
        public ITagAttribute Body { get; set; }

        #region ITag Members

        public abstract string TagName { get; }

        public abstract TagBodyMode TagBodyMode { get; }

        public string Evaluate(TagModel model)
        {
            var builder = new StringBuilder();
            IList list = ToList(GetIEnumerable(model));
            int start = GetAutoValueAsInt("Begin", model).Value;
            int end = GetAsInt(End, model) ?? list.Count;
            int step = GetAutoValueAsInt("Step", model).Value;
            string var = GetAutoValueAsString("Var", model);
            string varStatus = GetAutoValueAsString("VarStatus", model);
            model.PushTagStack(true);
            var status = new ForEachStatus(list.Count, start, end);
            if (list.Count > 0)
            {
                model.Tag[varStatus] = status;
                for (int i = start; i < end; i += step)
                {
                    status.Index = i;
                    model.Tag[var] = list[i];
                    builder.Append(GetAsString(Body, model) ?? String.Empty);
                }
                model.Tag[var] = null;
                model.Tag[varStatus] = null;
            }
            model.PopTagStack();
            return builder.ToString();
        }

        #endregion

        public abstract IEnumerable GetIEnumerable(TagModel model);

        private IList ToList(IEnumerable items)
        {
            if (items is IList)
            {
                return (IList) items;
            }
            if (items is ICollection)
            {
                return new ArrayList((ICollection) items);
            }
            var list = new ArrayList();
            foreach (object o in items)
            {
                list.Add(o);
            }
            return list;
        }

        #region Nested type: ForEachStatus

        public class ForEachStatus
        {
            private int _count;
            private int _first;
            private int _index;
            private int _last;


            public ForEachStatus(int count, int first, int last)
            {
                _count = count;
                _first = first;
                _last = last;
            }

            public int Index
            {
                get { return _index; }
                set { _index = value; }
            }

            public int Count
            {
                get { return _count; }
                set { _count = value; }
            }

            public int First
            {
                get { return _first; }
                set { _first = value; }
            }

            public int Last
            {
                get { return _last; }
                set { _last = value; }
            }

            public bool IsLast
            {
                get { return _index == _last - 1; }
            }

            public bool IsFirst
            {
                get { return _index == 0; }
            }

            public override string ToString()
            {
                return "From " + _first + " to " + _last + "/" + _count + " now at " + _index;
            }
        }

        #endregion
    }
}
