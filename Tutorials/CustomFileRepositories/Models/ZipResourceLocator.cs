using System;
using System.IO;
using System.Resources;
using System.Text;
using org.SharpTiles.Common;

public class ZipResourceLocator : IResourceLocator
{
    private readonly ZipFile _file;
    private readonly String _prefix;

    public ZipResourceLocator(ZipFile file) : this(file, null){}

    public ZipResourceLocator(ZipFile file, String prefix)
    {
        _file = file;
        _prefix = prefix;
    }

    public string GetDataAsString(string name)
    {
        return new StreamReader(GetStream(name)).ReadToEnd();
    }

    public string GetDataAsString(string name, Encoding encoding)
    {
        return encoding.GetString(_file[name]);
    }

    public Stream GetStream(string name)
    {
        return new MemoryStream(_file[name], false);
    }

    public byte[] GetData(string name)
    {
        return _file[name];
    }

    public bool Exists(string name)
    {
        return _file.Exists(name);
    }

    public string PreFixed(string path)
    {

        string prefixed = _prefix;
        if (!String.IsNullOrEmpty(prefixed))
        {
            prefixed = prefixed.EndsWith(".")
                           ?
                               prefixed
                           :
                               (prefixed + ".");
        }
        else
        {
            prefixed = "";
        }
        prefixed += path;
        return prefixed;
    }

    public IResourceLocator Update(string path)
    {
    
            var newLocator = new ZipResourceLocator(_file, _prefix);
            return newLocator;
    }

    public ResourceManager LoadBundle(string filePath)
    {
        var tempPath = Path.GetTempFileName();
        try
        {
            var fs = new FileStream(tempPath, FileMode.CreateNew);
            var data = _file[filePath];
            fs.Write(data, 0, data.Length);
            string file = Path.GetFileName(tempPath);
            string dir = Path.GetDirectoryName(tempPath);
            return ResourceManager.CreateFileBasedResourceManager(file, dir, null);
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    public DateTime? LastModified(string path)
    {
        return _file.EntryLastModified(path);
    }

    public string PathSeperator
    {
        get { return "."; }
    }
}