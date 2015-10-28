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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Common;
 using org.SharpTiles.NUnit;

namespace org.SharpTiles.Tags.Test
{
    [TestFixture]
    public class TagModelTest
    {
        [TearDown]
        public void TearDown()
        {
            TagModel.GlobalModel["NewValue"] = null;
        }

        [Test]
        public void GlobalResolveTest()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.Global["ResolveNewValue"] = "abc";
            Assert.That(model.Global["ResolveNewValue"], Is.EqualTo("abc"));
            Assert.That(model["ResolveNewValue"], Is.EqualTo("abc"));
        }

        [Test]
        public void GlobalTest()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.Global["ANewValue"] = "abc";
            Assert.That(model.Global["ANewValue"], Is.EqualTo("abc"));
            Assert.That(model[VariableScope.Global + ".ANewValue"], Is.EqualTo("abc"));
        }

        [Test]
        public void ModelAbovePageResolveTest()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.Page["NewValue"] = "Page";
            Assert.That(model["NewValue"], Is.EqualTo("Page"));
            model.Model["NewValue"] = "Model";
            Assert.That(model["NewValue"], Is.EqualTo("Model"));
        }

        [Test]
        public void ModelResolveTest()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.Model["NewValue"] = "abc";
            Assert.That(model["NewValue"], Is.EqualTo("abc"));
        }

        [Test]
        public void ModelTest()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.Model["NewValue"] = "abc";
            Assert.That(model.Model["NewValue"], Is.EqualTo("abc"));
            Assert.That(model[VariableScope.Model + ".NewValue"], Is.EqualTo("abc"));
        }

        [Test]
        public void NonExistingNestedResolveTest()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.Model["a"] = new Hashtable();
            model.Page["a"] = new Hashtable();

            try
            {
                object o = model["a.b"];
            }
            catch (ReflectionException Re)
            {
                Assert.That(Re.Message, Is.EqualTo(ReflectionException.NoSourceAvailable("b").Message));
            }
        }

        [Test]
        public void NonExistingNestedResolve_Prefers_Deepest_Nesting_First_One_Deepested_Nesting()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.Model["a"] = new Hashtable{{"b", new Hashtable()}};
            model.Page["a"] = new Hashtable();
            try
            {
                object o = model["a.b.c"];
            }
            catch (ReflectionException Re)
            {
                Assert.That(Re.Message, Is.EqualTo(ReflectionException.NoSourceAvailable("c").Message));
            }
        }

        [Test]
        public void NonExistingNestedResolve_Prefers_Deepest_Nesting_Second_One_Deepested_Nesting()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.Model["a"] = new Hashtable();
            model.Page["a"] = new Hashtable { { "b", new Hashtable() } };
            try
            {
                object o = model["a.b.c"];
            }
            catch (ReflectionException Re)
            {
                Assert.That(Re.Message, Is.EqualTo(ReflectionException.NoSourceAvailable("c").Message));
            }
        }


        [Test]
        public void NonExistingResolveTest()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            Assert.That(model["NonExisting"], Is.Null);
        }

        [Test]
        public void PageAboveSessionResolveTest()
        {
            var model = new TagModel(new Reflection(new Hashtable()), new MockSessionState());
            model.Session["NewValue"] = "Session";
            Assert.That(model["NewValue"], Is.EqualTo("Session"));
            model.Page["NewValue"] = "Page";
            Assert.That(model["NewValue"], Is.EqualTo("Page"));
        }

        [Test]
        public void PageResolveTest()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.Page["NewValue"] = "abc";
            Assert.That(model["NewValue"], Is.EqualTo("abc"));
        }

        [Test]
        public void PageTest()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.Page["NewValue"] = "abc";
            Assert.That(model.Page["NewValue"], Is.EqualTo("abc"));
            Assert.That(model[VariableScope.Page + ".NewValue"], Is.EqualTo("abc"));
        }

        [Test]
        public void SeachInTagScopeEmpyScope()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            Assert.That(model.SearchInTagScope("a"), Is.Null);
        }

        [Test]
        public void SeachInTagScopeEmpyScopeOneScope()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.PushTagStack();
            model.Tag["a"] = 1;
            Assert.That(model.SearchInTagScope("a"), Is.EqualTo(1));
        }

        [Test]
        public void SeachInTagScopeEmpyScopeThreeScopesValueInTheSecondScope()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.PushTagStack();
            model.PushTagStack();
            model.Tag["a"] = 1;
            model.PushTagStack();
            Assert.That(model.SearchInTagScope("a"), Is.EqualTo(1));
        }

        [Test]
        public void SeachInTagScopeEmpyScopeTwoScopesValueInTheFirstScope()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.PushTagStack();
            model.Tag["a"] = 1;
            model.PushTagStack();
            Assert.That(model.SearchInTagScope("a"), Is.EqualTo(1));
        }

        [Test]
        public void SeachInTagScopeEmpyScopeTwoScopesValueInTheSecondScope()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.PushTagStack();
            model.PushTagStack();
            model.Tag["a"] = 1;
            Assert.That(model.SearchInTagScope("a"), Is.EqualTo(1));
        }

        [Test]
        public void SeachInTagScopeEmpyScopeTwoScopesValuesOverride()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.PushTagStack();
            model.Tag["a"] = 0;
            model.PushTagStack();
            model.Tag["a"] = 1;
            Assert.That(model.SearchInTagScope("a"), Is.EqualTo(1));
            model.PopTagStack();
            Assert.That(model.SearchInTagScope("a"), Is.EqualTo(0));
        }

        [Test]
        public void SessionAboveGlobalResolveTest()
        {
            var model = new TagModel(new Reflection(new Hashtable()), new MockSessionState());
            model.Global["NewValue"] = "Global";
            Assert.That(model["NewValue"], Is.EqualTo("Global"));
            model.Session["NewValue"] = "Session";
            Assert.That(model["NewValue"], Is.EqualTo("Session"));
        }

        [Test]
        public void SessionResolveTest()
        {
            var model = new TagModel(new Reflection(new Hashtable()), new MockSessionState());
            model.Session["NewValue"] = "abc";
            Assert.That(model["NewValue"], Is.EqualTo("abc"));
        }

        [Test]
        public void SessionTest()
        {
            var model = new TagModel(new Reflection(new Hashtable()), new MockSessionState());
            model.Session["NewValue"] = "abc";
            Assert.That(model.Session["NewValue"], Is.EqualTo("abc"));
            Assert.That(model[VariableScope.Session + ".NewValue"], Is.EqualTo("abc"));
        }

        [Test]
        public void TagAboveModelResolveTest()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.PushTagStack();
            model.Model["NewValue"] = "Model";
            Assert.That(model["NewValue"], Is.EqualTo("Model"));
            model.Tag["NewValue"] = "Tag";
            Assert.That(model["NewValue"], Is.EqualTo("Tag"));
        }

        [Test]
        public void TagResolveTest()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.PushTagStack();
            model.Tag["NewValue"] = "abc";
            Assert.That(model["NewValue"], Is.EqualTo("abc"));
        }

        [Test]
        public void TagTest()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.PushTagStack();
            model.Tag["NewValue"] = "abc";
            Assert.That(model.Tag["NewValue"], Is.EqualTo("abc"));
            Assert.That(model[VariableScope.Tag + ".NewValue"], Is.EqualTo("abc"));
        }

       
        [Test]
        public void TestPushingAndPoppingOfTagStack()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.PushTagStack();
            model.Tag["NewValue"] = "abc";
            Assert.That(model.Tag["NewValue"], Is.EqualTo("abc"));
            model.PushTagStack();
            Assert.That(model.Tag["NewValue"], Is.Null);
            model.Tag["NewValue"] = "def";
            Assert.That(model.Tag["NewValue"], Is.EqualTo("def"));
            model.PopTagStack();
            Assert.That(model.Tag["NewValue"], Is.EqualTo("abc"));
        }

        [Test]
        public void TestTagStackWithUseOfPrefix()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.PushTagStack();
            model.Tag["NewValue"] = "abc";
            Assert.That(model ["Tag.NewValue"], Is.EqualTo("abc"));
            model.PushTagStack();
            Assert.That(model["Tag.NewValue"], Is.Null);
            model.Tag["NewValue"] = "def";
            Assert.That(model["Tag.NewValue"], Is.EqualTo("def"));
            model.PopTagStack();
            Assert.That(model["Tag.NewValue"], Is.EqualTo("abc"));
        }

        [Test]
        public void TestFlatteningOfTagStackResolving()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.PushTagStack();
            model.Tag["NewValue"] = "abc";
            Assert.That(model["NewValue"], Is.EqualTo("abc"));
            model.PushTagStack();
            Assert.That(model["NewValue"], Is.Null);
            model.Tag["NewValue"] = "def";
            Assert.That(model["NewValue"], Is.EqualTo("def"));
            model.PopTagStack();
            Assert.That(model["NewValue"], Is.EqualTo("abc"));
        }

        [Test]
        public void TestTagStackWithUseOfPrefixSettingThroughModel()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.PushTagStack();
            model["Tag.NewValue"] = "abc";
            Assert.That(model["Tag.NewValue"], Is.EqualTo("abc"));
            model.PushTagStack();
            Assert.That(model["Tag.NewValue"], Is.Null);
            model["Tag.NewValue"] = "def";
            Assert.That(model["Tag.NewValue"], Is.EqualTo("def"));
            model.PopTagStack();
            Assert.That(model["Tag.NewValue"], Is.EqualTo("abc"));
        }

        [Test]
        public void TestFlatteningOfTagStackResolvingSettingThroughModel()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.PushTagStack();
            model["Tag.NewValue"] = "abc";
            Assert.That(model["NewValue"], Is.EqualTo("abc"));
            model.PushTagStack();
            Assert.That(model["NewValue"], Is.Null);
            model["Tag.NewValue"] = "def";
            Assert.That(model["NewValue"], Is.EqualTo("def"));
            model.PopTagStack();
            Assert.That(model["NewValue"], Is.EqualTo("abc"));
        }

        public class Level1 
        {
            public Level2 Level2
            {
                get;
                set;
            }
        }

        public class Level2
        {
            public Level3 Level3
            {
                get;
                set;
            }
        }

        public class Level3
        {
            public int? Value
            {
                get;
                set;
            }
        }
        [Test]
        public void TagDeepNestedResolveTest()
        {
            var level1 = new Level1
            {
                Level2 = new Level2
                {
                    Level3 = new Level3
                    {
                        Value = 42
                    }

                }
            };
            var model = new TagModel(level1);
            model.PushTagStack();

            Assert.That(model["Level2.Level3.Value"], Is.EqualTo(42));
        }

        [Test]
        public void TagDeepNestedResolveWithNullValueTest()
        {
            var level1 = new Level1
            {
                Level2 = new Level2
                {
                    Level3 = new Level3
                    {
                        Value = default(int?)
                    }

                }
            };
            var model = new TagModel(level1);
            model.PushTagStack();

            Assert.That(model["Level2.Level3.Value"], Is.EqualTo(default(int?)));
        }

        [Test]
        public void PushTagWithPeekInParentShouldResolveParentValues()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.PushTagStack(true);
            model["Tag.NewValue"] = "abc";
            Assert.That(model["NewValue"], Is.EqualTo("abc"));
            model.PushTagStack(true);
            Assert.That(model["NewValue"], Is.EqualTo("abc"));
            model["Tag.NewValue"] = "def";
            Assert.That(model["NewValue"], Is.EqualTo("def"));
            model.PopTagStack();
            Assert.That(model["NewValue"], Is.EqualTo("abc"));
        }

        [Test]
        public void PushTagWithPeekInParentShouldResolveParentValuesWithOutTrhowinngReflectionExceptionsOnUnknownPathsOnChild()
        {
            var model = new TagModel(new Reflection(new Hashtable()));
            model.PushTagStack(true);
            model["Tag.NewValue"] = "abc";
            Assert.That(model["NewValue"], Is.EqualTo("abc"));
            Assert.That(model["OtherValue"], Is.Null);
            model.PushTagStack(true);
            model["Tag.OtherValue"] = "def";
            Assert.That(model["OtherValue"], Is.EqualTo("def"));
            Assert.That(model["NewValue"], Is.EqualTo("abc"));
            model.PopTagStack();
            Assert.That(model["NewValue"], Is.EqualTo("abc"));
            Assert.That(model["OtherValue"], Is.Null);
        }
    }
}
