using minecraft_launcher_v2.Utilities;
using System;
using System.Reflection;

namespace minecraft_launcher_v2.ConstantValues
{
    static class Constants
    {
        public static readonly string PATH_GLOBAL_SETTINGS = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\My Games\\Minecraft\\minecraft_launcher";
        public static readonly string PATH_EXECUTING_ASSEMBLY = Assembly.GetExecutingAssembly().Location;
        public const string PATH_FILE_JAVA_R = "\\bin\\java.exe";
        public const string PATH_FILE_JAVAW_R = "\\bin\\javaw.exe";

        public static readonly bool IS_64_BIT = Environment.Is64BitOperatingSystem;

        public const byte MEMORY_CLEAR_THREHOLD = 60;
        public const ulong RAM_MINIMUM_START = 268435456;
        public static readonly ulong RAM_TOTAL_BYTES = Singlet.ComputerInfo.TotalPhysicalMemory;

        public static readonly string JAVA_ARGUMENTS_OPTIMIZATION = " -XX:+UseBiasedLocking -Dfile.encoding=UTF-8 -XX:+UseConcMarkSweepGC -XX:-UseAdaptiveSizePolicy -XX:+OptimizeStringConcat" +
         " -XX:+AggressiveOpts -XX:+UseFastAccessorMethods -XX:SurvivorRatio=16 -XX:+UseParNewGC -XX:hashCode=5 -XX:ParallelGCThreads=" + Environment.ProcessorCount;
        public const string JAVA_ARGUMENTS_LOGGING_F = " -XX:-HeapDumpOnOutOfMemoryError -XX:ErrorFile={0} -XX:HeapDumpPath={1}";

        public const string REGISTRY_HOMEKEY_JDK = "SOFTWARE\\JavaSoft\\Java Development Kit";
        public const string REGISTRY_HOMEKEY_JRE = "SOFTWARE\\JavaSoft\\Java Runtime Environment";

        public const string URL_DROPBOX = "https://www.dropbox.com/sh/g6t0h1mk46fh5jq/AAA5s3V9N96qPHsXH45MwtKda";
        public const string URL_VERSIONS_MANIFEST = "http://launchermeta.mojang.com/mc/game/version_manifest.json";
        public const string URL_ASSETS = "http://resources.download.minecraft.net/";
        public const string URL_LIBRARIES = "https://libraries.minecraft.net/";
        public const string URL_AUTHENTICATE = "https://authserver.mojang.com/authenticate";
        public const string URL_REFRESH = "https://authserver.mojang.com/refresh";
        public const string URL_VALIDATE = "https://authserver.mojang.com/validate";
        public const string URL_SIGNOUT = "https://authserver.mojang.com/signout";
        public const string URL_INVALIDATE = "https://authserver.mojang.com/invalidate";
        public const string URL_OFFICIAL_PAGE = "https://www.minecraft.net/";

        public const string FILENAME_SETTINGS_GLOBAL = "launcher_global_settings.xml";
        public const string FILENAME_SETTINGS_COMMON = "launcher_common_settings.xml";
        public const string FILENAME_SETTINGS_VERSIONS = "launcher_versions_settings.xml";
        public const string FILENAME_ICON_MINECRAFT = "MinecraftLogo.ico";
        public const string FILENAME_ICON_JAVA = "JavaLogo.ico";
        public const string FILENAME_ICON_DROPBOX = "DropboxLogo.ico";
        public const string FILENAME_LOG_LAUNCHER_EXCEPTION_F = "launcher_log_exception_{0}.txt";
        public const string FILENAME_LOGS_LAUNCHER_ARCHIVE = "launcher_logs.zip";
        public const string FILENAME_LOG_JAVA_ERROR_F = "hs_err_{0}.log";
        public const string FILENAME_LOG_JAVA_HEAP_DUMP_F = "java_{0}.hprof";

        public const int SLIDE_TIME_MILISECONDS = 330;

        public static readonly string LAUNCHER_VERSION_CURRENT = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public const string LAUNCHER_VERSION_OFFICIAL = "25";
        public const int LAUNCHER_VERSION_FORMAT_OFFICIAL = 25;

        public const string RESPONSE_EMPTY = "#empty_response#";

    }
}
