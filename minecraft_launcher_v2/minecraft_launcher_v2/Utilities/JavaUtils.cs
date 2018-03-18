using Microsoft.Win32;
using minecraft_launcher_v2.Classes.Controls;
using minecraft_launcher_v2.ConstantValues;
using minecraft_launcher_v2.CustomStructs;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace minecraft_launcher_v2.Utilities
{
    static class JavaUtils
    {
        public static string GetJavaPath()
        {
            KeyValuePair<string, string> javaInfo = new KeyValuePair<string, string>("", "");

            RegistryKey baseJDK = null;
            RegistryKey baseJRE = null;

            if (Constants.IS_64_BIT)
            {
                baseJDK = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(Constants.REGISTRY_HOMEKEY_JDK);
                baseJRE = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(Constants.REGISTRY_HOMEKEY_JRE);
            }

            if (!Constants.IS_64_BIT || (baseJDK == null && baseJRE == null))
            {
                baseJDK = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(Constants.REGISTRY_HOMEKEY_JDK);
                baseJRE = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(Constants.REGISTRY_HOMEKEY_JRE);
            }

            if (baseJDK != null)
            {
                javaInfo = GetLatesJavaVersion(baseJDK.GetSubKeyNames(), baseJDK);

                baseJDK.Close();
                baseJDK.Dispose();
            }

            if (baseJRE != null)
            {
                var tempTuple = GetLatesJavaVersion(baseJRE.GetSubKeyNames(), baseJRE);

                if (CommonUtils.CompareStrings(tempTuple.Key, javaInfo.Key) > 0)
                {
                    javaInfo = tempTuple;
                }

                baseJRE.Close();
                baseJRE.Dispose();
            }

            return javaInfo.Value;
        }

        private static KeyValuePair<string, string> GetLatesJavaVersion(string[] javaVersions, RegistryKey baseKey)
        {
            javaVersions = javaVersions.OrderByAlphaNumeric(t => t).ToArray();
            KeyValuePair<string, string> latestVersion = new KeyValuePair<string, string>("", "");

            for (int x = javaVersions.Count() - 1; x >= 0; x--)
            {
                RegistryKey homeKey = baseKey.OpenSubKey(javaVersions[x]);

                if (JavaExists(homeKey.GetValue("JavaHome").ToString()))
                {
                    latestVersion = new KeyValuePair<string, string>(homeKey.Name.Substring(homeKey.Name.LastIndexOf("\\")), homeKey.GetValue("JavaHome").ToString());

                    homeKey.Close();
                    homeKey.Dispose();

                    break;
                }
            }

            return latestVersion;
        }

        public static bool JavaExists(string versionPath)
        {
            return File.Exists(versionPath + "\\bin\\java.exe") && File.Exists(versionPath + "\\bin\\javaw.exe");
        }

        public static BitDepth CheckJavaBitDebth(string javaPath)
        {
            if (!File.Exists(javaPath + "\\bin\\java.exe") || !File.Exists(javaPath + "\\bin\\javaw.exe"))
            {
                return BitDepth.Unknown;
            }

            BitDepth javaBitDebth = BitDepth.Unknown;
            BitDepth javawBitDebth = BitDepth.Unknown;


            switch (CommonUtils.GetDLLMachineType(javaPath + "\\bin\\java.exe"))
            {
                case MachineType.IMAGE_FILE_MACHINE_AMD64:
                case MachineType.IMAGE_FILE_MACHINE_IA64:
                    {
                        javaBitDebth = BitDepth.x64;
                        break;
                    }
                case MachineType.IMAGE_FILE_MACHINE_I386:
                    {
                        javaBitDebth = BitDepth.x32;
                        break;
                    }
                default:
                    {
                        javaBitDebth = BitDepth.Unknown;
                        break;
                    }
            }

            switch (CommonUtils.GetDLLMachineType(javaPath + "\\bin\\javaw.exe"))
            {
                case MachineType.IMAGE_FILE_MACHINE_AMD64:
                case MachineType.IMAGE_FILE_MACHINE_IA64:
                    {
                        javawBitDebth = BitDepth.x64;
                        break;
                    }
                case MachineType.IMAGE_FILE_MACHINE_I386:
                    {
                        javawBitDebth = BitDepth.x32;
                        break;
                    }
                default:
                    {
                        javawBitDebth = BitDepth.Unknown;
                        break;
                    }
            }

            if (((int)javaBitDebth + (int)javawBitDebth) == 4)
            {
                return BitDepth.x64;
            }
            else if (((int)javaBitDebth + (int)javawBitDebth) == 3)
            {
                return BitDepth.x32;
            }
            else
            {
                return BitDepth.Unknown;
            }
        }

        public static bool CanUse64BitJava()
        {
            return (Constants.IS_64_BIT && CheckJavaBitDebth(LauncherProfilesControl.JavaDirectory) == BitDepth.x64);
        }

    }
}
