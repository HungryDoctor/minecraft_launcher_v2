using minecraft_launcher_v2.ConstantValues;
using minecraft_launcher_v2.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace minecraft_launcher_v2.Classes.Controls.Static
{
    static class SettingsControl
    {
        public static event Action<string> MainDirectoryChangedEvent;


        #region Global Settings Properties

        public static string MainDirectory
        {
            get
            {
                return GlobalSettings.MainDirectory;
            }
            set
            {
                GlobalSettings.MainDirectory = value;

                if (MainDirectoryChangedEvent != null)
                {
                    MainDirectoryChangedEvent.Invoke(value);
                }
            }
        }
        public static DateTime RemindUpdateNotificationDate
        {
            get
            {
                return GlobalSettings.RemindUpdateNotificationDate;
            }
            set
            {
                GlobalSettings.RemindUpdateNotificationDate = value;
            }
        }

        #endregion


        #region Common Settings Properties

        public static bool ShowBeta
        {
            get
            {
                return CommonSettings.ShowBeta;
            }
            set
            {
                CommonSettings.ShowBeta = value;
            }
        }
        public static bool AutoUpdateAvailable
        {
            get
            {
                return CommonSettings.AutoUpdateAvailable;
            }
            set
            {
                CommonSettings.AutoUpdateAvailable = value;
            }
        }
        public static bool LoggedIn
        {
            get
            {
                return CommonSettings.LoggedIn;
            }
            set
            {
                CommonSettings.LoggedIn = value;
            }
        }

        #endregion


        #region Versions Settings Properties

        public static string AllocatedMemory
        {
            get
            {
                return VersionsSettings.AllocatedMemory;
            }
            set
            {
                VersionsSettings.AllocatedMemory = value;
            }
        }
        public static bool UseLicence
        {
            get
            {
                return VersionsSettings.UseLicence;
            }
            set
            {
                VersionsSettings.UseLicence = value;
            }
        }
        public static bool ShowConsole
        {
            get
            {
                return VersionsSettings.ShowConsole;
            }
            set
            {
                VersionsSettings.ShowConsole = value;
            }
        }
        public static bool AutoCloseConsole
        {
            get
            {
                return VersionsSettings.AutoCloseConsole;
            }
            set
            {
                VersionsSettings.AutoCloseConsole = value;
            }
        }
        public static bool AutoCloseLauncher
        {
            get
            {
                return VersionsSettings.AutoCloseLauncher;
            }
            set
            {
                VersionsSettings.AutoCloseLauncher = value;
            }
        }
        public static bool FindJava
        {
            get
            {
                return VersionsSettings.FindJava;
            }
            set
            {
                VersionsSettings.FindJava = value;
            }
        }
        public static bool UseOptimization
        {
            get
            {
                return VersionsSettings.UseOptimization;
            }
            set
            {
                VersionsSettings.UseOptimization = value;
            }
        }
        public static bool Use64Bit
        {
            get
            {
                return VersionsSettings.Use64Bit;
            }
            set
            {
                VersionsSettings.Use64Bit = value;
            }
        }

        #endregion



        #region Outer Interactions

        public static void LoadGlobalSettings()
        {
            GlobalSettings.LoadGlobalSettings();
        }

        public static void SaveGlobalSettings()
        {
            GlobalSettings.SaveGlobalSettings();
        }


        public static void LoadCommonSettings()
        {
            CommonSettings.LoadCommonSettings();
        }

        public static void SaveCommonSettings()
        {
            CommonSettings.SaveCommonSettings();
        }

        public static void ResetCommonSettings()
        {
            CommonSettings.ResetCommonSettings();
        }


        public static void SetCurrentVersionSettings(string version)
        {
            VersionsSettings.CurrentVersion = version;
        }

        public static void LoadVersionsSettings()
        {
            VersionsSettings.LoadVersionsSettings();
        }

        public static void SaveVersionsSettings()
        {
            VersionsSettings.SaveVersionsSettings();
        }

        public static void ResetVersionsSettings()
        {
            VersionsSettings.ResetCurrentVersionSettings();
        }

        public static void DeleteRestVerionsSettings(List<string> versions)
        {
            VersionsSettings.DeleteRestVerionsSettings(versions);
        }

        #endregion



        private static class GlobalSettings
        {
            public static string MainDirectory { get { return xmlGlobalSettings.MainDirectory; } set { xmlGlobalSettings.MainDirectory = value; } }
            public static DateTime RemindUpdateNotificationDate
            {
                get
                {
                    try { return DateTime.Parse(xmlGlobalSettings.RemindUpdateNotificationDate); }
                    catch
                    {
                        xmlGlobalSettings.RemindUpdateNotificationDate = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
                        return DateTime.MinValue;
                    }
                }
                set { xmlGlobalSettings.RemindUpdateNotificationDate = value.ToString("yyyy-MM-dd HH:mm:ss"); }
            }

            private static RootSettingsGlobal xmlGlobalSettings;
            private static XmlSerializer xmlSerializer;



            static GlobalSettings()
            {
                xmlGlobalSettings = new RootSettingsGlobal();
                xmlSerializer = XmlSerializer.FromTypes(new[] { typeof(RootSettingsGlobal) })[0];
            }



            public static void LoadGlobalSettings()
            {
                string fullPath = Constants.PATH_GLOBAL_SETTINGS + "\\" + Constants.FILENAME_SETTINGS_GLOBAL;

                if (!Directory.Exists(Constants.PATH_GLOBAL_SETTINGS))
                {
                    Directory.CreateDirectory(Constants.PATH_GLOBAL_SETTINGS);

                    ResetGlobalSettings();
                    SaveGlobalSettings();
                }
                else if (!File.Exists(fullPath))
                {
                    ResetGlobalSettings();
                    SaveGlobalSettings();
                }
                else if (File.Exists(fullPath))
                {
                    try
                    {
                        using (FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            using (StreamReader reader = new StreamReader(fileStream, Encoding.Unicode))
                            {
                                xmlGlobalSettings = (RootSettingsGlobal)xmlSerializer.Deserialize(reader);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Messages.ERROR_LOAD_SETTINGS_GLOBAL + ex.Message, Messages.CAPTION_COMMON);
                        ResetGlobalSettings();
                        SaveGlobalSettings();
                    }
                }
            }

            public static void SaveGlobalSettings()
            {
                string fullPath = Constants.PATH_GLOBAL_SETTINGS + "\\" + Constants.FILENAME_SETTINGS_GLOBAL;

                if (!Directory.Exists(Constants.PATH_GLOBAL_SETTINGS))
                {
                    Directory.CreateDirectory(Constants.PATH_GLOBAL_SETTINGS);
                }

                try
                {
                    using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        using (StreamWriter writer = new StreamWriter(fileStream, Encoding.Unicode))
                        {
                            xmlSerializer.Serialize(writer, xmlGlobalSettings);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Messages.ERROR_SAVE_SETTINGS_GLOBAL + ex.Message, Messages.CAPTION_COMMON);
                }
            }

            public static void ResetGlobalSettings()
            {
                RemindUpdateNotificationDate = DefaultSettings.GLOBAL_REMIND_UPDATE_NOTIFICATION_DATE;
            }

        }


        private static class CommonSettings
        {
            private static RootSettingsCommon xmlComonSettings;
            private static XmlSerializer xmlSerializer;


            public static bool ShowBeta
            {
                get
                {
                    return xmlComonSettings.ShowBeta;
                }
                set
                {
                    xmlComonSettings.ShowBeta = value;
                }
            }
            public static bool AutoUpdateAvailable
            {
                get
                {
                    return xmlComonSettings.AutoUpdateAvailable;
                }
                set
                {
                    xmlComonSettings.AutoUpdateAvailable = value;
                }
            }
            public static bool LoggedIn
            {
                get
                {
                    return xmlComonSettings.LoggedIn;
                }
                set
                {
                    xmlComonSettings.LoggedIn = value;
                }
            }



            static CommonSettings()
            {
                xmlComonSettings = new RootSettingsCommon();
                xmlSerializer = XmlSerializer.FromTypes(new[] { typeof(RootSettingsCommon) })[0];
            }



            public static void LoadCommonSettings()
            {
                string fullPath = GlobalSettings.MainDirectory + "\\" + Constants.FILENAME_SETTINGS_COMMON;
                string folderPath = fullPath.Substring(0, fullPath.LastIndexOf('\\'));

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);

                    ResetCommonSettings();
                    SaveCommonSettings();
                }
                else if (!File.Exists(fullPath))
                {
                    ResetCommonSettings();
                    SaveCommonSettings();
                }
                else if (File.Exists(fullPath))
                {
                    try
                    {
                        using (FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            using (StreamReader reader = new StreamReader(fileStream, Encoding.Unicode))
                            {
                                xmlComonSettings = (RootSettingsCommon)xmlSerializer.Deserialize(reader);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Messages.ERROR_LOAD_SETTINGS_COMMON + ex.Message, Messages.CAPTION_COMMON);
                        ResetCommonSettings();
                        SaveCommonSettings();
                    }
                }
            }

            public static void SaveCommonSettings()
            {
                string fullPath = GlobalSettings.MainDirectory + "\\" + Constants.FILENAME_SETTINGS_COMMON;
                string folderPath = fullPath.Substring(0, fullPath.LastIndexOf('\\'));

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                try
                {
                    using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        using (StreamWriter writer = new StreamWriter(fileStream, Encoding.Unicode))
                        {
                            xmlSerializer.Serialize(writer, xmlComonSettings);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Messages.ERROR_SAVE_SETTINGS_COMMON + ex.Message, Messages.CAPTION_COMMON);
                }
            }

            public static void ResetCommonSettings()
            {
                xmlComonSettings.ShowBeta = DefaultSettings.COMMON_SHOW_BETA;
                xmlComonSettings.AutoUpdateAvailable = DefaultSettings.COMMON_AUTO_UPDATE_AVAILABLE;
                xmlComonSettings.LoggedIn = DefaultSettings.COMMON_LOGGED_IN;
            }

        }


        private static class VersionsSettings
        {
            private static RootSettingsVersions xmlVersionsSettings;
            private static DataContractSerializer xmlSerializer;
            private static string currentVersion;


            #region Properties

            public static string CurrentVersion
            {
                get
                {
                    return currentVersion;
                }
                set
                {
                    currentVersion = value;
                    CheckDictionaryState();
                }
            }
            public static string AllocatedMemory
            {
                get
                {
                    CheckDictionaryState();

                    if (string.IsNullOrWhiteSpace(CurrentVersion))
                    {
                        return DefaultSettings.VERSION_ALLOCATED_MEMORY;
                    }
                    else
                    {
                        return xmlVersionsSettings.VersionsSettings[CurrentVersion].AllocatedMemory;
                    }
                }
                set
                {
                    if (!string.IsNullOrWhiteSpace(CurrentVersion))
                    {
                        xmlVersionsSettings.VersionsSettings[CurrentVersion].AllocatedMemory = value;
                    }
                }
            }
            public static bool UseLicence
            {
                get
                {
                    CheckDictionaryState();

                    if (string.IsNullOrWhiteSpace(CurrentVersion))
                    {
                        return DefaultSettings.VERSION_USE_LICENCE;
                    }
                    else
                    {
                        return xmlVersionsSettings.VersionsSettings[CurrentVersion].UseLicence;
                    }
                }
                set
                {
                    if (!string.IsNullOrWhiteSpace(CurrentVersion))
                    {

                        xmlVersionsSettings.VersionsSettings[CurrentVersion].UseLicence = value;
                    }
                }
            }
            public static bool ShowConsole
            {
                get
                {
                    CheckDictionaryState();

                    if (string.IsNullOrWhiteSpace(CurrentVersion))
                    {
                        return DefaultSettings.VERSION_SHOW_CONSOLE;
                    }
                    else
                    {
                        return xmlVersionsSettings.VersionsSettings[CurrentVersion].ShowConsole;
                    }
                }
                set
                {
                    if (!string.IsNullOrWhiteSpace(CurrentVersion))
                    {
                        xmlVersionsSettings.VersionsSettings[CurrentVersion].ShowConsole = value;
                    }
                }
            }
            public static bool AutoCloseConsole
            {
                get
                {
                    CheckDictionaryState();

                    if (string.IsNullOrWhiteSpace(CurrentVersion))
                    {
                        return DefaultSettings.VERSION_AUTO_CLOSE_CONSOLE;
                    }
                    else
                    {
                        return xmlVersionsSettings.VersionsSettings[CurrentVersion].AutoCloseConsole;
                    }
                }
                set
                {
                    if (!string.IsNullOrWhiteSpace(CurrentVersion))
                    {
                        xmlVersionsSettings.VersionsSettings[CurrentVersion].AutoCloseConsole = value;
                    }
                }
            }
            public static bool AutoCloseLauncher
            {
                get
                {
                    CheckDictionaryState();

                    if (string.IsNullOrWhiteSpace(CurrentVersion))
                    {
                        return DefaultSettings.VERSION_AUTO_CLOSE_LAUNCHER;
                    }
                    else
                    {
                        return xmlVersionsSettings.VersionsSettings[CurrentVersion].AutoCloseLauncher;
                    }
                }
                set
                {
                    if (!string.IsNullOrWhiteSpace(CurrentVersion))
                    {
                        xmlVersionsSettings.VersionsSettings[CurrentVersion].AutoCloseLauncher = value;
                    }
                }
            }
            public static bool FindJava
            {
                get
                {
                    CheckDictionaryState();

                    if (string.IsNullOrWhiteSpace(CurrentVersion))
                    {
                        return DefaultSettings.VERSION_FIND_JAVA;
                    }
                    else
                    {
                        return xmlVersionsSettings.VersionsSettings[CurrentVersion].FindJava;
                    }
                }
                set
                {
                    if (!string.IsNullOrWhiteSpace(CurrentVersion))
                    {
                        xmlVersionsSettings.VersionsSettings[CurrentVersion].FindJava = value;
                    }
                }
            }
            public static bool UseOptimization
            {
                get
                {
                    CheckDictionaryState();

                    if (string.IsNullOrWhiteSpace(CurrentVersion))
                    {
                        return DefaultSettings.VERSION_USE_OPTIMIZATION;
                    }
                    else
                    {
                        return xmlVersionsSettings.VersionsSettings[CurrentVersion].UseOptimization;
                    }
                }
                set
                {
                    if (!string.IsNullOrWhiteSpace(CurrentVersion))
                    {
                        xmlVersionsSettings.VersionsSettings[CurrentVersion].UseOptimization = value;
                    }
                }
            }
            public static bool Use64Bit
            {
                get
                {
                    CheckDictionaryState();

                    if (string.IsNullOrWhiteSpace(CurrentVersion))
                    {
                        return DefaultSettings.VERSION_USE_64_BIT;
                    }
                    else
                    {
                        return xmlVersionsSettings.VersionsSettings[CurrentVersion].Use64Bit;
                    }
                }
                set
                {
                    if (!string.IsNullOrWhiteSpace(CurrentVersion))
                    {
                        xmlVersionsSettings.VersionsSettings[CurrentVersion].Use64Bit = value;
                    }
                }
            }

            private static void CheckDictionaryState()
            {
                if (xmlVersionsSettings == null)
                {
                    xmlVersionsSettings = new RootSettingsVersions();
                }

                if (xmlVersionsSettings.VersionsSettings == null)
                {
                    xmlVersionsSettings.VersionsSettings = new Dictionary<string, VersionSettings>();
                }

                if (!string.IsNullOrWhiteSpace(CurrentVersion) && !xmlVersionsSettings.VersionsSettings.ContainsKey(CurrentVersion))
                {
                    ResetCurrentVersionSettings();
                }
            }

            #endregion



            static VersionsSettings()
            {
                xmlVersionsSettings = new RootSettingsVersions();
                xmlSerializer = new DataContractSerializer(typeof(RootSettingsVersions));
            }



            public static void LoadVersionsSettings()
            {
                string fullPath = GlobalSettings.MainDirectory + "\\" + Constants.FILENAME_SETTINGS_VERSIONS;
                string folderPath = fullPath.Substring(0, fullPath.LastIndexOf('\\'));

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);

                    SaveVersionsSettings();
                }
                else if (!File.Exists(fullPath))
                {
                    SaveVersionsSettings();
                }
                else if (File.Exists(fullPath))
                {
                    try
                    {
                        using (FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            using (XmlDictionaryReader dictionaryReader = XmlDictionaryReader.CreateTextReader(fileStream, Encoding.UTF8, new XmlDictionaryReaderQuotas(), null))
                            {
                                xmlVersionsSettings = (RootSettingsVersions)xmlSerializer.ReadObject(dictionaryReader);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Messages.ERROR_LOAD_SETTINGS_VERSION + ex.Message, Messages.CAPTION_COMMON);
                    }
                }
            }

            public static void SaveVersionsSettings()
            {
                string fullPath = GlobalSettings.MainDirectory + "\\" + Constants.FILENAME_SETTINGS_VERSIONS;
                string folderPath = fullPath.Substring(0, fullPath.LastIndexOf('\\'));

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                try
                {
                    using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        using (XmlDictionaryWriter dictionaryWriter = XmlDictionaryWriter.CreateTextWriter(fileStream, Encoding.UTF8, false))
                        {
                            xmlSerializer.WriteObject(dictionaryWriter, xmlVersionsSettings);

                            dictionaryWriter.Flush();
                            dictionaryWriter.Close();
                        }

                        fileStream.Flush();
                        fileStream.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Messages.ERROR_SAVE_SETTINGS_VERSION + ex.Message, Messages.CAPTION_COMMON);
                }
            }

            public static void ResetCurrentVersionSettings()
            {
                if (!string.IsNullOrWhiteSpace(CurrentVersion))
                {
                    if (!xmlVersionsSettings.VersionsSettings.ContainsKey(CurrentVersion))
                    {
                        VersionSettings settings = new VersionSettings();
                        ResetVersion(settings);

                        xmlVersionsSettings.VersionsSettings.Add(CurrentVersion, settings);
                    }
                    else
                    {
                        ResetVersion(xmlVersionsSettings.VersionsSettings[CurrentVersion]);
                    }
                }
            }


            private static void ResetVersion(VersionSettings settings)
            {
                settings.AllocatedMemory = DefaultSettings.VERSION_ALLOCATED_MEMORY;
                settings.UseLicence = DefaultSettings.VERSION_USE_LICENCE;
                settings.ShowConsole = DefaultSettings.VERSION_SHOW_CONSOLE;
                settings.AutoCloseConsole = DefaultSettings.VERSION_AUTO_CLOSE_CONSOLE;
                settings.AutoCloseLauncher = DefaultSettings.VERSION_AUTO_CLOSE_LAUNCHER;
                settings.FindJava = DefaultSettings.VERSION_FIND_JAVA;
                settings.UseOptimization = DefaultSettings.VERSION_USE_OPTIMIZATION;
                settings.Use64Bit = DefaultSettings.VERSION_USE_64_BIT;
            }

            public static void DeleteRestVerionsSettings(List<string> versions)
            {
                if (!string.IsNullOrWhiteSpace(CurrentVersion))
                {
                    var allKeys = xmlVersionsSettings.VersionsSettings.Keys.ToList();
                    for (int x = allKeys.Count - 1; x > 0; x--)
                    {
                        if (!versions.Contains(allKeys[x]))
                        {
                            xmlVersionsSettings.VersionsSettings.Remove(allKeys[x]);
                        }
                    }
                }
            }

        }

    }
}

