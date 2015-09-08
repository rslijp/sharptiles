using System;
using System.Web.Mvc;
using org.SharpTiles.Connectors;

public static class MultiViewExtensions
{
    private const string ROUTEDATE_KEY_CONTROLLER = "controller";
    private const string ROUTEDATE_KEY_ACTION = "action";
    public const string UNDEFINED = "undefined";

    public static string GetTilesViewName(this ControllerBase controller)
    {
        try
        {
            return GetTilesViewName(controller, controller.ControllerContext.RouteData.Values[ROUTEDATE_KEY_ACTION]);
        }
        catch
        {
            return UNDEFINED;
        }
    }

    public static string GetTilesViewName(this ControllerBase controller, object viewName)
    {
        try
        {
            var controllerStr = controller.ControllerContext.RouteData.Values[ROUTEDATE_KEY_CONTROLLER];
            return String.Format("{0}.{1}", controllerStr, viewName);
        }
        catch
        {
            return UNDEFINED;
        }
    }


    public static Controller Update(this Controller controller, string forId)
    {
        return Update(controller, forId, controller.GetTilesViewName());
    }

    public static Controller Update(this Controller controller, string forId, string viewName, object model)
    {
        var view = MultiViewEngine.GetMultiView();
        view.Add(
            forId,
            new TilesView(viewName, TilesViewEngine.Cache, false),
            new ViewDataDictionary(model)
        );
        return controller;
    }


    public static Controller Update(this Controller controller, string forId, string viewName)
    {
        var view = MultiViewEngine.GetMultiView();
        view.Add(
            forId,
            new TilesView(viewName, TilesViewEngine.Cache, false),
            controller.ViewData
        );
        return controller;
    }

    public static ViewResult Render(this Controller controller)
    {
        return new ViewResult
            {
                ViewData = controller.ViewData,
                ViewName = controller.GetTilesViewName(),
                TempData = controller.TempData,
            };
    }

    public static ViewResult RenderLayout(this Controller controller)
    {
        return new ViewResult
        {
            ViewData = controller.ViewData,
            ViewName = "LayOut",
            TempData = controller.TempData,
        };
    }
}