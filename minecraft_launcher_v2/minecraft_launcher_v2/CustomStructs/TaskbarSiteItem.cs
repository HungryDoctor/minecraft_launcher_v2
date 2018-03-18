namespace minecraft_launcher_v2.CustomStructs
{
    public class TaskbarSiteItem
    {
        private string siteLink;
        private string taskbarTitle;
        private string iconPath;
        private int iconResourceId;


        public string SiteLink
        {
            get
            {
                return siteLink;
            }
        }
        public string TaskbarTitle
        {
            get
            {
                return taskbarTitle;
            }
        }
        public string IconPath
        {
            get
            {
                return iconPath;
            }
        }
        public int IconResourceId
        {
            get
            {
                return iconResourceId;
            }
        }



        public TaskbarSiteItem(string siteLink, string taskbarTitle, string iconPath, int iconResourceId)
        {
            this.siteLink = siteLink;
            this.taskbarTitle = taskbarTitle;
            this.iconPath = iconPath;
            this.iconResourceId = iconResourceId;
        }

    }
}
