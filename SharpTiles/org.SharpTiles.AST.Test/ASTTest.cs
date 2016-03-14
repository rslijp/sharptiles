using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.AST.Nodes;
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
            var ast = new AST(formatter.ParsedTemplate);
            var expected = new AST().Add(
                new TagNode("c","out").With("Value","Hello World"));
            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        [Test]
        public void Single_Tag_Node_With_Variables()
        {
            const string TEMPLATE = @"<c:out value=""${Greetings}""/>";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate);
            var expected = new AST().Add(
                new TagNode("c", "out").With("Value", new ExpressionNode("Greetings", "Property", null)));
            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        [Test]
        public void Single_Tag_Node_With_Partial_Variables()
        {
            const string TEMPLATE = @"<c:out value=""Hello ${Thing}""/>";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate);
            var expected = new AST().Add(
                new TagNode("c", "out").With("Value", new TextNode("Hello "), new ExpressionNode("Thing", "Property", null)));
            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        [Test]
        public void Single_Content_Node()
        {
            const string TEMPLATE = @"Hello World";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate);
            var expected = new AST().Add(new TextNode("Hello World"));
            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        [Test]
        public void Single_Simple_Expression_Node()
        {
            const string TEMPLATE = @"${Greetings}";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate);
            var expected = new AST().Add(new ExpressionNode("Greetings", "Property",null));
            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        //        [Test]
        //        public void Single_Complex_Expression_Node()
        //        {
        //            const string TEMPLATE = @"${A + B}";
        //            var formatter = new Formatter(TEMPLATE).Parse();
        //            var ast = new AST(formatter.ParsedTemplate);
        //            var expected = new AST().Add(new ExpressionNode("A Add B", "Add", typeof(decimal)));
        //            Assert.That(ast, Deeply.Is.EqualTo(expected));
        //        }

        [Test]
        public void Single_Complex_Expression_Node()
        {
            const string TEMPLATE = @"${A+B}";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate);
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
            var ast = new AST(formatter.ParsedTemplate);
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
            var ast = new AST(formatter.ParsedTemplate);
            var expected = new AST().Add(new ExpressionNode("A Add B", "Add", typeof(decimal)).
                Add(new ExpressionNode("A", "Property", null)).
                Add(new ExpressionNode("B", "Property", null)));
            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        [Test]
        public void Two_Nodes_No_Variables()
        {
            const string TEMPLATE = @"<c:out value=""Hello""/><c:set var=""storage"" value=""World""/>";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate);
            
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
            var ast = new AST(formatter.ParsedTemplate);
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
            var ast = new AST(formatter.ParsedTemplate);
            var expected = new AST()
                .Add(new TagNode("c", "out").Add(new TextNode("Hello world")));

            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }


        [Test]
        public void AST_ToString()
        {
            const string TEMPLATE = @"
            <c:set var=""Status"" value=""Nice""/>
            <c:out value=""Hello"">
                <c:out value=""${Status}""/>
            </c:out>
            <c:out>World ${4+2*(A/B)}</c:out>";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.TrimEmptyTextNodes);
            Console.WriteLine(ast.ToString());
        }
    }
}
