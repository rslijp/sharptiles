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
using System.IO;
using System.Reflection;
using Castle.MonoRail.Framework;
using org.SharpTiles.Tags;

namespace org.SharpTiles.Connectors.Monorail
{
    public abstract class BaseViewEngine<T> : ViewEngineBase where T : IViewCache, new()
    {
        public const string UNDEFINED = "undefined";
        public static bool UseHttpErrors { get; set; }
        public static IViewCache Cache { get; set; }
        private static readonly object _cacheLock = new object();

        public void Init()
        {
            GuardInit(Assembly.GetCallingAssembly());
        }

        public static void GuardInit(Assembly assembly)
        {
            lock (_cacheLock)
            {
                if (Cache != null) return;
                if (!TagLib.Exists(Tiles.Tags.Tiles.TILES_GROUP_NAME)) TagLib.Register(new Tiles.Tags.Tiles());
                Cache = new T().GuardInit(assembly);
            }
        }

        private static string ResolveViewName(string templateName)
        {
            try
            {
                return templateName.Replace(@"\", Cache.PathSeperator);
            }
            catch
            {
                return UNDEFINED;
            }
        }

        public override bool HasTemplate(string templateName)
        {
            return false;
        }

        public override void Process(IRailsEngineContext context, Controller controller, string templateName)
        {
            var view = new TilesView(ResolveViewName(templateName), Cache, UseHttpErrors);
            PreSendView(controller, view);

            var writer = context.Response.Output;
            view.Render(context, writer);
            PostSendView(controller, view);
        }

        public override void Process(TextWriter output, IRailsEngineContext context, Controller controller, string templateName)
        {

        }

        public override object CreateJSGenerator(IRailsEngineContext context)
        {
            return null;
        }

        public override void GenerateJS(TextWriter output, IRailsEngineContext context, Controller controller, string templateName)
        {

        }

        public override void ProcessContents(IRailsEngineContext context, Controller controller, string contents)
        {

        }

        public override bool SupportsJSGeneration
        {
            get { return false; }
        }

        public override string ViewFileExtension
        {
            get { return ".tile"; }
        }

        public override string JSGeneratorFileExtension
        {
            get { return null; }
        }

        public override void ProcessPartial(TextWriter output, IRailsEngineContext context, Controller controller, string partialName)
        {

        }
    }
}