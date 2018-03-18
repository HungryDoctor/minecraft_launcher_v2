using minecraft_launcher_v2.ConstantValues;
using minecraft_launcher_v2.Serialization;
using minecraft_launcher_v2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace minecraft_launcher_v2.Classes.Controls
{
    static class VersionsControl
    {
        public static List<string> GetInstalledVersions()
        {
            string versions = SettingsControl.MainDirectory + "\\versions\\";
            if (Directory.Exists(versions))
            {
                List<string> installedVersions = null;
                try
                {
                    installedVersions = new List<string>(30);
                    DirectoryInfo versionPath = new DirectoryInfo(versions);
                    foreach (var file in versionPath.GetFiles("*.json", SearchOption.AllDirectories))
                    {
                        string folderName = file.Name.Replace(".json", "");
                        if ((file.DirectoryName.Equals(versions + folderName)) && (File.Exists(versions + folderName + "\\" + file.Name)) && folderName != null)
                        {
                            installedVersions.Add(folderName);
                        }
                    }
                    installedVersions.Capacity = installedVersions.Count;

                    return installedVersions;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Messages.ERROR_GET_INSTALLED_VERSIONS + ex.Message, Messages.CAPTION_COMMON);

                    if (installedVersions != null && installedVersions.Count > 0)
                    {
                        installedVersions.Capacity = installedVersions.Count;
                        return installedVersions;
                    }
                    else
                    {
                        return new List<string>();
                    }
                }
            }
            else
            {
                return new List<string>();
            }
        }


        public static List<ManifestVersion> ReloadAvailableVersions()
        {
            try
            {
                string mainDir = SettingsControl.MainDirectory;
                string content;
                using (StreamReader reader = new StreamReader(DownloadUtils.DownloadToStream(Constants.URL_VERSIONS_MANIFEST)))
                {
                    content = reader.ReadToEnd();
                }

                RootVersionsManifest versionsManifestJsnon = JsonConvert.DeserializeObject<RootVersionsManifest>(content);


                if (!Directory.Exists(mainDir))
                {
                    Directory.CreateDirectory(mainDir);
                }

                if (!File.Exists(mainDir + "\\version_manifest.json"))
                {
                    WriteVersionsManifest(JsonConvert.SerializeObject(versionsManifestJsnon, Formatting.Indented));
                }
                else
                {
                    string input = "";

                    using (FileStream fileStream = new FileStream(mainDir + "\\version_manifest.json", FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (StreamReader reader = new StreamReader(fileStream, Encoding.ASCII))
                        {
                            input = reader.ReadToEnd();
                        }
                    }

                    if (versionsManifestJsnon != JsonConvert.DeserializeObject<RootVersionsManifest>(input))
                    {
                        WriteVersionsManifest(JsonConvert.SerializeObject(versionsManifestJsnon, Formatting.Indented));
                    }
                }

                List<ManifestVersion> allVersions = versionsManifestJsnon.versions;
                allVersions.Reverse();

                return allVersions;
            }
            catch
            {
                return new List<ManifestVersion>();
            }
        }

        public static void WriteVersionsManifest(string output)
        {
            using (FileStream fileStream = new FileStream(SettingsControl.MainDirectory + "\\version_manifest.json", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter writer = new StreamWriter(fileStream, Encoding.ASCII))
                {
                    writer.Write(output);
                }
            }
        }

        public static List<ManifestVersion> GetAvailableVersions()
        {
            List<ManifestVersion> allVersions = null;

            try
            {
                StreamReader tryGetManifestReader = new StreamReader(DownloadUtils.DownloadToStream(Constants.URL_VERSIONS_MANIFEST));
            }
            catch
            {
                return new List<ManifestVersion>();
            }

            try
            {
                string mainDir = SettingsControl.MainDirectory;
                if (!Directory.Exists(mainDir) || !File.Exists(mainDir + "\\version_manifest.json"))
                {
                    Directory.CreateDirectory(mainDir);
                    allVersions = ReloadAvailableVersions();
                }
                else
                {
                    string input = "";

                    using (FileStream fileStream = new FileStream(mainDir + "\\version_manifest.json", FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (StreamReader reader = new StreamReader(fileStream, Encoding.UTF8))
                        {
                            input = reader.ReadToEnd();
                        }
                    }

                    allVersions = JsonConvert.DeserializeObject<RootVersionsManifest>(input).versions;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Messages.ERROR_READ_FILE_F, "version_manifest.json") + ex.Message, Messages.CAPTION_COMMON);
            }

            if (allVersions != null)
            {
                allVersions.Capacity = allVersions.Count;

                allVersions.Reverse();

                return allVersions;
            }
            else
            {
                return new List<ManifestVersion>();
            }
        }

    }
}
