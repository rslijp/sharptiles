using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

public class MultiView : IView
{
    private readonly IList<PartialView> _parts = new List<PartialView>();

    #region IView Members

    public void Render(ViewContext viewContext, TextWriter writer)
    {
        foreach (PartialView part in _parts)
        {
            viewContext.ViewData = part.Data;
            writer.WriteLine(string.Format("<div id=\"update-{0}\">", part.ForId));
            part.View.Render(viewContext, writer);
            writer.WriteLine(string.Format("</div>"));
        }
    }

    #endregion

    public void Add(string forId, IView view, ViewDataDictionary data)
    {
        _parts.Add(new PartialView {ForId = forId, View = view, Data = data});
    }

    #region Nested type: PartialView

    public class PartialView
    {
        public String ForId { get; set; }
        public IView View { get; set; }
        public ViewDataDictionary Data { get; set; }
    }

    #endregion
}