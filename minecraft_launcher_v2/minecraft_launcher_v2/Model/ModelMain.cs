using Microsoft.WindowsAPICodePack.Taskbar;
using minecraft_launcher_v2.Classes.Controls;
using minecraft_launcher_v2.Classes.Controls.Static;
using minecraft_launcher_v2.ConstantValues;
using minecraft_launcher_v2.CustomStructs;
using minecraft_launcher_v2.Serialization;
using minecraft_launcher_v2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Windows;

namespace minecraft_launcher_v2.Model
{
    class ModelMain
    {
        #region Authentication Control

        internal AuthenticationResult AuthenticationLogIn(SecureString password)
        {
            string response = AuthenticationControl.Authenticate(LauncherProfilesControl.Username, password);

            AuthenticationResult authenticationResult = CheckForErrors(response);

            if (authenticationResult == AuthenticationResult.Success)
            {
                RootAuthenticationResponse responseJson = null;

                try
                {
                    responseJson = JsonConvert.DeserializeObject<RootAuthenticationResponse>(response);
                }
                catch
                { }

                if (responseJson != null && responseJson.selectedProfile != null && !string.IsNullOrWhiteSpace(responseJson.accessToken) &&
                    !string.IsNullOrWhiteSpace(responseJson.clientToken) && !string.IsNullOrWhiteSpace(responseJson.selectedProfile.name))
                {
                    LauncherProfilesControl.DisplayName = responseJson.selectedProfile.name;
                    LauncherProfilesControl.UserId = responseJson.selectedProfile.id;
                    LauncherProfilesControl.ClientToken = responseJson.clientToken;
                    LauncherProfilesControl.AccessToken = responseJson.accessToken;
                    LauncherProfilesControl.UUID = responseJson.clientToken;
                }
                else
                {
                    authenticationResult = AuthenticationResult.UnknownError;
                }
            }

            return authenticationResult;
        }

        internal AuthenticationResult AuthenticationRefresh()
        {
            string response = AuthenticationControl.Refresh(LauncherProfilesControl.AccessToken, LauncherProfilesControl.ClientToken, "", "");

            AuthenticationResult authenticationResult = CheckForErrors(response);

            if (authenticationResult == AuthenticationResult.Success)
            {
                RootRefreshRequestResponse responseJson = null;

                try
                {
                    responseJson = JsonConvert.DeserializeObject<RootRefreshRequestResponse>(response);
                }
                catch
                { }

                if (responseJson != null && responseJson.selectedProfile != null && !string.IsNullOrWhiteSpace(responseJson.accessToken) &&
                    !string.IsNullOrWhiteSpace(responseJson.clientToken) && !string.IsNullOrWhiteSpace(responseJson.selectedProfile.name))
                {
                    LauncherProfilesControl.DisplayName = responseJson.selectedProfile.name;
                    LauncherProfilesControl.UserId = responseJson.selectedProfile.id;
                    LauncherProfilesControl.ClientToken = responseJson.clientToken;
                    LauncherProfilesControl.AccessToken = responseJson.accessToken;
                    LauncherProfilesControl.UUID = responseJson.clientToken;
                }
                else
                {
                    authenticationResult = AuthenticationResult.UnknownError;
                }
            }

            return authenticationResult;
        }

        internal AuthenticationResult AuthenticationSignOut(SecureString password)
        {
            string response = AuthenticationControl.SignOut(LauncherProfilesControl.Username, password);

            AuthenticationResult authenticationResult = CheckForErrors(response);

            if (response == "empty_response")
            {
                authenticationResult = AuthenticationResult.Success;
            }

            return AuthenticationResult.Success;
        }

        internal AuthenticationResult AuthenticationValidateSession()
        {
            string response = AuthenticationControl.Validate(LauncherProfilesControl.AccessToken, LauncherProfilesControl.ClientToken);

            AuthenticationResult authenticationResult = CheckForErrors(response);

            if (response == "empty_response")
            {
                authenticationResult = AuthenticationResult.Success;
            }

            return AuthenticationResult.Success;
        }

        internal AuthenticationResult AuthenticationInvalidateSession()
        {
            string response = AuthenticationControl.Invalidate(LauncherProfilesControl.AccessToken, LauncherProfilesControl.ClientToken);

            AuthenticationResult authenticationResult = CheckForErrors(response);

            if (response == "empty_response")
            {
                authenticationResult = AuthenticationResult.Success;
            }

            return AuthenticationResult.Success;
        }

        internal bool TokensPresent()
        {
            return !string.IsNullOrWhiteSpace(LauncherProfilesControl.AccessToken) && !string.IsNullOrWhiteSpace(LauncherProfilesControl.ClientToken);
        }


        private AuthenticationResult CheckForErrors(string responseString)
        {
            AuthenticationResult authenticationResult = AuthenticationResult.Success;

            if (string.IsNullOrWhiteSpace(responseString))
            {
                authenticationResult = AuthenticationResult.UnknownError;
            }
            else
            {
                RootError errorJson = null;

                try
                {
                    errorJson = JsonConvert.DeserializeObject<RootError>(responseString);
                }
                catch
                {
                    authenticationResult = AuthenticationResult.UnknownError;
                }

                if (errorJson != null && errorJson.error != null)
                {
                    switch (errorJson.error)
                    {
                        case "Not Found":
                            {
                                authenticationResult = AuthenticationResult.NotFound;
                                break;
                            }
                        case "Method Not Allowed":
                            {
                                authenticationResult = AuthenticationResult.MethodNotAllowed;
                                break;
                            }
                        case "ForbiddenOperationException":
                            {
                                if (errorJson.errorMessage == "Invalid credentials. Account migrated, use e-mail as username.")
                                {
                                    authenticationResult = AuthenticationResult.InvalidCredentialsMigrated;
                                }
                                else if (errorJson.errorMessage == "Invalid credentials. Invalid username or password.")
                                {
                                    authenticationResult = AuthenticationResult.InvalidCredentials;
                                }
                                else if (errorJson.errorMessage == "Invalid credentials.")
                                {
                                    authenticationResult = AuthenticationResult.InvalidCredentials;
                                }
                                break;
                            }
                        case "IllegalArgumentException":
                            {
                                if (errorJson.errorMessage == "Access token already has a profile assigned.")
                                {
                                    authenticationResult = AuthenticationResult.AccessTokenHasProfile;
                                }
                                else if (errorJson.errorMessage == "credentials is null")
                                {
                                    authenticationResult = AuthenticationResult.CredentialsIsNull;
                                }
                                break;
                            }
                        case "Unsupported Media Type":
                            {
                                authenticationResult = AuthenticationResult.UnsupportedMediaType;
                                break;
                            }
                        default:
                            {
                                authenticationResult = AuthenticationResult.UnknownError;
                                break;
                            }
                    }
                }
            }

            return authenticationResult;
        }

        #endregion


        #region Versions Control

        public Task<List<ManifestVersion>> ReloadAvailableVersionsAsync()
        {
            return Task.Factory.StartNew(new Func<List<ManifestVersion>>(() => VersionsControl.ReloadAvailableVersions()));
        }

        public Task<List<ManifestVersion>> GetAvailableVersionsAsync()
        {
            return Task.Factory.StartNew(new Func<List<ManifestVersion>>(() => VersionsControl.GetAvailableVersions()));
        }

        public Task<List<string>> UpdateInstalledVersionsAsync()
        {
            return Task.Factory.StartNew(new Func<List<string>>(() =>
            {
                List<string> installedVersions = VersionsControl.GetInstalledVersions().OrderByAlphaNumeric(t => t).ToList();
                SettingsControl.DeleteRestVerionsSettings(installedVersions);
                return installedVersions;
            }));
        }

        #endregion


        #region Download Control

        public void WatchDownloadPercentage(IntPtr windowPointer, Task<bool> downloadTask)
        {
            double percent = 0;

            while (!downloadTask.IsCanceled && !downloadTask.IsCompleted && !downloadTask.IsFaulted)
            {
                percent = DownloadControl.GetInstance().PercentDownloaded;

                if (percent < 1 && !downloadTask.IsCompleted)
                {
                    TaskbarUtils.SetTaskbarItemProgress(windowPointer, TaskbarProgressBarState.Normal, percent);
                }
                else if (!downloadTask.IsCompleted)
                {
                    TaskbarUtils.SetTaskbarItemProgress(windowPointer, TaskbarProgressBarState.Indeterminate, 1.0);
                    break;
                }
            }
        }

        public Task<bool> DownloadVersionAsync(string downloadVersion, IntPtr windowPointer)
        {
            Task<bool> downloadTask = new Task<bool>(new Func<bool>(() =>
            {
                if (!string.IsNullOrWhiteSpace(downloadVersion))
                {
                    return DownloadControl.GetInstance().DownloadVersion(downloadVersion);
                }
                else
                {
                    return false;
                }
            }));

            Task watchPercentageTask = new Task(new Action(() => WatchDownloadPercentage(windowPointer, downloadTask)));
            watchPercentageTask.Start();

            downloadTask.Start();

            return downloadTask;
        }

        #endregion


        #region StartGame Control

        private bool AreSettingsOk()
        {
            if (string.IsNullOrWhiteSpace(LauncherProfilesControl.LastVersionId))
            {
                MessageBox.Show(Messages.ERROR_LAUNCH_VERSION_EMPTY, Messages.CAPTION_COMMON);
            }
            else if (!Singlet.Regex_Memory.IsMatch(SettingsControl.AllocatedMemory))
            {
                MessageBox.Show(Messages.ERROR_RAM_FORMAT_INCORRECT, Messages.CAPTION_COMMON);
            }
            else
            {
                ulong allocatedMemoryBytes;
                char prefix = SettingsControl.AllocatedMemory.ToUpper()[SettingsControl.AllocatedMemory.Length - 1];

                if (prefix == 'M')
                {
                    allocatedMemoryBytes = ulong.Parse(SettingsControl.AllocatedMemory.Substring(0, SettingsControl.AllocatedMemory.Length - 1)) * 1000000;
                }
                else
                {
                    allocatedMemoryBytes = ulong.Parse(SettingsControl.AllocatedMemory.Substring(0, SettingsControl.AllocatedMemory.Length - 1)) * 1000000000;
                }

                if ((prefix == 'M' && allocatedMemoryBytes < Constants.RAM_MINIMUM_START) || allocatedMemoryBytes > Constants.RAM_TOTAL_BYTES)
                {
                    MessageBox.Show(string.Format(Messages.ERROR_RAM_ALLOCATION_SIZE_INCORRECT_F, Constants.RAM_MINIMUM_START / 1048576f), Messages.CAPTION_COMMON);
                }
                else if (allocatedMemoryBytes > CommonUtils.GetFreeRam())
                {
                    MessageBox.Show(Messages.ERROR_RAM_ALLOCATION_INSUFICIENT, Messages.CAPTION_COMMON);
                }
                else if (string.IsNullOrWhiteSpace(LauncherProfilesControl.JavaDirectory) || !JavaUtils.JavaExists(LauncherProfilesControl.JavaDirectory))
                {
                    MessageBox.Show(Messages.ERROR_JAVA_NOT_FOUND, Messages.CAPTION_COMMON);
                }
                else if (string.IsNullOrWhiteSpace(LauncherProfilesControl.Username))
                {
                    MessageBox.Show(Messages.ERROR_NICKNAME_INCORRECT, Messages.CAPTION_COMMON);
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        private Process Launch()
        {
            char quote = '"';

            string use64Bit = "";
            string javaParameters = StartControl.GetInstance().GetStartString(LauncherProfilesControl.LastVersionId);

            uint logNumber = 0;
            string javaLogging = "";
            string javaLogFolder = SettingsControl.MainDirectory + "\\error_logs\\";
            string javaLogError = javaLogFolder + Constants.FILENAME_LOG_JAVA_ERROR_F;
            string javaLogHeapDump = javaLogFolder + Constants.FILENAME_LOG_JAVA_HEAP_DUMP_F;

            if (javaParameters == "")
            {
                MessageBox.Show(Messages.ERROR_INITIALIZATION_VERISON, Messages.CAPTION_COMMON);
                return null;
            }

            if (SettingsControl.UseLicence)
            {
                javaParameters = javaParameters.Replace("${auth_player_name}", quote + LauncherProfilesControl.DisplayName + quote);
            }
            else
            {
                LauncherProfilesControl.UUID = Guid.NewGuid().ToString().ToLower();
                LauncherProfilesControl.AccessToken = LauncherProfilesControl.UUID;
                LauncherProfilesControl.ClientToken = LauncherProfilesControl.UUID;

                javaParameters = javaParameters.Replace("${auth_player_name}", quote + LauncherProfilesControl.Username + quote);
            }

            javaParameters = javaParameters.Replace("${auth_uuid}", quote + LauncherProfilesControl.UserId + quote);
            javaParameters = javaParameters.Replace("${auth_access_token}", quote + LauncherProfilesControl.AccessToken + quote);
            javaParameters = javaParameters.Replace("${auth_session}", quote + LauncherProfilesControl.AccessToken + quote);


            javaParameters = javaParameters.Replace("${version_name}", quote + LauncherProfilesControl.LastVersionId + quote);
            javaParameters = javaParameters.Replace("${user_properties}", "{}");
            javaParameters = javaParameters.Replace("${user_type}", quote + "mojang" + quote);

            if (SettingsControl.Use64Bit && JavaUtils.CanUse64BitJava(LauncherProfilesControl.JavaDirectory))
            {
                use64Bit = " -d64";
            }

            try
            {
                if (!Directory.Exists(javaLogFolder))
                {
                    Directory.CreateDirectory(javaLogFolder);
                }

                while (File.Exists(string.Format(javaLogError, logNumber)) && File.Exists(string.Format(javaLogHeapDump, logNumber)))
                {
                    logNumber++;
                }

                javaLogError = string.Format(javaLogError, logNumber);
                javaLogHeapDump = string.Format(javaLogHeapDump, logNumber);
                javaLogging = string.Format(Constants.JAVA_ARGUMENTS_LOGGING_F, javaLogError, javaLogHeapDump);
            }
            catch
            {
                javaLogging = "";
            }

            if (SettingsControl.UseOptimization)
            {
                javaParameters = string.Format("-Xms{0} -Xmx{0}", SettingsControl.AllocatedMemory) + use64Bit + javaLogging + Constants.JAVA_ARGUMENTS_OPTIMIZATION + " " + javaParameters.Trim();
            }
            else
            {
                javaParameters = string.Format("-Xms512M -Xmx{0}", SettingsControl.AllocatedMemory) + use64Bit + javaLogging + " " + javaParameters.Trim();
            }

            try
            {
                Process minecraft = new Process();
                ProcessStartInfo mcstart = new ProcessStartInfo("CMD.exe");
                string cmdArgs = "/c \"title Minecraft console && start /HIGH /b /wait \"\" \"" + LauncherProfilesControl.JavaDirectory;

                if (SettingsControl.ShowConsole)
                {
                    cmdArgs += Constants.PATH_FILE_JAVA_R + "\" " + javaParameters + quote;

                    if (!SettingsControl.AutoCloseConsole)
                    {
                        cmdArgs += " && @echo. && pause";
                    }
                }
                else
                {
                    cmdArgs += Constants.PATH_FILE_JAVAW_R + "\" " + javaParameters + quote;

                    mcstart.CreateNoWindow = true;
                    mcstart.UseShellExecute = false;
                }

                mcstart.Arguments = cmdArgs;
                minecraft.StartInfo = mcstart;
                minecraft.Start();

                return minecraft;
            }
            catch (Exception ex)
            {
                MessageBox.Show(Messages.ERROR_PROCESS_START_MINECRAFT + ex.Message, Messages.CAPTION_COMMON);

                return null;
            }
        }


        public Task<Process> StartGameAsync()
        {
            return Task.Factory.StartNew(new Func<Process>(() =>
            {
                if (AreSettingsOk())
                {
                    return Launch();
                }
                else
                {
                    return null;
                }
            }));
        }

        #endregion


        #region OSInteraction

        public void BlinkWindow(IntPtr windowHandle)
        {
            if (windowHandle != null)
            {
                try
                {
                    BlinkUtils.Flash(windowHandle);
                }
                catch
                {
                }
            }
        }


        private void AddStandartJumpList(Dictionary<string, List<TaskbarSiteItem>> jumpListItems)
        {
            List<TaskbarSiteItem> jumpListLinks = new List<TaskbarSiteItem>();

            string imagePath = Constants.PATH_GLOBAL_SETTINGS + "\\" + Constants.FILENAME_ICON_MINECRAFT;
            if (!File.Exists(imagePath))
            {
                CommonUtils.WriteResourceToFile(Constants.FILENAME_ICON_MINECRAFT, imagePath);
            }
            jumpListLinks.Add(new TaskbarSiteItem(Constants.URL_OFFICIAL_PAGE, Inscriptions.TASKBAR_OFFICIAL_PAGE, imagePath, 0));


            imagePath = Constants.PATH_GLOBAL_SETTINGS + "\\" + Constants.FILENAME_ICON_DROPBOX;
            if (!File.Exists(imagePath))
            {
                CommonUtils.WriteResourceToFile(Constants.FILENAME_ICON_DROPBOX, imagePath);
            }
            jumpListLinks.Add(new TaskbarSiteItem(Constants.URL_DROPBOX, Inscriptions.TASKBAR_UPDATE_PAGE, imagePath, 0));
            jumpListItems.Add(Inscriptions.TASKBAR_GROUP_WEBSITES, jumpListLinks);


            jumpListLinks = new List<TaskbarSiteItem>();
            jumpListLinks.Add(new TaskbarSiteItem(Constants.PATH_GLOBAL_SETTINGS, Inscriptions.TASKBAR_GLOBAL_SETTINGS, "", 0));
            jumpListLinks.Add(new TaskbarSiteItem(SettingsControl.MainDirectory, Inscriptions.TASKBAR_MAIN_DIRECTORY, "", 0));


            imagePath = Constants.PATH_GLOBAL_SETTINGS + "\\" + Constants.FILENAME_ICON_JAVA;
            if (!File.Exists(imagePath))
            {
                CommonUtils.WriteResourceToFile(Constants.FILENAME_ICON_JAVA, imagePath);
            }
            jumpListLinks.Add(new TaskbarSiteItem(LauncherProfilesControl.JavaDirectory, Inscriptions.TASKBAR_JAVA_DIRECTORY, imagePath, 0));
            jumpListItems.Add(Inscriptions.TASKBAR_GROUP_FOLDERS, jumpListLinks);
        }

        public Task SetTaskbarJumpListAsync(IntPtr windowPointer, Dictionary<string, List<TaskbarSiteItem>> additionalJumpListItems)
        {
            return Task.Factory.StartNew(new Action(() =>
            {
                Dictionary<string, List<TaskbarSiteItem>> jumpListItems = new Dictionary<string, List<TaskbarSiteItem>>();

                AddStandartJumpList(jumpListItems);


                if (additionalJumpListItems != null && additionalJumpListItems.Count > 0)
                {
                    foreach (var item in additionalJumpListItems)
                    {
                        if (jumpListItems.ContainsKey(item.Key))
                        {
                            jumpListItems[item.Key].AddRange(additionalJumpListItems[item.Key]);
                        }
                        else
                        {
                            jumpListItems.Add(item.Key, item.Value);
                        }
                    }
                }

                TaskbarUtils.SetTaskbarJumpListLink(windowPointer, jumpListItems);
            }));
        }

        #endregion


        public Task<string> CheckForUpdatesAsync()
        {
            return Task.Factory.StartNew(new Func<string>(() => CommonUtils.GetLauncherUpdates()));
        }

        private void StopDownloads()
        {
            DownloadControl.GetInstance().StopAllDownloads();
            StartControl.GetInstance().StopAllDownloads();
        }

    }
}
