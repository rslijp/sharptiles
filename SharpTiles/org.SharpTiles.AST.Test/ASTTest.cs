using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.AST.Nodes;
using org.SharpTiles.Common;
using org.SharpTiles.Templates;

namespace org.SharpTiles.AST.Test
{
    [TestFixture]
    public class ASTTest
    {

       
        [Test]
        public void Single_Tag_Node_No_Variables()
        {
            const string TEMPLATE =@"<c:out value=""Hello World""/>";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);
            var expected = new AST().Add(
                new TagNode("c","out").With("Value","Hello World"));
            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        [Test]
        public void Single_Tag_Node_With_Variables()
        {
            const string TEMPLATE = @"<c:out value=""${Greetings}""/>";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);
            var expected = new AST().Add(
                new TagNode("c", "out").With("Value", new ExpressionNode("Greetings", "Property", null)));
            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        [Test]
        public void Single_Tag_Node_With_Partial_Variables()
        {
            const string TEMPLATE = @"<c:out value=""Hello ${Thing}""/>";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);
            var expected = new AST().Add(
                new TagNode("c", "out").With("Value", new TextNode("Hello "), new ExpressionNode("Thing", "Property", null)));
            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        [Test]
        public void Single_Content_Node()
        {
            const string TEMPLATE = @"Hello World";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);
            var expected = new AST().Add(new TextNode("Hello World"));
            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        [Test]
        public void Single_Simple_Expression_Node()
        {
            const string TEMPLATE = @"${Greetings}";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);
            var expected = new AST().Add(new ExpressionNode("Greetings", "Property",null));
            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        //        [Test]
        //        public void Single_Complex_Expression_Node()
        //        {
        //            const string TEMPLATE = @"${A + B}";
        //            var formatter = new Formatter(TEMPLATE).Parse();
        //            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);
        //            var expected = new AST().Add(new ExpressionNode("A Add B", "Add", typeof(decimal)));
        //            Assert.That(ast, Deeply.Is.EqualTo(expected));
        //        }

        [Test]
        public void Single_Complex_Expression_Node()
        {
            const string TEMPLATE = @"${A+B}";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);
            var expected = new AST().Add(new ExpressionNode("A Add B", "Add", typeof(decimal)).
                Add(new ExpressionNode("A", "Property", null)).
                Add(new ExpressionNode("B", "Property", null)));
            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        [Test]
        public void Nested_Complex_Expression_Node()
        {
            const string TEMPLATE = @"${(A+B)*C}";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);
            Console.WriteLine(((ExpressionNode) ast.Nodes[0]).Nodes[0].Value);
            var expected = new AST().Add(new ExpressionNode("(A Add B) Multiply C", "Multiply", typeof(decimal)).
                Add(new ExpressionNode("(A Add B)", "Brackets", typeof(decimal)).Add(new ExpressionNode("A Add B", "Add", typeof(decimal)).
                Add(new ExpressionNode("A", "Property", null)).
                Add(new ExpressionNode("B", "Property", null)))).
                Add(new ExpressionNode("C", "Property", null)));
            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        [Test]
        public void Should_Trim_Expression()
        {
            const string TEMPLATE = @"${A + B}";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);
            var expected = new AST().Add(new ExpressionNode("A Add B", "Add", typeof(decimal)).
                Add(new ExpressionNode("A", "Property", null)).
                Add(new ExpressionNode("B", "Property", null)));
            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        [Test]
        public void Should_Collect_Functions()
        {
            const string TEMPLATE = @"${fn:concat(A,'+++')}";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);
            var expected = new AST().Add(new ExpressionNode("concat(A,'+++')", "Function", typeof(string)).
                Add(new ExpressionNode("(A,'+++')", "Brackets", null).
                Add(new ExpressionNode("A", "Property", null)).
                Add(new ExpressionNode("'+++'", "Constant", typeof(string)))));
            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        [Test]
        public void Two_Nodes_No_Variables()
        {
            const string TEMPLATE = @"<c:out value=""Hello""/><c:set var=""storage"" value=""World""/>";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);
            
            var expected = new AST()
                .Add(new TagNode("c", "out").With("Value", "Hello"))
                .Add(new TagNode("c", "set").With("Var", "storage").With("Value", "World"));

            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        [Test]
        public void Nested_Nodes_No_Variables()
        {
            const string TEMPLATE = @"<c:out value=""Hello""><c:out value=""World""/></c:out>";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);
            var expected = new AST()
                .Add(new TagNode("c", "out").With("Value", "Hello")
                    .Add(new TagNode("c", "out").With("Value", "World")));
            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        [Test]
        public void Nested_Nodes_Content()
        {
            const string TEMPLATE = @"<c:out>Hello world</c:out>";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);
            var expected = new AST()
                .Add(new TagNode("c", "out").Add(new TextNode("Hello world")));

            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        [Test]
        public void Should_Prune_Empty_Text_Nodes()
        {
            //Given
            const string TEMPLATE = @" ${HelloWorld} ";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);
            var nontrimmed = new AST()
                .Add(new TextNode(" "))
                .Add(new ExpressionNode("HelloWorld", "Property", null))
                .Add(new TextNode(" "));
            Assert.That(ast, Deeply.Is.EqualTo(nontrimmed));
            //When
            ast = new AST(formatter.ParsedTemplate, AST.Options.TrimEmptyTextNodes | AST.Options.DontTrackContext);
            var trimmed = new AST()
                .Add(new ExpressionNode("HelloWorld", "Property", null));
            //Then
            Assert.That(ast, Deeply.Is.EqualTo(trimmed));
        }

        [Test]
        public void Should_Not_Text_Nodes_Inside_Expression()
        {
            //Given
            const string TEMPLATE = @" ${fn:trim(' ')} ";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);
            var expected = new AST().Add(new ExpressionNode("trim(' ')", "Function", typeof (string)).
                Add(new ExpressionNode("(' ')", "Brackets", typeof(string)).
                    Add(new ExpressionNode("' '", "Constant", typeof(string)))));
            //When
            ast = new AST(formatter.ParsedTemplate, AST.Options.TrimEmptyTextNodes | AST.Options.DontTrackContext);
           
            //Then
            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        [Test]
        public void Should_Prune_All_Text_Nodes()
        {
            //Given
            const string TEMPLATE = @"_${HelloWorld}_";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);
            var nontrimmed = new AST()
                .Add(new TextNode("_"))
                .Add(new ExpressionNode("HelloWorld", "Property", null))
                .Add(new TextNode("_"));
            Assert.That(ast, Deeply.Is.EqualTo(nontrimmed));
            //When
            ast = new AST(formatter.ParsedTemplate, AST.Options.TrimAllTextNodes | AST.Options.DontTrackContext);
            var trimmed = new AST()
                .Add(new ExpressionNode("HelloWorld", "Property", null));
            //Then
            Assert.That(ast, Deeply.Is.EqualTo(trimmed));
        }

        [Test]
        public void Should_Prune_Expression_Leaves()
        {
            //Given
            const string TEMPLATE = @"${(A+B)*C}";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);
            var nonflat = new AST().Add(new ExpressionNode("(A Add B) Multiply C", "Multiply", typeof(decimal)).
                Add(new ExpressionNode("(A Add B)", "Brackets", typeof(decimal)).Add(new ExpressionNode("A Add B", "Add", typeof(decimal)).
                Add(new ExpressionNode("A", "Property", null)).
                Add(new ExpressionNode("B", "Property", null)))).
                Add(new ExpressionNode("C", "Property", null)));
            Assert.That(ast, Deeply.Is.EqualTo(nonflat));
            //When
            ast = new AST(formatter.ParsedTemplate, AST.Options.FlatExpression | AST.Options.DontTrackContext);
            var flat = new AST().Add(new ExpressionNode("(A Add B) Multiply C", "Multiply", typeof(decimal)));
            //Then
            Assert.That(ast, Deeply.Is.EqualTo(flat));
        }

        [Test]
        public void Should_XOR_Options()
        {
            //Given
            const string TEMPLATE = @" ${(A+B)*C}";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);
            var nonflat = new AST()
                .Add(new TextNode(" "))
                .Add(new ExpressionNode("(A Add B) Multiply C", "Multiply", typeof(decimal)).
                    Add(new ExpressionNode("(A Add B)", "Brackets", typeof(decimal)).Add(new ExpressionNode("A Add B", "Add", typeof(decimal)).
                    Add(new ExpressionNode("A", "Property", null)).
                    Add(new ExpressionNode("B", "Property", null)))).
                    Add(new ExpressionNode("C", "Property", null)));
            Assert.That(ast, Deeply.Is.EqualTo(nonflat));
            //When
            ast = new AST(formatter.ParsedTemplate, AST.Options.TrimEmptyTextNodes|AST.Options.FlatExpression | AST.Options.DontTrackContext);
            var flat = new AST().Add(new ExpressionNode("(A Add B) Multiply C", "Multiply", typeof(decimal)));
            //Then
            Assert.That(ast, Deeply.Is.EqualTo(flat));
        }


        [Test]
        public void Should_Track_Context()
        {           

            const string TEMPLATE = @"
<c:set var=""Status"" value=""Nice""/>
<c:out value=""Hello"">
    <c:out value=""${Status}""/>
</c:out>
<c:out>World ${(A+B)*C}</c:out>";

            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.TrimEmptyTextNodes);
            var expected = new AST().At(1,1).
                Add(new TagNode("c","set").At(2,4).With("Var","Status", new Context(2,13)).With("Value", "Nice", new Context(2, 28))).
                Add(new TagNode("c", "out").At(3, 4).With("Value", "Hello", new Context(3, 15)).
                    Add(new TagNode("c", "out").At(4, 8).With("Value", new ExpressionNode("Status", "Property", null).At(4,21)))).
                Add(new TagNode("c", "out").At(6, 4).Add(
                    new TextNode("World ").At(6,8)).Add(
                    new ExpressionNode("(A Add B) Multiply C", "Multiply", typeof(decimal)).At(6, 21).
                        Add(new ExpressionNode("(A Add B)", "Brackets", typeof(decimal)).At(6, 16).
                            Add(new ExpressionNode("A Add B", "Add", typeof(decimal)).At(6, 18).
                                Add(new ExpressionNode("A", "Property", null).At(6, 17)).
                                Add(new ExpressionNode("B", "Property", null).At(6, 19)))).
                            Add(new ExpressionNode("C", "Property", null).At(6, 22))));

            Assert.That(ast, Deeply.Is.EqualTo(expected));

        }

        [Test]
        public void Should_Still_Collect_Parse_Fragment()
        {
            var formatter = new Formatter("<c:out>${a}</c:out>${a}<c:out>");
            try
            {
                formatter.Parse();
                Assert.Fail();
            }
            catch (ExceptionWithContext ewc)
            {
                Assert.That(ewc.Context.Index, Is.EqualTo(26));
            }
            var ast = new AST(formatter.ParsedTemplate, AST.Options.TrimEmptyTextNodes | AST.Options.DontTrackContext);

            var expected = new AST().
                Add(new TagNode("c", "out").Add(new ExpressionNode("a", "Property", null))).
                Add(new ExpressionNode("a", "Property", null));

            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }
//
//        [Test]
//        public void Should_Still_Collect_Best_Effort_Parse_Fragment()
//        {
//            var formatter = new Formatter("<c:out>${a}<c:out>${a}</c:out>");
//            try
//            {
//                formatter.Parse();
//                Assert.Fail();
//            }
//            catch (ExceptionWithContext ewc)
//            {
//                Console.WriteLine(ewc.Message);
//                Assert.That(ewc.Context.Index, Is.EqualTo(26));
//            }
//            var ast = new AST(formatter.ParsedTemplate, AST.Options.TrimEmptyTextNodes | AST.Options.TrackContext);
//            Console.WriteLine(ast);
//        }
    }
}
