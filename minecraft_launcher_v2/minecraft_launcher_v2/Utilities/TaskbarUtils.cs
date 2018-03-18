using Microsoft.WindowsAPICodePack.Taskbar;
using minecraft_launcher_v2.CustomStructs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace minecraft_launcher_v2.Utilities
{
    static class TaskbarUtils
    {
        public static void SetTaskbarItemProgress(IntPtr windowPointer, TaskbarProgressBarState progressState, double progressPercentage)
        {
            if (progressPercentage != 0)
            {
                int value = (int)(progressPercentage * 1000);

                if (value > 1000)
                {
                    value = 1000;
                }
                else if (value < 0)
                {
                    value = 0;
                }

                TaskbarManager.Instance.SetProgressValue(value, 1000, windowPointer);
            }
            else
            {
                TaskbarManager.Instance.SetProgressValue(0, 0, windowPointer);
            }

            TaskbarManager.Instance.SetProgressState(progressState, windowPointer);
        }

        public static void SetTaskbarJumpListLink(IntPtr windowPointer, Dictionary<string, List<TaskbarSiteItem>> jumpListItems)
        {
            var jumpList = JumpList.CreateJumpListForIndividualWindow(Process.GetCurrentProcess().Id.ToString(), windowPointer);
            List<JumpListCustomCategory> categoriesList = new List<JumpListCustomCategory>();
            JumpListCustomCategory jumpListCustomCategory;

            if (jumpListItems.ContainsKey("Sites"))
            {

            }


            foreach (var item in jumpListItems)
            {
                jumpListCustomCategory = new JumpListCustomCategory(item.Key);

                foreach (var subItem in item.Value)
                {
                    if (subItem.TaskbarTitle == "Official Minecraft Site" && subItem.SiteLink != @"https://www.minecraft.net/")
                    {
                        continue;
                    }

                    JumpListLink jumpListLink = new JumpListLink(subItem.SiteLink, subItem.TaskbarTitle);

                    if (File.Exists(subItem.IconPath))
                    {
                        jumpListLink.IconReference = new Microsoft.WindowsAPICodePack.Shell.IconReference(Path.Combine(subItem.IconPath), subItem.IconResourceId);
                    }

                    jumpListCustomCategory.AddJumpListItems(jumpListLink);
                }

                categoriesList.Add(jumpListCustomCategory);
            }

            jumpList.AddCustomCategories(categoriesList.ToArray());
            jumpList.Refresh();
        }

    }
}
