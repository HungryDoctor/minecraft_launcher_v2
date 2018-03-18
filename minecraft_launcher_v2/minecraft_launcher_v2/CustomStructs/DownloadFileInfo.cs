using System.Collections.Generic;

namespace minecraft_launcher_v2.CustomStructs
{
    public class DownloadFileInfo
    {
        private HashSet<string> filePaths;
        private string fileUrl;
        private ulong fileSize;


        public HashSet<string> FilePaths
        {
            get
            {
                return filePaths;
            }
        }   
        public string FileUrl
        {
            get
            {
                return fileUrl;
            }
        }
        public ulong FileSize
        {
            get
            {
                return fileSize;
            }
        }



        public DownloadFileInfo(HashSet<string> filePaths, string fileUrl, ulong fileSize)
        {
            this.filePaths = filePaths;
            this.fileUrl = fileUrl;
            this.fileSize = fileSize;
        }

    }

}
