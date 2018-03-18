using System.Collections.Generic;

namespace minecraft_launcher_v2.Serialization
{
    public class UserCache
    {
        public string name { get; set; }
        public string uuid { get; set; }
        public string expiresOn { get; set; }
    }

    public class RootUserCache
    {
        public List<UserCache> usersCache { get; set; }
    }

}
