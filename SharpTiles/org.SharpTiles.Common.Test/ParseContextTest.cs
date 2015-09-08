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

namespace org.SharpTiles.Common.Test
{
    [TestFixture]
    public class ParseContextTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _tokenizer = new Tokenizer(CONTENT, true, true, '\\', SEPERATORS, new[] {'\''}, null);
            _tokenEnumerator = _tokenizer.GetTokenEnumerator();
        }

        #endregion

        private static readonly string[] SEPERATORS = new[] {"1", "2", "3", "4", "5"};

        //                           1234567890123456789012345
        public const String LINE1 = "1aaaaaaaaaaaaaaaaaaaaaaa";
        public const String LINE2 = "bbbbbb\\22bbbbbbbbbbbbbbb";
        public const String LINE3 = "cc'cccccccccccccccccccc'";
        public const String LINE4 = "4";
        public const String LINE5 = "eeeeeeeeeeeeeeeeeeeeee5";

        public const String LINE_WITH_TAB = "\t1";
        public const String LINE_WITH_TAB2 = "a\ta1";

        private static readonly string CONTENT =
            LINE1 + Environment.NewLine +
            LINE2 + Environment.NewLine +
            LINE3 + Environment.NewLine +
            LINE4 + Environment.NewLine +
            LINE5 + Environment.NewLine;


        public static String LINE1_SEP_WITHPOINTER =
            LINE1 + Environment.NewLine +
            "^";

        public static String LINE1_REG_WITHPOINTER =
            LINE1 + Environment.NewLine +
            "-^";

        public static String LINE2_WITHPOINTER =
            LINE2 + Environment.NewLine +
            "--------^";

        public static String LINE3_WITHPOINTER =
            LINE3 + Environment.NewLine +
            "--^";

        public static String LINE5_WITHPOINTER =
            LINE5 + Environment.NewLine +
            "----------------------^";

        public static String LINE_WITH_TAB_WITHPOINTER =
            LINE_WITH_TAB.Replace("\t", "    ") + Environment.NewLine +
            "---^";

        public static String LINE_WITH_TAB_WITHPOINTER2 =
            LINE_WITH_TAB2.Replace("\t", "    ") + Environment.NewLine +
            "------^";


        private Tokenizer _tokenizer;
        private TokenEnumerator _tokenEnumerator;

        [Test]
        public void EscapedToken()
        {
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.That(_tokenEnumerator.Token.Type, Is.EqualTo(TokenType.Seperator));

            ParseContext context = _tokenEnumerator.Token.Context;
            Assert.That(context.Text, Is.EqualTo(CONTENT));
            Assert.That(context.Line, Is.EqualTo(LINE2));
            Assert.That(context.LineNumber, Is.EqualTo(2));
            Assert.That(context.Context.Count, Is.EqualTo(4));
            Assert.That(context.LineWithPosition, Is.EqualTo(LINE2_WITHPOINTER));
        }

        [Test]
        public void FirstToken()
        {
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.That(_tokenEnumerator.Token.Type, Is.EqualTo(TokenType.Seperator));

            ParseContext context = _tokenEnumerator.Token.Context;
            Assert.That(context.Text, Is.EqualTo(CONTENT));
            Assert.That(context.Line, Is.EqualTo(LINE1));
            Assert.That(context.LineNumber, Is.EqualTo(1));
            Assert.That(context.Context.Count, Is.EqualTo(3));
            Assert.That(context.LineWithPosition, Is.EqualTo(LINE1_SEP_WITHPOINTER));
        }

        [Test]
        public void LiteralToken()
        {
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.That(_tokenEnumerator.Token.Type, Is.EqualTo(TokenType.Literal));

            ParseContext context = _tokenEnumerator.Token.Context;
            Assert.That(context.Text, Is.EqualTo(CONTENT));
            Assert.That(context.LineNumber, Is.EqualTo(3));
            Assert.That(context.Line, Is.EqualTo(LINE3));
            Assert.That(context.Context.Count, Is.EqualTo(5));
            Assert.That(context.LineWithPosition, Is.EqualTo(LINE3_WITHPOINTER));
        }

        [Test]
        public void RegularToken()
        {
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.That(_tokenEnumerator.Token.Type, Is.EqualTo(TokenType.Regular));

            ParseContext context = _tokenEnumerator.Token.Context;
            Assert.That(context.Text, Is.EqualTo(CONTENT));
            Assert.That(context.Line, Is.EqualTo(LINE1));
            Assert.That(context.LineNumber, Is.EqualTo(1));
            Assert.That(context.Context.Count, Is.EqualTo(3));
            Assert.That(context.LineWithPosition, Is.EqualTo(LINE1_REG_WITHPOINTER));
        }

        [Test]
        public void RegularTokenOnNewLine()
        {
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.That(_tokenEnumerator.Token.Type, Is.EqualTo(TokenType.Regular));

            ParseContext context = _tokenEnumerator.Token.Context;
            Assert.That(context.Text, Is.EqualTo(CONTENT));
            Assert.That(context.LineNumber, Is.EqualTo(5));
            Assert.That(context.Line, Is.EqualTo(LINE5));
            Assert.That(context.Context.Count, Is.EqualTo(3));
        }

        [Test]
        public void SeperatorAsOnlyCharactorOnALine()
        {
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.That(_tokenEnumerator.Token.Type, Is.EqualTo(TokenType.Seperator));

            ParseContext context = _tokenEnumerator.Token.Context;
            Assert.That(context.Text, Is.EqualTo(CONTENT));
            Assert.That(context.LineNumber, Is.EqualTo(4));
            Assert.That(context.Line, Is.EqualTo(LINE4));
            Assert.That(context.Context.Count, Is.EqualTo(4));
        }

        [Test]
        public void SeperatorTokenOnTheEnd()
        {
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.That(_tokenEnumerator.Token.Type, Is.EqualTo(TokenType.Seperator));

            ParseContext context = _tokenEnumerator.Token.Context;
            Assert.That(context.Text, Is.EqualTo(CONTENT));
            Assert.That(context.LineNumber, Is.EqualTo(5));
            Assert.That(context.Line, Is.EqualTo(LINE5));
            Assert.That(context.Context.Count, Is.EqualTo(3));
            Assert.That(context.LineWithPosition, Is.EqualTo(LINE5_WITHPOINTER));
        }

        [Test]
        public void TestContextAndTab()
        {
            _tokenizer = new Tokenizer(LINE_WITH_TAB, true, true, '\\', SEPERATORS, new[] {'\''}, null);
            _tokenEnumerator = _tokenizer.GetTokenEnumerator();
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            ParseContext context = _tokenEnumerator.Token.Context;

            Assert.That(context.LineWithPosition, Is.EqualTo(LINE_WITH_TAB_WITHPOINTER));
        }

        [Test]
        public void TestContextAndTab2()
        {
            _tokenizer = new Tokenizer(LINE_WITH_TAB2, true, true, '\\', SEPERATORS, new[] {'\''}, null);
            _tokenEnumerator = _tokenizer.GetTokenEnumerator();
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            ParseContext context = _tokenEnumerator.Token.Context;

            Assert.That(context.LineWithPosition, Is.EqualTo(LINE_WITH_TAB_WITHPOINTER2));
        }

        [Test]
        public void ThirdToken()
        {
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.IsTrue(_tokenEnumerator.MoveNext());
            Assert.That(_tokenEnumerator.Token.Type, Is.EqualTo(TokenType.Regular));

            ParseContext context = _tokenEnumerator.Token.Context;
            Assert.That(context.Text, Is.EqualTo(CONTENT));
            Assert.That(context.Line, Is.EqualTo(LINE2));
            Assert.That(context.LineNumber, Is.EqualTo(2));
            Assert.That(context.Context.Count, Is.EqualTo(4));
        }

        [Test]
        public void TestEmptyNewLine()
        {
            ParseContext context = new ParseContext("\r\n", 0, "\r\n");
            Assert.That(context.LineWithPosition, Is.EqualTo("\r\n---^"));
        }
    }
}
