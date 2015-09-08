/*
 * SharpTiles, R.Z. Slijp(2008), www.sharptiles.org
 *
 * This file is part of SharpTiles.
 * 
 * SharpTiles is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * SharpTiles is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with SharpTiles.  If not, see <http://www.gnu.org/licenses/>.
 */
 using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;

namespace org.SharpTiles.Tags.CoreTags
{
    [Category("URLRelated"), HasExample]
    public class Import : BaseUrlTagWithVariable
    {
        public static readonly string NAME = "import";

        public override string TagName
        {
            get { return NAME; }
        }

        [Required]
        public ITagAttribute Url { get; set; }

        protected override object InternalEvaluate(TagModel model)
        {
            var urlBuilder = new StringBuilder();
            urlBuilder.Append(GetAsUrl(Url, model));
            urlBuilder.Append(ParamsEvaluate(model));

            return ReadData(PlaceRequest(urlBuilder));
        }

        private static WebResponse PlaceRequest(StringBuilder urlBuilder)
        {
            WebRequest request = WebRequest.Create(urlBuilder.ToString());

            // execute the request
            return request.GetResponse();
        }

        private static string ReadData(WebResponse response)
        {
            Stream resStream = response.GetResponseStream();

            var buf = new byte[8192];
            var sb = new StringBuilder();
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
    }
}
