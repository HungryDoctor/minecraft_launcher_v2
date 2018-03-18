using System;
using System.IO;
using System.Net;

namespace minecraft_launcher_v2.Utilities
{
    static class DownloadUtils
    {
        public static void DownloadFile(string url, string savePath)
        {
            long bytes_total = 0;
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.KeepAlive = false;
                httpRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                using (var response = (HttpWebResponse)httpRequest.GetResponse())
                {
                    bytes_total = Convert.ToInt64(response.Headers["Content-Length"]);

                    if (File.Exists(savePath + ".temp"))
                    {
                        File.Delete(savePath + ".temp");
                    }

                    DownloadFile(response, savePath);

                    response.Close();
                    response.Dispose();
                }
            }
            catch (Exception ex)
            {
                if (bytes_total != 0 && File.Exists(savePath))
                {
                    FileInfo fileInfo = new FileInfo(savePath);
                    if (fileInfo.Length != bytes_total)
                    {
                        File.Delete(savePath);
                    }
                }

                if (File.Exists(savePath + ".temp"))
                {
                    File.Delete(savePath + ".temp");
                }

                throw new Exception(ex.Message);
            }
        }

        private static void DownloadFile(HttpWebResponse response, string savePath)
        {
            string directory = savePath.Substring(0, savePath.LastIndexOf("\\"));

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            Stream streamResponse = response.GetResponseStream();

            if (streamResponse != null)
            {
                if (File.Exists(savePath + ".temp"))
                {
                    File.Delete(savePath + ".temp");
                }

                using (FileStream fstr = new FileStream(savePath + ".temp", FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {

                    byte[] inBuf = new byte[response.ContentLength];
                    int bytesToRead = Convert.ToInt32(inBuf.Length);
                    int bytesRead = 0;
                    while (bytesToRead > 0)
                    {
                        int n = streamResponse.Read(inBuf, bytesRead, bytesToRead);
                        if (n == 0)
                        {
                            break;
                        }

                        fstr.Write(inBuf, bytesRead, n);

                        bytesRead += n;
                        bytesToRead -= n;
                    }

                    streamResponse.Close();
                    streamResponse.Dispose();

                    fstr.Flush();
                    fstr.Close();
                    fstr.Dispose();
                }

                if (File.Exists(savePath))
                {
                    File.Delete(savePath);
                }

                if (File.Exists(savePath + ".temp"))
                {
                    File.Move(savePath + ".temp", savePath);
                }
            }
        }

        public static MemoryStream DownloadToStream(string url)
        {
            HttpWebRequest httpRequest = WebRequest.CreateHttp(url);
            httpRequest.KeepAlive = false;
            httpRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)httpRequest.GetResponse())
            {
                Stream streamResponse = response.GetResponseStream();
                if (streamResponse != null)
                {
                    MemoryStream memoryStream = new MemoryStream();

                    byte[] inBuf = new byte[response.ContentLength];
                    int bytesToRead = Convert.ToInt32(inBuf.Length);
                    int bytesRead = 0;
                    while (bytesToRead > 0)
                    {
                        int n = streamResponse.Read(inBuf, bytesRead, bytesToRead);
                        if (n == 0)
                        {
                            break;
                        }

                        memoryStream.Write(inBuf, bytesRead, n);

                        bytesRead += n;
                        bytesToRead -= n;
                    }

                    memoryStream.Position = 0;

                    streamResponse.Close();
                    streamResponse.Dispose();

                    return memoryStream;
                }
                else
                {
                    return new MemoryStream();
                }
            }
        }

    }
}
