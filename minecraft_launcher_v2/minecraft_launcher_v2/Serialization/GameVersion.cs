using System.Collections.Generic;

namespace minecraft_launcher_v2.Serialization
{
    public class FileInfo
    {
        public ulong size { get; set; }
        public string sha1 { get; set; }
        public string path { get; set; }
        public string url { get; set; }
    }

    public class JSON_DownloadsMainFiles
    {
        public FileInfo client { get; set; }
        public FileInfo server { get; set; }
        public FileInfo windows_server { get; set; }
    }

    public class AssetIndex
    {
        public string id { get; set; }
        public string sha1 { get; set; }
        public ulong size { get; set; }
        public string url { get; set; }
        public ulong totalSize { get; set; }
    }

    public class DownloadsLibraries
    {
        public FileInfo artifact { get; set; }
        public Dictionary<string, FileInfo> classifiers { get; set; }
    }

    public class Natives
    {
        public string linux { get; set; }
        public string osx { get; set; }
        public string windows { get; set; }
        public ulong size { get; set; }
        public string sha1 { get; set; }
        public string path { get; set; }
        public string url { get; set; }
    }

    public class Os
    {
        public string name { get; set; }
        public string version { get; set; }
    }

    public class Rule
    {
        public string action { get; set; }
        public Os os { get; set; }
    }

    public class Extract
    {
        public List<string> exclude { get; set; }
    }

    public class Library
    {
        public string name { get; set; }
        public bool? clientreq { get; set; }
        public bool? serverreq { get; set; }
        public string url { get; set; }
        public DownloadsLibraries downloads { get; set; }
        public List<Rule> rules { get; set; }
        public Extract extract { get; set; }
        public Natives natives { get; set; }
    }

    public class RootGameVersion
    {
        public string mainClass { get; set; }
        public string minecraftArguments { get; set; }
        public string minimumLauncherVersion { get; set; }
        public string releaseTime { get; set; }
        public string time { get; set; }
        public string type { get; set; }
        public string assets { get; set; }
        public string inheritsFrom { get; set; }
        public string id { get; set; }
        public List<Library> libraries { get; set; }
        public AssetIndex assetIndex { get; set; }
        public JSON_DownloadsMainFiles downloads { get; set; }
    }

}
