using System.Collections.Generic;

namespace minecraft_launcher_v2.Serialization
{
    public class AssetObject
    {
        public string hash { get; set; }
        public ulong size { get; set; }
    }

    public class RootAssets
    {
        public Dictionary<string, AssetObject> objects { get; set; }
    }

}
