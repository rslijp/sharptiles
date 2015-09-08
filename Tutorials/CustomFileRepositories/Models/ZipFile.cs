using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web.Hosting;
using ICSharpCode.SharpZipLib.Zip;
using org.SharpTiles.Common;

public class ZipFile : RefreshableResource
{
    private readonly string _path;
    private IDictionary<string, byte[]> _cache;
    private IDictionary<string, DateTime> _dates;

    public ZipFile()
    {
        _path = HostingEnvironment.ApplicationPhysicalPath + "\\tiles.zip";
        LoadResource();
    }

    public override DateTime? ResourceLastModified
    {
        get { return File.GetLastWriteTime(_path); }
    }

    public byte[] this[string name]
    {
        get
        {
            return _cache[name];
        }
    }

    protected override void Load()
    {
        // For IO there should be exception handling but in this case its been ommitted
        _cache = new Dictionary<string, byte[]>();
        _dates = new Dictionary<string, DateTime>();

        try
        {
            using (var s = new ZipInputStream(File.OpenRead(_path)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    LoadFile(s, theEntry);
                }
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }
    }

    private void LoadFile(Stream s, ZipEntry theEntry)
    {
        Debug.WriteLine(String.Format("Name : {0}", theEntry.Name));
        Debug.WriteLine(String.Format("Date : {0}", theEntry.DateTime));
        _cache[theEntry.Name] = s.GetData();
        _dates[theEntry.Name] = theEntry.DateTime;
        Debug.WriteLine(String.Format("Size : {0}", _cache[theEntry.Name].Length));
        Debug.WriteLine(String.Format("---"));
    }

    public DateTime EntryLastModified(string name)
    {
        return _dates[name];
    }

    public bool Exists(string name)
    {
        return _cache.ContainsKey(name);
    }
}