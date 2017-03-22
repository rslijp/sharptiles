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
 using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace org.SharpTiles.Common.Test
{
    [TestFixture]
    public class TokenizerTest
    {
        [Test]
        public void AnTempalteWithOutAnySubsitutionsShouldBeReturnedAsSuch()
        {
            const string INPUT = "abcdefgh";
            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"."}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(INPUT, Is.EqualTo(enumerator.Current.Contents));
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void DoubleWhiteSpaceSurroundedSeperatorsShouldBeFoundWhenSurroundedByWhiteSpaces()
        {
            const string PART1 = "a";
            const string PART2 = "e";
            const string SEP = ",";
            const string INPUT = PART1 + " " + SEP + " " + SEP + " " + PART2;
            var tokenizer = new Tokenizer(INPUT, true, '\\', new[] {"."}, null, new[] {","});
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void DoubleWhiteSpaceSurroundedSeperatorsShouldBeFoundWhenSurroundedByWhiteSpacesAtEndOfInput()
        {
            const string PART1 = "abc";
            const string SEP = ",";
            const string INPUT = PART1 + " " + SEP + " " + SEP;
            var tokenizer = new Tokenizer(INPUT, true, '\\', new[] {"."}, null, new[] {","});
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP));
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void DoubleWhiteSpaceSurroundedSeperatorsShouldBeFoundWhenSurroundedByWhiteSpacesAtStartOfInput()
        {
            const string PART2 = "ec";
            const string SEP = ",";
            const string INPUT = SEP + " " + SEP + " " + PART2;
            var tokenizer = new Tokenizer(INPUT, true, '\\', new[] {"."}, null, new[] {","});
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void EmptyStringShouldNotCollapseTokenizer()
        {
            const string INPUT = "";
            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"."}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void EscapedEscapreCharactersShouldBeIgnored()
        {
            const string PART1 = "abc";
            const string PART2 = "defg";
            const string INPUT = PART1 + "\\\\" + PART2;

            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"."}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1 + "\\" + PART2));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }

        

        [Test]
        public void EscapedSeperatorsShouldBeIgnored()
        {
            const string PART1 = "abc";
            const string PART2 = "defg";
            const string PART3 = "hijklmnop";
            const string INPUT = PART1 + "\\." + PART2 + "." + PART3 + ".\\.";

            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"."}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1 + "." + PART2));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART3));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo("."));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }


        [Test]
        public void IllegalEscapedCharactersShouldThrowAnException()
        {
            const string INPUT = "abc\\a";

            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"."}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            try
            {
                enumerator.MoveNext();
            }
            catch (TokenException Te)
            {
                Assert.That(Te.Message, Is.EqualTo(TokenException.IllegalEscapeCharacter('a').Message));
            }
        }

        [Test]
        public void MissingEscapedCharactersShouldThrowAnException()
        {
            const string INPUT = "abc\\";

            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"."}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            try
            {
                enumerator.MoveNext();
            }
            catch (TokenException Te)
            {
                Assert.That(Te.MessageWithOutContext, Is.EqualTo(TokenException.MoreCharactersExpectedAt(4).Message));
            }
        }

        [Test]
        public void ResetShouldRestartEnumerator()
        {
            const string INPUT = "abcdefgh";
            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"."}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsFalse(enumerator.MoveNext());
            enumerator.Reset();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void SeperatorAndRegularTokensShouldBeDistinctable()
        {
            const string PART1 = "abc";
            const string PART2 = "defg";
            const string PART3 = "hijklmnop";
            const string INPUT = PART1 + "\\." + PART2 + "." + PART3 + ".\\.";

            var tokenizer = new Tokenizer(INPUT, true, '\\', new[] {"."}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }

        [Test]
        public void SeperatorAndWhiteSpaceSurroundedSeperatorsShouldBeBothBeAllowedInInput()
        {
            const string PART1 = "a";
            const string PART2 = "e";
            const string SEP1 = ",";
            const string SEP2 = ".";
            const string INPUT = PART1 + " " + SEP1 + " " + SEP2 + PART2;
            var tokenizer = new Tokenizer(INPUT, true, '\\', new[] {"."}, null, new[] {","});
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP2));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void TokenizerLargestTokenShouldBeReturned()
        {
            const string PART = "aaa";
            const string SEP_SMALLEST = "S";
            const string SEP_NORMAL = "SE";
            const string SEP_LARGEST = "SEP";
            const string INPUT = PART + SEP_LARGEST + PART + SEP_NORMAL + PART + SEP_SMALLEST + PART;

            var tokenizer = new Tokenizer(INPUT, true, '\\',
                                          new[] {SEP_SMALLEST, SEP_NORMAL, SEP_LARGEST}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP_LARGEST));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP_NORMAL));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP_SMALLEST));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }

        [Test]
        public void TokenizerShouldHandleEscapedThreeDigitSeperators()
        {
            const string PART1 = "abc";
            const string PART2 = "defg";
            const string PART3 = "hijklmnop";
            const string INPUT = PART1 + "\\SEP" + PART2 + "DOT" + PART3;

            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"SEP", "DOT"}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1 + "SEP" + PART2));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART3));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }

        [Test]
        public void TokenizerShouldHandleMultipleSeperators()
        {
            const string PART1 = "abc";
            const string PART2 = "defg";
            const string PART3 = "hijklmnop";
            const string INPUT = PART1 + "1" + PART2 + "2" + PART3;

            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"1", "2"}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART3));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }


        [Test]
        public void TokenizerShouldHandleMultipleThreeDigitSeperators()
        {
            const string PART1 = "abc";
            const string PART2 = "defg";
            const string PART3 = "hijklmnop";
            const string INPUT = PART1 + "SEP" + PART2 + "DOT" + PART3;

            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"SEP", "DOT"}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART3));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }

        [Test]
        public void TokenizerShouldHandleRepeatingSeperators()
        {
            const string PART1 = "abc";
            const string PART2 = "defg";
            const string PART3 = "hijklmnop";
            const string INPUT = PART1 + ".." + PART2 + "..." + PART3;

            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"."}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART3));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }

        [Test]
        public void TokenizerShouldHandleRepeatingSeperatorsAtTheEndOfTheString()
        {
            const string PART1 = "abc";
            const string PART2 = "defg";
            const string INPUT = PART1 + "." + PART2 + "..";

            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"."}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }

        [Test]
        public void TokenizerShouldHandleSeperatorsAtTheEndOfTheString()
        {
            const string PART1 = "abc";
            const string PART2 = "defg";
            const string PART3 = "hijklmnop";
            const string INPUT = PART1 + "1" + PART2 + "2" + PART3;

            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"1", "2"}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART3));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }

        [Test]
        public void TokenizerShouldHandleThreeDigitSeperatorsAtTheBegin()
        {
            const string PART1 = "abc";
            const string PART2 = "defg";
            const string INPUT = "SEP" + PART1 + "DOT" + PART2;

            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"SEP", "DOT"}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }

        [Test]
        public void TokenizerShouldHandleThreeDigitSeperatorsAtTheBeginWithReturnOfSeperators()
        {
            const string PART1 = "abc";
            const string PART2 = "defg";
            const string SEP1 = "SEP";
            const string SEP2 = "DOT";
            const string INPUT = SEP1 + PART1 + SEP2 + PART2;

            var tokenizer = new Tokenizer(INPUT, true, '\\', new[] {SEP1, SEP2}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP1));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP2));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }

        [Test]
        public void TokenizerShouldHandleThreeDigitSeperatorsAtTheEnd()
        {
            const string PART1 = "abc";
            const string PART2 = "defg";
            const string INPUT = PART1 + "SEP" + PART2 + "DOT";

            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"SEP", "DOT"}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }

        [Test]
        public void TokenizerShouldHandleThreeDigitSeperatorsAtTheEndWithReturnOfSeperators()
        {
            const string PART1 = "abc";
            const string PART2 = "defg";
            const string SEP1 = "SEP";
            const string SEP2 = "DOT";
            const string INPUT = PART1 + SEP1 + PART2 + SEP2;

            var tokenizer = new Tokenizer(INPUT, true, '\\', new[] {SEP1, SEP2}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP1));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP2));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }

        [Test]
        public void TokenizerShouldIgnoreEscapedLiteralInEmbeddedInLiterals()
        {
            const string INPUT = "abc'\\''defgh";
            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"."}, new[] {'\''});
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo("abc'''defgh"));
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void TokenizerShouldIgnoreNestedDoubleLiteral()
        {
            const string PART1 = "abc'\"a\"'defgh";
            const string PART2 = "wxz";
            const string INPUT = PART1 + "." + PART2;
            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"."}, new[] {'\'', '"'});
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void TokenizerShouldIgnoreNestedDoubleLiteralAndNestedSeperator()
        {
            const string PART1 = "a'\".\"'b";
            const string PART2 = "wxz";
            const string INPUT = PART1 + "." + PART2;
            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"."}, new[] {'\'', '"'});
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void TokenizerShouldIgnoreNestedLiteral()
        {
            const string PART1 = "abc'.\"'defgh";
            const string PART2 = "wxz";
            const string INPUT = PART1 + "." + PART2;
            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"."}, new[] {'\'', '"'});
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void TokenizerShouldIgnoreTokensEmbeddedInLiterals()
        {
            const string INPUT = "abc'.'defgh";
            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"."}, new[] {'\''});
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(INPUT));
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void TokenizerShouldIgnoreTokensEmbeddedInLiteralsSurroundedWithOtherText()
        {
            const string INPUT = "abc'ignore.ignore'defgh";
            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"."}, new[] {'\''});
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(INPUT));
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void TokenizerShouldNotIgnoreOtherTokensBesidesTheEmbeddedOnInLiterals()
        {
            const string PART1 = "abc'.'defgh";
            const string PART2 = "wxz";
            const string INPUT = PART1 + "." + PART2;
            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"."}, new[] {'\''});
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.IsFalse(enumerator.MoveNext());
        }


        [Test]
        public void TokenizerShouldReturnFollowingThreeDigitSeperators()
        {
            const string PART1 = "abc";
            const string PART2 = "defg";
            const string SEP = "SEP";
            const string INPUT = PART1 + SEP + SEP + PART2;

            var tokenizer = new Tokenizer(INPUT, true, '\\', new[] {SEP}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }

        [Test]
        public void TokenizerShouldReturnFollowingThreeDigitSeperatorsAtTheEnd()
        {
            const string PART1 = "abc";
            const string SEP = "SEP";
            const string INPUT = PART1 + SEP + SEP;

            var tokenizer = new Tokenizer(INPUT, true, '\\', new[] {SEP}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }

        [Test]
        public void TokenizerShouldReturnsLiteralsWhenAsked()
        {
            const string PART1 = "abc";
            const string PART2 = "defg";
            const string PART3 = "hijklmnop";
            const string PART4 = "qrst";
            const string PART5 = "uvw";
            const string INPUT = PART1 + "'" + PART2 + "'" + PART3 + "'" + PART4 + "'" + PART5;

            var tokenizer = new Tokenizer(INPUT, true, true, '\\', new[] {"."}, new[] {'\''}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Literal));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART3));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART4));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Literal));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART5));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }


        [Test]
        public void TokenizerShouldReturnsLiteralsWhenAskedAtStartAndEnd()
        {
            const string PART2 = "defg";
            const string PART3 = "hijklmnop";
            const string PART4 = "qrst";
            const string INPUT = "'" + PART2 + "'" + PART3 + "'" + PART4 + "'";

            var tokenizer = new Tokenizer(INPUT, true, true, '\\', new[] {"."}, new[] {'\''}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Literal));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART3));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART4));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Literal));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }

        [Test]
        public void TokenizerShouldReturnsTokens()
        {
            const string PART1 = "abc";
            const string PART2 = "defg";
            const string PART3 = "hijklmnop";
            const string PART4 = "qrst";
            const string PART5 = "uvw";
            const string INPUT = PART1 + "." + PART2 + "." + PART3 + "." + PART4 + "." + PART5;

            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"."}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART3));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART4));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART5));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }

        [Test]
        public void TokenizerShouldThrowErrorOnUnEndedLiteral()
        {
            const string INPUT = "abc'defgh";
            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"."}, new[] {'\''});
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            try
            {
                enumerator.MoveNext();
            }
            catch (TokenException Te)
            {
                Assert.That(Te.MessageWithOutContext, Is.EqualTo(TokenException.UnTerminatedLiteral(4).Message));
            }
        }

        [Test]
        public void TripleWhiteSpaceSurroundedSeperatorsShouldBeFoundWhenSurroundedByWhiteSpaces()
        {
            const string PART1 = "a";
            const string PART2 = "e";
            const string SEP = ",";
            const string INPUT = PART1 + " " + SEP + " " + SEP + " " + SEP + " " + PART2;
            var tokenizer = new Tokenizer(INPUT, true, '\\', new[] {"."}, null, new[] {","});
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void TripleWhiteSpaceSurroundedSeperatorsShouldBeFoundWhenSurroundedByWhiteSpacesAtEndOfInput()
        {
            const string PART1 = "abc";
            const string SEP = ",";
            const string INPUT = PART1 + " " + SEP + " " + SEP + " " + SEP;
            var tokenizer = new Tokenizer(INPUT, true, '\\', new[] {"."}, null, new[] {","});
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP));
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void TripleWhiteSpaceSurroundedSeperatorsShouldBeFoundWhenSurroundedByWhiteSpacesAtStartOfInput()
        {
            const string PART2 = "ec";
            const string SEP = ",";
            const string INPUT = SEP + " " + SEP + " " + SEP + " " + PART2;
            var tokenizer = new Tokenizer(INPUT, true, '\\', new[] {"."}, null, new[] {","});
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void WhenAskedTokenizerShouldReturnSeperator()
        {
            const string PART1 = "abc";
            const string PART2 = "defg";
            const string PART3 = "hijklmnop";
            const string INPUT = PART1 + "\\." + PART2 + "." + PART3 + ".\\.";

            var tokenizer = new Tokenizer(INPUT, true, '\\', new[] {"."}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1 + "." + PART2));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo("."));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART3));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo("."));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo("."));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }

        [Test]
        public void WhenMultipleSeperatorTokensProvidedTokenizerShouldReturnAllFourSeperatorAtTheEnd()
        {
            const string PART1 = "abc";
            const string INPUT = PART1 + "....";

            var tokenizer = new Tokenizer(INPUT, true, '\\', new[] {"."}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo("."));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo("."));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo("."));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo("."));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }

        [Test]
        public void WhenMultipleSeperatorTokensProvidedTokenizerShouldReturnAllSeperator()
        {
            const string PART1 = "abc";
            const string PART2 = "defg";
            const string INPUT = PART1 + "..." + PART2;

            var tokenizer = new Tokenizer(INPUT, true, '\\', new[] {"."}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo("."));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo("."));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo("."));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }

        [Test]
        public void WhenMultipleSeperatorTokensProvidedTokenizerShouldReturnAllThreeSeperatorAtTheEnd()
        {
            const string PART1 = "abc";
            const string INPUT = PART1 + "...";

            var tokenizer = new Tokenizer(INPUT, true, '\\', new[] {"."}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo("."));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo("."));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo("."));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }

        [Test]
        public void WhenMultipleSeperatorTokensProvidedTokenizerShouldReturnAllTwoSeperatorAtTheEnd()
        {
            const string PART1 = "abc";
            const string INPUT = PART1 + "..";

            var tokenizer = new Tokenizer(INPUT, true, '\\', new[] {"."}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo("."));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo("."));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }

        [Test]
        public void WhenMultipleSeperatorTokensProvidedTokenizerShouldReturnTheSeperatorAtTheEnd()
        {
            const string PART1 = "abc";
            const string INPUT = PART1 + "..";

            var tokenizer = new Tokenizer(INPUT, true, '\\', new[] {"."}, null);
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo("."));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo("."));
            Assert.IsFalse(enumerator.MoveNext());
            Assert.That(enumerator.Current, Is.Null);
        }

        [Test]
        public void WhiteSpaceSurroundedSeperatorsOnly()
        {
            const string INPUT = ",";
            var tokenizer = new Tokenizer(INPUT, true, '\\', new[] {"."}, null, new[] {","});
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(INPUT));
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void WhiteSpaceSurroundedSeperatorsOnlyNoReturnOfSeperator()
        {
            const string INPUT = ",";
            var tokenizer = new Tokenizer(INPUT, false, '\\', new[] {"."}, null, new[] {","});
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void WhiteSpaceSurroundedSeperatorsShouldBeFoundWhenSurroundedByWhiteSpaces()
        {
            const string PART1 = "a";
            const string PART2 = "e";
            const string SEP = ",";
            const string INPUT = PART1 + " " + SEP + " " + PART2;
            var tokenizer = new Tokenizer(INPUT, true, '\\', new[] {"."}, null, new[] {","});
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void WhiteSpaceSurroundedSeperatorsShouldBeFoundWhenSurroundedByWhiteSpacesAtEndOfInput()
        {
            const string PART1 = "abc";
            const string SEP = ",";
            const string INPUT = PART1 + " " + SEP;
            var tokenizer = new Tokenizer(INPUT, true, '\\', new[] {"."}, null, new[] {","});
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP));
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void WhiteSpaceSurroundedSeperatorsShouldBeFoundWhenSurroundedByWhiteSpacesAtStartOfInput()
        {
            const string PART2 = "ec";
            const string SEP = ",";
            const string INPUT = SEP + " " + PART2;
            var tokenizer = new Tokenizer(INPUT, true, '\\', new[] {"."}, null, new[] {","});
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Seperator));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(SEP));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void WhiteSpaceSurroundedSeperatorsShouldBeNotFoundWhenNotSurroundedByWhiteSpaces()
        {
            const string INPUT = "ab23cd,e23fgh";
            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"."}, null, new[] {","});
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(INPUT));
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void WhiteSpaceSurroundedSeperatorsShouldBeNotFoundWhenNotSurroundedByWhiteSpacesBoundaries()
        {
            const string INPUT = "a,sdfsa,h";
            var tokenizer = new Tokenizer(INPUT, '\\', new[] {"."}, null, new[] {","});
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Contents, Is.EqualTo(INPUT));
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void WhiteSpaceSurroundedSeperatorsShouldNotBeReturnedIfNotAsked()
        {
            const string PART1 = "xs";
            const string PART2 = "ec";
            const string SEP = ",";
            const string INPUT = PART1 + SEP + " " + SEP + " " + SEP + " " + PART2;
            var tokenizer = new Tokenizer(INPUT, false, '\\', new[] {"."}, null, new[] {","});
            IEnumerator<Token> enumerator = tokenizer.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART1 + SEP));
            Assert.IsTrue(enumerator.MoveNext());
            Assert.That(enumerator.Current.Type, Is.EqualTo(TokenType.Regular));
            Assert.That(enumerator.Current.Contents, Is.EqualTo(PART2));
            Assert.IsFalse(enumerator.MoveNext());
        }
    }
}
