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
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
 using org.SharpTiles.Tags;
 using org.SharpTiles.Tags.CoreTags;
using org.SharpTiles.Tags.FormatTags;

namespace org.SharpTiles.Documentation.Test
{
    [TestFixture]
    public class DocumentModelTest
    {
        private TagLib _lib;

        [SetUp]
        public void SetUp()
        {
            _lib = new TagLib();
//            _lib.Register(new Tiles.Tags.Tiles());
//            _lib.Register(new Sharp());
        }

        [Test]
        public void PropertyInTagsShouldBePresent()
        {
            var tag = new TagDocumentation(new ResourceKeyStack(), new Out(), new List<Func<ITag, TagDocumentation, bool>>());
            Assert.That(tag.Name, Is.EqualTo(new Out().TagName));
            Assert.That(tag.Properties, Is.Not.Null);
            Assert.That(tag.Properties.Count, Is.GreaterThan(0));
        }

        [Test]
        public void PropertyShouldReturnDescriptionKey()
        {
            var key  = new ResourceKeyStack();
            key = key.BranchFor(new Core());
            key = key.BranchFor(new Out());
            var property = new PropertyDocumentation(key,
                                                     typeof (Out).GetProperty("Value"));
            Assert.That(property.DescriptionKey, Is.EqualTo("description_Core_Out_Value"));
        }

        [Test]
        public void PropertyShouldReturnDescriptionKeyOfDeclaringType()
        {
            ResourceKeyStack key = new ResourceKeyStack();
            key = key.BranchFor(new Core());
            key = key.BranchFor(new Set());
            var property = new PropertyDocumentation(key,
                                                     typeof (Set).GetProperty("Var"));
            Assert.That(property.DescriptionKey, Is.EqualTo("description_BaseCoreTagWithVariable_Var"));
        }

        [Test]
        public void PropertyShouldReturnDescriptionKeyOfDeclaringGroup()
        {
            var key = new ResourceKeyStack();
            key = key.BranchFor(new Format());
            key = key.BranchFor(new Message());
            
            var property = new PropertyDocumentation(key,
                                                     typeof(Message).GetProperty("Scope"));
            Assert.That(property.DescriptionKey, Is.EqualTo("description_BaseCoreTagWithOptionalVariable_Scope"));
        }

        [Test]
        public void TagGroupShouldReturnDescriptionKey()
        {
            
            var taggroup = new TagGroupDocumentation(new ResourceKeyStack(), new Core(), new List<Func<ITag, TagDocumentation, bool>>());
            Assert.That(taggroup.DescriptionKey, Is.EqualTo("description_Core"));
        }

        [Test]
        public void TagGroupsInDocumentModelShouldBePresent()
        {
            var model = new DocumentModel(_lib, true);
            Assert.That(model.TagGroups, Is.Not.Null);
            Assert.That(model.TagGroups.Count, Is.GreaterThan(0));
        }

        [Test]
        public void TagInTagGroupsShouldBePresent()
        {
            var taggroup = new TagGroupDocumentation(new ResourceKeyStack(), new Core(), new List<Func<ITag, TagDocumentation, bool>>());
            Assert.That(taggroup.Name, Is.EqualTo(new Core().Name));
            Assert.That(taggroup.Tags, Is.Not.Null);
            Assert.That(taggroup.Tags.Count, Is.GreaterThan(0));
        }

        [Test]
        public void TagShouldReturnDescriptionKey()
        {
            var key = new ResourceKeyStack();
            key = key.BranchFor(new Core());
            
            var tag = new TagDocumentation(key, new Out(), new List<Func<ITag, TagDocumentation, bool>>());
            Assert.That(tag.DescriptionKey, Is.EqualTo("description_Core_Out"));
        }

        [Test]
        public void TagShouldReturnCategoryDescriptionKey()
        {
            var key = new ResourceKeyStack();
            key = key.BranchFor(new Core());

            var tag = new TagDocumentation(key, new Out(), new List<Func<ITag, TagDocumentation, bool>>());
            Assert.That(tag.CategoryDescriptionKey, Is.EqualTo("description_Core_GeneralPurpose"));
        }

        [Test]
        public void TestAllExpressionsHaveACategory()
        {
            foreach (var expression in new DocumentModel(_lib,true).Expressions)
            {
                Assert.That(expression.Category, Is.Not.Null, expression.Name+" should have category");
            }
        }

        [Test]
        public void TestAllTranslations()
        {
            var checker = new TranslationChecker(new ResourceBundle("templates/Documentation", null));
            foreach (var expression in new DocumentModel(_lib, true).Expressions)
            {
                checker.GuardDescription(expression);
                checker.GuardDescription(expression.CategoryDescriptionKey);
            }
            foreach (var function in new DocumentModel(_lib, true).Functions)
            {
                checker.GuardDescription(function);
            }
            CheckTranslationsOfTags(checker);
            checker.Guard();
        }

        private void CheckTranslationsOfTags(TranslationChecker checker)
        {
            foreach (var group in new DocumentModel(_lib, true).TagGroups)
            {
                checker.GuardDescription(group);
                if (group.ExampleKey != null)
                {
                    checker.GuardDescription(group.ExampleKey);
                }
                foreach (var tag in group.Tags)
                {
                    checker.GuardDescription(tag);
                    if(tag.Category!=null)
                    {
                        checker.GuardDescription(tag.CategoryDescriptionKey);
                    }
                    if(tag.ExampleKey!=null)
                    {
                        checker.GuardDescription(tag.ExampleKey);
                    }
                    foreach (var property in tag.Properties)
                    {
                        checker.GuardDescription(property);
                    }    
                }                
            }
        }

        private class TranslationChecker
        {
            private ResourceBundle _bundle;
            private IList<string> _missing = new List<string>();

            public TranslationChecker(ResourceBundle bundle)
            {
                _bundle = bundle;
            }

            public void GuardDescription(IDescriptionElement element)
            {
                GuardDescription(element.DescriptionKey);
            }

            public void GuardDescription(string key)
            {
                if (_bundle.Contains(key))
                {
                    _missing.Add(key);
                    Console.WriteLine(key + " is missing.");
                }
            }

            public void Guard()
            {
                Assert.That(_missing.Count, Is.EqualTo(0));
            }
        }

        
    }
}
