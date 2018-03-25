using minecraft_launcher_v2.ConstantValues;
using minecraft_launcher_v2.CustomStructs;
using minecraft_launcher_v2.Serialization;
using minecraft_launcher_v2.Utilities;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace minecraft_launcher_v2.Classes.Abstract
{
    abstract class Downloadable
    {
        protected static Downloadable instance;

        protected string downloadVersion;
        protected ulong totalBytesToDownload;
        protected ulong totalBytesDownloaded;
        protected ConcurrentQueue<DownloadFileInfo> downloadQueue;
        private Task[] currentDownloadTasks;

        protected volatile string errorMessages;
        protected volatile byte counter;

        protected CancellationTokenSource cancellationTokenSource;


        public double PercentDownloaded
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



        protected void StartDownloadFilesFromQueue(int threadsCount)
        {
            currentDownloadTasks = new Task[threadsCount];
            for (int x = 0; x < threadsCount; x++)
            {
                currentDownloadTasks[x] = Task.Factory.StartNew(new Action(() => { DownloadFilesFromQueue(); }), cancellationTokenSource.Token);
            }
        }

        private void DownloadFilesFromQueue()
        {
            while (true)
            {
                DownloadFileInfo downloadFileInfo;
                if (downloadQueue.TryDequeue(out downloadFileInfo))
                {
                    if (downloadFileInfo != null)
                    {
                        DownloadFile(downloadFileInfo);
                    }
                    else
                    {
                        cancellationTokenSource.Cancel();
                        break;
                    }
                }
                else
                {
                    Thread.Sleep(5);
                }

                cancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
        }

        private void DownloadFile(DownloadFileInfo downloadInfo)
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

                cancellationTokenSource.Cancel();
            }


            if (!cancellationTokenSource.IsCancellationRequested)
            {
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

                    cancellationTokenSource.Cancel();
                }

                counter++;
                ClearMemory();
            }
        }


        protected void SetTotalBytesToDownload(RootGameVersion versionJsnon)
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


        protected void ClearMemory()
        {
            if (counter >= Constants.MEMORY_CLEAR_THREHOLD)
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                counter = 0;
            }
        }

        protected void ResetTempVariables()
        {
            StopAllDownloads();

            try
            {
                WaitCurrentDownloadTasks(1000);
            }
            catch
            {
                MessageBox.Show(Messages.ERROR_GET_FILES + errorMessages, Messages.CAPTION_COMMON);
            }          

            currentDownloadTasks = null;
            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken());
            downloadVersion = "";
            errorMessages = "";
            totalBytesToDownload = 0;
            totalBytesDownloaded = 0;
            counter = 0;
        }

        public void StopAllDownloads()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }
        }


        protected bool WaitCurrentDownloadTasks(int timeoutMs)
        {
            return WaitTasks(currentDownloadTasks, timeoutMs);
        }

        protected bool WaitTasks(Task[] tasks, int timeoutMs)
        {
            foreach (var task in tasks)
            {
                task.ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        cancellationTokenSource.Cancel();
                    }
                },
                cancellationTokenSource.Token, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Current);
            }

            return Task.WaitAll(currentDownloadTasks, timeoutMs, cancellationTokenSource.Token);
        }

    }
}
