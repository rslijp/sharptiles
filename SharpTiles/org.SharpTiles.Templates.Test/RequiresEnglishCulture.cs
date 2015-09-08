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
 */using System.Globalization;
using System.Threading;
using NUnit.Framework;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.FormatTags;

namespace org.SharpTiles.Templates.Test
{
    [TestFixture]
    public class RequiresEnglishCulture
    {
        #region Setup/Teardown

        [SetUp]
        public virtual void SetUp()
        {
            _oldCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            new TagModel(new object()).Global[FormatConstants.LOCALE] = Thread.CurrentThread.CurrentCulture;
        }

        [TearDown]
        public virtual void TearDown()
        {
            Thread.CurrentThread.CurrentCulture = _oldCulture;
        }

        #endregion

        private CultureInfo _oldCulture;
    }
}
