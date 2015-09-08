using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;
using org.SharpTiles.Templates;

namespace LanguageService
{
    [ComVisible(true)]
    [Guid("C674518A-4321-4f00-9C4D-CD0DBBB8C761")]
    public class SharpTilesLanguage : Microsoft.VisualStudio.Package.LanguageService
    {
        private SharpTilesScanner scanner;

        public override LanguagePreferences GetLanguagePreferences()
        {
            throw new System.NotImplementedException();
        }

        public override IScanner GetScanner(IVsTextLines buffer)
        {
            if (scanner == null)
            {
                // Create new RegularExpressionScanner instance
                scanner = new SharpTilesScanner();
            }

            return scanner;

        }

        public override AuthoringScope ParseSource(ParseRequest req)
        {
            throw new System.NotImplementedException();
        }

        public override string GetFormatFilterList()
        {
            return "Tile File (*.tile) *.tile *.nstl *.nsp";
        }

        public override string Name
        {
            get { return "SharpTiles Language Service"; }
        }
    }

    public class SharpTilesScanner : IScanner
    {
        private string _sourceString;
        private int _offset;
        private Formatter _template;

        public void SetSource(string source, int offset)
        {
            _sourceString = source;
            _offset = offset;
            try
            {
                _template = new Formatter(_sourceString.Substring(_offset));
            } catch(Exception e)
            {
                Debug.WriteLine("Parsing failed. "+e.Message);
            }
        }

        public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
        {
            var index = tokenInfo.StartIndex;
            var part = _template.ParsedTemplate.FirstOrDefault(p=>p.Context.Index > index);
            if (part == null) return false;
            if(part is TextPart)
            {
                Debug.WriteLine("Text "+part.Context.Text);
                tokenInfo.Type = TokenType.Text;
                tokenInfo.Color = TokenColor.Text;
            } else if(part is ExpressionPart)
            {
                Debug.WriteLine("Text "+part.Context.Text);
                tokenInfo.Type = TokenType.Operator;
                tokenInfo.Color = TokenColor.Number;
            }
            else if (part is TagPart)
            {
                Debug.WriteLine("Tag " + part.Context.Text);
                tokenInfo.Type = TokenType.Keyword;
                tokenInfo.Color = TokenColor.Keyword;
            }
            return true;
        }
    }
}
