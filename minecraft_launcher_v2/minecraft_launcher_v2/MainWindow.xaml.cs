using minecraft_launcher_v2.Model;
using System;
using System.Windows;
using System.Windows.Interop;

namespace minecraft_launcher_v2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal IntPtr MainWindowPointer
        {
            get
            {
                IntPtr pointer = IntPtr.Zero;
                Dispatcher.Invoke(new Action(() =>
                {
                    if (IsActive)
                    {
                        pointer = new WindowInteropHelper(this).Handle;
                    }
                }));

                return pointer;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }


        bool _shown;
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_shown)
                return;

            _shown = true;

            var modelMain = new ModelMain();
            modelMain.SetTaskbarJumpListAsync(MainWindowPointer, null);
        }
    }
}
