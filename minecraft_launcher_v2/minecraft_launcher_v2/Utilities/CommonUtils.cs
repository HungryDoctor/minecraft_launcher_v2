using Ionic.Zip;
using minecraft_launcher_v2.ConstantValues;
using minecraft_launcher_v2.CustomStructs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace minecraft_launcher_v2.Utilities
{
    static class CommonUtils
    {
        public static int CompareStrings(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }


        public static string LibraryFullNameToPathConvert(string libraryName)
        {
            StringBuilder stringBuilder = new StringBuilder(50);
            stringBuilder.Append(libraryName);
            stringBuilder.Replace(':', '\\');

            int lastIndex = stringBuilder.ToString().IndexOf('\\');
            for (int x = lastIndex; x >= 0; x--)
            {
                if (stringBuilder[x] == '.')
                {
                    stringBuilder.Remove(x, 1).Insert(x, "\\");
                }
            }

            string libraryPath = stringBuilder.ToString();

            stringBuilder.Append('\\');
            stringBuilder.Append(libraryPath.Substring(libraryPath.Substring(0, lastIndex + 1).LastIndexOf('\\') + 1).Replace('\\', '-'));
            stringBuilder.Append(".jar");

            return stringBuilder.ToString();
        }

        public static KeyValuePair<string, string> LibraryFullNameToInfoConvert(string libraryName)
        {
            var firstIndex = libraryName.IndexOf(':') + 1;
            var lastIndex = libraryName.LastIndexOf(':');

            return new KeyValuePair<string, string>(libraryName.Substring(firstIndex, lastIndex - firstIndex), libraryName.Substring(lastIndex + 1));
        }


        public static string GetLauncherUpdates()
        {
            string result = "";

            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(Constants.URL_DROPBOX);
                httpRequest.KeepAlive = false;
                httpRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                using (HttpWebResponse responseFolder = (HttpWebResponse)httpRequest.GetResponse())
                {
                    if (responseFolder != null)
                    {
                        List<string> folders;
                        using (StreamReader streamReaderFolder = new StreamReader(responseFolder.GetResponseStream()))
                        {
                            folders = new List<string>();

                            Regex regexFolders = Singlet.Regex_DropBoxFolders;
                            foreach (Match item in regexFolders.Matches(streamReaderFolder.ReadToEnd()))
                            {
                                folders.Add(item.Value);
                            }

                            streamReaderFolder.Close();
                            streamReaderFolder.Dispose();
                        }

                        folders = folders.Distinct().ToList();
                        for (int x = folders.Count - 1; x >= 0; x--)
                        {
                            int lastIndex = folders[x].LastIndexOf('/') + 1;
                            string currentVersion = folders[x].Substring(lastIndex, folders[x].Substring(lastIndex).IndexOf('?'));

                            if (int.Parse(currentVersion.Replace(".", "")) > int.Parse(Constants.LAUNCHER_VERSION_CURRENT.Replace(".", "")))
                            {
                                HttpWebRequest httpRequestFile = (HttpWebRequest)WebRequest.Create(folders[x]);

                                using (var responseFile = (HttpWebResponse)httpRequestFile.GetResponse())
                                {
                                    using (StreamReader readStreamFile = new StreamReader(responseFile.GetResponseStream()))
                                    {
                                        Regex regexFile = new Regex(@"https:\/\/www\.dropbox\.com\/sh\/g6t0h1mk46fh5jq\/.{25}\/" + currentVersion.Replace(".", @"\.") + @"\/minecraft_launcher.exe\?dl=0");                            
                                        foreach (Match item in regexFile.Matches(readStreamFile.ReadToEnd()))
                                        {
                                            result = item.Value;
                                            break;
                                        }

                                        readStreamFile.Close();
                                        readStreamFile.Dispose();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                result = "";
            }

            return result;
        }

        public static void WriteResourceToFile(string resourceName, string fileName)
        {
            using (Stream resource =  Assembly.GetExecutingAssembly().GetManifestResourceStream("minecraft_launcher_v2.Resources." + resourceName))
            {
                using (Stream output = File.OpenWrite(fileName))
                {
                    resource.CopyTo(output);
                }
            }
        }

        public static void ExtractZip(string zipPath, string extractionFoler, bool overwrite)
        {
            using (ZipFile zip = ZipFile.Read(zipPath))
            {
                foreach (ZipEntry e in zip)
                {
                    if (overwrite)
                    {
                        e.Extract(extractionFoler, ExtractExistingFileAction.OverwriteSilently);
                    }
                    else
                    {
                        e.Extract(extractionFoler, ExtractExistingFileAction.DoNotOverwrite);
                    }
                }
            }
        }

        public static ulong GetFreeRam()
        {
            return Singlet.ComputerInfo.AvailablePhysicalMemory;
        }

        public static MachineType GetDLLMachineType(string filePath)
        {
            if (File.Exists(filePath))
            {
                MachineType result;
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        fs.Seek(0x3c, SeekOrigin.Begin);
                        Int32 peOffset = br.ReadInt32();
                        fs.Seek(peOffset, SeekOrigin.Begin);
                        UInt32 peHead = br.ReadUInt32();

                        if (peHead != 0x00004550)
                        {
                            result = MachineType.IMAGE_FILE_MACHINE_UNKNOWN;
                        }
                        else
                        {
                            result = (MachineType)br.ReadUInt16();
                        }

                        br.Close();
                        br.Dispose();
                    }

                    fs.Close();
                    fs.Dispose();
                }


                return result;
            }
            else
            {
                return MachineType.IMAGE_FILE_MACHINE_UNKNOWN;
            }
        }

        public static List<Process> GetChildPrecesses(int parentId)
        {
            var query = "Select * From Win32_Process Where ParentProcessId = " + parentId;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection processList = searcher.Get();

            List<Process> result = new List<Process>(2);

            foreach (var item in processList)
            {
                result.Add(Process.GetProcessById(Convert.ToInt32(item.GetPropertyValue("ProcessId"))));
            }

            return result;
        }

        public static bool IsOneAppInstance()
        {
            Process curr = Process.GetCurrentProcess();
            Process[] procs = Process.GetProcessesByName(curr.ProcessName);
            foreach (Process p in procs)
            {
                if (p.Id != curr.Id && p.MainModule.FileName == curr.MainModule.FileName)
                {
                    return false;
                }
            }

            return true;
        }

    }
}
