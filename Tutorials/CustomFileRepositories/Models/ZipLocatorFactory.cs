using System;
using System.Diagnostics;
using System.IO;
using org.SharpTiles.Common;
using org.SharpTiles.Tags.Creators;
using org.SharpTiles.Templates.Templates;
using org.SharpTiles.Tiles;
using org.SharpTiles.Tiles.Configuration;
using org.SharpTiles.Tiles.Tile;

public class ZipLocatorFactory : IResourceLocatorFactory
{
    private const string CONFIG_FILE = "tiles.xml";
    private static readonly ZipFile _zipFile;
        
    static ZipLocatorFactory()
    {
        _zipFile = new ZipFile();
    }

    public void Init(IConfiguration configuration){}

    public Stream GetConfiguration()
    {
        return GetNewLocator().GetStream(CONFIG_FILE);
    }

    public IResourceLocator GetNewLocator()
        {
            return new ZipResourceLocator(_zipFile);
        }

    public ITemplate Handle(string entry, bool throwException)
    {
        return Handle(entry, GetNewLocator(), throwException);
    }

    public ITemplate Handle(string entry, IResourceLocator locator, bool throwException)
    {   if (throwException || locator.Exists(entry))
            {
                return new RefreshableResourceTemplate(locator, this, entry);
            }
            return null;
        }

    public DateTime? ConfigurationLastModified
    {
        get { return _zipFile.ResourceLastModified; }
    }
}