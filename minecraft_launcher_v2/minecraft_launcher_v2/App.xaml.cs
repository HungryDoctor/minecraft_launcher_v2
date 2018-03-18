using Ionic.Zip;
using Ionic.Zlib;
using minecraft_launcher_v2.ConstantValues;
using minecraft_launcher_v2.Model;
using minecraft_launcher_v2.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace minecraft_launcher_v2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Current.DispatcherUnhandledException += App_DispatcherUnhandledException;
            Dispatcher.UnhandledException += App_DispatcherUnhandledException;

            //CultureInfo.DefaultThreadCurrentCulture = Constants.CULTURE_DEFAULT;
            //CultureInfo.DefaultThreadCurrentUICulture = Constants.CULTURE_DEFAULT;

#if !DEBUG
            if (!CommonUtils.IsOneAppInstance())
            {
                MessageBox.Show(Messages.ERROR_APPLICATION_RUNNING, Messages.CAPTION_COMMON);
                Current.Shutdown();
            }
#endif

            try
            {
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            }
            catch
            {
            }

            if (!Directory.Exists(Constants.PATH_SETTINGS_GLOBAL))
            {
                Directory.CreateDirectory(Constants.PATH_SETTINGS_GLOBAL);
            }
            ProfileOptimization.SetProfileRoot(Constants.PATH_SETTINGS_GLOBAL);
            ProfileOptimization.StartProfile("Launcher.Profile");

            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            ServicePointManager.DefaultConnectionLimit = CommonUtils.GetLogicalCoresCount() * 3;

            CosturaUtility.Initialize();

            new ModelMain();
        }



        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception);
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }


        private void HandleException(Exception exception)
        {
            uint logNumber = 0;
            string currentDirectory = Path.GetDirectoryName(Constants.PATH_EXECUTING_ASSEMBLY) + "\\";
            string zipPath = currentDirectory + Constants.FILENAME_LOGS_LAUNCHER_ARCHIVE;
            string logFileName = currentDirectory + Constants.FILENAME_LOG_LAUNCHER_EXCEPTION_F;
            string exceptionString = "";

            ZipFile zipFile = null;

            try
            {
                exceptionString = CreateExceptionString(exception);

                while (File.Exists(string.Format(logFileName, logNumber)))
                {
                    File.Delete(string.Format(logFileName, logNumber));
                    logNumber++;
                }

                logNumber = 0;

                if (File.Exists(zipPath))
                {
                    zipFile = ZipFile.Read(zipPath);

                    while (zipFile[string.Format(string.Format(Constants.FILENAME_LOG_LAUNCHER_EXCEPTION_F, logNumber))] != null)
                    {
                        logNumber++;
                    }
                }
                else
                {
                    zipFile = new ZipFile(zipPath);

                    zipFile.CompressionLevel = CompressionLevel.BestCompression;
                    zipFile.CompressionMethod = CompressionMethod.BZip2;
                }

                logFileName = string.Format(logFileName, logNumber);

                using (FileStream fileStream = new FileStream(logFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (StreamWriter writer = new StreamWriter(fileStream))
                    {
                        writer.Write(exceptionString);
                    }
                }

                zipFile.AddFile(logFileName, "");
                zipFile.Save();
            }
            catch (Exception ex)
            {
                string exceptionMessage = "";

                if (!string.IsNullOrWhiteSpace(exceptionString))
                {
                    exceptionMessage = exceptionString;
                }
                else
                {
                    exceptionMessage = exception.Message;
                }

                MessageBox.Show(Messages.ERROR_LOG_SAVE + ex.Message + "\n\n" + exceptionMessage, Messages.CAPTION_COMMON);
            }
            finally
            {
                if (zipFile != null)
                {
                    zipFile.Dispose();
                }

                if (File.Exists(logFileName))
                {
                    File.Delete(logFileName);
                }
            }
        }

        private string CreateExceptionString(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            Exception currentException = ex;
            bool finished = false;
            string indent = "";

            sb.Append(Messages.LOG_EXCEPTION_FOUND_IN_VERSION);
            sb.Append(Constants.LAUNCHER_VERSION_CURRENT);
            sb.AppendLine(":");

            string textType = Messages.LOG_EXCEPTION_TYPE;
            string textMessage = Messages.LOG_EXCEPTION_MESSAGE;
            string textSource = Messages.LOG_EXCEPTION_SOURCE;
            string textStacktrace = Messages.LOG_EXCEPTION_STACKTRACE;
            string textInnerException = Messages.LOG_EXCEPTION_INNER;


            while (!finished)
            {
                if (indent == null)
                {
                    indent = String.Empty;
                }
                else if (indent.Length > 0)
                {
                    sb.Append(indent);
                    sb.Append(textInnerException);
                }

                sb.Append(indent);
                sb.Append(textType);
                sb.AppendLine(currentException.GetType().FullName);

                sb.Append(indent);
                sb.Append(textMessage);
                sb.AppendLine(currentException.Message);

                sb.Append(indent);
                sb.Append(textSource);
                sb.AppendLine(currentException.Source);

                sb.Append(indent);
                sb.Append(textStacktrace);
                sb.AppendLine(currentException.StackTrace.TrimStart().Replace("   ", new string(' ', textStacktrace.Length + indent.Length)));

                if (currentException.InnerException != null)
                {
                    sb.AppendLine("");
                    sb.AppendLine("");
                    indent += "   ";
                    currentException = currentException.InnerException;
                }
                else
                {
                    finished = true;
                }
            }

            return sb.ToString();
        }

    }
}
