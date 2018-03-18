using System.Collections.Generic;

namespace minecraft_launcher_v2.Serialization
{
    public class Resolution
    {
        public int width { get; set; }
        public int height { get; set; }
    }

    public class LauncherVersion
    {
        public string name { get; set; }
        public int format { get; set; }
    }

    public class AuthenticationDatabase
    {
        public string displayName { get; set; }
        public string accessToken { get; set; }
        public string userid { get; set; }
        public string uuid { get; set; }
        public string username { get; set; }
    }

    public class Profile
    {
        public string name { get; set; }
        public string gameDir { get; set; }
        public string lastVersionId { get; set; }
        public string javaDir { get; set; }
        public string javaArgs { get; set; }
        public Resolution resolution { get; set; }
        public List<string> allowedReleaseTypes { get; set; }
        public string launcherVisibilityOnGameClose { get; set; }
    }

    public class RootLauncherProfiles
    {
        public Dictionary<string, Profile> profiles { get; set; }
        public string selectedProfile { get; set; }
        public string clientToken { get; set; }
        public Dictionary<string, AuthenticationDatabase> authenticationDatabase { get; set; }
        public string selectedUser { get; set; }
        public LauncherVersion launcherVersion { get; set; }
    }

}
