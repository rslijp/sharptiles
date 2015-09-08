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
using System.Text;

namespace org.SharpTiles.Tags.CoreTags
{
    [Category("URLRelated"), HasExample]
    public class Redirect : BaseUrlTag
    {
        public static readonly string NAME = "redirect";

        public override string TagName
        {
            get { return NAME; }
        }

        [Required]
        public ITagAttribute Url { get; set; }

        public override string Evaluate(TagModel model)
        {
            var urlBuilder = new StringBuilder();
            urlBuilder.Append(GetAsUrl(Url, model));
            urlBuilder.Append(ParamsEvaluate(model));

            model.Redirect(urlBuilder.ToString());


            return "";
        }
    }
}
