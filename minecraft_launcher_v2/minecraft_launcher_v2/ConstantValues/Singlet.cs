using Microsoft.VisualBasic.Devices;
using System.Text.RegularExpressions;

namespace minecraft_launcher_v2.ConstantValues
{
    static class Singlet
    {
        private static Regex regex_Memory;
        private static Regex regex_DropBoxFolders;
        private static ComputerInfo computerInfo;


        public static Regex Regex_Memory
        {
            get
            {
                return regex_Memory;
            }
        }
        public static Regex Regex_DropBoxFolders
        {
            get
            {
                return regex_DropBoxFolders;
            }
        }
        public static ComputerInfo ComputerInfo
        {
            get
            {
                return computerInfo;
            }
        }



        static Singlet()
        {
            regex_Memory = new Regex(@"([0-9]+)(G|M|g|m)", RegexOptions.Compiled);
            regex_DropBoxFolders = new Regex(@"https:\/\/www\.dropbox\.com\/sh\/g6t0h1mk46fh5jq\/.{25}\/.{7}\?dl=0\?lst", RegexOptions.Compiled);
            computerInfo = new ComputerInfo();
        }

    }
}
