using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeTag.Entity;

namespace TimeTag.Helper
{
    public static class ConfigHelper
    {
        public static TimeInputMode TimeInputMode
        {
            get
            {
                if (Properties.Settings.Default.TimeInputMode.Equals("Stopwatch", StringComparison.InvariantCulture))
                {
                    return TimeInputMode.Stopwatch;
                }
                else
                {
                    return TimeInputMode.ManualInput;
                }
            }
        }

        public static bool ListActivities
        {
            get
            {
                return Properties.Settings.Default.ListActivities;
            }
        }
    }
}
