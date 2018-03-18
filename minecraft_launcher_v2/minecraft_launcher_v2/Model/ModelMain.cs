using minecraft_launcher_v2.Classes.Controls;
using minecraft_launcher_v2.ConstantValues;
using minecraft_launcher_v2.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace minecraft_launcher_v2.Model
{
    class ModelMain
    {

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
            string javaParameters = StartControl.GetStartString(LauncherProfilesControl.LastVersionId);

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

            if (SettingsControl.Use64Bit && JavaUtils.CanUse64BitJava())
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
                ProcessStartInfo mcstart = null;

                if (SettingsControl.ShowConsole)
                {
                    string cmdParams = "/c \"title Minecraft console && start /HIGH /b /wait \"\" \"" + LauncherProfilesControl.JavaDirectory + "\\bin\\java.exe" + "\" " + javaParameters + quote;
                    if (!SettingsControl.AutoCloseConsole)
                    {
                        cmdParams += " && @echo. && pause";
                    }

                    mcstart = new ProcessStartInfo("CMD.exe", cmdParams);
                }
                else
                {
                    mcstart = new ProcessStartInfo("CMD.exe", "/c start /HIGH /b /wait \"\" \"" + LauncherProfilesControl.JavaDirectory + "\\bin\\javaw.exe" + "\" " + javaParameters + quote);
                    mcstart.CreateNoWindow = true;
                    mcstart.UseShellExecute = false;
                }

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
            Task<Process> task = new Task<Process>(new Func<Process>(() =>
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

            task.Start();

            return task;
        }

        #endregion

        public Task<string> CheckForUpdatesAsync()
        {
            Task<string> t = new Task<string>(new Func<string>(() =>
            {
                return CommonUtils.GetLauncherUpdates();
            }));
            t.Start();

            return t;
        }

    }
}
