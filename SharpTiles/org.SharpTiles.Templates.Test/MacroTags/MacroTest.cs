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

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Expressions;
using org.SharpTiles.Tags;
using org.SharpTiles.Templates.MacroTags;
using org.SharpTiles.Templates.Templates;

namespace org.SharpTiles.Templates.Test.MacroTags
{
    [TestFixture]
    public class MacroTest
    {
        [Test]
        public void Should_Add_Macro_Definition_To_Page_Model()
        {
            var model = new TagModel(this);
            ITag tag = CreateFactory().Parse("<macro:define name='testA'>aa</macro:define>");
            tag.Evaluate(model);
            Assert.That(model.Page["TestA"], Is.Not.Null);
            Assert.That((model.Page["TestA"] as DefineMacroTag.MarcoDefinition), Is.Not.Null);
        }

        [Test]
        public void Should_Not_Execute_Macro_In_Definition_Phase()
        {
            var model = new TagModel(this);
            ITag tag = CreateFactory().Parse("<macro:define name='testA'>aa</macro:define>");
            Assert.That(tag.Evaluate(model), Is.EqualTo(string.Empty));
            Assert.That(model.Page["TestA"], Is.Not.Null);          
        }

        [Test]
        public void Should_Store_And_Executable_Macro_Definition_To_Page_Model()
        {
            var model = new TagModel(this);
            ITag tag = CreateFactory().Parse("<macro:define name='testA'>aa</macro:define>");
            tag.Evaluate(model);
            Assert.That((model.Page["TestA"] as DefineMacroTag.MarcoDefinition), Is.Not.Null);
            Assert.That((model.Page["TestA"] as DefineMacroTag.MarcoDefinition).Evaluate(model), Is.EqualTo("aa"));
        }

        [Test]
        public void Should_Store_And_Executable_Correct_Macro_Definition_To_Page_Model()
        {
            var model = new TagModel(this);
            CreateFactory().Parse("<macro:define name='testA'>aa</macro:define>").Evaluate(model);
            CreateFactory().Parse("<macro:define name='testB'>bb</macro:define>").Evaluate(model);
            Assert.That((model.Page["TestA"] as DefineMacroTag.MarcoDefinition).Evaluate(model), Is.EqualTo("aa"));
            Assert.That((model.Page["TestB"] as DefineMacroTag.MarcoDefinition).Evaluate(model), Is.EqualTo("bb"));
        }

        [Test]
        public void Should_Not_Fail_But_Overwrite_Variable()
        {
            var model = new TagModel(this);
            CreateFactory().Parse("<macro:define name='testA'>aa</macro:define>").Evaluate(model);
            CreateFactory().Parse("<macro:define name='testA'>bb</macro:define>").Evaluate(model);
            Assert.That((model.Page["TestA"] as DefineMacroTag.MarcoDefinition).Evaluate(model), Is.EqualTo("bb"));
        }

        [Test]
        public void Should_Call_Correct_Macro()
        {
            var model = new TagModel(this);
            CreateFactory().Parse("<macro:define name='testA'>aa</macro:define>").Evaluate(model);
            CreateFactory().Parse("<macro:define name='testB'>bb</macro:define>").Evaluate(model);
            var result = CreateFactory().Parse("<macro:call name='testA'/>").Evaluate(model);
            Assert.That(result, Is.EqualTo("aa"));
        }

        [Test]
        public void Should_Fail_On_Unknown_Macro()
        {
            var model = new TagModel(this);
            CreateFactory().Parse("<macro:define name='testA'>aa</macro:define>").Evaluate(model);
            try
            {
               CreateFactory().Parse("<macro:call name='testC'/>").Evaluate(model);
            }
            catch (MacroException Te)
            {
                Assert.That(Te.MessageWithOutContext, Is.EqualTo(MacroException.NotFound("TestC").Message));                
            }
        }

        [Test]
        public void Should_Fail_On_Neither_Macro_Or_Function()
        {
            var model = new TagModel(this);
            model.Page["TestC"] = "X";
            CreateFactory().Parse("<macro:define name='testA'>aa</macro:define>").Evaluate(model);
            try
            {
                CreateFactory().Parse("<macro:call name='testC'/>").Evaluate(model);
            }
            catch (MacroException Te)
            {
                Assert.That(Te.MessageWithOutContext, Is.EqualTo(MacroException.NoMacroOrFunction("TestC").Message));
            }
        }

        [Test]
        public void Should_Call_Macro()
        {
            var model = new TagModel(this);
            CreateFactory().Parse("<macro:define name='testA'>aa</macro:define>").Evaluate(model);
            var result = CreateFactory().Parse("<macro:call name='testA'/>").Evaluate(model);
            Assert.That(result, Is.EqualTo("aa"));
        }


        [Test]
        public void Should_Add_Function_Definition_To_Page_Model()
        {
            var model = new TagModel(this);
            ITag tag = CreateFactory().Parse("<macro:function name='testA' argument-1='firstName' argument-2='lastName'>Hi ${firstName} ${lastName}</macro:function>");
            tag.Evaluate(model);
            Assert.That(model.Page["TestA"], Is.Not.Null);
            Assert.That((model.Page["TestA"] as DefineFunctionTag.FunctionDefinition), Is.Not.Null);
        }


        [Test]
        public void Should_Add_Arguments_To_Function_Definition()
        {
            var model = new TagModel(this);
            ITag tag = CreateFactory().Parse("<macro:function name='testA' argument-1='firstName' argument-2='lastName'>Hi ${firstName} ${lastName}</macro:function>");
            tag.Evaluate(model);
            Assert.That(model.Page["TestA"], Is.Not.Null);
            var function = (model.Page["TestA"] as DefineFunctionTag.FunctionDefinition);
            Assert.That(function.Arguments, Is.EquivalentTo(new string[]{"firstName","lastName"}));
        }

        [Test]
        public void Should_Not_Execute_Function_In_Definition_Phase()
        {
            var model = new TagModel(this);
            ITag tag = CreateFactory().Parse("<macro:function name='testA' argument-1='firstName' argument-2='lastName'>Hi ${firstName} ${lastName}</macro:function>");
            Assert.That(tag.Evaluate(model), Is.EqualTo(string.Empty));
            Assert.That(model.Page["TestA"], Is.Not.Null);
        }

        [Test]
        public void Should_Store_And_Executable_Function_Definition_To_Page_Model()
        {
            var model = new TagModel(this);
            ITag tag = CreateFactory().Parse("<macro:function name='testA' argument-1='firstName' argument-2='lastName'>Hi ${firstName} ${lastName}</macro:function>");
            tag.Evaluate(model);
            Assert.That((model.Page["TestA"] as DefineFunctionTag.FunctionDefinition), Is.Not.Null);
            var functionModel  = new TagModel(this);
            functionModel.PushTagStack(true);
            functionModel.Tag["firstName"] = "John";
            functionModel.Tag["lastName"] = "Doe";
            Assert.That((model.Page["TestA"] as DefineFunctionTag.FunctionDefinition).Evaluate(functionModel), Is.EqualTo("Hi John Doe"));
        }

        [Test]
        public void Should_Store_The_Correct_Definition_To_Page_Model()
        {
            var model = new TagModel(this);
            CreateFactory().Parse("<macro:function name='testA' argument-1='firstName' argument-2='lastName'>Hi ${firstName} ${lastName}</macro:function>").Evaluate(model);
            CreateFactory().Parse("<macro:function name='testB' argument-1='firstName' argument-2='lastName'>Bye ${firstName} ${lastName}</macro:function>").Evaluate(model);
            Assert.That((model.Page["TestA"] as DefineFunctionTag.FunctionDefinition), Is.Not.Null);
            var functionModel = new TagModel(this);
            functionModel.PushTagStack(true);
            functionModel.Tag["firstName"] = "John";
            functionModel.Tag["lastName"] = "Doe";
            Assert.That((model.Page["TestA"] as DefineFunctionTag.FunctionDefinition).Evaluate(functionModel), Is.EqualTo("Hi John Doe"));
            Assert.That((model.Page["TestB"] as DefineFunctionTag.FunctionDefinition).Evaluate(functionModel), Is.EqualTo("Bye John Doe"));
        }

        [Test]
        public void Should_Also_Call_The_Function_Using_The_Function()
        {
            var model = new TagModel(this);
            CreateFactory().Parse("<macro:function name='testA' argument-1='firstName' argument-2='lastName'>Hi ${firstName} ${lastName}</macro:function>").Evaluate(model);
            CreateFactory().Parse("<macro:function name='testB' argument-1='firstName' argument-2='lastName'>Bye ${firstName} ${lastName}</macro:function>").Evaluate(model);
            Assert.That((model.Page["TestA"] as DefineFunctionTag.FunctionDefinition), Is.Not.Null);
            Assert.That(new ExpressionLib(new MacroFunctionLib()).Parse("macro:call(TestA,'John','Doe')").Evaluate(model), Is.EqualTo("Hi John Doe"));
            Assert.That(new ExpressionLib(new MacroFunctionLib()).Parse("macro:call(TestB,'John','Doe')").Evaluate(model), Is.EqualTo("Bye John Doe"));
        }

        [Test]
        public void Should_Also_Call_The_Function_Using_The_Function_Should_Ignore_Default_Values()
        {
            var model = new TagModel(this);
            CreateFactory().Parse("<macro:function name='testA' argument-1='firstName(X)' argument-2='lastName(X)'>Hi ${firstName} ${lastName}</macro:function>").Evaluate(model);
            CreateFactory().Parse("<macro:function name='testB' argument-1='firstName(X)' argument-2='lastName(X)'>Bye ${firstName} ${lastName}</macro:function>").Evaluate(model);
            Assert.That((model.Page["TestA"] as DefineFunctionTag.FunctionDefinition), Is.Not.Null);
            Assert.That(new ExpressionLib(new MacroFunctionLib()).Parse("macro:call(TestA,'John','Doe')").Evaluate(model), Is.EqualTo("Hi John Doe"));
            Assert.That(new ExpressionLib(new MacroFunctionLib()).Parse("macro:call(TestB,'John','Doe')").Evaluate(model), Is.EqualTo("Bye John Doe"));
        }

        [Test]
        public void Should_Use_The_Result_Var_Of_The_Function()
        {
            var model = new TagModel(this);
            CreateFactory().Parse("<macro:function name='testA' argument-1='firstName' argument-2='lastName'>Hi ${firstName} ${lastName}</macro:function>").Evaluate(model);
            CreateFactory().Parse("<macro:function name='testB' argument-1='firstName' argument-2='lastName' result='output'>X<c:set var='output'>Bye ${firstName} ${lastName}</c:set>X</macro:function>").Evaluate(model);
            Assert.That((model.Page["TestA"] as DefineFunctionTag.FunctionDefinition), Is.Not.Null);
            Assert.That(new ExpressionLib(new MacroFunctionLib()).Parse("macro:call(TestA,'John','Doe')").Evaluate(model), Is.EqualTo("Hi John Doe"));
            Assert.That(new ExpressionLib(new MacroFunctionLib()).Parse("macro:call(TestB,'John','Doe')").Evaluate(model), Is.EqualTo("Bye John Doe"));
        }

        [Test]
        public void Should_Call_Correct_Function()
        {
            var model = new TagModel(this);
            CreateFactory().Parse("<macro:function name='testA' argument-1='firstName' argument-2='lastName'>Hi ${firstName} ${lastName}</macro:function>").Evaluate(model);
            CreateFactory().Parse("<macro:function name='testB' argument-1='firstName' argument-2='lastName'>Bye ${firstName} ${lastName}</macro:function>").Evaluate(model);
            var result = CreateFactory().Parse("<macro:call name='testA' firstName='John' lastName='Doe'/>").Evaluate(model);
            Assert.That(result, Is.EqualTo("Hi John Doe"));
        }

        [Test]
        public void Should_Not_Peek_In_Stack()
        {
            var model = new TagModel(this);
            CreateFactory().Parse("<macro:function name='testA' argument-1='firstName' argument-2='lastName'>Hi ${firstName} ${lastName}</macro:function>").Evaluate(model);
            CreateFactory().Parse("<c:set var='LastName' value='Zoe'/>").Evaluate(model);
            var result = CreateFactory().Parse("<macro:call name='testA' firstName='John' lastName='Doe'/>").Evaluate(model);
            Assert.That(result, Is.EqualTo("Hi John Doe"));
        }

        [Test]
        public void Should_Work_On_Forked_Model()
        {
            var model = new TagModel(this);
            CreateFactory().Parse("<macro:function name='testA' argument-1='firstName' argument-2='lastName'>Hi ${firstName} ${lastName}</macro:function>").Evaluate(model);
            CreateFactory().Parse("<c:set var='LastName' value='Zoe'/>").Evaluate(model);
            var result = CreateFactory().Parse("<macro:call name='testA' firstName='John' lastName='Doe'/>").Evaluate(model.Fork(new object()));
            Assert.That(result, Is.EqualTo("Hi John Doe"));
        }

        [Test]
        public void Should_Report_Missing_Argument_Function()
        {
            var model = new TagModel(this);
            CreateFactory().Parse("<macro:function name='testA' argument-1='firstName' argument-2='lastName'>Hi ${firstName} ${lastName}</macro:function>").Evaluate(model);
            CreateFactory().Parse("<macro:function name='testB' argument-1='firstName' argument-2='lastName'>Bye ${firstName} ${lastName}</macro:function>").Evaluate(model);
            try
            {
                CreateFactory().Parse("<macro:call name='testA' firstName='John'/>").Evaluate(model);
            }
            catch (MacroException Me)
            {
                Assert.That(Me.MessageWithOutContext, Is.EqualTo(MacroException.NullNotAllowed("lastName").Message));
            }
        }

        [Test]
        public void Should_Store_Definition_To_Page_Model_After_Denormalizing_Natural_Language()
        {
            var model = new TagModel(this);
            CreateFactory().Parse("<macro:function name='test-a' argument-1='firstName' argument-2='lastName'>Hi ${firstName} ${lastName}</macro:function>").Evaluate(model);
            CreateFactory().Parse("<macro:function name='test-b' argument-1='firstName' argument-2='lastName'>Bye ${firstName} ${lastName}</macro:function>").Evaluate(model);
            Assert.That((model.Page["TestA"] as DefineFunctionTag.FunctionDefinition), Is.Not.Null);
            var functionModel = new TagModel(this);
            functionModel.PushTagStack(true);
            functionModel.Tag["firstName"] = "John";
            functionModel.Tag["lastName"] = "Doe";
            Assert.That((model.Page["TestA"] as DefineFunctionTag.FunctionDefinition).Evaluate(functionModel), Is.EqualTo("Hi John Doe"));
            Assert.That((model.Page["TestB"] as DefineFunctionTag.FunctionDefinition).Evaluate(functionModel), Is.EqualTo("Bye John Doe"));
        }

        [Test]
        public void Should_Also_Call_The_Function_Using_The_Function_With_NaturalLanguage()
        {
            var model = new TagModel(this);
            CreateFactory().Parse("<macro:function name='test-a' argument-1='firstName' argument-2='lastName'>Hi ${firstName} ${lastName}</macro:function>").Evaluate(model);
            CreateFactory().Parse("<macro:function name='test-b' argument-1='firstName' argument-2='lastName'>Bye ${firstName} ${lastName}</macro:function>").Evaluate(model);
            Assert.That((model.Page["TestA"] as DefineFunctionTag.FunctionDefinition), Is.Not.Null);
            Assert.That(new ExpressionLib(new MacroFunctionLib()).Parse("macro:call(TestA,'John','Doe')").Evaluate(model), Is.EqualTo("Hi John Doe"));
            Assert.That(new ExpressionLib(new MacroFunctionLib()).Parse("macro:call(TestB,'John','Doe')").Evaluate(model), Is.EqualTo("Bye John Doe"));
        }


        private static TagLibParserFactory CreateFactory()
        {
            var lib = new TagLib();
            lib.Register(new Macro());

            return new TagLibParserFactory(new TagLibForParsing(lib), new ExpressionLib(new MacroFunctionLib()), new FileLocatorFactory(), null);
        }

        [Test]
        public void Should_User_Function_Defaults_For_Arguments()
        {
            var model = new TagModel(this);
            CreateFactory().Parse("<macro:function name='testA' argument-1='firstName(FIRST)' argument-2='lastName(LAST)'>Hi ${firstName} ${lastName}</macro:function>").Evaluate(model);
            model.PushTagStack(true);
            model.Tag["FIRST"] = "John";
            model.Tag["LAST"] = "Doe";
            var result = CreateFactory().Parse("<macro:call name='testA'/>").Evaluate(model);
            model.PopTagStack();
            Assert.That(result, Is.EqualTo("Hi John Doe"));
        }
    }
}
