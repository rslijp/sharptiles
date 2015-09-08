using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core;

namespace SharpTilesSampleTest
{
    public class BenchMarkHelper
    {
        private static int RUN = 20;
        private IE _ie = null;
        private string _baseUrl;

        public BenchMarkHelper(IE ie, string baseUrl)
        {
            _baseUrl = baseUrl;
            _ie = ie;
        }

        public void TestPlain(string url)
        {
            url = _baseUrl + url;
            ReadData(PlaceRequest(url));
            DateTime start = DateTime.Now;
            for (int i = 0; i < RUN; i++)
            {
                ReadData(PlaceRequest(url));
            }
            DateTime end = DateTime.Now;
            TimeSpan time = end.Subtract(start);
            double avg = (RUN / (time.TotalMilliseconds / 1000.0));
            Assert.That(avg, Is.GreaterThan((double)RUN / 10));
            Console.WriteLine("RAW: " + url + ": " + avg + " average formats per second");
        }

        private static WebResponse PlaceRequest(string url)
        {
            WebRequest request = WebRequest.Create(url);

            // execute the request
            return request.GetResponse();
        }

        private static string ReadData(WebResponse response)
        {
            Stream resStream = response.GetResponseStream();

            byte[] buf = new byte[8192];
            StringBuilder sb = new StringBuilder();
            string tempString = null;
            int count = 0;

            do
            {
                // fill the buffer with data
                count = resStream.Read(buf, 0, buf.Length);

                // make sure we read some data
                if (count != 0)
                {
                    // translate from bytes to ASCII text
                    tempString = Encoding.ASCII.GetString(buf, 0, count);

                    // continue building the string
                    sb.Append(tempString);
                }
            } while (count > 0); // any more data to read?
            return sb.ToString();
        }

        public void TestWithWatin(String url)
        {

            _ie.GoTo(_baseUrl + url);
            DateTime start = DateTime.Now;
            for (int i = 0; i < RUN; i++)
            {
                _ie.GoTo(_baseUrl + url);
            }
            DateTime end = DateTime.Now;
            TimeSpan time = end.Subtract(start);
            double avg = (RUN / (time.TotalMilliseconds / 1000.0));
            Assert.That(avg, Is.GreaterThan((double)RUN / 10));
            Console.WriteLine("IE : " + url + ": " + avg + " average formats per second");
        }

    }
}
