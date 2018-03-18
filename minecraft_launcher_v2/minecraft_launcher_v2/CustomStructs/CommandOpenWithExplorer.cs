using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;

namespace minecraft_launcher_v2.CustomStructs
{
    public class OpenWithExplorerCommand : ICommand
    {
        private event EventHandler continueWith;
        private event EventHandler canExecuteChanged;


        public event EventHandler ContinueWith
        {
            add
            {
                continueWith += value;
            }

            remove
            {
                continueWith -= value;
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                canExecuteChanged += value;
            }

            remove
            {
                canExecuteChanged -= value;
            }
        }



        public void RaiseCanExecuteChanged()
        {
            canExecuteChanged.Invoke(null, null);
        }

        public bool CanExecute(object parameter)
        {
            var textBox = parameter as TextBox;

            if (textBox != null && !string.IsNullOrWhiteSpace(textBox.Text))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Execute(object parameter)
        {
            var textBox = parameter as TextBox;

            if (textBox != null && !string.IsNullOrWhiteSpace(textBox.Text))
            {
                string directory = textBox.Text;

                if (File.GetAttributes(directory).HasFlag(FileAttributes.Directory) && Directory.Exists(directory))
                {
                    Process.Start(directory);
                    continueWith.Invoke(null, null);
                }
            }
        }

    }
}
