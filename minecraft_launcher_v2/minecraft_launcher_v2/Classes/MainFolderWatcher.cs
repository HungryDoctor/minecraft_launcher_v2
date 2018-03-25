using minecraft_launcher_v2.Classes.Controls.Static;
using System;
using System.IO;

namespace minecraft_launcher_v2.Classes
{
    class MainFolderWatcher : IDisposable
    {
        private FileSystemWatcher watcher;
        private DateTime lastRead;

        private FileSystemEventHandler onChangedEventHandler;
        private RenamedEventHandler onRenamedEventHandler;

        private bool isDisposed;



        public MainFolderWatcher(FileSystemEventHandler onChangedEventHandler, RenamedEventHandler onRenamedEventHandler)
        {
            lastRead = DateTime.MinValue;

            this.onChangedEventHandler = onChangedEventHandler;
            this.onRenamedEventHandler = onRenamedEventHandler;
            SettingsControl.MainDirectoryChangedEvent += SettingsControl_MainDirectoryChangedEvent;

            this.watcher = new FileSystemWatcher();
            watcher.Path = SettingsControl.MainDirectory;
            watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Size;
            watcher.Filter = "";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnRenamed;

            isDisposed = false;
        }



        public void Dispose()
        {
            if (!isDisposed)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();

                onChangedEventHandler = null;
                onRenamedEventHandler = null;

                isDisposed = true;
            }
        }


        private void SettingsControl_MainDirectoryChangedEvent(string newDirectory)
        {
            watcher.EnableRaisingEvents = false;
            watcher.Path = newDirectory;
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            FileSystemWatcher watcher = source as FileSystemWatcher;

            DateTime lastWriteTime = File.GetLastWriteTime(e.FullPath);
            if (lastWriteTime != lastRead)
            {
                if (onChangedEventHandler != null)
                {
                    onChangedEventHandler.Invoke(source, e);
                }

                lastRead = lastWriteTime;
            }
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            if (onRenamedEventHandler != null)
            {
                onRenamedEventHandler.Invoke(source, e);
            }
        }

    }
}
