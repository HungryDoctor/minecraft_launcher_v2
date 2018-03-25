using minecraft_launcher_v2.Classes.Abstract;
using minecraft_launcher_v2.Classes.Controls.Static;
using minecraft_launcher_v2.ConstantValues;
using minecraft_launcher_v2.CustomStructs;
using minecraft_launcher_v2.Serialization;
using minecraft_launcher_v2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace minecraft_launcher_v2.Classes.Controls
{
    class StartControl : Downloadable
    {
        private StartControl()
        {
        }

        public static StartControl GetInstance()
        {
            if (instance == null)
            {
                instance = new StartControl();
            }

            return (StartControl)instance;
        }



        private List<KeyValuePair<KeyValuePair<string, string>, string>> GetLibrariesPath(RootGameVersion versionJson)
        {
            List<KeyValuePair<KeyValuePair<string, string>, string>> libraries = new List<KeyValuePair<KeyValuePair<string, string>, string>>(50);
            KeyValuePair<string, string> libraryInfo;
            HashSet<string> filePaths = null;
            bool skipLibrary = false;
            string mainDir = SettingsControl.MainDirectory;

            try
            {
                foreach (var item in versionJson.libraries)
                {
                    skipLibrary = false;
                    if (item.rules != null)
                    {
                        foreach (var rule in item.rules)
                        {
                            if (rule.action != null && rule.action == "allow" && rule.os != null && rule.os.name != "windows")
                            {
                                skipLibrary = true;
                            }
                            else if (rule.action != null && rule.action == "disallow" && rule.os != null && rule.os.name == "windows")
                            {
                                skipLibrary = true;
                            }
                        }
                    }
                    if (skipLibrary)
                    {
                        continue;
                    }

                    if (item.downloads != null)
                    {
                        if (item.downloads.artifact != null)
                        {
                            libraryInfo = CommonUtils.LibraryFullNameToInfoConvert(item.name + "_A");

                            libraries.Add(new KeyValuePair<KeyValuePair<string, string>, string>(libraryInfo, mainDir + "\\libraries\\" + GetLibraryPath(item.downloads.artifact, item.name)));
                        }
                    }
                    else
                    {
                        libraryInfo = CommonUtils.LibraryFullNameToInfoConvert(item.name + "_A");
                        string convertedName = CommonUtils.LibraryFullNameToPathConvert(item.name);
                        string libraryPath = mainDir + "\\libraries\\" + convertedName;
                        string url = "";

                        libraries.Add(new KeyValuePair<KeyValuePair<string, string>, string>(libraryInfo, libraryPath));

                        if (!File.Exists(libraryPath))
                        {
                            if (item.url != null && item.name.Contains("forge"))
                            {
                                convertedName = convertedName.Insert(convertedName.Length - 4, "-universal");

                                url = item.url + convertedName.Replace('\\', '/');
                            }
                            else if (item.url != null)
                            {
                                url = item.url + convertedName.Replace('\\', '/');
                            }
                            else
                            {
                                url = Constants.URL_LIBRARIES + convertedName.Replace('\\', '/');
                            }

                            filePaths = new HashSet<string>();
                            filePaths.Add(libraryPath);
                            downloadQueue.Enqueue(new DownloadFileInfo(filePaths, url, 0));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Messages.ERROR_GET_LIBRARIES + ex.Message, Messages.CAPTION_COMMON);
                cancellationTokenSource.Cancel();
            }

            return libraries;
        }

        private string GetLibraryPath(Serialization.FileInfo fileInfo, string libaryName)
        {
            if (fileInfo == null || string.IsNullOrWhiteSpace(fileInfo.path))
            {
                return CommonUtils.LibraryFullNameToPathConvert(libaryName).Replace("/", "\\");
            }
            else
            {
                return fileInfo.path.Replace("/", "\\");
            }
        }

        private ExtractionResult ExtractNatives(RootGameVersion versionJson, string extractionPath)
        {
            ExtractionResult result = ExtractionResult.NothingToExtract;
            bool skipLibrary = false;
            string deletePath = "";
            string mainDir = SettingsControl.MainDirectory;

            foreach (var item in versionJson.libraries)
            {
                skipLibrary = false;
                if (item.rules != null)
                {
                    foreach (var rule in item.rules)
                    {
                        if (rule.action != null && rule.action == "allow" && rule.os != null && rule.os.name != "windows")
                        {
                            skipLibrary = true;
                        }
                        else if (rule.action != null && rule.action == "disallow" && rule.os != null && rule.os.name == "windows")
                        {
                            skipLibrary = true;
                        }
                    }
                }
                if (skipLibrary)
                {
                    continue;
                }

                if (item.extract != null)
                {

                    if (item.downloads.classifiers != null)
                    {
                        try
                        {
                            foreach (var classifier in item.downloads.classifiers)
                            {
                                if (classifier.Key.Contains("natives-windows"))
                                {
                                    if (Constants.IS_64_BIT && classifier.Key == "natives-windows-64")
                                    {
                                        CommonUtils.ExtractZip(mainDir + "\\libraries\\" + GetLibraryPath(classifier.Value, item.name), extractionPath, false);
                                        result = ExtractionResult.Successfully;
                                    }
                                    else if (!Constants.IS_64_BIT && classifier.Key == "natives-windows-32")
                                    {
                                        CommonUtils.ExtractZip(mainDir + "\\libraries\\" + GetLibraryPath(classifier.Value, item.name), extractionPath, false);
                                        result = ExtractionResult.Successfully;
                                    }
                                    else if (classifier.Key == "natives-windows")
                                    {
                                        CommonUtils.ExtractZip(mainDir + "\\libraries\\" + GetLibraryPath(classifier.Value, item.name), extractionPath, false);
                                        result = ExtractionResult.Successfully;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(string.Format(Messages.ERROR_EXTRACTION_FILES_F, extractionPath) + ex.Message, Messages.CAPTION_COMMON);
                            return ExtractionResult.Error;
                        }


                        if (item.extract.exclude != null && (item.downloads.classifiers != null || item.downloads.artifact != null))
                        {
                            foreach (var excludeItem in item.extract.exclude)
                            {
                                if (excludeItem[excludeItem.Length - 1] == '/')
                                {
                                    deletePath = extractionPath + "\\" + excludeItem.Replace("/", "\\");
                                    if (Directory.Exists(deletePath))
                                    {
                                        try
                                        {
                                            Directory.Delete(deletePath, true);
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(string.Format(Messages.ERROR_DELETE_FOLDER_F, deletePath) + ex.Message, Messages.CAPTION_COMMON);
                                        }
                                    }
                                }
                                else
                                {
                                    if (excludeItem.Contains("/"))
                                    {
                                        deletePath = extractionPath + "\\" + excludeItem.Replace("/", "\\");
                                    }
                                    else
                                    {
                                        deletePath = extractionPath + "\\" + excludeItem;
                                    }

                                    if (File.Exists(deletePath))
                                    {
                                        try
                                        {
                                            File.Delete(extractionPath + "\\" + excludeItem);
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(string.Format(Messages.ERROR_DELETE_FILE_F, extractionPath + "\\" + excludeItem) + ex.Message, Messages.CAPTION_COMMON);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }

            return result;
        }


        public string GetStartString(string version)
        {
            ResetTempVariables();

            StringBuilder startParams = new StringBuilder(5000);
            List<KeyValuePair<KeyValuePair<string, string>, string>> libraries = new List<KeyValuePair<KeyValuePair<string, string>, string>>(50);
            List<KeyValuePair<KeyValuePair<string, string>, string>> tempInfo;
            List<string> minecraftArguments = new List<string>(10);
            string startParamsString = "";
            string mainClass = "";
            string assets = "";
            string nativesPath = "";
            string usedNatives = "";
            char quote = '"';
            ExtractionResult extrResult;
            string mainDir = SettingsControl.MainDirectory;


            var coresCount = Environment.ProcessorCount;
            if (coresCount == 1 || coresCount == 2)
            {
                StartDownloadFilesFromQueue(1);
            }
            else
            {
                StartDownloadFilesFromQueue(coresCount - 1);
            }

            RootGameVersion versionJson = null;
            do
            {
                try
                {
                    versionJson = JsonConvert.DeserializeObject<RootGameVersion>(File.ReadAllText(mainDir + "\\versions\\" + version + "\\" + version + ".json"));

                    if (versionJson == null)
                    {
                        throw new Exception("Can't read data in " + mainDir + "\\versions\\" + version + "\\" + version + ".json");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format(Messages.ERROR_READ_FILE_F, version + ".json") + ex.Message, Messages.CAPTION_COMMON);
                    return "";
                }

                foreach (var item in GetLibrariesPath(versionJson))
                {
                    libraries.Add(new KeyValuePair<KeyValuePair<string, string>, string>(item.Key, item.Value));
                }

                nativesPath = mainDir + "\\versions\\" + versionJson.id + "\\" + versionJson.id + "-natives";
                extrResult = ExtractNatives(versionJson, nativesPath);
                if (extrResult == ExtractionResult.Successfully)
                {
                    usedNatives = versionJson.id;
                }
                else if (extrResult == ExtractionResult.Error)
                {
                    return "";
                }

                if (versionJson.minecraftArguments != null)
                {
                    minecraftArguments.AddRange(versionJson.minecraftArguments.Split(new string[] { "--" }, StringSplitOptions.RemoveEmptyEntries));
                }
                if (mainClass == "" && versionJson.mainClass != null)
                {
                    mainClass = versionJson.mainClass;
                }
                if (assets == "" && versionJson.assets != null)
                {
                    assets = versionJson.assets;
                }

                if (versionJson.inheritsFrom == null)
                {
                    break;
                }
                version = versionJson.inheritsFrom;

            } while (true);

            downloadQueue.Enqueue(null);
            if (!WaitCurrentDownloadTasks(-1))
            {
                ResetTempVariables();

                return "";
            }
            ResetTempVariables();

            libraries = libraries.OrderBy(x => x.Key.Key).ThenBy(x => x.Key.Value).ToList();
            for (int x = libraries.Count() - 1; x >= 0; x--)
            {
                tempInfo = libraries.Where(z => z.Key.Key == libraries[x].Key.Key).ToList();

                if (tempInfo.Count == 1)
                {
                    startParams.Append(libraries[x].Value);
                    startParams.Append(";");
                }
                else
                {
                    startParams.Append(libraries[x].Value);
                    startParams.Append(";");

                    x -= tempInfo.Count - 1;
                }
            }

            for (int x = 0; x < minecraftArguments.Count; x++)
            {
                minecraftArguments[x] = minecraftArguments[x].Trim();
            }

            startParamsString = startParams.ToString();
            startParams.Clear();

            startParams.Append("-Djava.library.path=\"");
            startParams.Append(mainDir);
            startParams.Append("\\versions\\");
            startParams.Append(usedNatives);
            startParams.Append("\\");
            startParams.Append(usedNatives);
            startParams.Append("-natives\" -cp \"");
            startParams.Append(startParamsString);
            startParams.Append(mainDir);
            startParams.Append("\\versions\\");
            startParams.Append(version);
            startParams.Append("\\");
            startParams.Append(version);
            startParams.Append(".jar\" ");

            startParams.Append(mainClass);
            startParams.Append(" --");
            startParams.Append(string.Join(" --", minecraftArguments.Distinct()));


            startParams.Replace("${game_directory}", quote + mainDir + quote);
            if (assets == "legacy")
            {
                startParams.Replace("${game_assets}", quote + mainDir + "\\assets\\virtual\\legacy\"");
            }
            else
            {
                startParams.Replace("${assets_index_name}", quote + assets + quote);
                startParams.Replace("${assets_root}", quote + mainDir + "\\assets\"");
            }

            startParams.Replace(" --versionType ${version_type}", "");          

            return startParams.ToString();
        }

    }
}
