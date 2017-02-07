using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TimeTag
{
    public class outz_Log
    {
        public static void LogError(string message)
        {
            try
            {
                HelperSetting.WriteWithTime(message, Properties.Settings.Default.ErrorLog, true);
            }
            catch
            {               
            }
        }

        public static void LogStatus(string message)
        {
            try
            {
                HelperSetting.WriteWithTime(message, Properties.Settings.Default.StatusLog, true);
            }
            catch
            {
            }
        }
    }
}
