using minecraft_launcher_v2.Utilities;
using System;

namespace minecraft_launcher_v2.ConstantValues
{
    static class DefaultSettings
    {
        public static readonly DateTime GLOBAL_REMIND_UPDATE_NOTIFICATION_DATE = DateTime.MinValue;


        public const bool COMMON_SHOW_BETA = false;
        public const bool COMMON_AUTO_UPDATE_AVAILABLE = false;
        public const bool COMMON_LOGGED_IN = false;

        public const string VERSION_ALLOCATED_MEMORY = "512M";
        public const bool VERSION_USE_LICENCE = false;
        public const bool VERSION_SHOW_CONSOLE = true;
        public const bool VERSION_AUTO_CLOSE_CONSOLE = true;
        public const bool VERSION_AUTO_CLOSE_LAUNCHER = false;
        public const bool VERSION_FIND_JAVA = true;
        public const bool VERSION_USE_OPTIMIZATION = true;
        public const bool VERSION_USE_64_BIT = false;


        public static readonly string PROFILE_SELECTED_USER = Guid.NewGuid().ToString().ToLower();
        public static readonly string PROFILE_USER_ID = PROFILE_SELECTED_USER;

        public static readonly string PROFILE_CLIENT_TOKEN = Guid.NewGuid().ToString().ToLower();
        public static readonly string PROFILE_ACCESS_TOKEN = PROFILE_CLIENT_TOKEN;
        public static readonly string PROFILE_UUID = PROFILE_CLIENT_TOKEN;

        public static readonly string PROFILE_JAVA_DIRECTORY = JavaUtils.GetJavaPath();

        public const string PROFILE_LAST_VERSION_ID = "";
        public const string PROFILE_PROFILE_NAME = "Default profile";
        public const string PROFILE_DISPLAY_NAME = "";
        public const string PROFILE_JAVA_ARGUMENTS = "";
        public const string PROFILE_USERNAME = "";

    }
}
