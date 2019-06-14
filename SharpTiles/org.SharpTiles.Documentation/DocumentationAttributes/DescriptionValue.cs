using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MarkdownDeep;

namespace org.SharpTiles.Documentation.DocumentationAttributes
{
    [DataContract]
    public class DescriptionValue
    {
        private static Regex INNER_CONTENT = new Regex("^<p>(.*?)</p>$");

        public DescriptionValue(string value, string category=null)
        {
            Value = value;
            Category = category;
        }

        public DescriptionValue(DescriptionAttribute attribute) : this(attribute.Value, attribute.Category)
        {
        }

        [DataMember]
        public string Value { get; private set; }
        [DataMember]
        public string Category { get; private set; }
        [ScriptIgnore]
        public string Html
        {
            get
            {
                try
                {
                    var html = new Markdown { ExtraMode = true }.Transform(Value);
                    html = html.Trim();
                    if (INNER_CONTENT.IsMatch(html))
                    {
                        html = INNER_CONTENT.Match(html).Groups[1].Value;
                    }
                    return html;
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
        }
        
    }

}
