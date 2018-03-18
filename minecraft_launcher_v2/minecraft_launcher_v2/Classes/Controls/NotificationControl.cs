using minecraft_launcher_v2.ConstantValues;
using minecraft_launcher_v2.Utilities;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Resources;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace minecraft_launcher_v2.Classes.Controls
{
    static class NotificationControl
    {
        private static Action downloadNowAction;
        private static Action downloadLaterAction;

        private static ToastNotifier notifier;
        private static ToastNotification toastNotification;


        public static event Action DownloadNowAction
        {
            add
            {
                downloadNowAction += value;
            }

            remove
            {
                downloadNowAction -= value;
            }
        }

        public static event Action DownloadLaterAction
        {
            add
            {
                downloadLaterAction += value;
            }

            remove
            {
                downloadLaterAction -= value;
            }
        }


        static NotificationControl()
        {
            if (Constants.NOTIFICATION_IS_ALLOWED)
            {
                notifier = ToastNotificationManager.CreateToastNotifier("minecraft_launcher");

                string logoPath = Constants.PATH_SETTINGS_GLOBAL + "\\" + Constants.FILENAME_PICTURE_NOTIFICATION;
                if (!File.Exists(logoPath))
                {
                    try
                    {
                        StreamResourceInfo sri = Application.GetResourceStream(new Uri("Resources/Image_Icon.ico", UriKind.Relative));
                        if (sri != null)
                        {
                            using (Stream s = sri.Stream)
                            {
                                Icon ico = new Icon(s);

                                CommonUtils.ConvertIcoToPng(ico).Save(logoPath, ImageFormat.Png);
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                XmlDocument xml = new XmlDocument();
                xml.LoadXml(string.Format(Constants.NOTIFICATION_UPDATE_MESSAGE_XML_F, logoPath, Messages.CAPTION_COMMON, Messages.NOTIFICATION_CONTENT,
                    Messages.NOTIFICATION_ACTION_DOWNLOAD, string.Format(Messages.NOTIFICATION_ACTION_LATER_F, Constants.NOTIFICATION_DOWNLOAD_LATER_DAYS)));

                toastNotification = new ToastNotification(xml);


                toastNotification.Activated += ToastActivated;
            }
        }



        private static void ToastActivated(ToastNotification sender, object args)
        {
            if (args is ToastActivatedEventArgs)
            {
                switch (((ToastActivatedEventArgs)args).Arguments)
                {
                    case "action=Download":
                        {
                            downloadNowAction.Invoke();
                            break;
                        }
                    case "action=Later":
                        {
                            downloadLaterAction.Invoke();
                            break;
                        }
                }
            }
        }


        public static void ShowNotification()
        {
            if (Constants.NOTIFICATION_IS_ALLOWED)
            {
                notifier.Show(toastNotification);
            }
        }

        public static void HideNotification()
        {
            if (Constants.NOTIFICATION_IS_ALLOWED)
            {
                notifier.Hide(toastNotification);
            }
        }

    }
}
