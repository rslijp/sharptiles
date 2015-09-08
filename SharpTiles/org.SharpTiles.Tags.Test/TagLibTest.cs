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
using System.ComponentModel;
using CategoryAttribute=System.ComponentModel.CategoryAttribute;

namespace org.SharpTiles.Tags.Test
{
    [TestFixture]
    public class TagLibTest
    {
        [Test]
        public void TestPresenceOfCategories()
        {
            foreach (var taggroup in TagLib.Tags)
            {
                if (!taggroup.Name.Equals("sharp"))
                {
                    Console.WriteLine(taggroup.Name);
                    GuardPresenceOfCategoriesOnTagGroup(taggroup);
                }
            }
            
        }

        private void GuardPresenceOfCategoriesOnTagGroup(ITagGroup taggroup)
        {
            foreach (var tag in taggroup)
            {
                Console.Write("\t"+tag.TagName+":");
                GuardPresenceOfCategoriesOnTag(tag);
            }
        }

        private void GuardPresenceOfCategoriesOnTag(ITag tag)
        {
            Type tagType = tag.GetType();
            object[] categories = tagType.GetCustomAttributes(typeof(CategoryAttribute), true);
            Assert.That(categories.Length, Is.EqualTo(1));
            Console.WriteLine(((CategoryAttribute) categories[0]).Category);
        }
    }
}

