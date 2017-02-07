using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTag.App_Code
{
    public static class UserInfoProvider
    {
        public static string PA
        {
            get
            {
                return Properties.Settings.Default.UserInfo[2].Split(new[] { ':' })[1];
            }
        }

        public static string LTO
        {
            get
            {
                return Properties.Settings.Default.UserInfo[0].Split(new[] { ':' })[1];
            }
        }

        public static string MID
        {
            get
            {
                return Properties.Settings.Default.UserInfo[1].Split(new[] { ':' })[1];
            }
        }

        public static bool IsNewDb
        {
            get
            {
                return Properties.Settings.Default.UserInfo[3].Split(new[] { ':' })[1] == "new";
            }
        }
    }
}
