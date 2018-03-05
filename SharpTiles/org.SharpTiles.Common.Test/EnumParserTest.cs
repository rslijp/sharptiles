using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace org.SharpTiles.Common.Test
{
    [TestFixture]
    public class EnumParserTest
    {
        [Test]
        public void Should_be_able_to_handle_duplicate_values()
        {
            try
            {
                // When
                var enumParser = new EnumParser<FileFormat>();

                // Then: No exception
            }
            catch (TypeInitializationException e)
            {
                Assert.Fail(e.Message);
            }
        }

        public enum FileFormat
        {
            Excel8 = 0,
            [Obsolete("Use FileFormat.Excel8")] XLS97 = 0,
            OpenXMLWorkbook = 2,
            CSV = 3,
            UnicodeText = 4,
            OpenXMLWorkbookMacroEnabled = 5,
        }
    }
}