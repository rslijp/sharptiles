using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
            var expected = new AST().Add(new ExpressionNode("A+B", "Add", typeof(decimal)).
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
            var expected = new AST().Add(new ExpressionNode("(A+B)*C", "Multiply", typeof(decimal)).
                Add(new ExpressionNode("(A+B)", "Brackets", typeof(decimal)).Add(new ExpressionNode("A+B", "Add", typeof(decimal)).
                Add(new ExpressionNode("A", "Property", null)).
                Add(new ExpressionNode("B", "Property", null)))).
                Add(new ExpressionNode("C", "Property", null)));
            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        [Test]
        public void Nested_Ternary_Expression_Node()
        {
            const string TEMPLATE = @"${C?(A+B):(A-B)}";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);
            var expected = new AST().Add(new ExpressionNode("C?(A+B):(A-B)", "BooleanTernaryExpression", typeof(object)).
                Add(new ExpressionNode("C", "Property", null)).
                Add(new ExpressionNode("(A+B)", "Brackets", typeof(decimal)).Add(new ExpressionNode("A+B", "Add", typeof(decimal)).
                Add(new ExpressionNode("A", "Property", null)).
                Add(new ExpressionNode("B", "Property", null)))).
                Add(new ExpressionNode("(A-B)", "Brackets", typeof(decimal)).Add(new ExpressionNode("A-B", "Minus", typeof(decimal)).
                Add(new ExpressionNode("A", "Property", null)).
                Add(new ExpressionNode("B", "Property", null)))));

            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        [Test]
        public void Should_Trim_Expression()
        {
            const string TEMPLATE = @"${A + B}";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);
            var expected = new AST().Add(new ExpressionNode("A+B", "Add", typeof(decimal)).
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
            var expected = new AST().Add(new ExpressionNode("fn:concat(A,'+++')", "Function", typeof(string)).
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
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext|AST.Options.PruneRawTexts);
            var expected = new AST()
                .Add(new TagNode("c", "out").With("Value", "Hello")
                    .Add(new TagNode("c", "out").With("Value", "World")));
            expected.Prune(AST.Options.PruneRawTexts);
            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        [Test]
        public void Nested_Nodes_Content()
        {
            const string TEMPLATE = @"<c:out>Hello world</c:out>";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext|AST.Options.PruneRawTexts);
            var expected = new AST()
                .Add(new TagNode("c", "out").Add(new TextNode("Hello world")));
            expected.Prune(AST.Options.PruneRawTexts);

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
            var expected = new AST().Add(new ExpressionNode("fn:trim(' ')", "Function", typeof (string)).
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
            var nonflat = new AST().Add(new ExpressionNode("(A+B)*C", "Multiply", typeof(decimal)).
                Add(new ExpressionNode("(A+B)", "Brackets", typeof(decimal)).Add(new ExpressionNode("A+B", "Add", typeof(decimal)).
                Add(new ExpressionNode("A", "Property", null)).
                Add(new ExpressionNode("B", "Property", null)))).
                Add(new ExpressionNode("C", "Property", null)));
            Assert.That(ast, Deeply.Is.EqualTo(nonflat));
            //When
            ast = new AST(formatter.ParsedTemplate, AST.Options.FlatExpression | AST.Options.DontTrackContext);
            var flat = new AST().Add(new ExpressionNode("(A+B)*C", "Multiply", typeof(decimal)));
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
                .Add(new ExpressionNode("(A+B)*C", "Multiply", typeof(decimal)).
                    Add(new ExpressionNode("(A+B)", "Brackets", typeof(decimal)).Add(new ExpressionNode("A+B", "Add", typeof(decimal)).
                    Add(new ExpressionNode("A", "Property", null)).
                    Add(new ExpressionNode("B", "Property", null)))).
                    Add(new ExpressionNode("C", "Property", null)));
            Assert.That(ast, Deeply.Is.EqualTo(nonflat));
            //When
            ast = new AST(formatter.ParsedTemplate, AST.Options.TrimEmptyTextNodes|AST.Options.FlatExpression | AST.Options.DontTrackContext);
            var flat = new AST().Add(new ExpressionNode("(A+B)*C", "Multiply", typeof(decimal)));
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
            var ast = new AST(formatter.ParsedTemplate, AST.Options.TrimEmptyTextNodes|AST.Options.PruneRawTexts);
            var expected = new AST().At(1,1).
                Add(new TagNode("c","set").At(2,1).With("Var","Status", new Context(2,13)).With("Value", "Nice", new Context(2, 28))).
                Add(new TagNode("c", "out").At(3, 1).With("Value", "Hello", new Context(3, 15)).
                    Add(new TagNode("c", "out").At(4, 5).With("Value", new ExpressionNode("Status", "Property", null).At(4,21)))).
                Add(new TagNode("c", "out").At(6, 1).Add(
                    new TextNode("World ").At(6,8)).Add(
                    new ExpressionNode("(A+B)*C", "Multiply", typeof(decimal)).At(6, 21).
                        Add(new ExpressionNode("(A+B)", "Brackets", typeof(decimal)).At(6, 16).
                            Add(new ExpressionNode("A+B", "Add", typeof(decimal)).At(6, 18).
                                Add(new ExpressionNode("A", "Property", null).At(6, 17)).
                                Add(new ExpressionNode("B", "Property", null).At(6, 19)))).
                            Add(new ExpressionNode("C", "Property", null).At(6, 22))));
            expected.Prune(AST.Options.PruneRawTexts);

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
            var ast = new AST(formatter.ParsedTemplate, AST.Options.TrimEmptyTextNodes | AST.Options.DontTrackContext|AST.Options.PruneRawTexts);

            var expected = new AST().
                Add(new TagNode("c", "out").Add(new ExpressionNode("a", "Property", null))).
                Add(new ExpressionNode("a", "Property", null));
            expected.Prune(AST.Options.PruneRawTexts);
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

        [Test]
        public void Should_Collected_NestedTags()
        {
            const string TEMPLATE = @"<c:choose><c:when test=""${Yes}"">WHEN</c:when><c:otherwise>OTHERWISE</c:otherwise></c:choose>";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext|AST.Options.PruneRawTexts);

            var expected = new AST()
                .Add(new TagNode("c", "choose").
                    Add(new TagNode("c", "when").With("Test",new ExpressionNode("Yes","Property",null)).Add(new TextNode("WHEN"))).
                    Add(new TagNode("c", "otherwise").Add(new TextNode("OTHERWISE")))
            );
            expected.Prune(AST.Options.PruneRawTexts);
            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        [Test]
        public void Should_collect_template()
        {
            const string TEMPLATE = @"<sharp:include file=""a.htm""/>";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);

            var expected = new AST()
                .Add(new TagNode("sharp", "include").With("File","a.htm").Add(
                    new TemplateNode().Add(new TextNode("aa"))
            ));

            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        [Test]
        public void Should_collect_prune_template()
        {
            const string TEMPLATE = @"<sharp:include file=""a.htm""/>";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext|AST.Options.PruneTemplates);

            var expected = new AST()
                .Add(new TagNode("sharp", "include").With("File", "a.htm"));

            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }

        [Test]
        public void Should_inline_template()
        {
            const string TEMPLATE = @"<sharp:include file=""a.htm""/>";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext | AST.Options.InlineTemlpates);

            var expected = new AST()
                .Add(new TagNode("sharp", "include").With("File", "a.htm").Add(
                    new TextNode("aa")
            ));

            Assert.That(ast, Deeply.Is.EqualTo(expected));
        }


        [Test]
        public void Single_Property_Tag_Attribute()
        {
            var attribute = new TagAttributeNode(new ExpressionNode("Greetings", "Property", null));
            Assert.That(attribute.Raw, Deeply.Is.EqualTo("${Greetings}"));
        }

        [Test]
        public void Single_Text_Tag_Attribute()
        {
            var attribute = new TagAttributeNode(new TextNode("Hello"));
            Assert.That(attribute.Raw, Deeply.Is.EqualTo("Hello"));
        }

        [Test]
        public void Combined_Property_Text_Tag_Attribute()
        {
            var attribute = new TagAttributeNode(new TextNode("Hello "), new ExpressionNode("Person", "Property", null));
            Assert.That(attribute.Raw, Deeply.Is.EqualTo("Hello ${Person}"));
        }

        [Test]
        public void Combined_Expression_Text_Tag_Attribute()
        {
            var attribute = new TagAttributeNode(new TextNode("Hello "), new ExpressionNode("(A+B)*C", "Multiply", typeof(decimal)).
                    Add(new ExpressionNode("(A Add B)", "Brackets", typeof(decimal)).Add(new ExpressionNode("A+B", "Add", typeof(decimal)).
                    Add(new ExpressionNode("A", "Property", null)).
                    Add(new ExpressionNode("B", "Property", null)))).
                    Add(new ExpressionNode("C", "Property", null)));
            Assert.That(attribute.Raw, Deeply.Is.EqualTo("Hello ${(A+B)*C}"));
        }

        [Test]
        public void Single_Expression_Text_Tag_Attribute()
        {
            var TEMPLATE = "<c:out value='${(A+B)*C}'/>";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext | AST.Options.PruneTemplates);

            var attribute = ((TagNode) ast.Nodes[0]).Attributes["Value"];
            Assert.That(attribute.Raw, Deeply.Is.EqualTo("${(A+B)*C}"));
        }

        [Test]
        public void Single_Function_Tag_Attribute()
        {
            var TEMPLATE = "<c:out value='${fn:length(fn:concat(A,B))}'/>";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext | AST.Options.PruneTemplates);

            var attribute = ((TagNode)ast.Nodes[0]).Attributes["Value"];
            Assert.That(attribute.Raw, Deeply.Is.EqualTo("${fn:length(fn:concat(A,B))}"));
        }

        [Test]
        public void Single_Math_Function_Tag_Attribute()
        {
            var TEMPLATE = "<c:out value='${ceil(A)}'/>";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext | AST.Options.PruneTemplates);

            var attribute = ((TagNode)ast.Nodes[0]).Attributes["Value"];
            Assert.That(attribute.Raw, Deeply.Is.EqualTo("${ceil(A)}"));
        }

        [Test]
        public void StaticContent_Of_TagNode_As_Raw()
        {
            const string TEMPLATE = @"<c:out>Hello world</c:out>";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);
            var expected = new AST()
                .Add(new TagNode("c", "out").Add(new TextNode("Hello world")));
            var raw = (((TagNode) ast.Nodes[0]).Raw);
            Assert.That(raw, Is.EqualTo("Hello world"));
        }

        [Test]
        public void Combined_Content_Of_TagNode_As_Raw()
        {
            const string TEMPLATE = @"<c:out>Hello ${Stranger}</c:out>";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);
            var expected = new AST()
                .Add(new TagNode("c", "out").Add(new TextNode("Hello world")));
            var raw = (((TagNode)ast.Nodes[0]).Raw);
            Assert.That(raw, Is.EqualTo("Hello ${Stranger}"));
        }

        [Test]
        public void Nested_Content_Of_TagNode_As_Raw()
        {
            const string TEMPLATE = @"<c:out>Hello <c:out>${Stranger}</c:out></c:out>";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);
            var raw = (((TagNode)ast.Nodes[0]).Raw);
            Assert.That(raw, Is.EqualTo("Hello <c:out>${Stranger}</c:out>"));
        }

        [Test]
        public void Template_Content_Of_TagNode_As_Raw()
        {
            const string TEMPLATE = @"<c:out>Here <sharp:include file=""a.htm""/></c:out>";
            var formatter = new Formatter(TEMPLATE).Parse();
            var ast = new AST(formatter.ParsedTemplate, AST.Options.DontTrackContext);
            var raw = (((TagNode)ast.Nodes[0]).Raw);
            Assert.That(raw, Is.EqualTo(@"Here <sharp:include file='a.htm'>aa</sharp:include>"));
        }
    }
}
