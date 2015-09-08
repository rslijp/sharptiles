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
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Tags;
using org.SharpTiles.Templates.Templates;
using org.SharpTiles.Tiles.Tile;

namespace org.SharpTiles.Tiles.Test.Tile
{
    [TestFixture]
    public class AttributeSetTest
    {
        [Test]
        public void AttributeSetShouldBeAbleToHandleNull()
        {
            var set = new AttributeSet(null);
            Assert.That(set.Count, Is.EqualTo(0));
        }

        [Test]
        public void AttributeSetShouldBeMakeANameTileMappping()
        {
            var firstTile = new TileAttribute("first", new StringTile("1"));
            var secondTile = new TileAttribute("second", new StringTile("2"));
            var set = new AttributeSet("TILE", new List<TileAttribute> { firstTile, secondTile });
            Assert.That(set.Count, Is.EqualTo(2));
        }

        [Test]
        public void TileAttributesShouldBeRetrievableByName()
        {
            var firstTile = new TileAttribute("first", new StringTile("1"));
            var secondTile = new TileAttribute("second", new StringTile("2"));
            var set = new AttributeSet("TILE", new List<TileAttribute> { firstTile, secondTile });
            Assert.That(set["first"], Is.Not.Null);
            Assert.That(set["first"], Is.SameAs(firstTile));
        }

        [Test]
        public void TileAttributesShouldThrowExceptionOnNonExistingName()
        {
            var firstTile = new TileAttribute("first", new StringTile("1"));
            var secondTile = new TileAttribute("second", new StringTile("2"));
            var set = new AttributeSet("TILE", new List<TileAttribute> { firstTile, secondTile });
            try
            {
                Assert.That(set["nonexisting"], Is.Not.Null);
                Assert.Fail("Expected exception");
            } catch (TileException Te)
            {
                Assert.That(Te.Message, Is.EqualTo(TileException.AttributeNotFound("nonexisting", "TILE").Message));
            }
        }

        [Test]
        public void ContainsOnTileAttributesShouldReturnTrueForExistingName()
        {
            var firstTile = new TileAttribute("first", new StringTile("1"));
            var secondTile = new TileAttribute("second", new StringTile("2"));
            var set = new AttributeSet("TILE", new List<TileAttribute> { firstTile, secondTile });
            Assert.That(set.HasDefinitionFor("first"));
        }

        [Test]
        public void ContainsOnTileAttributesShouldReturnFlaseForNonExistingName()
        {
            var firstTile = new TileAttribute("first", new StringTile("1"));
            var secondTile = new TileAttribute("second", new StringTile("2"));
            var set = new AttributeSet("TILE", new List<TileAttribute> { firstTile, secondTile });
            Assert.That(!set.HasDefinitionFor("nonexisting"));
     
        }

        [Test]
        public void TileAttributesShouldThrowExceptionWhenAddingTheSameName()
        {
            var firstTile = new TileAttribute("first", new StringTile("1"));
            var secondTile = new TileAttribute("first", new StringTile("2"));
            try
            {
                new AttributeSet("TILE", new List<TileAttribute> { firstTile, secondTile });
                Assert.Fail("Expected exception");
            }
            catch (TileException Te)
            {
                Assert.That(Te.Message, Is.EqualTo(TileException.AttributeNameAlreadyUsed("first", "TILE").Message));
            }
        }

        [Test]
        public void FlattenShouldAddExistingAttributesToNewSet()
        {
            var firstSet = new AttributeSet("TILE", new TileAttribute("first", new StringTile("1")));
            var secondSet = new AttributeSet("TILE", new TileAttribute("second", new StringTile("1")));
            Assert.That(firstSet.Count, Is.EqualTo(1));
            Assert.That(secondSet.Count, Is.EqualTo(1));
            firstSet.Merge(secondSet);
            Assert.That(firstSet.Count, Is.EqualTo(2));
            Assert.That(secondSet.Count, Is.EqualTo(1));
        }

        [Test]
        public void FlattenShouldAddExistingAttributesToNewSetButNotOverwriteExistingOnes()
        {
            var firstSet = new AttributeSet(
                "TILE", 
                new TileAttribute("new", new StringTile("1")),
                new TileAttribute("parent", new StringTile("2")));
            var secondSet = new AttributeSet("OTHER", new TileAttribute("parent", new StringTile("1")));
            Assert.That(firstSet.Count, Is.EqualTo(2));
            Assert.That(secondSet.Count, Is.EqualTo(1));
            firstSet.Merge(secondSet);
            Assert.That(firstSet.Count, Is.EqualTo(2));
            Assert.That(secondSet.Count, Is.EqualTo(1));
        }

        [Test]
        public void FlattenShouldNotOverwriteExistingAttributesValues()
        {
            var firstSet = new AttributeSet(
                "TILE",
                new TileAttribute("new", new StringTile("1")),
                new TileAttribute("parent", new StringTile("2")));
            var secondSet = new AttributeSet("TILE", new TileAttribute("parent", new StringTile("1")));
            firstSet.Merge(secondSet);
            Assert.That(
                firstSet["parent"].Value.Render(null),
                Is.EqualTo("2")
            );
            Assert.That(
               secondSet["parent"].Value.Render(null),
               Is.EqualTo("1")
            );
            Assert.That(
                firstSet["parent"].Value.Render(null), 
                Is.Not.EqualTo(secondSet["parent"].Value.Render(null))
            );
        }

        [Test]
        public void FlattenShouldHandleNull()
        {
            var firstSet = new AttributeSet(
                "TILE",
                new TileAttribute("new", new StringTile("1")),
                new TileAttribute("parent", new StringTile("2")));
            firstSet.MergeTileLazy(null);
        }

        [Test]
        public void FlattenShouldBeDoneLazilyToCopeWithNotYetDefinedDefinitions()
        {
            var map = new TilesMap();
            var firstSet = new AttributeSet("TILE", new TileAttribute("new", new StringTile("1")));
            var parentTile = new TileReference("reference", map);
            firstSet.MergeTileLazy(parentTile);
            map.AddTile(new TemplateTile("reference", new FileTemplate("a.htm"), new AttributeSet("PARENT", new TileAttribute("parent", new StringTile("2")))));
            Assert.That(firstSet["parent"].Value.Render(new TagModel(null)), Is.EqualTo("2"));
        }
    }
}
