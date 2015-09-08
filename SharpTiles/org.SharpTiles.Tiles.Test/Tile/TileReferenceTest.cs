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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Templates;
using org.SharpTiles.Tiles.Tile;

namespace org.SharpTiles.Tiles.Test.Tile
{
    [TestFixture]
    public class TileReferenceTest
    {
        [Test]
        public void InitOfTileShouldBeLazy()
        {
            var fallBack = new StringTile("val");
            var map = new TilesMap();
            var tile = new TileReference("name", map, fallBack);
            Assert.That(tile.Tile, Is.Null);
            tile.Render(new TagModel(new object()));
            Assert.That(tile.Tile, Is.Not.Null);
        }

        [Test]
        public void GuardShouldInitTile()
        {
            var fallBack = new StringTile("val");
            var map = new TilesMap();
            var tile = new TileReference("name", map, fallBack);
            Assert.That(tile.Tile, Is.Null);
            tile.GuardInit();
            Assert.That(tile.Tile, Is.Not.Null);
        }

        [Test]
        public void WhenTheTileIsNotAvailableTheFallBackTileShouldBeUsed()
        {
            var fallBack = new StringTile("val");
            var map = new TilesMap();
            var tile = new TileReference("name", map, fallBack);
            tile.GuardInit();
            Assert.That(tile.Tile, Is.SameAs(fallBack));
        }

        [Test]
        public void WhenTheTileIsAvailableTheTileShouldBeUsedAndNotTheFallBack()
        {
            var fallBack = new StringTile("val");
            var source = new MockTile("name");
            var map = new TilesMap();
            map.AddTile(source);
            var tile = new TileReference(source.Name, map, fallBack);
            tile.GuardInit();
            Assert.That(tile.Tile, Is.SameAs(source));
        }

        [Test]
        public void WhenTheTileIsNotAvailableAndNoFallBackIsAvailableGuardInitShouldFail()
        {
            var map = new TilesMap();
            var tile = new TileReference("name", map);
            try
            {
                tile.GuardInit();
                Assert.Fail("Expected exception");
            }
            catch (TemplateException Te)
            {
                Assert.That(Te.Message, Is.EqualTo(TemplateException.TemplateNotFound("name").Message));
            }
        }

        [Test]
        public void WhenTheTileIsAvailableTheTileShouldBeUsed()
        {
            var source = new MockTile("name");
            var map = new TilesMap();
            map.AddTile(source);
            var tile = new TileReference(source.Name, map);
            tile.GuardInit();
            Assert.That(tile.Tile, Is.SameAs(source));
        }

        [Test]
        public void TheRenderingShouldBeDelegatedToTheReferenceTileInCase()
        {
            var model = new TagModel(new object());
            var fallBack = new StringTile("val");
            var source = new MockTile("name");
            var map = new TilesMap();
            map.AddTile(source);
            var tile = new TileReference(source.Name, map, fallBack);
            tile.GuardInit();
            Assert.That(fallBack.Render(model), Is.Not.EqualTo((source.Render(model))), "Precondition");
            Assert.That(tile.Render(model), Is.EqualTo((source.Render(model))));
        }

        [Test]
        public void TheRenderingShouldBeDelegatedToTheReferenceTileInCaseOfFallBack()
        {
            var model = new TagModel(new object());
            var fallBack = new StringTile("val");
            var map = new TilesMap();
            var tile = new TileReference("name", map, fallBack);
            tile.GuardInit();
            Assert.That(tile.Render(model), Is.EqualTo((fallBack.Render(model))));
        }
    }
}
