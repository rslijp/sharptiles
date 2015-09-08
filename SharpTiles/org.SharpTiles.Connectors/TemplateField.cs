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
using System;
using System.ComponentModel;
using System.IO;
using System.Web.Hosting;
 using System.Web.UI;
using System.Web.UI.WebControls;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Tiles;
using org.SharpTiles.Tiles.Configuration;

namespace org.SharpTiles.Connectors
{
    [System.ComponentModel.DefaultProperty("Text")]
    [ToolboxData("<{0}:TemplateField runat=server></{0}:TemplateField>")]
    public class TemplateField : WebControl, INamingContainer
    {
        private static readonly TilesSet TILES;

        private object _model;
        private string _view;
        private static TileXmlConfigurator _configuration;

        static TemplateField()
        {
            TilesConfigurationSection config = TilesConfigurationSection.Get();
            var prefix = TemplateFieldPrefixHelper.BuildPrefix(HostingEnvironment.ApplicationPhysicalPath, config.FilePrefix);
            RefreshJob.REFRESH_INTERVAL = config.RefreshIntervalSeconds;
            _configuration = new TileXmlConfigurator(
                config.ConfigFilePath,
                prefix
                );
            TILES = new TilesSet(_configuration);
            
        }

      
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string View
        {
            get { return _view; }

            set { _view = value; }
        }

        [Bindable(true)]
        [Category("Data")]
        [DefaultValue(null)]
        [Localizable(false)]
        public object Model
        {
            get { return _model; }
            set { _model = value; }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            ITile tile = TILES[_view];
            var model = new TagModel(_model, new SimpleHttpSessionState(), null, new PageWrapper(Page), _configuration.GetFactory());
            model.Page["Request"] = Page.Request.Params;
            writer.Write(tile.Render(model));
        }
    }
}
