using System;
using System.Collections;
using System.Web.UI;

namespace SharpTilesSample
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var model = new Hashtable();
            var list = new ArrayList();
            list.Add("This is a paragraph");
            list.Add("Some text");
            model.Add("paragraph", list);

            var table = new Hashtable();
            var header = new ArrayList(new[] {"H1", "H2", "H3"});
            var body = new ArrayList();
            body.Add(new[] {"", "X", ""});
            body.Add(new[] {"X", "", "X"});
            body.Add(new[] {"", "X", ""});
            table.Add("header", header);
            table.Add("body", body);
            model.Add("table", table);

            template.Model = model;
        }
    }
}