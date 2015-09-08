using System;
using System.Configuration;
using System.Diagnostics;
using WatiN.Core;

namespace SharpTilesSampleTest
{
    public class DevWebServer
    {
        private const string VPATH = "/SharpTilesSample";
        private const string BASE_URL = "http://localhost";
        private static string _currentPort = "3030";

        private Process devWebServerProcess;

        public void Start()
        {
            // create a new process to start the ASP.Net Development Server
            devWebServerProcess = new Process();

            // set the initial properties 
            string from = ConfigurationManager.AppSettings["Test.Project.Path"];
            string to = ConfigurationManager.AppSettings["WebApp.Project.Path"];
            string path = Environment.CurrentDirectory.Replace(from, to);
            devWebServerProcess.StartInfo.FileName = @"C:\Program Files\Common Files\microsoft shared\DevServer\9.0\WebDev.WebServer.EXE";
            devWebServerProcess.StartInfo.Arguments = String.Format("/port:{0} /path:\"{1}\" /vpath:{2}", _currentPort, path, VPATH);
            devWebServerProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            
            // start the process
            devWebServerProcess.Start();

            // Show tests during run
            IE.Settings.HighLightElement = true;
            IE.Settings.MakeNewIeInstanceVisible = true;
            // Don't wait to long for an element
            IE.Settings.WaitUntilExistsTimeOut = 5;
        }

        public void Stop()
        {
            if (devWebServerProcess != null)
            {
                devWebServerProcess.Kill();
            }
        }

        public string BaseUrl
        {
            get
            {
                return BASE_URL + ":" + _currentPort + VPATH + "/";
            }
        }
    }
}
