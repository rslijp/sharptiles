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
 namespace org.SharpTiles.Tags.FormatTags
{
    public class Format : BaseTagGroup<Format>
    {
        public Format()
        {
            Register<SetLocale>();
            Register<RequestEncoding>();
            Register<Bundle>();
            Register<SetBundle>();
            Register<Message>();
//            Register<Param>();
            Register<FormatNumber>();
            Register<ParseNumber>();
            Register<FormatDate>();
            Register<ParseDate>();
        }

        public override string Name
        {
            get { return "fmt"; }
        }
    }
}
