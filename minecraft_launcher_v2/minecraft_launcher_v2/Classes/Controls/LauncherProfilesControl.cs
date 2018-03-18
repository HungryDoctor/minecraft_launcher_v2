using minecraft_launcher_v2.ConstantValues;
using minecraft_launcher_v2.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace minecraft_launcher_v2.Classes.Controls
{
    static class LauncherProfilesControl
    {
        public static string JavaArguments { get; set; }
        public static string JavaDirectory { get; set; }
        public static string LastVersionId { get; set; }
        public static string ProfileName { get; set; }
        public static string SelectedProfile { get; set; }
        public static string SelectedUser { get; set; }

        public static string AccessToken { get; set; }
        public static string ClientToken { get; set; }
        public static string DisplayName { get; set; }
        public static string UserId { get; set; }
        public static string Username { get; set; }
        public static string UUID { get; set; }



        public static void SaveProfile()
        {
            string mainDir = SettingsControl.MainDirectory;
            if (!Directory.Exists(mainDir))
            {
                Directory.CreateDirectory(mainDir);
            }

            RootLauncherProfiles rootLauncherProfiles = new RootLauncherProfiles();
            LauncherVersion launcherVersion = new LauncherVersion();
            AuthenticationDatabase authenticationDatabase = new AuthenticationDatabase();
            Profile profile = new Profile();

            profile.javaArgs = JavaArguments;
            profile.javaDir = JavaDirectory;
            profile.lastVersionId = LastVersionId;
            profile.name = ProfileName;
            profile.gameDir = mainDir;

            rootLauncherProfiles.profiles = new Dictionary<string, Profile>(1);
            rootLauncherProfiles.profiles.Add(ProfileName, profile);


            authenticationDatabase.accessToken = AccessToken;
            authenticationDatabase.displayName = DisplayName;
            authenticationDatabase.userid = UserId;
            authenticationDatabase.username = Username;
            authenticationDatabase.uuid = UUID;

            rootLauncherProfiles.authenticationDatabase = new Dictionary<string, AuthenticationDatabase>(1);
            rootLauncherProfiles.authenticationDatabase.Add(SelectedUser, authenticationDatabase);


            rootLauncherProfiles.launcherVersion = launcherVersion;
            rootLauncherProfiles.selectedUser = SelectedUser;
            rootLauncherProfiles.clientToken = ClientToken;

            if (!string.IsNullOrWhiteSpace(SelectedProfile))
            {
                rootLauncherProfiles.selectedProfile = SelectedProfile;
            }
            else
            {
                rootLauncherProfiles.selectedProfile = LastVersionId;
            }

            launcherVersion.name = Constants.LAUNCHER_VERSION_OFFICIAL;
            launcherVersion.format = Constants.LAUNCHER_VERSION_FORMAT_OFFICIAL;

            try
            {
                using (FileStream fileStream = new FileStream(mainDir + "\\launcher_profiles.json", FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (StreamWriter writer = new StreamWriter(fileStream, Encoding.ASCII))
                    {
                        writer.Write(JsonConvert.SerializeObject(rootLauncherProfiles, Formatting.Indented));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Messages.ERROR_SAVE_SETTINGS_PROFILE + ex.Message, Messages.CAPTION_COMMON);
            }
        }

        public static void LoadProfile()
        {
            string mainDir = SettingsControl.MainDirectory;
            if (!Directory.Exists(mainDir))
            {
                Directory.CreateDirectory(mainDir);

                ResetProfile();
                SaveProfile();
            }
            else if (!File.Exists(mainDir + "\\launcher_profiles.json"))
            {
                ResetProfile();
                SaveProfile();
            }
            else if (File.Exists(mainDir + "\\launcher_profiles.json"))
            {
                try
                {
                    string input = "";

                    using (FileStream fileStream = new FileStream(mainDir + "\\launcher_profiles.json", FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (StreamReader reader = new StreamReader(fileStream, Encoding.ASCII))
                        {
                            input = reader.ReadToEnd();
                        }
                    }

                    RootLauncherProfiles profilesJson = JsonConvert.DeserializeObject<RootLauncherProfiles>(input);

                    if (profilesJson.selectedProfile != null)
                    {
                        SelectedProfile = profilesJson.selectedProfile;
                    }

                    if (profilesJson.profiles != null)
                    {
                        var profilesArray = profilesJson.profiles.ToArray();
                        if (profilesArray.Count() > 0)
                        {
                            bool foundSelectedProfile = false;

                            foreach (var item in profilesArray)
                            {
                                if (item.Key == SelectedProfile)
                                {
                                    Profile profile = item.Value;

                                    JavaArguments = profile.javaArgs;
                                    JavaDirectory = profile.javaDir;
                                    LastVersionId = profile.lastVersionId;
                                    ProfileName = profile.name;
                                    break;
                                }
                            }


                            if (!foundSelectedProfile)
                            {
                                Profile profile = profilesArray[0].Value;

                                JavaArguments = profile.javaArgs;
                                JavaDirectory = profile.javaDir;
                                LastVersionId = profile.lastVersionId;
                                ProfileName = profile.name;
                            }
                        }
                        else
                        {
                            JavaArguments = DefaultSettings.PROFILE_JAVA_ARGUMENTS;
                            JavaDirectory = DefaultSettings.PROFILE_JAVA_DIRECTORY;
                            LastVersionId = DefaultSettings.PROFILE_LAST_VERSION_ID;
                            ProfileName = DefaultSettings.PROFILE_PROFILE_NAME;
                        }
                    }

                    if (profilesJson.authenticationDatabase != null)
                    {
                        var authenticationDatabaseArray = profilesJson.authenticationDatabase.ToArray();
                        if (authenticationDatabaseArray.Count() > 0)
                        {
                            AuthenticationDatabase authenticationDatabase = authenticationDatabaseArray[0].Value;

                            AccessToken = authenticationDatabase.accessToken;
                            DisplayName = authenticationDatabase.displayName;
                            UserId = authenticationDatabase.userid;
                            Username = authenticationDatabase.username;
                            UUID = authenticationDatabase.uuid;
                            ClientToken = profilesJson.clientToken;
                            SelectedUser = authenticationDatabaseArray[0].Key;
                        }
                        else
                        {
                            AccessToken = DefaultSettings.PROFILE_ACCESS_TOKEN;
                            ClientToken = DefaultSettings.PROFILE_CLIENT_TOKEN;
                            DisplayName = DefaultSettings.PROFILE_DISPLAY_NAME;
                            UserId = DefaultSettings.PROFILE_USER_ID;
                            Username = DefaultSettings.PROFILE_USERNAME;
                            UUID = DefaultSettings.PROFILE_UUID;
                            SelectedUser = DefaultSettings.PROFILE_SELECTED_USER;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Messages.ERROR_LOAD_SETTINGS_PROFILE + ex.Message, Messages.CAPTION_COMMON);
                    ResetProfile();
                    SaveProfile();
                }
            }
        }

        public static void ResetProfile()
        {
            JavaArguments = DefaultSettings.PROFILE_JAVA_ARGUMENTS;
            JavaDirectory = DefaultSettings.PROFILE_JAVA_DIRECTORY;
            LastVersionId = DefaultSettings.PROFILE_LAST_VERSION_ID;
            ProfileName = DefaultSettings.PROFILE_PROFILE_NAME;
            SelectedUser = DefaultSettings.PROFILE_SELECTED_USER;

            AccessToken = DefaultSettings.PROFILE_ACCESS_TOKEN;
            ClientToken = DefaultSettings.PROFILE_CLIENT_TOKEN;
            DisplayName = DefaultSettings.PROFILE_DISPLAY_NAME;
            UserId = DefaultSettings.PROFILE_USER_ID;
            Username = DefaultSettings.PROFILE_USERNAME;
            UUID = DefaultSettings.PROFILE_UUID;
        }

    }
}
