using System;
using Castle.MonoRail.Framework;

namespace SharpTilesSample.Controllers
{
    [Rescue("generalerror")]
    public class HomeController : SmartDispatcherController
    {
        public void Index()
        {
            PropertyBag["Title"] = "Home Page";
            PropertyBag["Message"] = "Welcome to Monorail with SharpTiles!";
            RenderView("Index");
        }

        public void About()
        {
            PropertyBag["Title"] = "About";
            RenderView("About");
        }

        public void Exception()
        {
            throw new Exception("I am a bad action!");
        }
    }
}