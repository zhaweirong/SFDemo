using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFDemo.BLL
{
    internal class GlobalConfig

    {
        public static readonly string folderPath = GetAppsettingStr("Path");
        public static readonly string SFWAY = GetAppsettingStr("SFWAY");
        public static readonly int trigPollingtime = int.Parse(GetAppsettingStr("TrigPollingTime"));

        public static readonly int RetryCount = int.Parse(GetAppsettingStr("RetryCount"));
        public static readonly double RetryInterval = (double.Parse(GetAppsettingStr("RetryInterval"))) / 1000;

        public static readonly string[] CheckLogName = GetAppsettingStr("CheckLogName").Split(',');
        public static readonly string[] LinkLogName = GetAppsettingStr("LinkLogName").Split(',');

        public static string GetAppsettingStr(string str)
        {
            AppSettingsReader appReader = new AppSettingsReader();
            return appReader.GetValue(str, typeof(string)).ToString();
        }
    }
}
