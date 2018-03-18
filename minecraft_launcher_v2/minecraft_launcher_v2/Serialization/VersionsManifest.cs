using System.Collections.Generic;

namespace minecraft_launcher_v2.Serialization
{
    public class Latest
    {
        public string snapshot { get; set; }
        public string release { get; set; }
    }

    public class ManifestVersion
    {
        public string id { get; set; }
        public string type { get; set; }
        public string time { get; set; }
        public string releaseTime { get; set; }
        public string url { get; set; }
    }

    public class RootVersionsManifest
    {
        public Latest latest { get; set; }
        public List<ManifestVersion> versions { get; set; }
    }

}
