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
 namespace org.SharpTiles.Tags.CoreTags
{
    public class Core : BaseTagGroup<Core>
    {
        public Core()
        {
            Register<Out>();
            Register<Set>();
            Register<Remove>();
            Register<Catch>();
            Register<If>();
            Register<Choose>();
//            Register<When>();
//            Register<Otherwise>();
            Register<ForEach>();
            Register<ForTokens>();
            Register<Url>();
            Register<Import>();
            Register<Redirect>();
            Register<Param>();
        }


        public override string Name
        {
            get { return "c"; }
        }
    }
}
