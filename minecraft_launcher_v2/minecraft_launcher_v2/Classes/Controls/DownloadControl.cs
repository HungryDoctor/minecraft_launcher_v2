using minecraft_launcher_v2.Classes.Abstract;
using minecraft_launcher_v2.Classes.Controls.Static;
using minecraft_launcher_v2.ConstantValues;
using minecraft_launcher_v2.CustomStructs;
using minecraft_launcher_v2.Serialization;
using minecraft_launcher_v2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace minecraft_launcher_v2.Classes.Controls
{
    class DownloadControl : Downloadable
    {
        private DownloadControl()
        {                
        }

        public static DownloadControl GetInstance()
        {
            if (instance == null)
            {
                instance = new DownloadControl();
            }

            return (DownloadControl)instance;
        }



        private void GetMainFilesUrls(RootGameVersion versionJsnon, string jsnonUrl)
        {
            HashSet<string> filePaths = null;
            string downloadPath = SettingsControl.MainDirectory + "\\versions\\" + downloadVersion + "\\";

            filePaths = new HashSet<string>();
            filePaths.Add(downloadPath + downloadVersion + ".json");
            downloadQueue.Enqueue(new DownloadFileInfo(filePaths, jsnonUrl, 0));

            filePaths = new HashSet<string>();
            filePaths.Add(downloadPath + downloadVersion + ".jar");
            downloadQueue.Enqueue(new DownloadFileInfo(filePaths, versionJsnon.downloads.client.url, versionJsnon.downloads.client.size));


            if (versionJsnon.downloads.server != null && !string.IsNullOrEmpty(versionJsnon.downloads.server.url))
            {
                filePaths = new HashSet<string>();
                filePaths.Add(downloadPath + "minecraft_server." + downloadVersion + ".jar");
                downloadQueue.Enqueue(new DownloadFileInfo(filePaths, versionJsnon.downloads.server.url, versionJsnon.downloads.server.size));
            }
            if (versionJsnon.downloads.windows_server != null && !string.IsNullOrEmpty(versionJsnon.downloads.windows_server.url))
            {
                filePaths = new HashSet<string>();
                filePaths.Add(downloadPath + "minecraft_server." + downloadVersion + ".exe");
                downloadQueue.Enqueue(new DownloadFileInfo(filePaths, versionJsnon.downloads.windows_server.url, versionJsnon.downloads.windows_server.size));
            }
        }

        private void GetLibrariesUrls(RootGameVersion versionJsnon)
        {
            bool skipLibrary = false;
            HashSet<string> filePaths = null;
            string libariesPath = SettingsControl.MainDirectory + "\\libraries\\";

            var librariesList = versionJsnon.libraries.GroupBy(x => x.name).Select(y => y.First()).ToList();
            for (int x = librariesList.Count - 1; x >= 0; x--)
            {
                skipLibrary = false;

                if (librariesList[x].rules != null)
                {
                    foreach (var rule in librariesList[x].rules)
                    {
                        if (rule.action != null && rule.action == "allow" && rule.os != null && rule.os.name != "windows")
                        {
                            skipLibrary = true;
                            break;
                        }
                        else if (rule.action != null && rule.action == "disallow" && rule.os != null && rule.os.name == "windows")
                        {
                            skipLibrary = true;
                            break;
                        }
                    }
                }
                if (skipLibrary)
                {
                    librariesList.RemoveAt(x);
                    continue;
                }

                if (librariesList[x].downloads.artifact != null)
                {
                    filePaths = new HashSet<string>();
                    filePaths.Add(libariesPath + librariesList[x].downloads.artifact.path.Replace("/", "\\"));
                    downloadQueue.Enqueue(new DownloadFileInfo(filePaths, librariesList[x].downloads.artifact.url, librariesList[x].downloads.artifact.size));
                }

                if (librariesList[x].downloads.classifiers != null)
                {
                    foreach (var classifier in librariesList[x].downloads.classifiers)
                    {
                        if (classifier.Key.Contains("windows"))
                        {
                            filePaths = new HashSet<string>();
                            filePaths.Add(libariesPath + classifier.Value.path.Replace("/", "\\"));
                            downloadQueue.Enqueue(new DownloadFileInfo(filePaths, classifier.Value.url, classifier.Value.size));
                        }
                    }
                }

                librariesList.RemoveAt(x);
            }
        }

        private void GetAssetsUrls(RootGameVersion versionJsnon)
        {
            HashSet<string> savePaths = null;
            string content = "";
            string assetsDirPath = "";
            string assetUrl = "";
            string mainFileHash = "";
            ulong totalBytesToDownload = 0;
            string mainDir = SettingsControl.MainDirectory;

            try
            {
                using (StreamReader reader = new StreamReader(DownloadUtils.DownloadToStream(versionJsnon.assetIndex.url)))
                {
                    content = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Messages.ERROR_GET_ASSETS_URLS + ex.Message, Messages.CAPTION_COMMON);
                throw ex;
            }


            savePaths = new HashSet<string>();
            if (versionJsnon.assets == "legacy")
            {
                savePaths.Add(mainDir + "\\assets\\indexes\\legacy.json");
            }
            else
            {
                savePaths.Add(mainDir + "\\assets\\indexes\\" + versionJsnon.assets + ".json");
            }
            downloadQueue.Enqueue(new DownloadFileInfo(savePaths, versionJsnon.assetIndex.url, versionJsnon.assetIndex.size));


            if (versionJsnon.assets == "legacy")
            {
                assetsDirPath = mainDir + "\\assets\\virtual\\legacy\\";
            }
            else
            {
                assetsDirPath = mainDir + "\\assets\\objects\\";
            }

            var assetsInfo = JsonConvert.DeserializeObject<RootAssets>(content).objects.ToList();

            for (int x = assetsInfo.Count - 1; x >= 0; x--)
            {
                savePaths = new HashSet<string>();
                mainFileHash = assetsInfo[x].Value.hash;
                totalBytesToDownload = 0;

                assetUrl = Constants.URL_ASSETS + mainFileHash.Substring(0, 2) + "/" + mainFileHash;

                for (int i = x; i >= 0; i--)
                {
                    if (assetsInfo[i].Value.hash == mainFileHash)
                    {
                        totalBytesToDownload += assetsInfo[i].Value.size;
                        savePaths.Add(GetAssetSavePath(assetsInfo[i], versionJsnon.assetIndex.id, assetsDirPath));
                        assetsInfo.RemoveAt(i);
                    }
                }

                x = assetsInfo.Count - 1;

                downloadQueue.Enqueue(new DownloadFileInfo(savePaths, assetUrl, totalBytesToDownload));
            }
        }

        private string GetAssetSavePath(KeyValuePair<string, AssetObject> assetInfo, string assetIndex, string assetsDirPath)
        {
            if (assetIndex == "legacy")
            {
                string assetFullName = assetInfo.Key;
                if (assetFullName.Contains("/"))
                {
                    assetFullName = assetFullName.Replace("/", "\\");
                }

                return assetsDirPath + assetFullName;
            }
            else
            {
                return assetsDirPath + assetInfo.Value.hash.Substring(0, 2) + "\\" + assetInfo.Value.hash;
            }
        }


        public bool DownloadVersion(string newDownloadVersion)
        {
            ResetTempVariables();
            downloadVersion = newDownloadVersion;

            string content = "";
            bool downloaded = false;

            RootVersionsManifest versionsManifest = null;
            try
            {
                string mainDir = SettingsControl.MainDirectory;
                if (!File.Exists(mainDir + "\\version_manifest.json"))
                {
                    DownloadUtils.DownloadFile(Constants.URL_VERSIONS_MANIFEST, mainDir + "\\version_manifest.json");
                }

                versionsManifest = JsonConvert.DeserializeObject<RootVersionsManifest>(File.ReadAllText(mainDir + "\\version_manifest.json"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Messages.ERROR_READ_FILE_F, "version_manifest.json") + ex.Message, Messages.CAPTION_COMMON);
                return false;
            }

            ManifestVersion versionManifest = null;
            try
            {
                versionManifest = versionsManifest.versions.Where(x => x.id == downloadVersion).First();

                if (versionManifest == null)
                {
                    MessageBox.Show(string.Format(Messages.ERROR_GET_VERSION_FROM_MANIFEST_F, downloadVersion), Messages.CAPTION_COMMON);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Messages.ERROR_GET_VERSION_FROM_MANIFEST_F, downloadVersion) + ex.Message, Messages.CAPTION_COMMON);
                return false;
            }


            try
            {
                using (StreamReader reader = new StreamReader(DownloadUtils.DownloadToStream(versionManifest.url)))
                {
                    content = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Messages.ERROR_GET_VERSION_JSON + ex.Message, Messages.CAPTION_COMMON);
                return false;
            }


            RootGameVersion versionJsnon = null;
            try
            {
                versionJsnon = JsonConvert.DeserializeObject<RootGameVersion>(content);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Messages.ERROR_READ_FILE_F, downloadVersion + ".json") + ex.Message, Messages.CAPTION_COMMON);
                return false;
            }


            SetTotalBytesToDownload(versionJsnon);
            StartDownloadFilesFromQueue(Environment.ProcessorCount);

            Task getAssetsUrls = Task.Factory.StartNew(new Action(() => GetAssetsUrls(versionJsnon)), cancellationTokenSource.Token);
            Task getLibrariesUrls = Task.Factory.StartNew(new Action(() => GetLibrariesUrls(versionJsnon)), cancellationTokenSource.Token);
            Task getMainFilesUrls = Task.Factory.StartNew(new Action(() => GetMainFilesUrls(versionJsnon, versionManifest.url)), cancellationTokenSource.Token);
            
            if (WaitTasks(new Task[] { getAssetsUrls, getLibrariesUrls, getMainFilesUrls }, -1))
            {
                downloadQueue.Enqueue(null);

                if (WaitCurrentDownloadTasks(-1))
                {
                    downloaded = true;
                }
            }

            ResetTempVariables();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

            return downloaded;
        }

       
    }
}
