using System.Runtime.Remoting.Messaging;
using System.Web.Mvc;
using org.SharpTiles.Connectors;

public class MultiViewEngine : IViewEngine
{

    public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
    {
        return FindView(controllerContext, partialViewName, null, useCache);
    }

    public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
    {
        if(viewName.Equals("LayOut"))
        {
            return new TilesViewEngine().FindView(controllerContext, viewName, masterName, useCache);
        }
        var view = GetMultiView();
        return new ViewEngineResult(
                view,
                this
                );
    }

    public static MultiView GetMultiView()
    {
        var view =  (MultiView) CallContext.GetData("View");
        if(view==null)
        {
            view = new MultiView();
            CallContext.SetData("View", view);
        }
        return view;
    }

    public void ReleaseView(ControllerContext controllerContext, IView view)
    {
        CallContext.SetData("View", null);
    }
}