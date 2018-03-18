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
    static class DownloadControl
    {
        private static string downloadVersion;
        private static ulong totalBytesToDownload;
        private static ulong totalBytesDownloaded;

        private static CancellationTokenSource cts;
        private static volatile byte counter;
        private static volatile string errorMessages;


        public static double PercentDownloaded
        {
            get
            {
                if (totalBytesToDownload == 0)
                {
                    return 0;
                }

                return totalBytesDownloaded / (double)totalBytesToDownload;
            }
        }



        static DownloadControl()
        {
            downloadVersion = "";
            errorMessages = "";

            totalBytesToDownload = 0;
            totalBytesDownloaded = 0;
        }



        private static void GetMainFilesUrls(RootGameVersion versionJsnon, string jsnonUrl, ConcurrentQueue<DownloadFileInfo> queueMainFiles)
        {
            HashSet<string> filePaths = null;
            string downloadPath = SettingsControl.MainDirectory + "\\versions\\" + downloadVersion + "\\";

            filePaths = new HashSet<string>();
            filePaths.Add(downloadPath + downloadVersion + ".json");
            queueMainFiles.Enqueue(new DownloadFileInfo(filePaths, jsnonUrl, 0));

            filePaths = new HashSet<string>();
            filePaths.Add(downloadPath + downloadVersion + ".jar");
            queueMainFiles.Enqueue(new DownloadFileInfo(filePaths, versionJsnon.downloads.client.url, versionJsnon.downloads.client.size));


            if (versionJsnon.downloads.server != null && !string.IsNullOrEmpty(versionJsnon.downloads.server.url))
            {
                filePaths = new HashSet<string>();
                filePaths.Add(downloadPath + "minecraft_server." + downloadVersion + ".jar");
                queueMainFiles.Enqueue(new DownloadFileInfo(filePaths, versionJsnon.downloads.server.url, versionJsnon.downloads.server.size));
            }
            if (versionJsnon.downloads.windows_server != null && !string.IsNullOrEmpty(versionJsnon.downloads.windows_server.url))
            {
                filePaths = new HashSet<string>();
                filePaths.Add(downloadPath + "minecraft_server." + downloadVersion + ".exe");
                queueMainFiles.Enqueue(new DownloadFileInfo(filePaths, versionJsnon.downloads.windows_server.url, versionJsnon.downloads.windows_server.size));
            }
        }

        private static void GetLibrariesUrls(RootGameVersion versionJsnon, ConcurrentQueue<DownloadFileInfo> queueLibraries)
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
                    queueLibraries.Enqueue(new DownloadFileInfo(filePaths, librariesList[x].downloads.artifact.url, librariesList[x].downloads.artifact.size));
                }

                if (librariesList[x].downloads.classifiers != null)
                {
                    foreach (var classifier in librariesList[x].downloads.classifiers)
                    {
                        if (classifier.Key.Contains("windows"))
                        {
                            filePaths = new HashSet<string>();
                            filePaths.Add(libariesPath + classifier.Value.path.Replace("/", "\\"));
                            queueLibraries.Enqueue(new DownloadFileInfo(filePaths, classifier.Value.url, classifier.Value.size));
                        }
                    }
                }

                librariesList.RemoveAt(x);
            }
        }

        private static void GetAssetsUrls(RootGameVersion versionJsnon, ConcurrentQueue<DownloadFileInfo> queueAssets)
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
            queueAssets.Enqueue(new DownloadFileInfo(savePaths, versionJsnon.assetIndex.url, versionJsnon.assetIndex.size));


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

                queueAssets.Enqueue(new DownloadFileInfo(savePaths, assetUrl, totalBytesToDownload));
            }
        }

        private static string GetAssetSavePath(KeyValuePair<string, AssetObject> assetInfo, string assetIndex, string assetsDirPath)
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


        private static void DownloadFilesFromQueue(ParallelOptions parallelOptions, ConcurrentQueue<DownloadFileInfo> queue, Task task)
        {
            Parallel.ForEach(new InfinitePartitioner(), parallelOptions, (ignored, loopState) =>
            {
                if (!cts.IsCancellationRequested)
                {
                    DownloadFileInfo downloadFileInfo;
                    bool dequeued = queue.TryDequeue(out downloadFileInfo);

                    if (dequeued)
                    {
                        DownloadFile(downloadFileInfo);
                    }
                    else if (!dequeued && task.IsCompleted)
                    {
                        loopState.Stop();
                    }
                }

                if (task.IsFaulted || task.IsCanceled || cts.IsCancellationRequested)
                {
                    loopState.Stop();
                    cts.Cancel();
                }
            });
        }

        private static void DownloadFile(DownloadFileInfo downloadInfo)
        {
            string mainFilePath = "";
            string filePath = "";
            mainFilePath = downloadInfo.FilePaths.First();
            downloadInfo.FilePaths.Remove(mainFilePath);

            try
            {
                DownloadUtils.DownloadFile(downloadInfo.FileUrl, mainFilePath);
                totalBytesDownloaded += downloadInfo.FileSize;
            }
            catch (Exception ex)
            {
                if (!errorMessages.Contains(ex.Message))
                {
                    errorMessages += ex.Message + "\n";
                }

                cts.Cancel();
                cts.Token.ThrowIfCancellationRequested();
            }

            try
            {
                if (File.Exists(mainFilePath))
                {
                    for (int x = 0; x < downloadInfo.FilePaths.Count; x++)
                    {
                        filePath = downloadInfo.FilePaths.First();
                        downloadInfo.FilePaths.Remove(filePath);

                        string directory = filePath.Substring(0, filePath.LastIndexOf("\\"));

                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        File.Copy(mainFilePath, filePath, true);
                        totalBytesDownloaded += downloadInfo.FileSize;
                    }
                }
            }
            catch (Exception ex)
            {
                if (!errorMessages.Contains(ex.Message))
                {
                    errorMessages += ex.Message + "\n";
                }

                cts.Cancel();
                cts.Token.ThrowIfCancellationRequested();
            }

            counter++;
            ClearMemory();
        }

        private static void GetTotalBytesToDownload(RootGameVersion versionJsnon)
        {
            totalBytesToDownload = 0;

            if (versionJsnon.downloads.client != null && string.IsNullOrWhiteSpace(versionJsnon.downloads.client.url)
                && versionJsnon.downloads.client.size > 0)
            {
                totalBytesToDownload += versionJsnon.downloads.client.size;
            }

            if (versionJsnon.downloads.server != null && string.IsNullOrWhiteSpace(versionJsnon.downloads.server.url)
                && versionJsnon.downloads.server.size > 0)
            {
                totalBytesToDownload += versionJsnon.downloads.server.size;
            }

            if (versionJsnon.downloads.windows_server != null && string.IsNullOrWhiteSpace(versionJsnon.downloads.windows_server.url)
                && versionJsnon.downloads.windows_server.size > 0)
            {
                totalBytesToDownload += versionJsnon.downloads.windows_server.size;
            }


            if (versionJsnon.assetIndex != null && string.IsNullOrWhiteSpace(versionJsnon.assetIndex.url)
                && versionJsnon.assetIndex.totalSize > 0)
            {
                totalBytesToDownload += versionJsnon.assetIndex.totalSize;
            }


            bool skipLibrary = false;
            var librariesList = versionJsnon.libraries;
            if (versionJsnon.libraries != null)
            {
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
                        continue;
                    }

                    if (librariesList[x].downloads.artifact != null && librariesList[x].downloads.artifact.size > 0)
                    {
                        totalBytesToDownload += librariesList[x].downloads.artifact.size;
                    }

                    if (librariesList[x].downloads.classifiers != null)
                    {
                        foreach (var classifier in librariesList[x].downloads.classifiers)
                        {
                            if (classifier.Key.Contains("natives-windows"))
                            {
                                if (Constants.IS_64_BIT && classifier.Key == "natives-windows-64" && classifier.Value.size > 0)
                                {
                                    totalBytesToDownload += classifier.Value.size;
                                }
                                else if (!Constants.IS_64_BIT && classifier.Key == "natives-windows-32" && classifier.Value.size > 0)
                                {
                                    totalBytesToDownload += classifier.Value.size;
                                }
                                else if (classifier.Value.size > 0)
                                {
                                    totalBytesToDownload += classifier.Value.size;
                                }
                            }
                        }
                    }
                }
            }

            totalBytesToDownload *= 8;
        }



        public static bool DownloadVersion(string newDownloadVersion)
        {
            downloadVersion = newDownloadVersion;
            counter = 0;
            totalBytesToDownload = 0;
            totalBytesDownloaded = 0;

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


            CancellationToken cToken = new CancellationToken();
            cts = CancellationTokenSource.CreateLinkedTokenSource(cToken);

            ParallelOptions parallelOptions = new ParallelOptions();
            parallelOptions.CancellationToken = cToken;
            parallelOptions.MaxDegreeOfParallelism = CommonUtils.GetLogicalCoresCount();

            GetTotalBytesToDownload(versionJsnon);


            ConcurrentQueue<DownloadFileInfo> queueMainFiles = new ConcurrentQueue<DownloadFileInfo>();
            ConcurrentQueue<DownloadFileInfo> queueLibraries = new ConcurrentQueue<DownloadFileInfo>();
            ConcurrentQueue<DownloadFileInfo> queueAssets = new ConcurrentQueue<DownloadFileInfo>();

            Task getAssetsUrls = Task.Factory.StartNew(new Action(() => GetAssetsUrls(versionJsnon, queueAssets)), cToken);
            Task downloadAssets = Task.Factory.StartNew(new Action(() => DownloadFilesFromQueue(parallelOptions, queueAssets, getAssetsUrls)), cToken);

            Task getLibrariesUrls = Task.Factory.StartNew(new Action(() => GetLibrariesUrls(versionJsnon, queueLibraries)), cToken);
            Task downloadLibraries = Task.Factory.StartNew(new Action(() => DownloadFilesFromQueue(parallelOptions, queueLibraries, getLibrariesUrls)), cToken);

            Task getMainFilesUrls = Task.Factory.StartNew(new Action(() => GetMainFilesUrls(versionJsnon, versionManifest.url, queueMainFiles)), cToken);
            Task downloadMainFiles = Task.Factory.StartNew(new Action(() => DownloadFilesFromQueue(parallelOptions, queueMainFiles, getMainFilesUrls)), cToken);

            Task[] tasks = new Task[] { getAssetsUrls, downloadAssets, getLibrariesUrls, downloadLibraries, getMainFilesUrls, downloadMainFiles };
            try
            {
                downloaded = WaitAllTasks(tasks, -1, cts);
            }
            catch
            {
                MessageBox.Show(Messages.ERROR_GET_FILES + errorMessages, Messages.CAPTION_COMMON);
            }

            downloadVersion = "";
            errorMessages = "";
            totalBytesToDownload = 0;
            totalBytesDownloaded = 0;
            cts = null;

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

            return downloaded;
        }

        private static bool WaitAllTasks(Task[] tasks, int timeout, CancellationTokenSource cts)
        {
            foreach (var task in tasks)
            {
                task.ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled) cts.Cancel();
                },
                cts.Token, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Current);
            }

            return Task.WaitAll(tasks, timeout, cts.Token);
        }


        public static void StopAllDownloads()
        {
            if (cts != null)
            {
                cts.Cancel();
            }
        }

        private static void ClearMemory()
        {
            if (counter >= Constants.MEMORY_CLEAR_THREHOLD)
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                counter = 0;
            }
        }

    }
}
