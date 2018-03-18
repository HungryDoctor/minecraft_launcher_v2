using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace minecraft_launcher_v2.Serialization
{
    [XmlRoot("Launcher_Global_Settings")]
    public class RootSettingsGlobal
    {
        public string MainDirectory { get; set; }
        public string RemindUpdateNotificationDate { get; set; }
    }


    [XmlRoot("Launcher_Common_Settings")]
    public class RootSettingsCommon
    {
        public bool AutoUpdateAvailable { get; set; }
        public bool ShowBeta { get; set; }
        public bool LoggedIn { get; set; }
    }


    [DataContract]
    public class VersionSettings
    {
        [DataMember]
        public string AllocatedMemory { get; set; }

        [DataMember]
        public bool UseLicence { get; set; }

        [DataMember]
        public bool ShowConsole { get; set; }

        [DataMember]
        public bool AutoCloseConsole { get; set; }

        [DataMember]
        public bool AutoCloseLauncher { get; set; }

        [DataMember]
        public bool FindJava { get; set; }

        [DataMember]
        public bool UseOptimization { get; set; }

        [DataMember]
        public bool Use64Bit { get; set; }
    }

    [DataContract(Name = "Launcher_Versions_Setting")]
    public class RootSettingsVersions
    {
        [DataMember]
        public Dictionary<string, VersionSettings> VersionsSettings { get; set; }
    }

}
